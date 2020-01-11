using Microsoft.EntityFrameworkCore;
using RemoteBot.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace RemoteBot.Models
{
    public class TelegramContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<UserState> UserStates { get; set; }

        public TelegramContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=tcp:learningappbot.database.windows.net,1433; Initial Catalog=learningAppBot; User ID = nvsap; Password = Q1W2E3R4T5Y6q;");
        }
    }
}
