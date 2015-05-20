using System.Web.Http;
using System.Web.Http.Description;
using MediatR;

namespace MediatrAutofacReferenceApp.Web.Controllers
{
    public class PingController : ApiController
    {
        private readonly IMediator _mediator;
        public PingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("ping")]
        [ResponseType(typeof(string))]
        public IHttpActionResult Ping([FromUri] string message)
        {
            return Ok(_mediator.Send(new PingRequest {MessageBody = message}));
        }
    }
}
