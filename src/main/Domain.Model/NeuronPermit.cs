using SQLite;
using System;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    public class NeuronPermit
    {
        [PrimaryKey]
        public Guid UserNeuronId { get; set; }

        [PrimaryKey]
        public Guid NeuronId { get; set; }

        public Nullable<DateTime> ExpirationDate { get; set; }
    }
}
