using CandidateTesting.RodineiRiboli.Core.Interfaces;

namespace CandidateTesting.RodineiRiboli.Core.Services
{
    public class ConvertLogFormat : IConvertLogFormat
    {
        private readonly IConsumeAwsS3 _consumeAwsS3;

        public ConvertLogFormat(IConsumeAwsS3 consumeAwsS3)
        {
            _consumeAwsS3 = consumeAwsS3;
        }

        public async Task ConvertLogs()
        {
            HeadMount();
            
            var uri = InputUri();
            var response = await _consumeAwsS3.GetLogMinhaCdn(uri);

            Console.Write(response);
        }

        private static string InputUri()
        {
            Console.WriteLine("\n\n * Informe a URI do arquivo de log a ser convertido e precione \"ENTER\"");
            var uri = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(uri))
            {
                Console.WriteLine("\n\n * É necessário Informa a URI do arquivo de log a ser convertido e precionar \"ENTER\" para proceguir");
                uri = Console.ReadLine();
            }

            return uri;
        }

        private static void HeadMount()
        {
            Console.WriteLine("_____________________________________________________________________");
            Console.WriteLine("\n\t\tWelcome to the iTaaS Solution Convert");
            Console.WriteLine("_____________________________________________________________________");
            Console.WriteLine("\n\tConversão do formato \"MINHA CDN\" para formato \"Agora\"");
            Console.WriteLine("---------------------------------------------------------------------");
        }
    }
}
