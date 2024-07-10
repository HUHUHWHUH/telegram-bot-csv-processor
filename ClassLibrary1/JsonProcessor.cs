using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    /// <summary>
    /// Обработчик json файлов
    /// </summary>
    public class JsonProcessor
    {
        /// <summary>
        /// Чтение csv файла из потока
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static List<AttractionType> Read(StreamReader sr)
        {
            if (sr == null)
            {
                throw new ArgumentNullException();
            }
            List<AttractionType> atts = JsonSerializer.Deserialize<List<AttractionType>>(sr.ReadToEnd());
            return atts;
        }

        /// <summary>
        /// Запись json файла для отправки пользователю
        /// </summary>
        /// <param name="atts"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static MemoryStream Write(List<AttractionType> atts)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            string str = "";
            if (atts == null)
            {
                throw new ArgumentNullException();
            }
                
            str = JsonSerializer.Serialize(atts, options);
            // преобразуем json-строку в массив байтов
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(str);

            // создаем поток из массива байтов
            var stream = new MemoryStream(jsonBytes);

            // сбрасываем позицию потока в начало
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
