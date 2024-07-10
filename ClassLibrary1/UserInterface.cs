using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ClassLibrary1
{
    /// <summary>
    /// Интерфейс взаимодействия с пользователем
    /// </summary>
    public class UserInterface
    {
        /// <summary>
        /// Выбор дейстыия с файлом
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public async static void ActionInterface(ITelegramBotClient botClient, Update update)
        {
            
            string text = "Выберите действие с файлом";
            var ikm = new InlineKeyboardMarkup(new[]
            {
                new[] {InlineKeyboardButton.WithCallbackData("Произвести выборку по AdmArea", ActionNames.admAreaSelect)},
                new[] {InlineKeyboardButton.WithCallbackData("Произвести выборку по geoarea", ActionNames.geoAreaSelect) },
                new[] {InlineKeyboardButton.WithCallbackData("Произвести выборку по District и geoarea", ActionNames.districtGeoAreaSelect) },
                new[] {InlineKeyboardButton.WithCallbackData("Осортировать по Name в прямом порядке", ActionNames.nameStraight)},
                new[] {InlineKeyboardButton.WithCallbackData("Осортировать по Name в обратном порядке", ActionNames.nameBackwards)},
                new[] {InlineKeyboardButton.WithCallbackData("Скачать обработанный файл в формате CSV", ActionNames.giveCSV)},
                new[] {InlineKeyboardButton.WithCallbackData("Скачать обработанный файл в формате JSON", ActionNames.giveJSON)},
            });
            var receiver = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery != null ? update.CallbackQuery.From.Id : throw new ArgumentNullException();
            var action = await botClient.SendTextMessageAsync(receiver, text, replyMarkup: ikm);
        }

        /// <summary>
        /// Обработчик действий пользователя
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public async static Task ActionHandler(ITelegramBotClient botClient, Update update)
        {
            var action = update.CallbackQuery.Data;
            string selectType;
            try
            {
                switch (action)
                {
                    case ActionNames.admAreaSelect:
                        await botClient.SendTextMessageAsync(update?.CallbackQuery?.From?.Id, "Введите значение для выборки: ");
                        //Далее действия происходят в update
                        break;
                    case ActionNames.geoAreaSelect:
                        await botClient.SendTextMessageAsync(update?.CallbackQuery?.From?.Id, "Введите значение для выборки: ");
                        //Далее действия происходят в update
                        break;
                    case ActionNames.districtGeoAreaSelect:
                        await botClient.SendTextMessageAsync(update?.CallbackQuery?.From?.Id, "Введите значение для выборки. " +
                                                            "Сначала district, потом geoarea через \";\". (Пример: disrict;geoarea): ");
                        //Далее действия происходят в update
                        break;
                    case ActionNames.nameStraight:
                        DataProcessor.SortByName();
                        await botClient.SendTextMessageAsync(update?.CallbackQuery?.From?.Id, "Данные успешно отсортированны.");
                        ActionInterface(botClient, update);
                        break;
                    case ActionNames.nameBackwards:
                        DataProcessor.SortByName(reverse: true);
                        await botClient.SendTextMessageAsync(update?.CallbackQuery?.From?.Id, "Данные успешно отсортированны.");
                        ActionInterface(botClient, update);
                        break;
                    case ActionNames.giveCSV:
                        DataProcessor.SendCsv(botClient, update);
                        //botClient.SendDocumentAsync(update.CallbackQuery.From.Id);
                        break;
                    case ActionNames.giveJSON:
                        DataProcessor.SendJson(botClient, update);
                        break;
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                await botClient.SendTextMessageAsync(update?.CallbackQuery?.From?.Id, $"Непредвиденная ошибка. {ex.Message}"); 
            }

        }
    }
}
