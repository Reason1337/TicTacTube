using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using NYoutubeDL.Helpers;
using TagLib;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;
using TicTacTubeCore.Sources.Files;
using TicTacTubeCore.Telegram.Schedulers;
using TicTacTubeCore.YoutubeDL.Sources.Files.External;
using File = System.IO.File;

namespace TicTacTubeDemo
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
			XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            var scheduler = new TelegramScheduler(File.ReadAllText("telegram.token"));

			scheduler.Start();

			scheduler.Join();
		}

		private class TelegramScheduler : TelegramBotBaseScheduler
		{
			private static readonly ILog Log = LogManager.GetLogger(typeof(TelegramScheduler));

            public TelegramScheduler(string apiToken, UserList userList = UserList.None, IWebProxy proxy = null) : base(apiToken,
				userList, proxy)
			{
				WelcomeText = "Hey there! Just send me youtube links ... :)";
			}

            protected override bool ProcessCustomCommands(Message message)
            {
                if(message.Text.StartsWith("/test")) {
                    SendTextMessage(message, "Heyho");
                }
                return true;
            }

            protected async override void BotOnInlineQuery(object sender, InlineQueryEventArgs inlineQueryEventArgs)
            {
                Log.Info("Inline called " + inlineQueryEventArgs.InlineQuery.From.FirstName);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("1.1"),
                            InlineKeyboardButton.WithCallbackData("1.2"),
                        },
                        new [] // second row
                        {
                            InlineKeyboardButton.WithCallbackData("2.1"),
                            InlineKeyboardButton.WithCallbackData("2.2"),
                        }
                    });

                await BotClient.SendTextMessageAsync(
                    inlineQueryEventArgs.InlineQuery.Id,
                    "Choose",
                    replyMarkup: inlineKeyboard);

            }

            protected override void ProcessTextMessage(Message message)
			{
				Task.Run(() =>
				{
					try
					{
						SendTextMessage(message, "brb ...");

						Log.Info($"{message.From.Username} + {message.From.FirstName} requested {message.Text}");
						var youtubeSource = new YoutubeDlSource(message.Text, Enums.AudioFormat.mp3, true);
						IFileSource source = new FileSource(youtubeSource,
							Path.Combine(Path.GetTempPath(), "TicTacTube"));

						Execute(source);

						Log.Info($"{message.From.Username} + {message.From.FirstName} downloaded {source.FileName}");

						var f = TagLib.File.Create(source.FileInfo.FullName, ReadStyle.Average);

						SendTextMessage(message,
							$"{source.FileName}\nTitle:\t{f.Tag.Title}\nArtists:\t{string.Join(", ", f.Tag.Performers)}");


						BotClient.SendAudioAsync(message.Chat.Id,
							new FileToSend(source.FileInfo.FullName, File.OpenRead(source.FileInfo.FullName)), f.Tag.Lyrics,
							(int) f.Properties.Duration.TotalSeconds, string.Join(' ', f.Tag.Performers), f.Tag.Title);
					}
					catch (Exception e)
					{
						SendTextMessage(message, e.Message);
						Log.Error(e);
					}
				});
			}
		}
	}
}