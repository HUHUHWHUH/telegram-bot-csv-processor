using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    /// <summary>
    /// Класс с методом для проверки структуры файла
    /// </summary>
    public class StructureChecker
    {
        /// <summary>
        /// Метод для проверки структуры файла
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StructureIsCorrect(string str) 
        {
            //using (StreamReader sr = new StreamReader(str))
            {
                string[] headers = str.Split(";").Select(x => x.Replace("\"", "")).ToArray();
                if (headers.Length == 13 && headers[0] == "ID" && headers[1] == "Name" && headers[2] == "Photo" &&
                    headers[3] == "global_id" && headers[4] == "AdmArea" && headers[5] == "District" &&
                    headers[6] == "Location" && headers[7] == "RegistrationNumber" && headers[8] == "State" &&
                    headers[9] == "LocationType" && headers[10] == "geodata_center" && headers[11] == "geoarea")
                {
                    Console.WriteLine("Correct");
                    return true;
                }
                else
                {
                    //Console.WriteLine("Incorrect");
                    return false;
                }
                    
            }
        }
    }
}
