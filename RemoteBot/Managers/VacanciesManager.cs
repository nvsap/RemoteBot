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
using User = RemoteBot.Models.Objects.User;

namespace RemoteBot.Managers
{
    public class VacanciesManager
    {
        public static async Task SendMessage(TelegramBotClient botClient, Message message, string text, InlineKeyboardMarkup keyboardMarkup = null)
        {
            try
            {
                var mess = await botClient.SendTextMessageAsync(message.Chat.Id, text, Telegram.Bot.Types.Enums.ParseMode.Html, false, false, 0, replyMarkup: keyboardMarkup);
                StateManager.SetLastMessage(botClient, (int)message.Chat.Id, mess.MessageId);
            }
            catch { }
        }
        public static async void MainMenu(Message message, Telegram.Bot.TelegramBotClient botClient)
        {
            var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Условия", CallbackData = "ShowPrice"}

                                }
                            );

            var mess = await botClient.SendTextMessageAsync(message.Chat.Id, $"Здравствуйте, {message.Chat.FirstName}.\n\n Если вы хотите выставить вакансию, " +
                $"будет необходимо следовать инструкциям бота и отвечать на них.  Старайтесь писать кратко и по делу, донося основную суть задачи или проекта.",
                Telegram.Bot.Types.Enums.ParseMode.Markdown, false, false, 0, keyboard);
            StateManager.SetLastMessage(botClient, (int)message.Chat.Id, mess.MessageId);
        }
        public static async void ShowPrice(Telegram.Bot.TelegramBotClient botClient, Message message)
        {
            var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Добавить вакансию", CallbackData = "AddVacancy"}

                                }
                            );
            var messageText = $"Стоимость размещения вакансий:\n\n <b>690р</b> - один пост, то есть одна вакансия\n" +
                                                                 $"<b>1990р</b> - ОПТ, 4 поста в месяц(по одной вакансии на пост)\n\n" +
                                                                $"Вы получите: \n1.До 15 000 просмотров за первые сутки, а это до 150 откликов.\n" +
                                                                $"2.Ваша вакансия никогда не будет удаляться из канала наберёт 24 000 - 30 000 просмотров исполнителями.\n" +
                                                                $"3.Пост не уйдет быстро в ленту потому, что в день на канале мы публикуем до 4 публикаций.";

            await SendMessage(botClient, message, messageText, keyboard);
        }

        public static async void AddVacancу(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var vacancу = new Vacancу()
                {
                    User = db.Users.Where(x => x.Id == (int)message.Chat.Id).SingleOrDefault(),
                    CreatedOn = DateTime.UtcNow,
                    IsCreationFinished = false
                };
                var userStare = db.UserStates.Where(x => x.User.Id == vacancу.User.Id).SingleOrDefault();
                userStare.Vacancy = vacancу;
                db.Vacancies.Add(vacancу);
                db.UserStates.Update(userStare);
                db.SaveChanges();

                StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddHeader);

                var messageText = $"Введите название проекта/задачи/должности\n\nПример: Разработка сайта, Менеджер по продажам";

                await SendMessage(botClient, message, messageText);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, botClient, message);
            }
        }

        public static async void AddHeader(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

                userState.Vacancy.Header = message.Text;
                db.UserStates.Update(userState);
                db.SaveChanges();

                var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Подтвердить", CallbackData = "AddDescription"},
                                            new InlineKeyboardButton{ Text = "Изменить", CallbackData = "AddHeader"}
                                }
                            );

                var messageText = $"<b>{userState.Vacancy.Header}</b>\n\nПодтвердите введённый заголовок или измените его.";

                await SendMessage(botClient, message, messageText, keyboard); 
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, botClient, message);
            }
        }

        public static async void AddDescription(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

                userState.Vacancy.Description = message.Text;
                db.UserStates.Update(userState);
                db.SaveChanges();

                var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Подтвердить", CallbackData = "AddResponsibilities"},
                                            new InlineKeyboardButton{ Text = "Изменить", CallbackData = "AddDescription"}
                                }
                            );
                var description = string.IsNullOrWhiteSpace(userState.Vacancy.Description) ? "" : $"{userState.Vacancy.Description}\n\n";

                var messageText = $"<b>{userState.Vacancy.Header}</b>\n\n" +
                    $"{description}" +
                    $"Подтвердите описание вашей компании или измените его.";

                await SendMessage(botClient, message, messageText, keyboard);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, botClient, message);
            }
        }

        public static async void AddResponsibilities(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

                userState.Vacancy.Responsibilities += $"• {message.Text}\n";
                db.UserStates.Update(userState);
                db.SaveChanges();

                var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Следующий шаг", CallbackData = "AddRequirements"},
                                            new InlineKeyboardButton{ Text = "Заново", CallbackData = "AddResponsibilities"}
                                }
                            );
                var description = string.IsNullOrWhiteSpace(userState.Vacancy.Description) ? "" : $"{userState.Vacancy.Description}\n\n";

                var messageText = $"<b>{userState.Vacancy.Header}</b>\n\n" +
                    $"{description}" +
                    $"<b>Что делать:</b>\n{userState.Vacancy.Responsibilities}\n" +
                    $"Что бы добавить ещё один пункт в \"Что делать\" отправте его новым сообщением.\n" +
                    $"Что бы перейти к следующему шагу нажмите соответствующую кнопку.\n" +
                    $"Что бы заполнить пункты \"Что делать?\" заново нажмите кнопку \"Заново\"";

                await SendMessage(botClient, message, messageText, keyboard);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, botClient, message);
            }
        }

        public static async void AddRequirements(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

                userState.Vacancy.Requirements += $"• {message.Text}\n";
                db.UserStates.Update(userState);
                db.SaveChanges();

                var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Следующий шаг", CallbackData = "AddOffered"},
                                            new InlineKeyboardButton{ Text = "Заново", CallbackData = "AddRequirements"}
                                }
                            );

                var description = string.IsNullOrWhiteSpace(userState.Vacancy.Description) ? "" : $"{userState.Vacancy.Description}\n\n";
                var requirements = string.IsNullOrWhiteSpace(userState.Vacancy.Requirements) ? "" : $"<b>Требования:</b>\n{userState.Vacancy.Requirements}\n";

                var messageText = $"<b>{userState.Vacancy.Header}</b>\n\n" +
                    $"{description}" +
                    $"<b>Что делать:</b>\n{userState.Vacancy.Responsibilities}\n" +
                    $"{requirements}" +
                    $"Что бы добавить ещё один пункт в \"Требования\" отправте его новым сообщением.\n" +
                    $"Что бы перейти к следующему шагу нажмите соответствующую кнопку.\n" +
                    $"Что бы заполнить пункты \"Требования\" заново нажмите кнопку \"Заново\"";

                await SendMessage(botClient, message, messageText, keyboard);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, botClient, message);
            }
        }

        public static async void AddOffered(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

                userState.Vacancy.Offered += $"• {message.Text}\n";
                db.UserStates.Update(userState);
                db.SaveChanges();

                var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Следующий шаг", CallbackData = "AddPaymentOffers"},
                                            new InlineKeyboardButton{ Text = "Заново", CallbackData = "AddOffered"}
                                }
                            );

                var description = string.IsNullOrWhiteSpace(userState.Vacancy.Description) ? "" : $"{userState.Vacancy.Description}\n\n";
                var requirements = string.IsNullOrWhiteSpace(userState.Vacancy.Requirements) ? "" : $"<b>Требования:</b>\n{userState.Vacancy.Requirements}\n";
                var offered = string.IsNullOrWhiteSpace(userState.Vacancy.Offered) ? "" : $"<b>Что предлагаем</b>:\n{userState.Vacancy.Offered}\n";

                var messageText = $"<b>{userState.Vacancy.Header}</b>\n\n" +
                    $"{description}" +
                    $"<b>Что делать:</b>\n{userState.Vacancy.Responsibilities}\n" +
                    $"{requirements}" +
                    $"{offered}" +
                    $"Что бы добавить ещё один пункт в \"Что предлагаем\" отправте его новым сообщением.\n" +
                    $"Что бы перейти к следующему шагу нажмите соответствующую кнопку.\n" +
                    $"Что бы заполнить пункты \"Что предлагаем\" заново нажмите кнопку \"Заново\"";

                await SendMessage(botClient, message, messageText, keyboard);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, botClient, message);
            }
        }

        public static async void AddPaymentOffers(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

                userState.Vacancy.PaymentOffers = message.Text;
                db.UserStates.Update(userState);
                db.SaveChanges();

                var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Подтвердить", CallbackData = "AddAdditionalComment"},
                                            new InlineKeyboardButton{ Text = "Изменить", CallbackData = "AddPaymentOffers"}
                                }
                            );

                var description = string.IsNullOrWhiteSpace(userState.Vacancy.Description) ? "" : $"{userState.Vacancy.Description}\n\n";
                var requirements = string.IsNullOrWhiteSpace(userState.Vacancy.Requirements) ? "" : $"<b>Требования:</b>\n{userState.Vacancy.Requirements}\n";
                var offered = string.IsNullOrWhiteSpace(userState.Vacancy.Offered) ? "" : $"<b>Что предлагаем</b>:\n{userState.Vacancy.Offered}\n";

                var messageText = $"<b>{userState.Vacancy.Header}</b>\n\n" +
                    $"{description}" +
                    $"<b>Что делать:</b>\n{userState.Vacancy.Responsibilities}\n" +
                    $"{requirements}" +
                    $"{offered}" +
                    $"<b>Оплата:</b>\n{userState.Vacancy.PaymentOffers}\n\n" +
                    $"Подтвердите пункт \"Оплата\" или измените его.";

                await SendMessage(botClient, message, messageText, keyboard);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, botClient, message);
            }
        }

        public static async void AddAdditionalComment(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

                userState.Vacancy.AdditionalComment = message.Text;
                db.UserStates.Update(userState);
                db.SaveChanges();

                var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Подтвердить", CallbackData = "AddContacts"},
                                            new InlineKeyboardButton{ Text = "Изменить", CallbackData = "AddAdditionalComment"}
                                }
                            );

                var description = string.IsNullOrWhiteSpace(userState.Vacancy.Description) ? "" : $"{userState.Vacancy.Description}\n\n";
                var requirements = string.IsNullOrWhiteSpace(userState.Vacancy.Requirements) ? "" : $"<b>Требования:</b>\n{userState.Vacancy.Requirements}\n";
                var offered = string.IsNullOrWhiteSpace(userState.Vacancy.Offered) ? "" : $"<b>Что предлагаем</b>:\n{userState.Vacancy.Offered}\n";
                var additionalComment = string.IsNullOrWhiteSpace(userState.Vacancy.AdditionalComment) ? "" : $"{userState.Vacancy.AdditionalComment}\n\n";

                var messageText = $"<b>{userState.Vacancy.Header}</b>\n\n" +
                    $"{description}" +
                    $"<b>Что делать:</b>\n{userState.Vacancy.Responsibilities}\n" +
                    $"{requirements}" +
                    $"{offered}" +
                    $"<b>Оплата:</b>\n{userState.Vacancy.PaymentOffers}\n\n" +
                    $"{additionalComment}" +
                    $"Подтвердите дополнительный комментраий или измените его.";

                await SendMessage(botClient, message, messageText, keyboard);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, botClient, message);
            }
        }

        public static async void AddContacts(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

                userState.Vacancy.Contacts += $"{message.Text}\n";
                db.UserStates.Update(userState);
                db.SaveChanges();

                var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Следующий шаг", CallbackData = "ConfirmVacancу"},
                                            new InlineKeyboardButton{ Text = "Заново", CallbackData = "AddContacts"}
                                }
                            );

                var description = string.IsNullOrWhiteSpace(userState.Vacancy.Description) ? "" : $"{userState.Vacancy.Description}\n\n";
                var requirements = string.IsNullOrWhiteSpace(userState.Vacancy.Requirements) ? "" : $"<b>Требования:</b>\n{userState.Vacancy.Requirements}\n";
                var offered = string.IsNullOrWhiteSpace(userState.Vacancy.Offered) ? "" : $"<b>Что предлагаем</b>:\n{userState.Vacancy.Offered}\n";
                var additionalComment = string.IsNullOrWhiteSpace(userState.Vacancy.AdditionalComment) ? "" : $"{userState.Vacancy.AdditionalComment}\n\n";

                var messageText = $"<b>{userState.Vacancy.Header}</b>\n\n" +
                    $"{description}" +
                    $"<b>Что делать:</b>\n{userState.Vacancy.Responsibilities}\n" +
                    $"{requirements}" +
                    $"{offered}" +
                    $"<b>Оплата:</b>\n{userState.Vacancy.PaymentOffers}\n\n" +
                    $"{additionalComment}" +
                    $"<b>Контакты:</b>\n{userState.Vacancy.Contacts}\n" +
                    $"Что бы добавить ещё один пункт в \"Контакты\" отправте его новым сообщением.\n" +
                    $"Что бы перейти к следующему шагу нажмите соответствующую кнопку.\n" +
                    $"Что бы заполнить пункты \"Контакты\" заново нажмите кнопку \"Заново\"";

                await SendMessage(botClient, message, messageText, keyboard);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, botClient, message);
            }
        }


        public static async void ConfirmVacancу(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

                var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Отправить", CallbackData = "SaveVacancy"},
                                            new InlineKeyboardButton{ Text = "Главное меню", CallbackData = "MainMenu"}
                                }
                            );
                var description = string.IsNullOrWhiteSpace(userState.Vacancy.Description) ? "" : $"{userState.Vacancy.Description}\n\n";
                var requirements = string.IsNullOrWhiteSpace(userState.Vacancy.Requirements) ? "" : $"<b>Требования:</b>\n{userState.Vacancy.Requirements}\n";
                var offered = string.IsNullOrWhiteSpace(userState.Vacancy.Offered) ? "" : $"<b>Что предлагаем</b>:\n{userState.Vacancy.Offered}\n";
                var additionalComment = string.IsNullOrWhiteSpace(userState.Vacancy.AdditionalComment) ? "" : $"{userState.Vacancy.AdditionalComment}\n\n";

                var messageText = $"<b>{userState.Vacancy.Header}</b>\n\n" +
                    $"{description}" +
                    $"<b>Что делать:</b>\n{userState.Vacancy.Responsibilities}\n" +
                    $"{requirements}" +
                    $"{offered}" +
                    $"<b>Оплата:</b>\n{userState.Vacancy.PaymentOffers}\n\n" +
                    $"{additionalComment}" +
                    $"<b>Контакты:</b>\n{userState.Vacancy.Contacts}\n" +
                    $"Подтвердите правильность всех данных или начните с начала. Если нужно внести коректировки в шаблон - укажите это при связи с администратором.";

                await SendMessage(botClient, message, messageText, keyboard);
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex, botClient, message);
            }
        }

        public static async void SaveVacancу(TelegramBotClient botClient, Message message, TelegramContext db)
        {
            try
            {
                var userState = db.UserStates.Include(x => x.User).Where(x => x.User.Id == message.Chat.Id).Include(x => x.Vacancy).SingleOrDefault();

                userState.Vacancy.IsCreationFinished = true;

                var description = string.IsNullOrWhiteSpace(userState.Vacancy.Description) ? "" : $"{userState.Vacancy.Description}\n\n";
                var requirements = string.IsNullOrWhiteSpace(userState.Vacancy.Requirements) ? "" : $"<b>Требования:</b>\n{userState.Vacancy.Requirements}\n";
                var offered = string.IsNullOrWhiteSpace(userState.Vacancy.Offered) ? "" : $"<b>Что предлагаем</b>:\n{userState.Vacancy.Offered}\n";
                var additionalComment = string.IsNullOrWhiteSpace(userState.Vacancy.AdditionalComment) ? "" : $"{userState.Vacancy.AdditionalComment}\n\n";

                var vacancyText = $"<b>{userState.Vacancy.Header}</b>\n\n" +
                    $"{description}" +
                    $"<b>Что делать:</b>\n{userState.Vacancy.Responsibilities}\n" +
                    $"{requirements}" +
                    $"{offered}" +
                    $"<b>Оплата:</b>\n{userState.Vacancy.PaymentOffers}\n\n" +
                    $"{additionalComment}" +
                    $"<b>Контакты:</b>\n{userState.Vacancy.Contacts}\n";

                userState.Vacancy.Text = vacancyText;
                db.UserStates.Update(userState);
                db.SaveChanges();

                var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Главное меню", CallbackData = "MainMenu"}
                                }
                            );

                var messageText = $"Вакансия №{userState.Vacancy.Id} создана! \n\nДля оплаты и выбора даты свяжитесь с нами и укажите номер вакансии: @VLADvesna.";
                await SendMessage(botClient, message, messageText, keyboard);

                var username = message.Chat.Username;
                var adminMessage = string.IsNullOrEmpty(message.Chat.Username)
                    ? $"Вакансия №{userState.Vacancy.Id} \n\n{userState.Vacancy.Text}"
                    : $"Вакансия №{userState.Vacancy.Id} от @{message.Chat.Username} \n\n{userState.Vacancy.Text}";//@{message.Chat.Username}


                await botClient.SendTextMessageAsync("481884934", adminMessage, Telegram.Bot.Types.Enums.ParseMode.Html, false, false, 0, replyMarkup: null);

                StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.Empty);
            }
            catch (Exception ex)
            {
                 SendErrorMessage(ex, botClient, message);
            }
        }

        private static async void SendErrorMessage(Exception ex, TelegramBotClient botClient, Message message)
        {
            var keyboard = new InlineKeyboardMarkup(
                                   new InlineKeyboardButton[]
                                   {
                                            new InlineKeyboardButton{ Text = "Главное меню", CallbackData = "MainMenu"}
                                   }
                               );
            var messageText = $"Ошибка:{ex.Message}";
            await SendMessage(botClient, message, messageText, keyboard);
        }
    }
}
