namespace ClassLibrary1
{
    /// <summary>
    /// Класс для считывания токена бота из файла
    /// </summary>
    public class TokenGetter
    {
        /// <summary>
        /// Метод, считывающий токен бота их файла
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetToken(string path)
        {
            if (!File.Exists(path))
                return "";
            string token = "";
            using (StreamReader sr = new StreamReader(path))
                token = sr.ReadToEnd();
            return token;
        }
    }
}