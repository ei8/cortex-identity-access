using ei8.Cortex.IdentityAccess.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Application
{
    public interface IAuthorApplicationService
    {
        Task<AuthorInfo> GetAuthorBySubjectId(Guid subjectId, CancellationToken token = default(CancellationToken));
    }
}
