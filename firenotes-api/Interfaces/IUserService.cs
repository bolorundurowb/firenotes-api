using System.Threading.Tasks;
using firenotes_api.Models.Binding;
using firenotes_api.Models.Data;

namespace firenotes_api.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUser(string id);

        Task<User> GetUserByEmail(string email);

        Task Add(User user);
        
        Task Update(string id, UserBindingModel user);

        Task Archive(string id);
    }
}