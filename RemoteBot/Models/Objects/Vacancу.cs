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

        public User? User { get; set; }

        public string Text { get; set; }

        public string Header { get; set; }

        public string Description { get; set; }

        public string Responsibilities { get; set; }

        public string Requirements { get; set; }

        public string Offered { get; set; }

        public string PaymentOffers { get; set; }

        public string AdditionalComment { get; set; }

        public string Contacts { get; set; }

        public bool IsCreationFinished { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
