using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteBot.Models
{
    public class AppSettings
    {
        //ngrok http 1234 -host-header=”localhost:44353”
        public static string Url { get; set; } = "https://c3fcdf4b.ngrok.io";// "https://learnlikeabot.biz";//"https://568ccc42.ngrok.io";
        //public static string Url { get; set; } = "https://fe5f868c.ngrok.io";
        public static string Name { get; set; } = "RemoteBot";

        //public static string Key { get; set; } = "694699768:AAFsYDKo1qn5eIizqA5CIl8m9xXUoYeZyM8";
        public static string Key { get; set; } = "811049575:AAFcDU8wsn60vPx1B5FcXjNIUOMzU60pPgQ"; 
    }
}
