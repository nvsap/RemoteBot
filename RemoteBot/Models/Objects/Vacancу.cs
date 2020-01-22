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
<<<<<<< HEAD
        public User? User { get; set; }
=======
        public User User { get; set; }
>>>>>>> master

        public string Text { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
