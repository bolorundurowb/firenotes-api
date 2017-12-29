using firenotes_api.Models.Data;

namespace firenotes_api.Interfaces
{
    public interface IUserService
    {
        User GetUser(string id);

        void Update(string id, User user);

        void Archive(string id);
    }
}