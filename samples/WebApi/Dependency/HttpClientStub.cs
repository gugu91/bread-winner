using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApi.Dependency
{
    public class HttpClientStub : IHttpClient
    {
        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent("Test server running..."),
                StatusCode = HttpStatusCode.OK
            };

            return Task.FromResult(response);
        }
    }
}