using ei8.Cortex.IdentityAccess.Application.Extensions;
using ei8.Cortex.IdentityAccess.Domain.Model;
using neurUL.Common.Domain.Model;
using System;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Application
{
    public class PermitApplicationService : IPermitApplicationService
    {
        private readonly INeuronPermitRepository repository;

        public PermitApplicationService(INeuronPermitRepository repository)
        {
            this.repository = repository;
        }

        public async Task CreateRequestAsync(Guid neuronId, Guid userNeuronId)
        {
            AssertionConcern.AssertArgumentValid(g => g.IsNotEmpty(), userNeuronId, Constants.Messages.Exception.InvalidUserId, nameof(userNeuronId));
            AssertionConcern.AssertArgumentValid(g => g.IsNotEmpty(), neuronId, Constants.Messages.Exception.InvalidId, nameof(neuronId));

            var entity = new NeuronPermit()
            {
                NeuronId = neuronId,
                UserNeuronId = userNeuronId,
                ValidUntilUtc = null
            };

            await this.repository.Initialize();
            await this.repository.InsertAsync(entity);
        }

        public async Task GrantAccess(Guid neuronId, Guid userNeuronId, TimeSpan duration)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
