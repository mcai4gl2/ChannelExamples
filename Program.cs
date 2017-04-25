using System;
using System.Threading.Tasks;
using BusterWood.Channels;

namespace ChannelExamples
{
    class Program
    {
        // This replicates example from: https://gobyexample.com/channels
        static void SimpleMessage()
        {
            var channel = new Channel<String>();
            Task.Run(async () => {
                await channel.SendAsync("Hello World!");
            });
            var message = channel.Receive();
            Console.WriteLine(message);
        }

        static void Main(string[] args)
        {
            SimpleMessage();
        }
    }
}
