using ei8.Avatar.Application;
using ei8.Avatar.Domain.Model;
using ei8.Avatar.Port.Adapter.Common;
using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Nancy.Responses;
using Nancy.Security;
using Newtonsoft.Json;
using Nito.AsyncEx;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ei8.Avatar.Port.Adapter.In.Api
{
    public class InputModule : NancyModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public InputModule(IResourceApplicationService resourceApplicationService) : base(string.Empty)
        {
            AsyncContext.Run(() => resourceApplicationService.GetResources())
                .ToList().ForEach(r =>
                    r.Methods.Split(',').ToList().ForEach(m => 
                        this.AddRoute(
                            m, 
                            r.PathPattern, 
                            async (parameters, token) => 
                                (
                                    m == "GET" ? 
                                        await InputModule.ProcessReadMethod(this, r.OutUri) : 
                                        await InputModule.ProcessWriteMethod(this, new HttpMethod(m), r.InUri)
                                ), 
                            (nc) => true, 
                            r.PathPattern
                            )
                        )                
                    );
        }


        #region TODO: transfer to domain.model
        private static async Task<Response> ProcessReadMethod(NancyModule module, string outUri)
        {
            var result = new Response();
            HttpResponseMessage response = null;
            var responseContent = string.Empty;
            try
            {
                var hc = new HttpClient()
                {
                    BaseAddress = new Uri(outUri)
                };

                hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var initialPath = InputModule.GetPath(module.Context.Request);
                response = await hc.GetAsync(
                    initialPath +
                    (initialPath.Contains('?') && initialPath.Contains('=') ? "&" : "?") +
                    "subjectid=" +
                    InputModule.GetUserSubjectId(module)
                    );
                responseContent = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                return new TextResponse(HttpStatusCode.OK, responseContent);
            }
            catch (Exception ex)
            {
                result = new TextResponse(HttpStatusCode.BadRequest, (response != null) ? responseContent : ex.ToString());
            }
            return result;
        }
                
        private static async Task<Response> ProcessWriteMethod(NancyModule module, HttpMethod method, string inUri)
        {
            var result = new Response();
            HttpResponseMessage response = null;
            var responseContent = string.Empty;
            try
            {
                var hc = new HttpClient()
                {
                    BaseAddress = new Uri(inUri)
                };

                string jsonString = RequestStream.FromStream(module.Request.Body).AsString();
                dynamic jsonObj = string.IsNullOrEmpty(jsonString) ? 
                    new ExpandoObject() :
                    JsonConvert.DeserializeObject<ExpandoObject>(jsonString);

                jsonObj.SubjectId = InputModule.GetUserSubjectId(module);
                var content = new StringContent(JsonConvert.SerializeObject(jsonObj));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                
                var message = new HttpRequestMessage(
                        method,
                        InputModule.GetPath(module.Context.Request)
                    )
                { Content = content };
                foreach (var kvp in module.Context.Request.Headers.ToList())
                    if (Array.IndexOf(new string[] { "Content-Length", "Content-Type" }, kvp.Key) < 0)
                        message.Headers.Add(kvp.Key, string.Join(',', kvp.Value));
                response = await hc.SendAsync(message);
                responseContent = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                result = new TextResponse(HttpStatusCode.OK, responseContent);
            }
            catch (Exception ex)
            {
                result = new TextResponse(HttpStatusCode.BadRequest, (response != null) ? responseContent : ex.ToString());
            }
            return result;
        }
        #endregion

        private static string GetPath(Request request)
        {
            return request.Url.ToString().Substring(request.Url.ToString().IndexOf(request.Path));
        }

        internal static string GetUserSubjectId(NancyModule module)
        {
            var result = string.Empty;

            if (bool.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.RequireAuthentication), out bool value) && value)
            {
                InputModule.logger.Info("Authentication required...");
                if (module.Context.CurrentUser == null)
                {
                    result = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.AnonymousUserSubjectId);
                    InputModule.logger.Info($"User is anonymous. Using subjectId - {{{LoggerProperties.SubjectId}}}", result);
                }
                else
                {
                    module.RequiresAuthentication();
                    result = module.Context.CurrentUser.Claims.First(c => c.Type == "sub").Value;
                    InputModule.logger.Info($"User has been authenticated. Using subjectId - {{{LoggerProperties.SubjectId}}}", result);
                }
            }
            else
            {
                result = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ProxyUserSubjectId);
                InputModule.logger.Info($"Authentication not required. Using subjectId - {{{LoggerProperties.SubjectId}}}", result);
            }

            return result;
        }
    }
}
