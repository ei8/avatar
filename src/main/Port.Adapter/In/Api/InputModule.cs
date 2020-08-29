using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Nancy.Responses;
using Nancy.Security;
using Newtonsoft.Json;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ei8.Avatar.Application;
using ei8.Avatar.Port.Adapter.Common;

namespace ei8.Avatar.Port.Adapter.In.Api
{
    public class InputModule : NancyModule
    {
        public InputModule(IResourceApplicationService resourceApplicationService) : base(string.Empty)
        {
            // TODO: handle single path, 3 paths etc.
            this.Post("/{path1}/{path2}/{any*}", (Func<dynamic, Task<Response>>)(async (parameters) =>
            {
                return await InputModule.ProcessWriteMethod(
                    this, 
                    HttpMethod.Post, 
                    resourceApplicationService, 
                    parameters
                    );
            })
            );

            this.Patch("/{path1}/{path2}/{any*}", async (parameters) =>
            {
                return await InputModule.ProcessWriteMethod(
                    this,
                    new HttpMethod("PATCH"),
                    resourceApplicationService,
                    parameters
                    );
            }
            );

            this.Get("/{path1}/{path2}/{any*}", async (parameters) =>
            {
                var result = new Response();
                HttpResponseMessage response = null;
                var responseContent = string.Empty;
                try
                {
                    var resourcePath = InputModule.GetCombinedPath(parameters);
                    var resource = await resourceApplicationService.GetByPath(resourcePath);
                    AssertionConcern.AssertStateTrue(resource != null, string.Format($"Resource '{resourcePath}' was not recognized."));
                    var hc = new HttpClient()
                    {
                        BaseAddress = new Uri(resource.OutUri)
                    };

                    hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var initialPath = InputModule.GetPath(this.Context.Request);
                    response = await hc.GetAsync(
                        initialPath + 
                        (initialPath.Contains('?') && initialPath.Contains('=') ? "&" : "?") +
                        "subjectid=" +
                        InputModule.GetUserSubjectId(this)
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
            );

            this.Delete("/{path1}/{path2}/{any*}", async (parameters) =>
            {
                return await InputModule.ProcessWriteMethod(
                    this,
                    HttpMethod.Delete,
                    resourceApplicationService,
                    parameters
                    );
            }
            );
        }

        // TODO: transfer to domain.model
        private static async Task<Response> ProcessWriteMethod(NancyModule module, HttpMethod method, IResourceApplicationService resourceApplicationService, dynamic parameters)
        {
            var result = new Response();
            HttpResponseMessage response = null;
            var responseContent = string.Empty;
            try
            {
                var resourcePath = InputModule.GetCombinedPath(parameters);
                var resource = await resourceApplicationService.GetByPath(resourcePath);
                AssertionConcern.AssertStateTrue(resource != null, string.Format($"Resource '{resourcePath}' was not recognized."));
                var hc = new HttpClient()
                {
                    BaseAddress = new Uri(resource.InUri)
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

        private static string GetCombinedPath(dynamic parameters)
        {
            var result = string.Empty;
            var parametersDict = ((IDictionary<string, object>)parameters);
            int i = 1;
            while (parametersDict.ContainsKey("path" + i))
            {
                result += "/" + parametersDict["path" + i].ToString();
                i++;
            }

            return result;
        }

        private static string GetPath(Request request)
        {
            return request.Url.ToString().Substring(request.Url.ToString().IndexOf(request.Path));
        }

        internal static string GetUserSubjectId(NancyModule module)
        {
            var result = string.Empty;

            if (bool.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.RequireAuthentication), out bool value) && value)
            {
                if (module.Context.CurrentUser == null)
                    result = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.AnonymousUserSubjectId);
                else
                {
                    module.RequiresAuthentication();
                    result = module.Context.CurrentUser.Claims.First(c => c.Type == "sub").Value;
                }                   
            }
            else
                result = Environment.GetEnvironmentVariable(EnvironmentVariableKeys.ProxyUserSubjectId);

            return result;
        }
    }
}
