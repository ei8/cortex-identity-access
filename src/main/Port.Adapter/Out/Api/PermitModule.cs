using ei8.Cortex.IdentityAccess.Application;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.Out.Api
{
    public class PermitModule : NancyModule
    {
        public PermitModule(IPermitApplicationService permitApplicationService) : base("/identityaccess/permits")
        {
            this.Get("/neuronids", async (parameters) =>
            {
                var result = new Response { StatusCode = HttpStatusCode.OK };

                if (this.Request.Query["userneuronid"].HasValue)
                {
                    string userNeuronIdsString = this.Request.Query.@userneuronid.ToString();
                    var userNeuronIds = userNeuronIdsString.Split(',').Select(uni => Guid.Parse(uni));
                    IEnumerable<Guid> filterNeuronIds = null;

                    if (this.Request.Query["neuronid"].HasValue)
                    {
                        string filterNeuronIdsString = this.Request.Query.@neuronid.ToString();
                        filterNeuronIds = filterNeuronIdsString.Split(',').Select(fni => Guid.Parse(fni)); ;
                    }

                    var neuronIds = await permitApplicationService.GetNeuronIdsByUserNeuronIds(userNeuronIds, filterNeuronIds);
                    result = new TextResponse(JsonConvert.SerializeObject(neuronIds));
                }
                else
                    result = new TextResponse(HttpStatusCode.BadRequest, "'UserNeuronId' is invalid or missing.");

                return result;
            }
            );
        }
    }
}
