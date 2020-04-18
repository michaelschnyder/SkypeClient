using System;
using System.Linq;
using System.Threading.Tasks;
using Skype.Client;
using Skype.Client.CefSharp.OffScreen;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace SkypeNotifier.Cmd
{
    class Program
    {
        private static TelegramBotClient bot;
        private static string telegramTarget;

        static async Task Main(string[] args)
        {
            if (args.Length >= 3)
            {
                await ConfigureTelegram(args);

                var client = new SkypeCefOffScreenClient();

                client.StatusChanged += (sender, eventArgs) => Console.WriteLine($"Status: {client.Status}");
                client.IncomingCall += ClientOnIncomingCall;
                client.CallStatusChanged += (sender, eventArgs) => Console.WriteLine(eventArgs);
                client.MessageReceived += (sender, eventArgs) => Console.WriteLine(eventArgs);

                client.Login(args[0], args[1]);

                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Parameters mismatch");
            }
        }

        private static async Task ConfigureTelegram(string[] args)
        {
            bot = new TelegramBotClient(args[2]);

            var me = await bot.GetMeAsync();
            Console.WriteLine($"Telegram Bot started. I am user {me.Id} and my name is {me.FirstName}.");

            var updates = await bot.GetUpdatesAsync();
            telegramTarget = "";
            if (args.Length == 4)
            {
                telegramTarget = args[3];
            }

            if (string.IsNullOrWhiteSpace(telegramTarget) || !updates.Any(u =>
                    u.Message?.Chat?.Id.ToString() == telegramTarget || u.ChannelPost?.Chat?.Id.ToString() == telegramTarget))
            {
                var chats = updates.Where(u => u.Message != null && u.Message.Chat.Type == ChatType.Private);
                foreach (var update in chats)
                {
                    Console.WriteLine(
                        $"Bot has access to chat with '{update.Message.Chat.FirstName}', id: {update.Message.Chat.Id}");
                }

                var groups = updates.Where(u => u.Message != null && u.Message.Chat.Type == ChatType.Group);
                foreach (var update in groups)
                {
                    Console.WriteLine(
                        $"Bot has access to group chat named '{update.Message.Chat.Title}', id: {update.Message.Chat.Id}");
                }

                var channels = updates.Where(u => u.ChannelPost != null);
                foreach (var update in channels)
                {
                    Console.WriteLine(
                        $"Bot has access to channel '{update.ChannelPost.Chat.Title}' with id: {update.ChannelPost.Chat.Id}");
                }

                Console.WriteLine("Unknown or not specified target for telegram notifications.");
            }
        }

        private static void ClientOnIncomingCall(object? sender, CallEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(telegramTarget))
                {
                    Console.WriteLine("Incoming call. Telegram not configured");
                    return;
                }

                Console.WriteLine($"Incoming call. Sending notification to {telegramTarget}");
                var result = bot.SendTextMessageAsync(telegramTarget, $"New Call from {e.CallerName}").Result;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private static void ClientOnStatusChanged(object? sender, StatusChangedEventArgs e)
        {
            if (e.New == AppStatus.Ready)
            {
                Console.WriteLine("Connected");
            }
        }
    }
}
