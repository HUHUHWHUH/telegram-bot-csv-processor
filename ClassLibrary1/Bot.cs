using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ClassLibrary1
{
    /// <summary>
    /// Телеграмм бот
    /// </summary>
    public class  Bot
    {
        string _token;
        TelegramBotClient _botClient;
        public Bot(string token) =>_token = token;
        Logger _logger;
        bool _receiveAfterCallbackQuery = false;
        string action;
        bool DataExists = false;

        /// <summary>
        /// Установка логов, если требуются
        /// </summary>
        /// <param name="logPath"></param>
        /// <param name="deletePreviousLogs"></param>
        public void SetLogging(string logPath = null, bool deletePreviousLogs = false)
        {
            if (deletePreviousLogs && System.IO.File.Exists(logPath))
                System.IO.File.Delete(logPath);

            if (logPath != null)
                _logger = new Logger(logPath, deletePreviousLogs);
            else
                _logger = null;
        }

        /// <summary>
        /// Запуск бота
        /// </summary>
        public async void StartBot()
        {
            _botClient = new TelegramBotClient(_token);
            _botClient.StartReceiving(Update, Error, null);
        }

        /// <summary>
        /// Хэндлер ошибки
        /// </summary>
        /// <param name="client"></param>
        /// <param name="exception"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task Error(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            try
            {
                throw exception;
            }
            catch (Exception ex)
            {
                if (_logger != null)
                    _logger.WriteUpdateLog(ex);
                return;
            }
           
        }

        /// <summary>
        /// Обработчик поступившего сообщения
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            try
            {
                var message = update.Message;
                //Проверяем нужно ли вести логи
                if (_logger != null)
                    _logger.WriteUpdateLog(update);
                switch (update.Type)
                {
                    case UpdateType.Message:
                        //Попадаем сюда после того, как пользователь ввел поле для выборки
                        if (_receiveAfterCallbackQuery && message.Text != null)
                        {
                            _receiveAfterCallbackQuery = false;
                            if (update.Message == null)
                                return;
                            DataProcessor.FieldSelect(action, message?.Text);
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Данные успешно отфильтрованы");
                            UserInterface.ActionInterface(botClient, update);
                            break;
                        }
                        else if(_receiveAfterCallbackQuery && message.Text == null)
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Не удалось отфильтровать данные. Повторите ввод:");
                            break;
                        }

                        if (message.Document != null)
                        {
                            //Проверяем расширение файла
                            if (Path.GetExtension(message.Document.FileName) != ".csv" && Path.GetExtension(message.Document.FileName).ToLower() != ".json")
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Файл должен иметь расширение \".csv\" или \".json\".");
                                return;
                            }
                            //Скачиваем файл
                            string path = $"..\\..\\..\\{message.Document.FileName}";
                            await using FileStream fs = System.IO.File.OpenWrite(path);
                            await botClient.DownloadFileAsync((await botClient.GetFileAsync(update.Message.Document.FileId)).FilePath, fs);
                            fs.Dispose();
                            //ставим флаг, что данные считаны 
                            DataExists = DataProcessor.MainProcessor(path, botClient, update, _logger);

                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Пришлите .csv или .json файл для обработки.");
                            return;
                        }
                        break;
                    case UpdateType.CallbackQuery:
                        if (_receiveAfterCallbackQuery)
                            _receiveAfterCallbackQuery = false;
                        //Если нет считанных данных то нечего обрабатывать
                        if (!DataExists)
                        {
                            //await botClient.SendTextMessageAsync(message?.Chat?.Id, "Пришлите .csv или .json файл для обработки.");
                            return;
                        }

                        action = update?.CallbackQuery?.Data;
                        //Обрабатывать сообщение нужно только там, где результат действия зависит от данных, введенных пользователем
                        if (action == ActionNames.admAreaSelect || action == ActionNames.geoAreaSelect || action == ActionNames.districtGeoAreaSelect)
                            _receiveAfterCallbackQuery = true;
                        UserInterface.ActionHandler(botClient, update);

                        break;
                    default:
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Пришлите .csv или .json файл для обработки.");
                        break;
                }
            }catch(Exception ex)
            {
                if (_logger != null)
                    _logger.WriteUpdateLog(update);
                Console.WriteLine("Непредвиденное исключени: " + ex.ToString());
            }
        }
    }
}
