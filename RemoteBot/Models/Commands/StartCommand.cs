using RemoteBot.Managers;
using RemoteBot.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = RemoteBot.Models.Objects.User;

namespace RemoteBot.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }

        public override async Task ExecuteAsync(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            using (TelegramContext db = new TelegramContext())
            {
                var User = db.Users.Where(p => p.UserId == message.From.Id).SingleOrDefault();
                if (User == null)
                {
                    User user = new User { UserId = message.From.Id, Name = message.From.Username };
                    UserState US = new UserState { UserId = message.From.Id, State = (int)UserStatesEnum.Empty };
                    db.Users.Add(user);
                    db.UserStates.Add(US);
                    db.SaveChanges();
                    VacanciesManager.MainMenu(user, db, message, botClient);
                }
                else
                    VacanciesManager.MainMenu(User, db, message, botClient);
            }
        }
    }
}
