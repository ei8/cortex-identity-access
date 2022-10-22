using CQRSlite.Commands;
using CQRSlite.Domain.Exception;
using ei8.Cortex.IdentityAccess.Application.AccessRequest.Commands;
using Nancy;
using neurUL.Common.Api;
using System;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.In.Api
{
    public class AccessRequestModule : NancyModule
    {
        internal static readonly Func<Exception, HttpStatusCode> ConcurrencyExceptionSetter = new Func<Exception, HttpStatusCode>((ex) => {
            if (ex is ConcurrencyException)
                return HttpStatusCode.Conflict;

            return HttpStatusCode.InternalServerError;
        });

        public AccessRequestModule(ICommandSender commandSender) : base("accessRequest")
        {
            this.Post("/neuron/{neuronId}", async (parameters) =>
            {
                return await this.Request.ProcessCommand(
                           false,
                           async (bodyAsObject, bodyAsDictionary, expectedVersion) =>
                           {
                               await commandSender.Send(new CreateNeuronRequestAccess(
                                   Guid.Parse(parameters.neuronId),
                                   Guid.Parse(bodyAsObject.UserId.ToString())
                                   )
                               );
                           },
                           ConcurrencyExceptionSetter,
                           new string[0],
                           new string[] { "UserId" }
                       );
            });
        }
    }
}
