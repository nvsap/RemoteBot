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
        public static async void MainMenu(User user, TelegramContext db, Message message, Telegram.Bot.TelegramBotClient botClient)
        {
            var keyboard = new InlineKeyboardMarkup(
                                new InlineKeyboardButton[]
                                {
                                            new InlineKeyboardButton{ Text = "Добавить вакансию", CallbackData = "AddVacancies"}

                                }
                            );

            var mess = await botClient.SendTextMessageAsync(message.Chat.Id, "Привет, " + message.Chat.FirstName, Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, keyboard);
            
        }
        public static async void AddVacancу(TelegramBotClient botClient, TelegramContext db, Message message)
        {
            try
            {
                StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddVacancies);
                
                var rkm = new ReplyKeyboardMarkup();                                                                                                
                                                                                                                                                    
                rkm.Keyboard =                                                                                                                      
                            new KeyboardButton[][]                                                                                                  
                            {                                                                                                                       
                                new KeyboardButton[]                                                                                                
                                {                                                                                                                   
                                    new KeyboardButton("Подтвердить!")                                                                              
                                }                                                                                                                  
                            };                                                                                                                      
                                                                                                                                                    
                await botClient.SendTextMessageAsync(message.Chat.Id, @"Пришлите ваше предложение в следующем формате!", Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, false, false, 0, rkm);
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка: " + ex.Message, Telegram.Bot.Types.Enums.ParseMode.Default);
            }
        }
        public static async void SaveVacancу(TelegramBotClient botClient, TelegramContext db, Message message)
        {
            try
            {
                StateManager.StateUpdate(message.Chat.Id, (int)UserStatesEnum.AddVacancies);
                await botClient.SendTextMessageAsync(message.Chat.Id, @"Пришлите ваше предложение в следующем формате!", Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, false, false, 0);

                var rkm = new ReplyKeyboardMarkup();

                rkm.Keyboard =
                            new KeyboardButton[][]
                            {
                                new KeyboardButton[]
                                {
                                    new KeyboardButton("Подтвердить!")
                                }
                            };

                await botClient.SendTextMessageAsync(message.Chat.Id, @"text", Telegram.Bot.Types.Enums.ParseMode.MarkdownV2, false, false, 0, rkm);
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Ошибка: " + ex.Message, Telegram.Bot.Types.Enums.ParseMode.Default);
            }
        }

    }
}
