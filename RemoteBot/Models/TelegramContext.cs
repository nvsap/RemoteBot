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

        public DbSet<Vacancу> Vacancies { get; set; }

        public TelegramContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = tcp:93.190.46.34,33301; Initial Catalog=uh1141519_db2; User Id = uh1141519_user; Password = x1rQkvnk96;");
        }
    }
}
