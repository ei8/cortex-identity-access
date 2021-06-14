using System;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    public interface IAuthorRepository
    {
        Task<Author> GetByUserId(string userId);

        Task Initialize();
    }
}
