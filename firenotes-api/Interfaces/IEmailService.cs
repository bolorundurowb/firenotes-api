using System.Threading.Tasks;

namespace firenotes_api.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string recipient, string subject, string payload);
    }
}