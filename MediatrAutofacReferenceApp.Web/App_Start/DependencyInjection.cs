using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Core;
using Autofac.Extras.CommonServiceLocator;
using Autofac.Features.Variance;
using Autofac.Integration.WebApi;
using FluentValidation;
using MediatR;

namespace MediatrAutofacReferenceApp.Web
{
    public class DependencyInjection
    {
        public static IContainer Configure(ContainerBuilder builder, HttpConfiguration config, Action<ContainerBuilder> extraContainerBuilder = null)
        {
            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterHttpRequestMessage(config);
            builder.RegisterWebApiFilterProvider(config);

            // register validators
            builder.RegisterAssemblyTypes(typeof(PingRequest.Validator).Assembly)
                .AsClosedTypesOf(typeof(IValidator<>))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            RegisterMediator(builder);

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return container;
        }

        private static void RegisterMediator(ContainerBuilder builder)
        {
            builder.RegisterType<Mediator>()
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();

            // wireup autofac for the common service locator
            builder.RegisterSource(new ContravariantRegistrationSource());
            builder.RegisterType<AutofacServiceLocator>().AsImplementedInterfaces();

            RegisterHandlers(
                builder: builder,
                handlerType: typeof(IRequestHandler<,>),
                assemblies: typeof(PingRequestHandler).Assembly,
                decorators: typeof(ValidatorHandlerPipeline<,>)
                );
        }

        private static void RegisterHandlers(ContainerBuilder builder, Type handlerType, Assembly assemblies, params Type[] decorators)
        {
            RegisterHandlers(builder, handlerType, assemblies);

            for (var i = 0; i < decorators.Length; i++)
            {
                RegisterGenericDecorator(
                    builder,
                    decorators[i],
                    handlerType,
                    i == 0 ? handlerType : decorators[i - 1],
                    i != decorators.Length - 1);
            }
        }

        private static void RegisterHandlers(ContainerBuilder builder, Type handlerType, Assembly assemblies)
        {
            builder.RegisterAssemblyTypes(assemblies)
                .As(t => t.GetInterfaces()
                        .Where(v => v.IsClosedTypeOf(handlerType))
                        .Select(v => new KeyedService(handlerType.Name, v)))
                .InstancePerRequest();
        }

        private static void RegisterGenericDecorator(
            ContainerBuilder builder,
            Type decoratorType,
            Type decoratedServiceType,
            Type fromKeyType,
            bool hasKey)
        {
            var result = builder.RegisterGenericDecorator(
               decoratorType,
               decoratedServiceType,
               fromKeyType.Name);

            if (hasKey)
            {
                result.Keyed(decoratorType.Name, decoratedServiceType);
            }
        }
    }
}