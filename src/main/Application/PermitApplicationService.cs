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
        private readonly INeuronValidationService neuronValidationService;

        public PermitApplicationService(INeuronPermitRepository repository, 
            INeuronValidationService neuronValidationService
            )
        {
            this.repository = repository;
            this.neuronValidationService = neuronValidationService;
        }

        public async Task CreateRequestAsync(Guid neuronId, Guid userNeuronId)
        {
            AssertionConcern.AssertArgumentValid(g => g.IsNotEmpty(), userNeuronId, Constants.Messages.Exception.InvalidUserId, nameof(userNeuronId));
            AssertionConcern.AssertArgumentValid(g => g.IsNotEmpty(), neuronId, Constants.Messages.Exception.InvalidId, nameof(neuronId));
            AssertionConcern.AssertArgumentTrue(await this.neuronValidationService.Exists(neuronId), Constants.Messages.Exception.NeuronNotFound);

            var entity = new NeuronPermit()
            {
                NeuronId = neuronId,
                UserNeuronId = userNeuronId,
                ExpirationDate = null
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
