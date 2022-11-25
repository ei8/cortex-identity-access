using System;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    public interface INeuronPermitRepository
    {
        Task InsertAsync(NeuronPermit permit);

        Task<NeuronPermit> GetAsync(Guid userNeuronId, Guid neuronId);

        Task UpdateAsync(NeuronPermit permit);

        Task Initialize();
    }
}
