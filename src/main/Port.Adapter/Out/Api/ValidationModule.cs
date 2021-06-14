using ei8.Cortex.IdentityAccess.Application;
using ei8.Cortex.IdentityAccess.Common;
using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Nancy.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.Out.Api
{
    public class ValidationModule : NancyModule
    {
        public ValidationModule(IValidationApplicationService validationApplicationService) : base("/identityaccess/validations")
        {
            // based on ei8.Cortex.Diary.Nucleus.Port.Adapter.In.Api.Helper
            // TODO: centralize with ei8.Cortex.Diary.Nucleus.Port.Adapter.In.Api.Helper
            this.Post("/createneuron", async (parameters) =>
            {
                var result = new Response { StatusCode = HttpStatusCode.OK };
                string[] requiredFields = new string[] { "NeuronId", "UserId" };

                dynamic bodyAsObject = null;
                Dictionary<string, object> bodyAsDictionary = null;
                var jsonString = RequestStream.FromStream(this.Request.Body).AsString();
                string[] missingFields = null;

                if (!string.IsNullOrEmpty(jsonString))
                {
                    bodyAsDictionary = JObject.Parse(jsonString).ToObject<Dictionary<string, object>>();
                    missingFields = requiredFields.Where(s => !bodyAsDictionary.ContainsKey(s)).ToArray();
                }
                else
                    missingFields = requiredFields;

                try
                {
                    if (missingFields.Length == 0)
                    {
                        bodyAsObject = JsonConvert.DeserializeObject(jsonString);
                        Guid? regionId = null;
                        
                        if (bodyAsDictionary.ContainsKey("RegionId"))
                            if (Guid.TryParse(bodyAsObject.RegionId.ToString(), out Guid tempRegionId))
                                regionId = tempRegionId;

                        ActionValidationResult validationResult = await validationApplicationService.CreateNeuron(
                            System.Guid.Parse(bodyAsObject.NeuronId.ToString()),
                            regionId,
                            bodyAsObject.UserId.ToString()
                            );

                        result = ValidationModule.CreateResponse(validationResult);
                    }
                    else
                    {
                        result = new TextResponse(
                            HttpStatusCode.BadRequest,
                            $"Required field(s) '{ string.Join("', '", missingFields) }' not found."
                        );
                    }
                }
                catch (Exception ex)
                {
                    result = new TextResponse(HttpStatusCode.InternalServerError, ex.ToString());
                }
                return result;
            }
            );

            this.Post("/updateneuron", async (parameters) =>
            {
                var result = new Response { StatusCode = HttpStatusCode.OK };
                string[] requiredFields = new string[] { "NeuronId", "UserId" };

                dynamic bodyAsObject = null;
                Dictionary<string, object> bodyAsDictionary = null;
                var jsonString = RequestStream.FromStream(this.Request.Body).AsString();
                string[] missingFields = null;

                if (!string.IsNullOrEmpty(jsonString))
                {
                    bodyAsDictionary = JObject.Parse(jsonString).ToObject<Dictionary<string, object>>();
                    missingFields = requiredFields.Where(s => !bodyAsDictionary.ContainsKey(s)).ToArray();
                }
                else
                    missingFields = requiredFields;

                try
                {
                    if (missingFields.Length == 0)
                    {
                        bodyAsObject = JsonConvert.DeserializeObject(jsonString);
                        ActionValidationResult validationResult = await validationApplicationService.UpdateNeuron(
                            Guid.Parse(bodyAsObject.NeuronId.ToString()),
                            bodyAsObject.UserId.ToString()
                            );

                        result = ValidationModule.CreateResponse(validationResult);
                    }
                    else
                    {
                        result = new TextResponse(
                            HttpStatusCode.BadRequest,
                            $"Required field(s) '{ string.Join("', '", missingFields) }' not found."
                        );
                    }
                }
                catch (Exception ex)
                {
                    result = new TextResponse(HttpStatusCode.InternalServerError, ex.ToString());
                }
                return result;
            }
            );

            this.Post("/readneurons", async (parameters) =>
            {
                var result = new Response { StatusCode = HttpStatusCode.OK };
                string[] requiredFields = new string[] { "NeuronIds", "UserId" };

                Dictionary<string, object> bodyAsDictionary = null;
                var jsonString = RequestStream.FromStream(this.Request.Body).AsString();
                string[] missingFields = null;

                var definition = new { NeuronIds = new string[] { }, UserId = string.Empty };
                if (!string.IsNullOrEmpty(jsonString))
                {
                    bodyAsDictionary = JObject.Parse(jsonString).ToObject<Dictionary<string, object>>();
                    missingFields = requiredFields.Where(s => !bodyAsDictionary.ContainsKey(s)).ToArray();
                }
                else
                    missingFields = requiredFields;

                try
                {
                    if (missingFields.Length == 0)
                    {
                        var bodyAsObject = JsonConvert.DeserializeAnonymousType(jsonString, definition);
                        ActionValidationResult validationResult = await validationApplicationService.ReadNeurons(
                            bodyAsObject.NeuronIds.Select(ni => Guid.Parse(ni)),
                            bodyAsObject.UserId.ToString()
                            );

                        result = ValidationModule.CreateResponse(validationResult);
                    }
                    else
                    {
                        result = new TextResponse(
                            HttpStatusCode.BadRequest,
                            $"Required field(s) '{ string.Join("', '", missingFields) }' not found."
                        );
                    }
                }
                catch (Exception ex)
                {
                    result = new TextResponse(HttpStatusCode.InternalServerError, ex.ToString());
                }
                return result;
            }
            );
        }

        private static TextResponse CreateResponse(ActionValidationResult validationResult)
        {
            return new TextResponse(
                validationResult.HasErrors ?
                    HttpStatusCode.BadRequest :
                    HttpStatusCode.OK,
                JsonConvert.SerializeObject(validationResult)
                );
        }
    }
}
