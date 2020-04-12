using System;
using Skype.Client.CefSharp.OffScreen;

namespace SkypeNotifier.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                System.Console.WriteLine("Starting SkypeClient");
                var app = new SkypeCefOffScreenClient();
                app.Login(args[0], args[1]);

                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Parameters mismatch");
            }
        }
    }
}
