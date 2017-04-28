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
        
        // This replicates example from: https://gobyexample.com/channel-synchronization
        static void ChannelSychronization() 
        {
            var channel = new Channel<bool>();
            Task.Run(async () => {
                Console.Write("Working...");
                await Task.Delay(1000);
                Console.WriteLine("done");
                await channel.SendAsync(true);
            });
            channel.ReceiveAsync().Wait();
        }

        // This replicates example from: https://gobyexample.com/select
        static void Select() 
        {
            var channel1 = new Channel<String>();
            var channel2 = new Channel<String>();

            Task.Run(async () => {
                await Task.Delay(1000);
                await channel1.SendAsync("one");
            });
            Task.Run(async () => {
                await Task.Delay(2000);
                await channel1.SendAsync("two");
            });

            for (var i = 0; i < 2; i++) 
            {
                new Select()
                    .OnReceive(channel1, msg1 => {
                        Console.WriteLine("received " + msg1);
                    })
                    .OnReceive(channel2, msg2 => {
                        Console.WriteLine("received " + msg2);
                    }).ExecuteAsync().Wait();
            }
        }

        static void Main(string[] args)
        {
            SimpleMessage();
            ChannelSychronization();
            Select();
        }
    }
}
