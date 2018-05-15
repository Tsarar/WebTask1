using System;
using System.Threading;
using RabbitMQ.Client;
using WebTask1.Storages;

namespace WebTask1.RabbitMQ
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new RabbitServer();
            server.StartAsync(CancellationToken.None);

            WaitCtrlPlusC();
        }

        private static void WaitCtrlPlusC()
        {
            var isRun = true;

            Console.CancelKeyPress += (sender, e) =>
            {
                isRun = false;
                e.Cancel = true;
            };

            while (isRun)
            {
                Thread.Sleep(100);
            }
        }
    }
}
