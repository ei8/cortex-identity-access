using ei8.Cortex.IdentityAccess.Application;
using Nancy;
using neurUL.Common.Api;
using System;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.In.Api
{
    public class PermitModule : NancyModule
    {
        internal static readonly Func<Exception, HttpStatusCode> ConcurrencyExceptionSetter = new Func<Exception, HttpStatusCode>((ex) => {
            return HttpStatusCode.InternalServerError;
        });

        public PermitModule(IPermitApplicationService permitApplicationService) : base("/identityaccess/permits")
        {
            this.Post("/neurons", async (parameters) =>
            {
                return await this.Request.ProcessCommand(
                           false,
                           async (bodyAsObject, bodyAsDictionary, expectedVersion) =>
                               await permitApplicationService.CreateNeuronPermitAsync(
                                   Guid.Parse(bodyAsObject.NeuronId.ToString()), 
                                   Guid.Parse(bodyAsObject.UserNeuronId.ToString()),
                                   bodyAsDictionary.ContainsKey("ExpirationDate") && bodyAsObject.ExpirationDate != null ? 
                                        DateTime.Parse(bodyAsObject.ExpirationDate.ToString()) : null
                                   ),
                           PermitModule.ConcurrencyExceptionSetter,
                           new string[0],
                           "NeuronId", 
                           "UserNeuronId"
                       );
            });
        }
    }
}
