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
    public class AuthorModule : NancyModule
    {
        public AuthorModule(IAuthorApplicationService authorApplicationService) : base("/identityaccess/authors")
        {
            this.Get(string.Empty, async (parameters) =>
            {
                var result = new Response { StatusCode = HttpStatusCode.OK };
                var subjectId = Guid.Empty;

                if (this.Request.Query["subjectid"].HasValue && Guid.TryParse(this.Request.Query["subjectid"].ToString(), out subjectId))
                {
                    var author = await authorApplicationService.GetAuthorBySubjectId(subjectId);
                    result = new TextResponse(JsonConvert.SerializeObject(author));
                }
                else
                    result = new TextResponse(HttpStatusCode.BadRequest, "SubjectId was invalid or not specified.");

                return result;
            }
            );
        }
    }
}
