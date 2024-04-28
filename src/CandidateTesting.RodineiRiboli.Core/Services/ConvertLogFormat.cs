using CandidateTesting.RodineiRiboli.Core.Interfaces;
using System.IO;

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

            //var uri = InputUri();
            (string command, string uri, string targetPath) = Input();

            if (!command.Equals("convert"))
            {
                Console.WriteLine("Intrução inválida");
            }


            //var targetPath = GetTargetPath();

            var response = await _consumeAwsS3.GetLogMinhaCdn(uri);

            string newFormattedText = ConvertToAgoraFormat(response);

            var filePath = WriteFile(newFormattedText, targetPath);

            if (filePath.Contains(targetPath))
            {
                Console.WriteLine($"\n\nO arquivo foi salvo em {Path.GetFullPath(filePath)}");
            }
            else
            {
                Console.WriteLine("\n\nNão foi possível localizar o caminho informado." +
                                    $" Contudo seu arquivo foi salvo em {Path.GetFullPath(filePath)}");
            }
        }

        private static (string command, string uri, string path) Input()
        {
            Console.WriteLine("\n\n * Informe o comando no seguinte formato \"comando URI caminho\", por exemplo:" +
                "\n  \"convert http://logstorage.com/minhaCdn1.txt ./output/minhaCdn1.txt\" e pressione \"ENTER\" para posseguir\n");

            var input = Console.ReadLine();

            do
            {
                input = NewMethod(input);
            } while (!input.Split(" ")[0].Equals("convert"));

            var command = input.Split(" ")[0].Trim();
            var uri = input.Split(" ")[1].Trim();
            var path = input.Split(" ")[2].Trim();

            return (command, uri, path);
        }

        private static string NewMethod(string? input)
        {
            while (string.IsNullOrWhiteSpace(input) || !input.Split(" ")[0].Equals("convert"))
            {
                Console.WriteLine("\n\n * É necessário informar um comando válido e pressionar \"ENTER\" para prosseguir.\n" +
                    "  Verifique a primeira instrução informada.\n");
                input = Console.ReadLine();
            }

            return input;
        }

        //private string[,,] InputUri2()
        //{
        //    string [command, uri, path] = ("", "", "");
        //    Console.WriteLine("\n\n * Informe a URI do arquivo de log a ser convertido e pressione \"ENTER\"");
        //    var uri = Console.ReadLine();

        //    while (string.IsNullOrWhiteSpace(uri))
        //    {
        //        Console.WriteLine("\n\n * É necessário informar a URI do arquivo de log a ser convertido e pressionar \"ENTER\" para prosseguir");
        //        uri = Console.ReadLine();
        //    }

        //    return uri.Trim();
        //}

        private static string WriteFile(string newFormattedText, string targetPath)
        {



            //string fullPath = Path.GetFullPath(targetPath, basePath);

            string createdPath = "";
            if (!Directory.Exists(targetPath))
            {
                var basePath = Environment.CurrentDirectory;
                if (!targetPath.StartsWith('/'))
                {
                    targetPath = $"./{targetPath}";
                }

                createdPath = Directory.CreateDirectory($"{basePath}{targetPath}").FullName;
            }


            //var created = Environment.GetFolderPath(targetPath);// Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)targetPath);



            string? logPath;

            if (Directory.Exists(createdPath))
            {
                logPath = targetPath;
            }
            else
            {
                logPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            var lines = newFormattedText.Split("\n");

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(logPath, "minhaCdn1.txt")))
            {
                foreach (string line in lines)
                    outputFile.WriteLine(line);
            }

            return logPath;
        }

        private static string ConvertToAgoraFormat(string response)
        {
            var line = response.Split('\n');
            var stringMount = HeaderMount();

            foreach (var itemLine in line)
            {
                if (!string.IsNullOrWhiteSpace(itemLine))
                {
                    var str = itemLine.Split('|');

                    stringMount += $"\n\"MINHA CDN\" {str[3].Split(" ")[0].Replace("\"", "")} {str[1]} {str[3].Split(" ")[1]} {str[4].Split(".")[0]} {str[0]} {str[2]}";
                }

            }

            stringMount = stringMount.Replace("INVALIDATE", "REFRESH_HIT");

            return stringMount;
        }

        private static string HeaderMount()
        {
            var date = DateTime.Now;

            return $"#Version: 1.0\r\n#Date: {date}\r\n#Fields: provider http-method status-code uri-path time-taken response-size cache-status";
        }

        private static string GetTargetPath()
        {
            Console.WriteLine("\n\n * Informe o caminho onde deseja que o arquivo de log seja salvo e pressione \"ENTER\"");
            var targetPath = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(targetPath))
            {
                Console.WriteLine("\n\n * É necessário informar o caminho onde deseja que o arquivo de log seja salvo e pressionar \"ENTER\" para prosseguir");
                Console.WriteLine("\n\n * Por exemplo ./MeusLogsConvertidos");
                targetPath = Console.ReadLine();
            }

            return targetPath.Trim();
        }

        private static string InputUri()
        {
            Console.WriteLine("\n\n * Informe a URI do arquivo de log a ser convertido e pressione \"ENTER\"");
            var uri = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(uri))
            {
                Console.WriteLine("\n\n * É necessário informar a URI do arquivo de log a ser convertido e pressionar \"ENTER\" para prosseguir");
                uri = Console.ReadLine();
            }

            return uri.Trim();
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
