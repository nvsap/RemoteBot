using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RemoteBot.Models.Objects
{
    public class UserLock
    {
        public long Id { get; set; }

        public bool Locked { get; set; }
    }
}
