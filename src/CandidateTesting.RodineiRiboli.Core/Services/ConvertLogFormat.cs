using CandidateTesting.RodineiRiboli.Core.Enums;
using CandidateTesting.RodineiRiboli.Core.Interfaces;
using System.Globalization;

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
            try
            {
                HeaderConsoleMount();

                (var uri, var targetPath) = Input();

                var response = await _consumeAwsS3.GetLogMinhaCdn(uri);

                var newFormattedText = ConvertToAgoraFormat(response);

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
            catch (Exception)
            {
                throw;
            }
        }

        public (string uri, string path) Input()
        {
            Console.WriteLine("\n\n * Informe o comando no seguinte formato \"comando URI caminho\", por exemplo:" +
                "\n  \"convert http://logstorage.com/minhaCdn1.txt ./output/minhaCdn1.txt\" e pressione \"ENTER\" para posseguir\n");

            string input = Console.ReadLine() ?? "";

            do
            {
                input = CheckEntry(input);
            } while (!input.Split(" ")[0].Equals(Commands.Convert));

            var uri = input.Split(" ")[1].Trim();
            var path = input.Split(" ")[2].Trim();

            return (uri, path);
        }

        public string CheckEntry(string input)
        {
            var inputSplitted = input.Split(" ");

            while (string.IsNullOrWhiteSpace(input)
                || inputSplitted.Length != 3
                || !inputSplitted[0].Equals(Commands.Convert)
                || string.IsNullOrWhiteSpace(inputSplitted[1])
                || string.IsNullOrWhiteSpace(inputSplitted[2]))
            {
                Console.WriteLine("\n\n * É necessário informar um comando válido e pressionar \"ENTER\" para prosseguir.\n" +
                    "   Verifique a primeira instrução informada.\n");
                input = Console.ReadLine() ?? "";

                inputSplitted = input.Split(" ");
            }

            return input;
        }

        private static string WriteFile(string newFormattedText, string targetPath)
        {
            try
            {
                SplitTargetPathFile(targetPath, out string fileName, out string path);

                if (!path.StartsWith("./"))
                {
                    path = $"./{path}";
                }

                if (!Directory.Exists(path))
                {
                    try
                    {
                        var basePath = Environment.CurrentDirectory;

                        Directory.CreateDirectory($"{basePath}{path}");
                    }
                    catch (Exception)
                    {
                        path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    }
                }

                var lines = newFormattedText.Split("\n");

                using (var outputFile = new StreamWriter(Path.Combine(path, fileName)))
                {
                    foreach (string line in lines)
                        outputFile.WriteLine(line);
                }
                return $"{path}/{fileName}";
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void SplitTargetPathFile(string targetPath, out string fileName, out string path)
        {
            try
            {
                if (!targetPath.Contains(".txt"))
                {
                    fileName = "minhaCdn1.txt";
                    path = targetPath.EndsWith('/') ? targetPath[..^1] : targetPath;
                    return;
                }

                var logPathSplitted = targetPath.Split("/");
                fileName = logPathSplitted.Last();

                var leng = targetPath.Replace(fileName, "").Length - 1;
                var pathLength = leng == -1 ? 0 : leng;
                path = targetPath[..pathLength];
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ConvertToAgoraFormat(string response)
        {
            try
            {
                var headerMount = HeaderFileMount();
                var body = BodyFileMount(response);

                var stringFile = string.Concat(headerMount, body);

                return stringFile;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string BodyFileMount(string response)
        {
            var ci = CultureInfo.GetCultureInfo("en-US");
            var line = response.Split('\n');
            var stringMount = "";
            foreach (var itemLine in line)
            {
                if (!string.IsNullOrWhiteSpace(itemLine))
                {
                    var str = itemLine.Split('|');

                    stringMount += $"\n\"MINHA CDN\" {str[3].Split(" ")[0].Replace("\"", "")} {str[1]} {str[3].Split(" ")[1]} {Convert.ToInt32(Convert.ToDecimal(str[4], ci))} {str[0]} {str[2]}";
                }

            }

            stringMount = stringMount.Replace("INVALIDATE", "REFRESH_HIT");

            return stringMount;
        }

        private static string HeaderFileMount()
        {
            var date = DateTime.Now;

            return $"#Version: 1.0\n#Date: {date}\n#Fields: provider http-method status-code uri-path time-taken response-size cache-status";
        }

        private static void HeaderConsoleMount()
        {
            Console.WriteLine("_____________________________________________________________________");
            Console.WriteLine("\n\t\tWelcome to the iTaaS Solution Convert");
            Console.WriteLine("_____________________________________________________________________");
            Console.WriteLine("\n\tConversão do formato \"MINHA CDN\" para formato \"Agora\"");
            Console.WriteLine("---------------------------------------------------------------------");
        }
    }
}
