using ei8.Cortex.Graph.Client;
using ei8.Cortex.Graph.Common;
using ei8.Cortex.IdentityAccess.Common;
using ei8.EventSourcing.Client.Out;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Domain.Model
{
    public class ValidationService : IValidationService
    {
        private readonly ISettingsService settingsService;
        private readonly INotificationClient notificationClient;
        private readonly INeuronGraphQueryClient neuronGraphQueryClient;
        private readonly IAuthorRepository authorRepository;
        private readonly INeuronPermitRepository neuronPermitRepository;

        public ValidationService(ISettingsService settingsService, INotificationClient notificationClient, INeuronGraphQueryClient neuronGraphQueryClient, IAuthorRepository authorRepository, INeuronPermitRepository neuronPermitRepository)
        {
            this.settingsService = settingsService;
            this.notificationClient = notificationClient;
            this.neuronGraphQueryClient = neuronGraphQueryClient;
            this.authorRepository = authorRepository;
            this.neuronPermitRepository = neuronPermitRepository;
        }

        public async Task<ActionValidationResult> CreateNeuron(Guid neuronId, Guid? neuronRegionId, string userId, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, neuronId, Constants.Messages.Exception.InvalidId, nameof(neuronId));

            var actionErrors = new List<ErrorInfo>();
            var neuronErrors = new List<ErrorInfo>();
            var regionTag = string.Empty;
            Author author = null;
            Guid userNeuronId = Guid.Empty;

            // if non-root region was specified, check if it exists
            if (neuronRegionId.HasValue)
            {
                // ensure that layer is a valid neuron
                var qs = (await this.neuronGraphQueryClient.GetNeuronById(
                    this.settingsService.CortexGraphOutBaseUrl + "/",
                    neuronRegionId.Value.ToString(),
                    new NeuronQuery() { NeuronActiveValues = ActiveValues.All },
                    token: token
                    ));
                if (qs == null || qs.Neurons == null || qs.Neurons.Count() == 0)
                    neuronErrors.Add(new ErrorInfo("Invalid region specified.", ErrorType.Error));
                else
                    regionTag = qs.Neurons.First().Tag;
            }
            else
                regionTag = "Base";

            // Ensure that Neuron Id is equal to AuthorId if first Neuron is being created
            if ((await this.notificationClient.GetNotificationLog(
                this.settingsService.EventSourcingOutBaseUrl + "/",
                string.Empty
                )).NotificationList.Count == 0)
            {
                AssertionConcern.AssertArgumentValid(g => g == string.Empty, userId, Constants.Messages.Exception.AnonymousUserExpected, nameof(userId));

                userNeuronId = neuronId;
            }
            // Ensure that Neuron Id is not equal to AuthorId if non-first Neuron is being created
            else
            {
                AssertionConcern.AssertArgumentNotEmpty(userId, Constants.Messages.Exception.InvalidUserId, nameof(userId));

                await this.authorRepository.Initialize();
                author = await this.authorRepository.GetByUserId(userId);

                if (author == null)
                    actionErrors.Add(new ErrorInfo(Constants.Messages.Exception.UnauthorizedUserAccess, ErrorType.Error));
                else
                {
                    userNeuronId = author.User.NeuronId;

                    // get write permit of author user for region
                    var permit = author.Permits.SingleOrDefault(p => p.RegionNeuronId == neuronRegionId && p.WriteLevel > 0);

                    // if author user doesn't have a write permit
                    if (permit == null)
                        neuronErrors.Add(new ErrorInfo(string.Format(Constants.Messages.Exception.UnauthorizedRegionWriteTemplate, regionTag), ErrorType.Error));
                }
            }

            return new ActionValidationResult(
                    userNeuronId,
                    actionErrors.ToArray(),
                    neuronErrors.Count > 0 ?
                        new NeuronValidationResult[] { new NeuronValidationResult(neuronId, neuronErrors.ToArray()) } :
                        new NeuronValidationResult[0]
                    );
        }

        public async Task<ActionValidationResult> UpdateNeuron(Guid neuronId, string userId, CancellationToken token = default)  
        {
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, neuronId, Constants.Messages.Exception.InvalidId, nameof(neuronId));
            AssertionConcern.AssertArgumentNotEmpty(userId, Constants.Messages.Exception.InvalidUserId, nameof(userId));

            var actionErrors = new List<ErrorInfo>();
            var neuronErrors = new List<ErrorInfo>();
            Author author = null;

            // get reference to neuron being modified
            var neuron = (await this.neuronGraphQueryClient.GetNeuronById(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                neuronId.ToString(),
                new NeuronQuery() { NeuronActiveValues = ActiveValues.All },
                token
                )).Neurons.FirstOrDefault();

            AssertionConcern.AssertArgumentValid(n => n != null, neuron, Constants.Messages.Exception.NeuronNotFound, nameof(neuronId));

            await this.authorRepository.Initialize();
            author = await this.authorRepository.GetByUserId(userId);

            if (author == null)
                actionErrors.Add(new ErrorInfo(Constants.Messages.Exception.UnauthorizedUserAccess, ErrorType.Error));
            else
            {
                var regionTag = neuron.Region.Id != null ? neuron.Region.Tag : "Base";
                // get write permit of author user for region
                var permit = author.Permits.SingleOrDefault(p =>
                    p.RegionNeuronId.EqualsString(neuron.Region.Id)
                    && p.WriteLevel > 0
                );

                // does author user have a write permit
                if (permit == null)
                    neuronErrors.Add(new ErrorInfo(string.Format(Constants.Messages.Exception.UnauthorizedRegionWriteTemplate, regionTag), ErrorType.Error));
                // does author user have an admin write access, or author user is the author of this neuron
                else if (!(permit.WriteLevel == 2 || neuron.Creation.Author.Id == author.User.NeuronId.ToString()))
                    neuronErrors.Add(new ErrorInfo(string.Format(Constants.Messages.Exception.UnauthorizedNeuronWriteTemplate, neuron.Tag), ErrorType.Error));
            }

            return new ActionValidationResult(
                    author != null ? author.User.NeuronId : Guid.Empty,
                    actionErrors.ToArray(),
                    neuronErrors.Count > 0 ?
                        new NeuronValidationResult[] { new NeuronValidationResult(neuronId, neuronErrors.ToArray()) } :
                        new NeuronValidationResult[0]
                    );
        }

        public async Task<ActionValidationResult> ReadNeurons(IEnumerable<Guid> neuronIds, string userId, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentNotNull(neuronIds, nameof(neuronIds));
            neuronIds.ToList().ForEach(g => AssertionConcern.AssertArgumentValid(
                guid => guid != Guid.Empty, g, Constants.Messages.Exception.InvalidId, nameof(neuronIds)
                ));
            AssertionConcern.AssertArgumentNotEmpty(userId, Constants.Messages.Exception.InvalidUserId, nameof(userId));

            var query = new NeuronQuery() {
                Id = neuronIds.Select(g => g.ToString()),
                NeuronActiveValues = ActiveValues.All,
                PageSize = neuronIds.Count()
            };

            // get neurons which are also inactive
            var queryResult = await this.neuronGraphQueryClient.GetNeurons(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                query,
                token
                );
            var actionErrors = new List<ErrorInfo>();
            var neuronResults = new List<NeuronValidationResult>();

            await this.authorRepository.Initialize();
            Author author = await this.authorRepository.GetByUserId(userId);

            if (author == null)
            {
                actionErrors.Add(new ErrorInfo(Constants.Messages.Exception.UnauthorizedUserAccess, ErrorType.Error));
            }
            else if (queryResult.Neurons.Count() > 0)
            {
                // loop through each neuron
                foreach (var neuron in queryResult.Neurons)
                {
                    var regionTag = neuron.Region.Id != null ? neuron.Region.Tag : "Base";
                    // get region permit of author user for region
                    var regionPermit = author.Permits.SingleOrDefault(p =>
                        p.RegionNeuronId.EqualsString(neuron.Region.Id)
                        && p.ReadLevel > 0
                    );

                    await this.neuronPermitRepository.Initialize();
                    var neuronPermit = await this.neuronPermitRepository.GetAsync(author.User.NeuronId, Guid.Parse(neuron.Id));

                    // does author user have a read permit
                    if (regionPermit == null)
                    {
                        if (neuronPermit == null)
                        {
                            neuronResults.Add(new NeuronValidationResult(Guid.Parse(neuron.Id), new ErrorInfo[]
                            {
                                new ErrorInfo(string.Format(Constants.Messages.Exception.UnauthorizedRegionReadTemplate, regionTag), ErrorType.Warning)
                            }));
                        }
                        else
                        {
                            if (!neuronPermit.ExpirationDate.HasValue)
                            {
                                neuronResults.Add(new NeuronValidationResult(Guid.Parse(neuron.Id), new ErrorInfo[]
                                {
                                    new ErrorInfo(Constants.Messages.Exception.NeuronPermitPendingOrDisabled, ErrorType.Warning)
                                }));
                            }
                            else if (DateTime.Now > neuronPermit.ExpirationDate.Value)
                            {
                                neuronResults.Add(new NeuronValidationResult(Guid.Parse(neuron.Id), new ErrorInfo[]
                                {
                                    new ErrorInfo(string.Format(Constants.Messages.Exception.ExpiredNeuronPermitTemplate, neuronPermit.ExpirationDate), ErrorType.Warning)
                                }));
                            }
                        }
                    }
                }
            }

            return new ActionValidationResult(
                    author != null ? author.User.NeuronId : Guid.Empty,
                    actionErrors.ToArray(),
                    neuronResults.ToArray()
                    );
        }
    }
}
