using FluentValidation;
using MediatR;

namespace MediatrAutofacReferenceApp.Web
{
    public class PingRequest : IRequest<string>
    {
        public string MessageBody { get; set; }

        public class Validator : AbstractValidator<PingRequest>
        {
            public Validator()
            {
                RuleFor(v => v.MessageBody).NotEmpty();
            }
        }
    }

    public class PingRequestHandler : IRequestHandler<PingRequest, string>
    {
        public string Handle(PingRequest message)
        {
            return "pong " + message.MessageBody;
        }
    }
}