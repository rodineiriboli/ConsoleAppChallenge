using CandidateTesting.RodineiRiboli.Core.Interfaces;
using CandidateTesting.RodineiRiboli.Core.Services;
using CandidateTesting.RodineiRiboli.Infrasctructure.ExternalApi;
using Microsoft.Extensions.DependencyInjection;

namespace CandidateTesting.RodineiRiboli.Infrastructure.IoC
{
    public static class DependencyResolve
    {
        public static ServiceProvider Configure()
        {
            return new ServiceCollection()
                .ConfigureServices()
                .BuildServiceProvider();
        }

        private static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IConvertLogFormat, ConvertLogFormat>()
                .AddScoped<IConsumeAwsS3, ConsumeAwsS3>()
                .AddHttpClient();
        }
    }
}
