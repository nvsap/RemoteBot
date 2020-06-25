using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteBot.Models
{
    public class AppSettings
    {
        //ngrok http 1234 -host-header=”localhost:44303”
        //public static string Url { get; set; } = "https://0a865685a3a6.ngrok.io";//test url
        public static string Url { get; set; } = "https://learnlikeabot.biz";//production url

        public static string Name { get; set; } = "worzavr_vacancies_bot";

        //public static string Key { get; set; } = "1019921761:AAE1KqTauwXV82Q1-9KOvegAJSupdTxiIMc"; //test bot
        public static string Key { get; set; } = "810437567:AAGVHR_kn4MPSWUWZJZDJ7d1R3FGi_EwivY"; //production bot
    }
}
