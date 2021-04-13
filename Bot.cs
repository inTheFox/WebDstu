using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using WebDstu.Database;
using WebDstu.Models;

namespace WebDstu
{
    public class Bot
    {
        public Dictionary<long, string> pData = new Dictionary<long, string>();
        private FileToSend photo;

        public async void StartListen()
        {

            var bot = new TelegramBotClient("1794355084:AAFlARMlsA6HlVpX1IXELAOdTxjWCrYgCWE");
            await bot.SetWebhookAsync("");
            try
            {

                photo = new FileToSend(
                    "https://sun9-46.userapi.com/impg/yP7m1fjPEW27EEPT0ZC5MBCmErrYcEMNKeYBGw/KY22S9UjkNw.jpg?size=1036x588&quality=96&sign=679220da15c5f3bf9ac1c7de9b26453b&type=album");

                bot.OnMessage += async (object sender, Telegram.Bot.Args.MessageEventArgs e) =>
                {
                    try
                    {
                        string message = e.Message.Text;

                        if (message == "/start")
                        {
                            Logger.Input("Hello world");

                            using (DatabaseContext db = new DatabaseContext())
                            {
                                List<DSTUSaved> saved = db.Saved.ToList();

                                saved = saved.OrderBy(p => p.SortId).ToList();

                                await bot.SendPhotoAsync(e.Message.Chat.Id, photo,
                                    "Ректор рекомендует!\nСтуденту от студентов! В 2021 году Студенческий совет разработал для тебя полезного бота! Мы выяснили, что необходимо студенту, чтобы комфортно адаптироваться в университете.Мы рады предложить тебе навигатор, который поможет в первые дни учебы)",
                                    false, 0, createKeyboardMenu(saved, false));
                            }

                            return;
                        }

                        if (message == "Назад")
                        {
                            using (DatabaseContext db = new DatabaseContext())
                            {
                                List<DSTUSaved> saved = db.Saved.ToList();

                                saved = saved.OrderBy(p => p.SortId).ToList();

                                await bot.SendPhotoAsync(e.Message.Chat.Id, photo,
                                    "Ректор рекомендует!\nСтуденту от студентов! В 2021 году Студенческий совет разработал для тебя полезного бота! Мы выяснили, что необходимо студенту, чтобы комфортно адаптироваться в университете.Мы рады предложить тебе навигатор, который поможет в первые дни учебы)",
                                    false, 0, createKeyboardMenu(saved, false));
                            }
                        }

                        using (DatabaseContext db = new DatabaseContext())
                        {
                            List<DSTUSaved> ar = db.Saved.ToList();
                            ar = ar.OrderBy(p => p.SortId).ToList();

                            foreach (var item in ar)
                            {
                                if (item.Action == message)
                                {
                                    if (item.SubActions == "null")
                                    {
                                        await bot.SendTextMessageAsync(e.Message.Chat.Id, item.OptionsJson, ParseMode.Default, false, false);
                                    }
                                    else
                                    {
                                        await bot.SendTextMessageAsync(e.Message.Chat.Id, item.OptionsJson, ParseMode.Default, false, false, 0, createSubMenu(JsonConvert.DeserializeObject<Dictionary<string, string>>(item.SubActions), true));
                                    }
                                    await bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                                    return;
                                }

                                if (item.SubActions != "null")
                                {
                                    Dictionary<string, string> subActions = JsonConvert.DeserializeObject<Dictionary<string, string>>(item.SubActions);

                                    if (subActions.ContainsKey(message))
                                    {
                                        await bot.SendTextMessageAsync(e.Message.Chat.Id, subActions[message]);
                                        await bot.DeleteMessageAsync(e.Message.Chat.Id, e.Message.MessageId);
                                    }
                                }
                            }
                        }

                    }
                    catch(Exception ex)
                    {
                        await bot.SendTextMessageAsync(e.Message.Chat.Id, ex.Message);
                    }

                };

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            bot.StartReceiving();

            while (true)
            {
                
            }

            
        }

        public static ReplyKeyboardMarkup createKeyboardMenu(List<DSTUSaved> array, bool isSubAction)
        {
            var rkm = new ReplyKeyboardMarkup();
            var rows = new List<KeyboardButton[]>();
            var cols = new List<KeyboardButton>();

            foreach (var item in array)
            {
                cols = new List<KeyboardButton>();
                cols.Add(new KeyboardButton(item.Action));
                rows.Add(cols.ToArray());
            }

            if (isSubAction)
            {
                cols = new List<KeyboardButton>();
                cols.Add(new KeyboardButton("Назад"));
                rows.Add(cols.ToArray());
            }

            rkm.Keyboard = rows.ToArray();

            return rkm;
        }
        public static ReplyKeyboardMarkup createSubMenu(Dictionary<string, string> array, bool isSubAction)
        {
            var rkm = new ReplyKeyboardMarkup();
            var rows = new List<KeyboardButton[]>();
            var cols = new List<KeyboardButton>();

            foreach (var item in array)
            {
                cols = new List<KeyboardButton>();
                cols.Add(new KeyboardButton(item.Key));
                rows.Add(cols.ToArray());
            }

            if (isSubAction)
            {
                cols = new List<KeyboardButton>();
                cols.Add(new KeyboardButton("Назад"));
                rows.Add(cols.ToArray());
            }

            rkm.Keyboard = rows.ToArray();

            return rkm;
        }
        public static InlineKeyboardMarkup BackButton(string backAction)
        {
            var rkm = new InlineKeyboardMarkup();
            var rows = new List<InlineKeyboardButton[]>();
            var cols = new List<InlineKeyboardButton>();

            cols = new List<InlineKeyboardButton>();
            cols.Add(new InlineKeyboardCallbackButton("Назад", backAction));
            rows.Add(cols.ToArray());

            rkm.InlineKeyboard = rows.ToArray();

            return rkm;
        }

    }
}
