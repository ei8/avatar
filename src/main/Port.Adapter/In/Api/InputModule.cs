using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Nancy.Responses;
using Nancy.Security;
using Newtonsoft.Json;
using org.neurul.Common.Domain.Model;
using System;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using works.ei8.Cortex.Sentry.Application;
using works.ei8.Cortex.Sentry.Port.Adapter.Common;

namespace works.ei8.Cortex.Sentry.Port.Adapter.In.Api
{
    public class InputModule : NancyModule
    {
        public InputModule(IAuthorApplicationService authorApplicationService) : base(string.Empty)
        {
            if (bool.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.RequireAuthentication), out bool value) && value)
                this.RequiresAuthentication();

            // TODO: PATCH, DELETE
            this.Post("/{avatarId}/nuclei/d23/(.*)", async (parameters) =>
            {
                var result = new Response();
                HttpResponseMessage response = null;
                var responseContent = string.Empty;
                try
                {
                    var hc = new HttpClient()
                    {
                        BaseAddress = new Uri("http://192.168.8.135:60020")
                    };

                    string jsonString = RequestStream.FromStream(this.Request.Body).AsString();
                    string avatarId = parameters.avatarId;
                    var subjectId = GetUserSubjectId(this.Context);
                    var author = await authorApplicationService.GetAuthorBySubjectId(avatarId, subjectId);
                    // DEL: string authorId = "2cafd291-f025-40b4-80bb-325067786a32"; // author.User.NeuronId.ToString();
                    dynamic jsonObj = JsonConvert.DeserializeObject<ExpandoObject>(jsonString);

                    jsonObj.AuthorId = author.User.NeuronId.ToString();
                    // DEL: jsonObj.RegionId = "2cafd291-f025-40b4-80bb-325067786a32";
                    var content = new StringContent(JsonConvert.SerializeObject(jsonObj));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    response = await hc.PostAsync(this.Context.Request.Path, content);
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
            );

            // TODO: Transfer to Port.Adapter.Out.OutputModule
            this.Get("/{avatarId}/nuclei/d23/{any*}", async (parameters) =>
            {
                var hc = new HttpClient()
                {
                    BaseAddress = new Uri("http://192.168.8.135:60021")
                };

                hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await hc.GetAsync(this.Context.Request.Path);  
                // DEL: $"/{parameters.avatarId}/nuclei/eventstore");

                return new TextResponse(
                    response.IsSuccessStatusCode ? 
                        HttpStatusCode.OK : 
                        HttpStatusCode.BadRequest, 
                    await response.Content.ReadAsStringAsync()
                    );
            }
            );
        }

        // TODO: Get User Subject Id to specify subject id in call to AuthorApplicationService
        internal static Guid GetUserSubjectId(NancyContext context)
        {
            Guid result = Guid.Empty;

            if (bool.TryParse(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.RequireAuthentication), out bool value) && value)
            {
                AssertionConcern.AssertArgumentValid(c => c.CurrentUser != null, context, "Context User is null or not found.", nameof(context));
                result = Guid.Parse(context.CurrentUser.Claims.First(c => c.Type == "sub").Value);
            }
            else
                result = Guid.Parse(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.TestUserSubjectId));

            return result;
        }
    }
}
