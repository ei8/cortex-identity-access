using ei8.Cortex.IdentityAccess.Common;
using ei8.Cortex.IdentityAccess.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Application
{
    public class ValidationApplicationService : IValidationApplicationService
    {
        private readonly IValidationService validationService;

        public ValidationApplicationService(IValidationService validationService)
        {
            this.validationService = validationService;
        }

        public async Task<ActionValidationResult> CreateNeuron(Guid neuronId, Guid? regionId, string userId, CancellationToken token = default)
        {
            return await this.validationService.CreateNeuron(neuronId, regionId, userId, token);
        }

        public async Task<ActionValidationResult> UpdateNeuron(Guid neuronId, string userId, CancellationToken token = default)
        {
            return await this.validationService.UpdateNeuron(neuronId, userId, token);
        }

        public async Task<ActionValidationResult> ReadNeurons(IEnumerable<Guid> neuronIds, string userId, CancellationToken token = default)
        {
            return await this.validationService.ReadNeurons(neuronIds, userId, token);
        }

    }
}
