using ei8.Cortex.IdentityAccess.Application;
using ei8.Cortex.IdentityAccess.Common;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.IdentityAccess.Port.Adapter.Out.Api
{
    public class AuthorModule : NancyModule
    {
        public AuthorModule(IAuthorApplicationService authorApplicationService) : base("/identityaccess/authors")
        {
            this.Get(string.Empty, async (parameters) =>
            {
                var result = new Response { StatusCode = HttpStatusCode.OK };

                if (this.Request.Query["userid"].HasValue)
                {
                    AuthorInfo author = await authorApplicationService.GetAuthorByUserId(this.Request.Query["userid"].ToString());
                    result = new TextResponse(JsonConvert.SerializeObject(author));
                }
                else
                    result = new TextResponse(HttpStatusCode.BadRequest, "UserId is invalid or missing.");

                return result;
            }
            );
        }
    }
}
