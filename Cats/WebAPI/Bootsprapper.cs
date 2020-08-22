using Application.Core;
using Application.Infrastructure;
using CommonServiceLocator;
using LMP.Data.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using Unity.ServiceLocation;

namespace WebAPI
{
    public static class Bootstrapper
    {
        public static IApplicationBuilder BuildServiceLocation(this IApplicationBuilder applicationBuilder)
        {
            var container = new UnityContainerWrapper(new UnityContainer());

            ApplicationServiceModule.Initialize(container);
            DataModule.Initialize(container);

            container.Container.RegisterInstance(typeof(IConfiguration),
                applicationBuilder.ApplicationServices.GetService<IConfiguration>());

            var locator = new UnityServiceLocator(container.Container);

            ServiceLocator.SetLocatorProvider(() => locator);

            return applicationBuilder;
        }
    }
}