using System;
using Skype.Client.CefSharp.OffScreen;

namespace Skype.Client.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                Console.WriteLine("Creating new instance of client");
                var client = new SkypeCefOffScreenClient();

                client.StatusChanged += OnAppOnStatusChanged;
                client.IncomingCall += (sender, eventArgs) => Console.WriteLine(eventArgs);
                client.CallStatusChanged += (sender, eventArgs) => Console.WriteLine(eventArgs);
                client.MessageReceived += (sender, eventArgs) => Console.WriteLine(eventArgs);

                Console.WriteLine("Starting authentication. This might take a few seconds.");

                client.Login(args[0], args[1]);

                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Parameters mismatch");
            }
        }

        private static void OnAppOnStatusChanged(object sender, StatusChangedEventArgs eventArgs)
        {
            if (eventArgs.New != AppStatus.Connected)
            {
                Console.WriteLine($"Client status: {eventArgs.New}");
            }
            else
            {
                Console.WriteLine("Ready! :). You will see incoming messages and calls on this command line shell. Press any key to exit.");
            }
        }
    }
}
