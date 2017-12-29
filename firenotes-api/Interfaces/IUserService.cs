using System.Threading.Tasks;
using firenotes_api.Models.Data;

namespace firenotes_api.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUser(string id);

        void Update(string id, User user);

        void Archive(string id);
    }
}