using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

//наггет пакеты для работы логов
//install-package Microsoft.Extensions.Logging
//install-package Microsoft.Extensions.Logging
//install-package Microsoft.Extensions.Logging.Console
//install-package Microsoft.Extensions.Logging.Debug

namespace ClassLibrary1
{
    /// <summary>
    /// Класс для логирования 
    /// </summary>
    public partial class Logger
    {
        string _logPath = "";
        bool _deletePreviousLogs;
        public Logger(string logPath, bool deletePreviousLogs = false)
        {
            _logPath = logPath;
            _deletePreviousLogs = deletePreviousLogs;
        }

        /// <summary>
        /// Метод для записи данных сообщений в логи
        /// </summary>
        /// <param name="message"></param>
        public void WriteUpdateLog(Update exception)
        {
            using (StreamWriter logFileWriter = new StreamWriter(_logPath, append: true))
            {
                //строка с данными о сообщении для вывода в логи
                string logString = "";
                if (exception.Message != null || exception.CallbackQuery != null)
                {
                    var message = exception.Message;
                    var callback = exception.CallbackQuery;
                    string action = callback != null ? "CallbackQuery" : message != null ? "Message": "";
                    logString = $"{DateTime.Now}: Action: {action}, From: {message?.Chat.Username ?? callback?.From?.Username} " +
                                $"[{message?.Chat?.Id ?? callback?.From?.Id}], Message: {message?.Text ?? message?.Caption}, " +
                                $"Document: {message?.Document?.FileName??""}, Callback: {exception?.CallbackQuery?.Data??""}";
                }

                ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                {
                    //Добавляем вывод в консоль
                    builder.AddConsole();
                    //Добавляем свой провайдер для записи в файлы
                    builder.AddProvider(new FileLoggerProvider(logFileWriter, logString));
                });
                
                //ILogger для вывода в консоль
                ILogger logger = loggerFactory.CreateLogger(Path.GetFileName(_logPath));
                // вывод в консоь
                using (logger.BeginScope(""))
                    logger.LogInformation(logString);
            }
        }

        /// <summary>
        /// Метод для записи исключений сообщений в логи
        /// </summary>
        /// <param name="exception"></param>
        public void WriteUpdateLog(Exception exception)
        {
            using (StreamWriter logFileWriter = new StreamWriter(_logPath, append: true))
            {
                //строка с данными о сообщении для вывода в логи
                string logString = "";
                if (exception != null)
                {
                    var message = exception.Message;
                    logString = $"{DateTime.Now}: Exception : {message}";
                }

                ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                {
                    //Добавляем вывод в консоль
                    builder.AddConsole();
                    //Добавляем свой провайдер для записи в файлы
                    builder.AddProvider(new FileLoggerProvider(logFileWriter, logString));
                });

                //ILogger для вывода в консоль
                ILogger logger = loggerFactory.CreateLogger(Path.GetFileName(_logPath));
                // вывод в консоь
                using (logger.BeginScope(""))
                    logger.LogInformation(logString);
            }
        }
    }
    
    /// <summary>
    /// Кастомный класс-наследник ILogger для записи в файл
    /// </summary>
    public class FileLogger : ILogger
    {
        private StreamWriter filePath;
        private string _logString;
        public FileLogger(StreamWriter path, string logString)
        {
            filePath = path;
            _logString = logString; 
        }
        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            using (StreamWriter sw = filePath)
                filePath.WriteLine(_logString);
        }
    }
    /// <summary>
    /// Кастомный логер провайдер для записи в файл
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {
        private StreamWriter path;
        private string _logString;
        public FileLoggerProvider(StreamWriter _path, string logString)
        {
            if(_path == null || logString == null)
                throw new ArgumentNullException();
            path = _path;
            _logString = logString;
        }
        public ILogger CreateLogger(string categoryName)=> new FileLogger(path, _logString);
        
        public void Dispose()
        {}
    }

}
