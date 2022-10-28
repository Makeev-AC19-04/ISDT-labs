using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SocketSharpClient
{
    class Program
    {
        static void ProcessMessages()
        {
            while (true)
            {
                var m = Message.send(MessageRecipients.MR_BROKER, MessageTypes.MT_GETDATA);
                switch (m.header.type)
                {
                    case MessageTypes.MT_DATA:
                        Console.WriteLine("New message: " + m.data);
                        Console.WriteLine("From: " + m.header.from);
                        break;
                    default:
                        Thread.Sleep(100);
                        break;
                }
            }
        }
        static void Main(string[] args)
        {
            Thread t = new Thread(ProcessMessages);
            t.Start();
            Console.WriteLine("Sharp client has started");
            var m = Message.send(MessageRecipients.MR_BROKER, MessageTypes.MT_INIT);
            Message.send(MessageRecipients.MR_SUPSERVER, MessageTypes.MT_LAST_MESSAGES);
            while (true)
            {
                Console.WriteLine("Menu:\n1.Send to all\n2.Send to one\n3. Exit");
                int menu;
                if (int.TryParse(Console.ReadLine(), out menu))
                {
                    switch (menu)
                    {
                        case 1:
                            {
                                Console.WriteLine("Enter your message");
                                var s = Console.ReadLine();
                                if (s is not null)
                                {
                                    Message.send(MessageRecipients.MR_ALL, MessageTypes.MT_DATA, s);
                                }
                                break;
                            }
                        case 2:
                            {
                                Console.WriteLine("Enter client's id");
                                int id = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("Enter your message");
                                var s = Console.ReadLine();
                                if (s is not null & id != 0)
                                {
                                    Message.send((MessageRecipients)id, MessageTypes.MT_DATA, s);
                                }
                                break;
                            }
                        case 3:
                            {
                                Message.send(MessageRecipients.MR_BROKER, MessageTypes.MT_EXIT);
                                System.Environment.Exit(0);
                                break;
                            }
                    }
                }
            }
        }
    }
}
