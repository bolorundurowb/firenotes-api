using System.Threading.Tasks;
using RestSharp;

namespace firenotes_api.Interfaces
{
    public interface IEmailService
    {
        Task<IRestResponse> SendAsync(string recipient, string subject, string payload);
    }
}