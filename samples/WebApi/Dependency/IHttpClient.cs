using System.Net.Http;
using System.Threading.Tasks;

namespace WebApi.Dependency
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}