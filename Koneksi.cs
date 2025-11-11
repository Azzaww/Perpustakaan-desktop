using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desainperpus_fatimah
{
    internal class Koneksi
    {
        public static string conn = "Data Source = DESKTOP-GRKT4MQ; Initial Catalog = perpus; Integrated Security = True";
    }

    public class Model
    {
        public static string name { get; set; }
    }
}
