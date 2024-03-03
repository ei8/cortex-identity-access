using ei8.Cortex.IdentityAccess.Application.Extensions;
using ei8.Cortex.IdentityAccess.Domain.Model;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task CreateNeuronPermitAsync(Guid neuronId, Guid userNeuronId, DateTime? expirationDate = null)
        {
            AssertionConcern.AssertArgumentValid(g => g.IsNotEmpty(), userNeuronId, Constants.Messages.Exception.InvalidUserId, nameof(userNeuronId));
            AssertionConcern.AssertArgumentValid(g => g.IsNotEmpty(), neuronId, Constants.Messages.Exception.InvalidId, nameof(neuronId));

            var entity = new NeuronPermit()
            {
                NeuronId = neuronId,
                UserNeuronId = userNeuronId,
                ExpirationDate = expirationDate
            };

            await this.repository.Initialize();
            await this.repository.InsertAsync(entity);
        }

        public async Task<IEnumerable<Guid>> GetNeuronIdsByUserNeuronIds(IEnumerable<Guid> userNeuronIds, IEnumerable<Guid> filterNeuronIds = null)
        {
            AssertionConcern.AssertArgumentValid(g => g != null && g.Any(), userNeuronIds, Constants.Messages.Exception.EnumerableNullOrEmpty, nameof(userNeuronIds));

            await this.repository.Initialize();
            return await this.repository.GetNeuronIdsByUserNeuronIds(userNeuronIds, filterNeuronIds);
        }

        public async Task GrantNeuronPermitAccessAsync(Guid neuronId, Guid userNeuronId, TimeSpan duration)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
