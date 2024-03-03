using SQLite;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.IO.Persistence.SQLite
{
    public class NeuronIdData
    {
        [PrimaryKey]
        public string NeuronId { get; set; }
    }
}
