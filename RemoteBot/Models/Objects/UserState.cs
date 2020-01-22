using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteBot.Models.Objects
{

    public class UserState
    {
        public int Id { get; set; }

        public int State { get; set; }
<<<<<<< HEAD
        
        public int UserId { get; set; }

        [ForeignKey("Id")]
        public Vacancу? Vacancy { get; set; }
=======

        [ForeignKey("Id")]
        public User User { get; set; }

        [ForeignKey("Id")]
        public Vacancу Vacancy { get; set; }
>>>>>>> master
    }

    public enum UserStatesEnum
    {
        Empty,
        AddVacancies
    }
}
