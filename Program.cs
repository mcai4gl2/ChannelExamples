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
        
        // This replicates example from: https://gobyexample.com/timeouts
        static void Timeouts() 
        {
            var channel1 = new Channel<String>();
            Task.Run(async () => {
                await Task.Delay(2000);
                await channel1.SendAsync("result 1");
            });
            new Select()
                .OnReceive(channel1, msg => {
                    Console.WriteLine(msg);
                })
                .ExecuteAsync(TimeSpan.FromSeconds(1))
                .ContinueWith(timedOut => {
                    if (timedOut.Result) {
                        Console.WriteLine("timeout 1");
                    }
                }).Wait();
            
            var channel2 = new Channel<String>();
            Task.Run(async () => {
                await Task.Delay(2000);
                await channel2.SendAsync("result 2");
            });
            new Select()
                .OnReceive(channel2, msg => {
                    Console.WriteLine(msg);
                })
                .ExecuteAsync(TimeSpan.FromSeconds(3))
                .ContinueWith(timedOut => {
                    if (timedOut.Result) {
                        Console.WriteLine("timeout 2");
                    }
                }).Wait();
        }
        
        // This replicates example from: https://gobyexample.com/non-blocking-channel-operations
        static void NonBlockingChannels()
        {
            var messages = new Channel<String>();

            String message;
            if (messages.TryReceive(out message)) {
                Console.WriteLine("received message " + message);
            } else {
                Console.WriteLine("no message received");
            }

            message = "hi";
            if (messages.TrySend(message)) {
                Console.WriteLine("sent message ", message);
            } else {
                Console.WriteLine("no message sent");
            }
        }

        static void Main(string[] args)
        {
            SimpleMessage();
            ChannelSychronization();
            Select();
            Timeouts();
            NonBlockingChannels();
        }
    }
}
