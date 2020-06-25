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
                var userState = db.UserStates.Where(x => x.User.Id == userId).SingleOrDefault();
                userState.State = state;
                db.UserStates.Update(userState);
                db.SaveChanges();
            }
        }

        public static async void RemoveLastMessage(Telegram.Bot.TelegramBotClient botClient, int chatId)
        {
            using (TelegramContext db = new TelegramContext())
            {
                try
                {
                    var userState = db.UserStates.Where(x => x.User.Id == chatId).SingleOrDefault();
                    if (userState != null && userState.LastMessageId != null)
                    {
                        await botClient.DeleteMessageAsync(chatId, (int)userState.LastMessageId);
                        userState.LastMessageId = null;
                        db.UserStates.Update(userState);
                        db.SaveChanges();
                    }
                }
                catch { }
            }
        }

        public static async void SetLastMessage(Telegram.Bot.TelegramBotClient botClient, int chatId, int newMessageId)
        {
            using (TelegramContext db = new TelegramContext())
            {
                var userState = db.UserStates.Where(x => x.User.Id == chatId).SingleOrDefault();
                if (userState != null)
                {
                    userState.LastMessageId = newMessageId;
                    db.UserStates.Update(userState);
                    await db.SaveChangesAsync();
                }
            }
        }

        public static void StateControl(Telegram.Bot.TelegramBotClient botClient, Update update)
        {
            using (TelegramContext db = new TelegramContext())
            {
                var userState = db.UserStates.Where(x => x.User.Id == update.Message.From.Id).SingleOrDefault();
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
                    VacanciesManager.ConfirmVacancу(botClient, update.Message, db);
                    break;
                case (int)UserStatesEnum.AddHeader:
                    VacanciesManager.AddHeader(botClient, update.Message, db);
                    break;
                case (int)UserStatesEnum.AddDescription:
                    VacanciesManager.AddDescription(botClient, update.Message, db);
                    break;
                case (int)UserStatesEnum.AddResponsibilities:
                    VacanciesManager.AddResponsibilities(botClient, update.Message, db);
                    break;
                case (int)UserStatesEnum.AddRequirements:
                    VacanciesManager.AddRequirements(botClient, update.Message, db);
                    break;
                case (int)UserStatesEnum.AddOffered:
                    VacanciesManager.AddOffered(botClient, update.Message, db);
                    break;
                case (int)UserStatesEnum.AddPaymentOffers:
                    VacanciesManager.AddPaymentOffers(botClient, update.Message, db);
                    break;
                case (int)UserStatesEnum.AddAdditionalComment:
                    VacanciesManager.AddAdditionalComment(botClient, update.Message, db);
                    break;
                case (int)UserStatesEnum.AddContacts:
                    VacanciesManager.AddContacts(botClient, update.Message, db);
                    break;
            }
        }
    }
}
