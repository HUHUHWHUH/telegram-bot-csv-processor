using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ClassLibrary1
{
    /// <summary>
    /// Класс для обработки csv файлов
    /// </summary>
    public class CsvProcessor
    {
        /// <summary>
        /// Чтение csv файла из потока
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<AttractionType> Read(StreamReader sr)
        {
            List<AttractionType> atts = new List<AttractionType>();
            using (sr)
            {
                if (sr.Peek() != -1)
                {
                    var x = sr.ReadLine();
                    if (!StructureChecker.StructureIsCorrect(x))
                        throw new ArgumentException();
                }
                    
                sr.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                while (sr.Peek() != -1)
                {
                    string str = sr.ReadLine();
                    string[] splittedLine = str.Split(";").Select(x => x.Replace("\"", "")).ToArray();
                    //13, т.к. после последнего поля тоже стоит ";"
                    int id = 0;
                    double globalId = 0;
                    if(splittedLine.Length == 13)
                        //записываем данные только если нет названий полей
                        if(int.TryParse(splittedLine[0], out id) && double.TryParse(splittedLine[3], out globalId))
                            atts.Add(new AttractionType(id, splittedLine[1], splittedLine[2], globalId, splittedLine[4],
                             splittedLine[5], splittedLine[6], splittedLine[7], splittedLine[8], splittedLine[9],
                             splittedLine[10], splittedLine[11]));
                }
            }
            return atts;
        }

        /// <summary>
        /// Запись Csv файла для отправки пользователю
        /// </summary>
        /// <param name="atts"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static MemoryStream Write(List<AttractionType> atts)
        {
            string str = "";
            if (atts == null)
            {
                throw new ArgumentNullException();
            }
            
            string headers = "\"ID\";\"Name\";\"Photo\";\"global_id\";\"AdmArea\";\"District\";\"Location\";\"RegistrationNumber\";\"State\";\"LocationType\";\"geodata_center\";\"geoarea\";\r\"Локальный идентификатор\";\"Название объекта\";\"Фотография\";\"global_id\";\"Административный округ по адресу\";\"Район\";\"Месторасположение\";\"Государственный регистрационный знак\";\"Состояние регистрации\";\"Тип места расположения\";\"geodata_center\";\"geoarea\";\r";
            foreach (AttractionType att in atts)
            {
                str += $"\"{att.Id}\";\"{att.Name}\";\"{att.Photo}\";\"{att.global_id}\";\"{att.AdmArea}\";" +
                    $"\"{att.District}\";\"{att.Location}\";\"{att.RegistrationNumber}\"" +
                    $";\"{att.State}\";\"{att.LocationType}\";\"{att.geodata_center}\";\"{att.geoarea}\";\r";
            }
            // преобразуем строку в массив байтов
            var strBytes = System.Text.Encoding.UTF8.GetBytes(headers + str);
            var stream = new MemoryStream(strBytes);
            // сбрасываем позицию потока в начало
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
