using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Testing;
using Shouldly;
using Xunit;

namespace MediatrAutofacReferenceApp.Tests
{
    public class SimpleTests
    {
        [Fact]
        public async Task Does_a_call_to_ping_work()
        {
            HttpResponseMessage httpResponseMessage;
            using (var server = TestServer.Create<StartupWithFullStack>())
            {
                httpResponseMessage = await server.HttpClient.GetAsync("ping?message=Hi mom");
            }

            httpResponseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
            var response = await httpResponseMessage.Content.ReadAsAsync<string>();
            response.ShouldBe("pong Hi mom");
        }

        [Fact]
        public async Task Does_a_bad_call_to_ping_cause_the_validation_pipeline_to_kick_in()
        {
            HttpResponseMessage httpResponseMessage;
            using (var server = TestServer.Create<StartupWithFullStack>())
            {
                httpResponseMessage = await server.HttpClient.GetAsync("ping?message=");
            }

            httpResponseMessage.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
            var response = await httpResponseMessage.Content.ReadAsAsync<HttpError>();
            response["ExceptionMessage"].ToString().ShouldContain("'Message Body' should not be empty");
        }
    }
}
