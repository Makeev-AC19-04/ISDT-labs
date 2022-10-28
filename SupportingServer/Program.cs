using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Data.SQLite;
using Microsoft.EntityFrameworkCore;


namespace SupportingServer
{
    class Program
    {
        static void ProcessMessages()
        {
            using SQLiteConnection dbc = new SQLiteConnection("Data Source = C:/Users/wane2/Documents/Учеба/ТРИС/MyMsgSockets/x64/Debug/net6.0/ServerDb.db; Version = 3");
            dbc.Open();
            while (true)
            {
                var m = Message.send(MessageRecipients.MR_BROKER, MessageTypes.MT_GETDATA);
                using SQLiteCommand command = dbc.CreateCommand();
                switch (m.header.type)
                {
                    case MessageTypes.MT_DATA:
                        Console.WriteLine("New message: " + m.data);
                        Console.WriteLine("From: " + m.header.from);
                        Console.WriteLine("To: " + m.header.to + "\n");
                        command.CommandText = "insert into Messages(data, to_client, from_client) values (@data, @to, @from);";
                        command.Parameters.Add("@data", System.Data.DbType.String).Value = m.data;
                        command.Parameters.Add("@to", System.Data.DbType.String).Value = m.header.to;
                        command.Parameters.Add("@from", System.Data.DbType.String).Value = m.header.from;
                        if (m.header.to != MessageRecipients.MR_SUPSERVER && m.header.from != MessageRecipients.MR_SUPSERVER)
                        {
                            command.ExecuteNonQuery();
                        }
                        break;
                    case MessageTypes.MT_LAST_MESSAGES:
                        {
                            command.CommandText = "select * from Messages where to_client = @clientid or (to_client = 'MR_ALL' and from_client <> @clientid)";
                            command.Parameters.Add("@clientid", System.Data.DbType.String).Value = m.header.from;
                            using SQLiteDataReader result = command.ExecuteReader();
                            if (result.HasRows)
                            {
                                while (result.Read())
                                {
                                    string msgtext = "From: " + result["from_client"] + "\nText: " + result["data"];
                                    string s = (string)result["from_client"];
                                    int from = Int32.Parse(s);

                                    string t = (string)result["to_client"];
                                    int to;
                                    if (t == "MR_ALL") to = (int)MessageRecipients.MR_ALL;
                                    else to = Int32.Parse(t);
                                    Console.WriteLine("Client: " + t);
                                    m.data = (string)result["data"];
                                    Message.sendPrevMessage((MessageRecipients)to, (MessageRecipients)from, MessageTypes.MT_HISTORY, m.data);
                                    Console.WriteLine(msgtext);
                                }
                            }
                            else
                            {
                                Message.send(m.header.from, MessageTypes.MT_DATA, "This client doesn't have any recent messages\n");
                            }
                            break;
                        }
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
            //Console.WriteLine("Sharp client has started");
            var m = Message.send(MessageRecipients.MR_BROKER, MessageTypes.MT_INIT_SUPSERVER);
        }
    }
}
