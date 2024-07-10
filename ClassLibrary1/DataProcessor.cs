using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ClassLibrary1
{
    /// <summary>
    /// Обработчие данных
    /// </summary>
    public class DataProcessor
    {
        /// <summary>
        /// Коллекция с информацией иб атракционах
        /// </summary>
        static List<AttractionType> _atts;
        
        /// <summary>
        /// Основной обработчик данных
        /// </summary>
        /// <param name="path"></param>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool MainProcessor(string path, ITelegramBotClient botClient, Update update, Logger logger)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                    if (Path.GetExtension(path) == ".csv")
                        _atts = CsvProcessor.Read(sr);
                    else if (Path.GetExtension(path) == ".json")
                        _atts = JsonProcessor.Read((sr));
                    else return false;
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                logger.WriteUpdateLog(ex);
                var receiver = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery != null ? update.CallbackQuery.From.Id : throw new ArgumentNullException();
                botClient.SendTextMessageAsync(receiver, "Неверная структура файла. Скиньте правильный.");
                return false;
            }

            UserInterface.ActionInterface(botClient, update);
            return true;
            //Console.WriteLine(JsonSerializer.Serialize(h, options));
        }

        /// <summary>
        /// Метод фильтрации по полям
        /// </summary>
        /// <param name="fieldAction"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void FieldSelect(string fieldAction, string value)
        {
            if (fieldAction == null || value == null)
            {
                throw new ArgumentNullException();
            }

            if (fieldAction == ActionNames.admAreaSelect)
                _atts = _atts.Where(x => x.AdmArea == value).ToList();
            else if (fieldAction == ActionNames.geoAreaSelect)
                _atts = _atts.Where(x => x.geoarea == value).ToList();
            else if (fieldAction == ActionNames.districtGeoAreaSelect)
            {
                string district;
                string geoarea;
                if (value.Split(";").Length == 2)
                {
                    district = value.Split(";")[0];
                    geoarea = value.Split(";")[1];
                    _atts = _atts.Where(x => x.District.Replace("\"\"", "") == district).ToList();
                }            
                else _atts = new List<AttractionType>();
            }         
            else throw new ArgumentException();
        }

        /// <summary>
        /// Отправка Csv файла пользователю
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        public async static void SendCsv(ITelegramBotClient botClient, Update update)
        {
            var receiver = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery != null ? update.CallbackQuery.From.Id : throw new ArgumentNullException();
            try
            {
                MemoryStream s = CsvProcessor.Write(_atts);
                await botClient.SendDocumentAsync(receiver, InputFile.FromStream(s, "document.csv"));
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                await botClient.SendTextMessageAsync(receiver, "Не удалось послать файл.");
            }
        }

        /// <summary>
        /// Отправка json файла пользователю
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        public async static void SendJson(ITelegramBotClient botClient, Update update)
        {
            var receiver = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery != null ? update.CallbackQuery.From.Id : throw new ArgumentNullException();
            try
            {
                MemoryStream s = JsonProcessor.Write(_atts);
                await botClient.SendDocumentAsync(receiver, InputFile.FromStream(s, "document.json"));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                await botClient.SendTextMessageAsync(receiver, "Не удалось послать файлю");
            }
            
        }

        /// <summary>
        /// Сортировка по имени атракциона
        /// </summary>
        /// <param name="reverse"></param>
        public async static void SortByName(bool reverse = false)
        {
            if (_atts == null) return;
            _atts = _atts.OrderBy(x => x.Name).ToList();
            if(reverse)
            {
                _atts.Reverse();
            }
        }
    }
}
