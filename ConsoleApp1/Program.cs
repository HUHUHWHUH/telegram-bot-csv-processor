using ClassLibrary1;
using Telegram.Bot;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text;

namespace ConsoleApp1
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Bot bot = new Bot(TokenGetter.GetToken(@"..\..\..\token.txt"));
                bot.SetLogging("..\\..\\..\\var.txt", true);
                bot.StartBot();
                Console.WriteLine("Чтобы выключить бота нажмите Enter.");
                Console.ReadLine();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //[LoggerMessage(Level = LogLevel.Information, Message = "Hello World! Logging is {Description}.")]
        //static partial void LogStartupMessage(ILogger logger, string description);
    }
}