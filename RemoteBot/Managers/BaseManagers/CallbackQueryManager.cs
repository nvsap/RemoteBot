using Microsoft.EntityFrameworkCore;
using RemoteBot.Models;
using RemoteBot.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace RemoteBot.Managers.BaseManagers
{
    public class CallbackQueryManager
    {
        public static async void ChoseCallBackQuery(Telegram.Bot.TelegramBotClient botClient, Update update)
        {
            try
            {
                using (TelegramContext db = new TelegramContext())
                {
                    var user = db.Users.Where(x => x.Id == update.CallbackQuery.From.Id).Single();
                    var callBack = update.CallbackQuery;

                    await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    if (callBack.Data == "ShowPrice")
                    {
                        VacanciesManager.ShowPrice(botClient, update.CallbackQuery.Message);
                    }
                    else if(callBack.Data == "AddVacancy")
                    {
                        VacanciesManager.AddVacancу(botClient, update.CallbackQuery.Message, db);
                    }
                    else if (callBack.Data == "AddHeader")
                    {
                        AddHeaderCallback(botClient, update.CallbackQuery.Message);
                    }
                    else if (callBack.Data == "AddDescription")
                    {
                        AddDescriptionCallback(botClient, update.CallbackQuery.Message);
                    }
                    else if (callBack.Data == "AddResponsibilities")
                    {
                        AddResponsibilitiesCallback(botClient, update.CallbackQuery.Message, db);
                    }
                    else if (callBack.Data == "AddRequirements")
                    {
                        AddRequirementsCallback(botClient, update.CallbackQuery.Message, db);
                    }
                    else if (callBack.Data == "AddOffered")
                    {
                        AddOfferedCallback(botClient, update.CallbackQuery.Message, db);
                    }
                    else if (callBack.Data == "AddPaymentOffers")
                    {
                        AddPaymentOffersCallback(botClient, update.CallbackQuery.Message);
                    }
                    else if (callBack.Data == "AddAdditionalComment")
                    {
                        AddAdditionalCommentCallback(botClient, update.CallbackQuery.Message);
                    }
                    else if (callBack.Data == "AddContacts")
                    {
                        AddContactsCallback(botClient, update.CallbackQuery.Message, db);
                    }
                    else if (callBack.Data == "ConfirmVacancу")
                    {
                        VacanciesManager.ConfirmVacancу(botClient, update.CallbackQuery.Message, db);
                    }
                    else if(callBack.Data == "SaveVacancy")
                    {
                        VacanciesManager.SaveVacancу(botClient, update.CallbackQuery.Message, db);
                    }
                    else if (callBack.Data == "MainMenu")
                    {
                        VacanciesManager.MainMenu(update.CallbackQuery.Message, botClient);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async void AddHeaderCallback(TelegramBotClient botClient, Message message)
        {
            var messageText = $"Введите название проекта/задачи/должности\n\nПример: Разработка сайта, Менеджер по продажам";
            StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddHeader);
            await VacanciesManager.SendMessage(botClient, message, messageText);
        }

        private static async void AddDescriptionCallback(TelegramBotClient botClient, Message message)
        {
            var keyboard = new InlineKeyboardMarkup(
                              new InlineKeyboardButton[]
                              {
                                            new InlineKeyboardButton{ Text = "Пропустить", CallbackData = "AddResponsibilities"}
                              }
                          );
            StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddDescription);
            var messageText = $"Введите описание кампании или проекта. Если хотите пропустить этот шаг нажмите кнопку \"Пропустить\".";
            await VacanciesManager.SendMessage(botClient, message, messageText, keyboard);
        }

        private static async void AddResponsibilitiesCallback(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

            userState.Vacancy.Responsibilities = "";
            db.UserStates.Update(userState);
            db.SaveChanges();
            StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddResponsibilities);

            var messageText = $"Введите список обязаностей соискателя (меню \"Что нужно делать\"). Каждый пункт отправляйте отдельным сообщением.";
            await VacanciesManager.SendMessage(botClient, message, messageText);
        }

        private static async void AddRequirementsCallback(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

            userState.Vacancy.Requirements = "";
            db.UserStates.Update(userState);
            db.SaveChanges();

            var keyboard = new InlineKeyboardMarkup(
                               new InlineKeyboardButton[]
                               {
                                            new InlineKeyboardButton{ Text = "Пропустить", CallbackData = "AddOffered"}
                               }
                           );

            StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddRequirements);

            var messageText = $"Введите список требований к соискателю (меню \"Требования\"). Каждый пункт отправляйте отдельным сообщением." +
                $"\nЕсли хотите пропустить этот шаг нажмите кнопку \"Пропустить\".";
            await VacanciesManager.SendMessage(botClient, message, messageText, keyboard);
        }

        private static async void AddOfferedCallback(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

            userState.Vacancy.Offered = "";
            db.UserStates.Update(userState);
            db.SaveChanges();

            var keyboard = new InlineKeyboardMarkup(
                               new InlineKeyboardButton[]
                               {
                                            new InlineKeyboardButton{ Text = "Пропустить", CallbackData = "AddPaymentOffers"}
                               }
                           );

            StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddOffered);

            var messageText = $"Введите список того, что вы предлагаете соискателю, кроме заработной платы, (меню \"Что предлагаем\"). Каждый пункт отправляйте отдельным сообщением." + 
                $"\nЕсли хотите пропустить этот шаг нажмите кнопку \"Пропустить\".";
            await VacanciesManager.SendMessage(botClient, message, messageText, keyboard);
        }

        private static async void AddPaymentOffersCallback(TelegramBotClient botClient, Message message)
        {
            var messageText = $"Введите суму оплаты.";
            StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddPaymentOffers);
            await VacanciesManager.SendMessage(botClient, message, messageText);
        }

        private static async void AddAdditionalCommentCallback(TelegramBotClient botClient, Message message)
        {
            var keyboard = new InlineKeyboardMarkup(
                              new InlineKeyboardButton[]
                              {
                                            new InlineKeyboardButton{ Text = "Пропустить", CallbackData = "AddContacts"}
                              }
                          );

            StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddAdditionalComment);
            var messageText = $"Введите дополнительный комментарий. Если хотите пропустить этот шаг нажмите кнопку \"Пропустить\".";
            await VacanciesManager.SendMessage(botClient, message, messageText, keyboard);
        }
        private static async void AddContactsCallback(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

            userState.Vacancy.Contacts = "";
            db.UserStates.Update(userState);
            db.SaveChanges();

            StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddContacts);

            var messageText = $"Введите ваши контакты (меню \"Контакты\"). Каждый пункт отправляйте отдельным сообщением.";
            await VacanciesManager.SendMessage(botClient, message, messageText);
        }
    }
}
