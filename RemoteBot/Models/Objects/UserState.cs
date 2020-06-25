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
        
        public User User { get; set; }

        public Vacancу Vacancy { get; set; }

        public int? LastMessageId { get; set; }
    }

    public enum UserStatesEnum
    {
        Empty,
        AddVacancies,
        AddHeader,
        AddDescription,
        AddResponsibilities,
        AddRequirements,
        AddOffered,
        AddPaymentOffers,
        AddAdditionalComment,
        AddContacts
    }
}
