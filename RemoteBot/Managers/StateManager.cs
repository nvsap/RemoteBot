using RemoteBot.Models;
using RemoteBot.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace RemoteBot.Managers
{
    public class StateManager
    {
        public static void StateUpdate(long userId, int state)
        {
            using (TelegramContext db = new TelegramContext())
            {
                var userState = db.UserStates.Where(x => x.Id == userId).SingleOrDefault();
                userState.State = state;
                db.UserStates.Update(userState);
                db.SaveChanges();
            }
        }
        public static void StateControl(Telegram.Bot.TelegramBotClient botClient, Update update)
        {
            using (TelegramContext db = new TelegramContext())
            {
                var userState = db.UserStates.Where(x => x.Id == update.Message.From.Id).SingleOrDefault();
                ActionStateSelected(userState, db, botClient, update);
            }
        }
        public static void ActionStateSelected(UserState userState,
                                                    TelegramContext db,
                                                    Telegram.Bot.TelegramBotClient botClient,
                                                    Update update)
        {
            switch (userState.State)
            {
                case (int)UserStatesEnum.Empty://Empty
                    break;
                case (int)UserStatesEnum.AddVacancies:
                    break;
            }
        }
    }
}
