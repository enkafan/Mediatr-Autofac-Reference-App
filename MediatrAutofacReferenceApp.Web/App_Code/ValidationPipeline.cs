using FluentValidation;
using MediatR;
using System.Diagnostics;
using System.Linq;

namespace MediatrAutofacReferenceApp.Web
{
    class ValidatorHandlerPipeline<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {

        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly IValidator<TRequest>[] _validators;

        public ValidatorHandlerPipeline(IRequestHandler<TRequest, TResponse> inner, IValidator<TRequest>[] validators)
        {
            _inner = inner;
            _validators = validators;
        }

        public TResponse Handle(TRequest request)
        {
            Debug.WriteLine("Validator Pipeline handled.");

            var context = new ValidationContext(request);

            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
                throw new ValidationException(failures);

            return _inner.Handle(request);
        }
    }
}