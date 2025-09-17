using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUserNameAsync(string userName);
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}
