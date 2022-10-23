using CQRSlite.Commands;
using ei8.Cortex.IdentityAccess.Application.AccessRequest.Commands;
using neurUL.Common.Domain.Model;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Application.AccessRequest
{
    public class AccessRequestCommandHandlers : ICancellableCommandHandler<CreateNeuronAccessRequest>
    {
        private readonly IPermitApplicationService permitApplicationService;

        public AccessRequestCommandHandlers(IPermitApplicationService permitApplicationService)
        {
            this.permitApplicationService = permitApplicationService;
        }

        public async Task Handle(CreateNeuronAccessRequest message, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentNotNull(message, nameof(message));

            await this.permitApplicationService.CreateRequestAsync(message.NeuronId, message.UserNeuronId);
        }
    }
}
