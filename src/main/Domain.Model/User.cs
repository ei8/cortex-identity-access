using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    /// <summary>
    /// Represents User Neurons. TODO: Should later be persisted as User Neurons, instead of as records in User databases.
    /// </summary>
    public class User
    {
        /// <summary>
        /// User Id of this User in Identity Access server. This property should be stored as a separate Neuron in a secure layer when it is converted.
        /// </summary>
        [PrimaryKey]
        public string UserId { get; set; }

        /// <summary>
        /// NeuronId of User Neuron.
        /// </summary>
        public Guid NeuronId { get; set; }

        public bool Active { get; set; }
    }
}
