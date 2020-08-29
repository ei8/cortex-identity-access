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

        public ValidationService(ISettingsService settingsService, INotificationClient notificationClient, INeuronGraphQueryClient neuronGraphQueryClient, IAuthorRepository authorRepository)
        {
            this.settingsService = settingsService;
            this.notificationClient = notificationClient;
            this.neuronGraphQueryClient = neuronGraphQueryClient;
            this.authorRepository = authorRepository;
        }

        public async Task<ActionValidationResult> CreateNeuron(Guid neuronId, Guid neuronRegionId, Guid subjectId, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, neuronId, Constants.Messages.Exception.InvalidId, nameof(neuronId));

            var actionErrors = new List<ErrorInfo>();
            var neuronErrors = new List<ErrorInfo>();
            var regionTag = string.Empty;
            Author author = null;
            Guid userNeuronId = Guid.Empty;

            // if non-root region was specified, check if it exists
            if (neuronRegionId != Guid.Empty)
            {
                // ensure that layer is a valid neuron
                var region = (await this.neuronGraphQueryClient.GetNeuronById(
                    this.settingsService.CortexGraphOutBaseUrl + "/",
                    neuronRegionId.ToString(),
                    new NeuronQuery() { NeuronActiveValues = ActiveValues.All },
                    token: token
                    ));
                if (region == null)
                    neuronErrors.Add(new ErrorInfo("Invalid region specified.", ErrorType.Error));
                else
                    regionTag = region.Tag;
            }
            else
                regionTag = "Base";

            // Ensure that Neuron Id is equal to AuthorId if first Neuron is being created
            if ((await this.notificationClient.GetNotificationLog(
                this.settingsService.EventSourcingOutBaseUrl + "/",
                string.Empty
                )).NotificationList.Count == 0)
            {
                AssertionConcern.AssertArgumentValid(g => g == Guid.Empty, subjectId, Constants.Messages.Exception.AnonymousUserExpected, nameof(subjectId));

                userNeuronId = neuronId;
            }
            // Ensure that Neuron Id is not equal to AuthorId if non-first Neuron is being created
            else
            {
                AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, subjectId, Constants.Messages.Exception.InvalidId, nameof(subjectId));

                await this.authorRepository.Initialize();
                author = await this.authorRepository.GetBySubjectId(subjectId);

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

        public async Task<ActionValidationResult> UpdateNeuron(Guid neuronId, Guid subjectId, CancellationToken token = default)  
        {
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, neuronId, Constants.Messages.Exception.InvalidId, nameof(neuronId));
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, subjectId, Constants.Messages.Exception.InvalidId, nameof(subjectId));

            var actionErrors = new List<ErrorInfo>();
            var neuronErrors = new List<ErrorInfo>();
            Author author = null;

            // get reference to neuron being modified
            var neuron = (await this.neuronGraphQueryClient.GetNeuronById(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                neuronId.ToString(),
                new NeuronQuery() { NeuronActiveValues = ActiveValues.All },
                token
                ));

            await this.authorRepository.Initialize();
            author = await this.authorRepository.GetBySubjectId(subjectId);

            if (author == null)
                actionErrors.Add(new ErrorInfo(Constants.Messages.Exception.UnauthorizedUserAccess, ErrorType.Error));
            else
            {
                var regionTag = neuron.RegionId != null ? neuron.RegionTag : "Base";
                // get write permit of author user for region
                var permit = author.Permits.SingleOrDefault(p => 
                    (
                        p.RegionNeuronId.ToString() == neuron.RegionId ||
                        (
                            neuron.RegionId == null &&
                            p.RegionNeuronId == Guid.Empty
                        )
                    )
                    && p.WriteLevel > 0
                );

                // does author user have a write permit
                if (permit == null)
                    neuronErrors.Add(new ErrorInfo(string.Format(Constants.Messages.Exception.UnauthorizedRegionWriteTemplate, regionTag), ErrorType.Error));
                // does author user have an admin write access, or author user is the author of this neuron
                else if (!(permit.WriteLevel == 2 || neuron.AuthorId == author.User.NeuronId.ToString()))
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

        public async Task<ActionValidationResult> ReadNeurons(IEnumerable<Guid> neuronIds, Guid subjectId, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentNotNull(neuronIds, nameof(neuronIds));
            neuronIds.ToList().ForEach(g => AssertionConcern.AssertArgumentValid(
                guid => guid != Guid.Empty, g, Constants.Messages.Exception.InvalidId, nameof(neuronIds)
                ));
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, subjectId, Constants.Messages.Exception.InvalidId, nameof(subjectId));

            var query = new NeuronQuery() {
                Id = neuronIds.Select(g => g.ToString()), 
                NeuronActiveValues = ActiveValues.All
            };

            // get neurons which are also inactive
            var neurons = await this.neuronGraphQueryClient.GetNeurons(
                this.settingsService.CortexGraphOutBaseUrl + "/",
                query,
                token
                );
            var actionErrors = new List<ErrorInfo>();
            var neuronResults = new List<NeuronValidationResult>();

            await this.authorRepository.Initialize();
            Author author = await this.authorRepository.GetBySubjectId(subjectId);

            if (author == null)
            {
                actionErrors.Add(new ErrorInfo(Constants.Messages.Exception.UnauthorizedUserAccess, ErrorType.Error));
            }
            else if (neurons.Count() > 0)
            {
                // loop through each neuron
                foreach (var neuron in neurons)
                {
                    var regionTag = neuron.RegionId != null ? neuron.RegionTag : "Base";
                    // get region permit of author user for region
                    var permit = author.Permits.SingleOrDefault(p =>
                        (
                            p.RegionNeuronId.ToString() == neuron.RegionId ||
                            (
                                neuron.RegionId == null &&
                                p.RegionNeuronId == Guid.Empty
                            )
                        )
                        && p.ReadLevel > 0
                    );

                    // does author user have a write permit
                    if (permit == null)
                    {
                        neuronResults.Add(new NeuronValidationResult(Guid.Parse(neuron.Id), new ErrorInfo[]
                        {
                            new ErrorInfo(string.Format(Constants.Messages.Exception.UnauthorizedRegionReadTemplate, regionTag), ErrorType.Warning)
                        }));
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
