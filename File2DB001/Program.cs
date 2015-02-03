using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;

using System.Web.Script.Serialization;  // ref System.Web.Extensions

namespace File2DB001
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: File2DB001 file.json");
                return;
            }

            // Read file
            string jsonText = System.IO.File.ReadAllText(args[0]);

            // deserialize to dictionary
            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<Dictionary<string, dynamic>>(jsonText);

            CarModels001.Models.CarMakesModels cCMM = new CarModels001.Models.CarMakesModels();
            cCMM.Create(ref dict);

        }
    }
}