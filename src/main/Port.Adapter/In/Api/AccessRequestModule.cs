using ei8.Cortex.IdentityAccess.Application;
using Nancy;
using neurUL.Common.Api;
using System;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.In.Api
{
    public class AccessRequestModule : NancyModule
    {
        internal static readonly Func<Exception, HttpStatusCode> ConcurrencyExceptionSetter = new Func<Exception, HttpStatusCode>((ex) => {
            return HttpStatusCode.InternalServerError;
        });

        public AccessRequestModule(IPermitApplicationService permitApplicationService) : base("accessRequest")
        {
            this.Post("/neuron/{neuronId}", async (parameters) =>
            {
                return await this.Request.ProcessCommand(
                           false,
                           async (bodyAsObject, bodyAsDictionary, expectedVersion) =>
                           {
                               await permitApplicationService.CreateRequestAsync(parameters.neuronId, Guid.Parse(bodyAsObject.UserId.ToString()));
                           },
                           ConcurrencyExceptionSetter,
                           new string[0],
                           new string[] { "UserId" }
                       );
            });
        }
    }
}
