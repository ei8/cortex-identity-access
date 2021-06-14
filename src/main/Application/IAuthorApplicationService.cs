using ei8.Cortex.IdentityAccess.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Application
{
    public interface IAuthorApplicationService
    {
        Task<AuthorInfo> GetAuthorByUserId(string userId, CancellationToken token = default(CancellationToken));
    }
}
