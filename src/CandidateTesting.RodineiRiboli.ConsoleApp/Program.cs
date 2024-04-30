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

            try
            {
                await convert.ConvertLogs();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A aplicação foi finalizada com o erro: {ex.Message}");
            }
            Console.WriteLine("\n\nPressione alguma tecla para sair");
            Console.ReadKey();
        }
    }
}
