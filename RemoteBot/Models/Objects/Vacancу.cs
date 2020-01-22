using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteBot.Models.Objects
{
    public class Vacancу
    {
        public int Id { get; set; }

        [ForeignKey("Id")]
        public User? User { get; set; }

        public string Text { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
