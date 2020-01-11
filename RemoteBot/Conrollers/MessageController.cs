using Microsoft.AspNetCore.Mvc;
using RemoteBot.Managers;
using RemoteBot.Managers.BaseManagers;
using RemoteBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace RemoteBot.Conrollers
{
    [Route("api/message/update")]
    public class MessageController : Controller
    {
        // GET api/values/5
        [HttpGet]
        public string Get()
        {
            return "Method GET unuvalable";
        }

        [HttpPost]
        public async Task<OkResult> Post([FromBody]Update update)
        {

            if (update == null) return Ok();

            var commands = Bot.Commands;
            var message = update.Message;
            var botClient = await Bot.GetBotClientAsync();

            try
            {
                if (update.Type == UpdateType.Message)
                {

                    foreach (var command in commands)
                    {
                        if (command.Name == message.Text)
                        {
                            await command.ExecuteAsync(message, botClient);
                            return Ok();
                        }
                    }
                    StateManager.StateControl(botClient, update);
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    CallbackQueryManager.ChoseCallBackQuery(botClient, update);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Возникла ошибка. Обратитесь к администратору с следующим текстом: " + ex.Message, Telegram.Bot.Types.Enums.ParseMode.Default);
                return Ok();
            }
        }
    }
}
