using ei8.Cortex.Graph.Client;
using ei8.Cortex.Graph.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    public class NeuronValidationService : INeuronValidationService
    {
        private readonly INeuronGraphQueryClient neuronGraphQueryClient;
        private readonly ISettingsService settingsService;

        public NeuronValidationService(INeuronGraphQueryClient neuronGraphQueryClient, ISettingsService settingsService)
        {
            this.neuronGraphQueryClient = neuronGraphQueryClient;
            this.settingsService = settingsService;
        }

        public async Task<bool> Exists(Guid neuronId, CancellationToken token = default)
        {
            // ensure that layer is a valid neuron
            var qs = await this.neuronGraphQueryClient.GetNeuronById(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                neuronId.ToString(),
                new NeuronQuery() { NeuronActiveValues = ActiveValues.All },
                token: token
                );

            return qs?.Neurons?.Any() ?? false;
        }
    }
}
