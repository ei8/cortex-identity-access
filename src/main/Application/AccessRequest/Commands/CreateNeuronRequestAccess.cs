using CQRSlite.Commands;
using System;

namespace ei8.Cortex.IdentityAccess.Application.AccessRequest.Commands
{
    public class CreateNeuronRequestAccess : ICommand
    {
        public CreateNeuronRequestAccess(Guid neuronId, Guid userNeuronId)
        {
            NeuronId = neuronId;
            UserNeuronId = userNeuronId;
        }

        public Guid NeuronId { get; set; }
        public Guid UserNeuronId { get; set; }

        public int ExpectedVersion { get; private set; }
    }
}
