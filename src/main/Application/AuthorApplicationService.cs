using ei8.Cortex.IdentityAccess.Common;
using ei8.Cortex.IdentityAccess.Domain.Model;
using neurUL.Common.Domain.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Application
{
    public class AuthorApplicationService : IAuthorApplicationService
    {
        private readonly IAuthorRepository authorRepository;
        
        public AuthorApplicationService(IAuthorRepository authorRepository)
        {
            this.authorRepository = authorRepository;
        }

        public async Task<AuthorInfo> GetAuthorBySubjectId(Guid subjectId, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, subjectId, Constants.Messages.Exception.InvalidId, nameof(subjectId));

            await this.authorRepository.Initialize();
            var author = await this.authorRepository.GetBySubjectId(subjectId);
            return author != null ? new AuthorInfo(author.User.NeuronId, author.User.Active) : null;
        }
    }
}
