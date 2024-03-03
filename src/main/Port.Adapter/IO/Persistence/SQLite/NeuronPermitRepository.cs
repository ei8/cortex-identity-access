using ei8.Cortex.IdentityAccess.Application.Extensions;
using ei8.Cortex.IdentityAccess.Domain.Model;
using neurUL.Common.Domain.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.IO.Persistence.SQLite
{
    public class NeuronPermitRepository : INeuronPermitRepository
    {
        private readonly ISettingsService settingsService;
        private SQLiteAsyncConnection connection;

        public NeuronPermitRepository(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public async Task InsertAsync(NeuronPermit permit)
        {
            AssertionConcern.AssertArgumentValid(g => g.IsNotEmpty(), permit.UserNeuronId, Constants.Messages.Exception.InvalidUserId, nameof(permit.UserNeuronId));
            AssertionConcern.AssertArgumentValid(g => g.IsNotEmpty(), permit.NeuronId, Constants.Messages.Exception.InvalidId, nameof(permit.NeuronId));

            var isNewPermit = await this.connection.Table<NeuronPermit>()
                .FirstOrDefaultAsync(np => np.UserNeuronId == permit.UserNeuronId && np.NeuronId == permit.NeuronId) == null;
            AssertionConcern.AssertArgumentValid(
                (p) => isNewPermit,
                permit,
                Constants.Messages.Exception.NeuronPermitAlreadyExists,
                nameof(permit)
                );

            await this.connection.InsertAsync(permit);
        }

        public async Task<NeuronPermit> GetAsync(Guid userNeuronId, Guid neuronId)
        {
            AssertionConcern.AssertArgumentValid(g => g.IsNotEmpty(), userNeuronId, Constants.Messages.Exception.InvalidUserId, nameof(userNeuronId));
            AssertionConcern.AssertArgumentValid(g => g.IsNotEmpty(), neuronId, Constants.Messages.Exception.InvalidId, nameof(neuronId));

            return await this.connection.Table<NeuronPermit>()
                                        .FirstOrDefaultAsync(np => np.UserNeuronId == userNeuronId && 
                                                                   np.NeuronId == neuronId);
        }

        public async Task<IEnumerable<Guid>> GetNeuronIdsByUserNeuronIds(IEnumerable<Guid> userNeuronIds, IEnumerable<Guid> filterNeuronIds = null)
        {
            AssertionConcern.AssertArgumentValid(g => g != null && g.Any(), userNeuronIds, Constants.Messages.Exception.EnumerableNullOrEmpty, nameof(userNeuronIds));

            var userNeuronIdsString = $"'{string.Join("', '", userNeuronIds.Select(uni => uni.ToString()))}'";
            var filterNeuronIdsString = filterNeuronIds != null && filterNeuronIds.Any() ? 
                $" AND NeuronId IN ('{string.Join("', '", filterNeuronIds.Select(fni => fni.ToString()))}')" : 
                string.Empty;
            var query = $@"
		-- Query based on precisely the specified UserNeuronIds, ie. participants in the conversation
		SELECT NeuronId
		FROM NeuronPermit
		WHERE 
			-- UserNeuronIds
			UserNeuronId IN ({userNeuronIdsString}) 
			-- NeuronIds
			{ filterNeuronIdsString }
		GROUP BY NeuronId
		-- count of userNeuronIds
		HAVING count(NeuronId) == { userNeuronIds.Count() }";

            return (await this.connection.QueryAsync<NeuronIdData>(query)).Select(nid => Guid.Parse(nid.NeuronId));
        }

        public async Task UpdateAsync(NeuronPermit permit)
        {
            await this.connection.UpdateAsync(permit);
        }

        public async Task Initialize()
        {
            this.connection = await AuthorRepository.CreateConnection<User>(this.settingsService.UserDatabasePath);
        }

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
