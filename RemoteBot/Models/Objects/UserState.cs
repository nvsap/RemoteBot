using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteBot.Models.Objects
{

    public class UserState
    {
        public int Id { get; set; }

        public int State { get; set; }

        public int UserId { get; set; }

        public int BuildingId { get; set; }
    }

    public enum UserStatesEnum
    {
        Empty,
        AddVacancies
    }
}
