using CandidateTesting.RodineiRiboli.Core.Interfaces;
using CandidateTesting.RodineiRiboli.Infrastructure.IoC;
using Microsoft.Extensions.DependencyInjection;


namespace CandidateTesting.RodineiRiboli.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var serviceProvider = DependencyResolve.Configure();
            var convert = serviceProvider.GetRequiredService<IConvertLogFormat>();

            await convert.ConvertLogs();
        }
    }
}
