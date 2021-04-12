using ei8.Avatar.Application;
using ei8.Avatar.Domain.Model;
using ei8.Avatar.Port.Adapter.Common;
using IdentityModel.Client;
using Microsoft.Net.Http.Headers;
using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Nancy.Responses;
using Nancy.Security;
using neurUL.Common.Http;
using Newtonsoft.Json;
using Nito.AsyncEx;
using NLog;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace ei8.Avatar.Port.Adapter.In.Api
{
    public class InputModule : NancyModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public InputModule(IResourceApplicationService resourceApplicationService, IRequestProvider requestProvider) : base(string.Empty)
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
                                        await InputModule.ProcessReadMethod(this, r.OutUri, requestProvider) : 
                                        await InputModule.ProcessWriteMethod(this, new HttpMethod(m), r.InUri, requestProvider)
                                ), 
                            (nc) => true, 
                            r.PathPattern
                            )
                        )                
                    );
        }


        #region TODO: transfer to domain.model
        private static async Task<Nancy.Response> ProcessReadMethod(NancyModule module, string outUri, IRequestProvider requestProvider)
        {
            var result = new Nancy.Response();
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
                    await InputModule.GetUserSubjectId(module, requestProvider)
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
                
        private static async Task<Nancy.Response> ProcessWriteMethod(NancyModule module, HttpMethod method, string inUri, IRequestProvider requestProvider)
        {
            var result = new Nancy.Response();
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

                jsonObj.SubjectId = await InputModule.GetUserSubjectId(module, requestProvider);
                var content = new StringContent(JsonConvert.SerializeObject(jsonObj));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                
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

        private static string GetPath(Nancy.Request request)
        {
            return request.Url.ToString().Substring(request.Url.ToString().IndexOf(request.Path));
        }

        internal static async Task<string> GetUserSubjectId(NancyModule module, IRequestProvider requestProvider)
        {
            var result = string.Empty;

            if (bool.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.RequireAuthentication), out bool value) && value)
            {
                InputModule.logger.Info("Authentication required...");
                var accessToken = module.Request.Headers[HeaderNames.Authorization].FirstOrDefault();
                if (accessToken == null)
                {
                    result = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.AnonymousUserSubjectId);
                    InputModule.logger.Info($"User is anonymous. Using subjectId - {{{LoggerProperties.SubjectId}}}", result);
                }
                else
                {
                    var introspectionResponse = await requestProvider.HttpClient.IntrospectTokenAsync(new TokenIntrospectionRequest
                    {
                        // TODO: use SettingsService for the following values which are duplicated in ei8.Avatar.Port.Adapter.In.Api.Startup.ConfigureServices
                        Address = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.TokenIssuerAddress) + "/connect/introspect",
                        ClientId = "avatarapi", 
                        // TODO: ClientSecret = "secret",
                        Token = accessToken.Substring(accessToken.IndexOf(" ") + 1)
                    });
                    if (!introspectionResponse.IsActive)
                    {
                        InputModule.logger.Error($"Specified token is inactive.");
                        throw new AuthenticationException("Specified access token is inactive.");
                    }

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
