using neurUL.Common.Domain.Model;
using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
using ei8.Cortex.IdentityAccess.Domain.Model;
using ei8.Cortex.IdentityAccess.Port.Adapter.Common;
using ei8.Cortex.IdentityAccess.Application;
using ei8.Cortex.Graph.Client;
using ei8.Cortex.Graph.Common;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.IO.Persistence.IdentityAccess
{
    public class AuthorRepository : IAuthorRepository
    {
        private SQLiteAsyncConnection connection;
        private ISettingsService settingsService;
        private INeuronGraphQueryClient neuronGraphQueryClient;

        public AuthorRepository(ISettingsService settingsService, INeuronGraphQueryClient neuronGraphQueryClient)
        {
            this.settingsService = settingsService;
            this.neuronGraphQueryClient = neuronGraphQueryClient;
        }

        public async Task<Author> GetBySubjectId(Guid subjectId)
        {
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, subjectId, Constants.Messages.Exception.InvalidId, nameof(subjectId));

            Author result = null;

            var results = this.connection.Table<User>().Where(e => e.SubjectId == subjectId);
            var user = (await results.ToListAsync()).SingleOrDefault();

            if (user != null)
            {
                // check if null if neuron is inactive or deactivated, if so, should throw exception
                var userNeuron = (await this.neuronGraphQueryClient.GetNeuronById(
                    this.settingsService.CortexGraphOutBaseUrl + "/",
                    user.NeuronId.ToString(),
                    new NeuronQuery() { NeuronActiveValues = ActiveValues.All }
                    )
                    ).Neurons.FirstOrDefault();

                AssertionConcern.AssertStateTrue(userNeuron != null, Constants.Messages.Exception.NeuronNotFound);
                AssertionConcern.AssertStateTrue(userNeuron.Active, Constants.Messages.Exception.NeuronInactive);

                var permits = await (this.connection.Table<RegionPermit>().Where(e => e.UserNeuronId == user.NeuronId)).ToArrayAsync();
                result = new Author(user, permits);
            }

            return result;
        }

        public async Task Initialize()
        {
            this.connection = await AuthorRepository.CreateConnection<User>(this.settingsService.UserDatabasePath);

            //sample data creator - call Initialize from CustomBootstrapper to invoke
            //await this.connection.InsertAsync(new User()
            //{
            //    NeuronId = Guid.NewGuid(),
            //    SubjectId = Guid.NewGuid()
            //});

            // sample data creator - call Initialize from CustomBootstrapper to invoke
            //await this.connection.InsertAsync(new RegionPermit()
            //{
            //    UserNeuronId = Guid.NewGuid(),
            //    RegionNeuronId = Guid.NewGuid(), 
            //    WriteLevel = 1,
            //    CanRead = true
            //});
        }

        // TODO: Transfer to NeurUL.Common
        internal static async Task<SQLiteAsyncConnection> CreateConnection<TTable>(string databasePath) where TTable : new()
        {
            SQLiteAsyncConnection result = null;

            if (!databasePath.Contains(":memory:"))
                AssertionConcern.AssertPathValid(databasePath, nameof(databasePath));

            result = new SQLiteAsyncConnection(databasePath);
            await result.CreateTableAsync<TTable>();
            return result;
        }
    }
}
