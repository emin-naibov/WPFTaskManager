using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ServerForRemoteTM
{
    public class Message
    {
        public string Type { get; set; }
        public string Data { get; set; }

    }
    class User
    {
        public string Username { get; set; }
        public StreamReader Reader { get; set; }
        public StreamWriter Writer { get; set; }
        public TcpClient Client { get; set; }
    }

    public class MyTask
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }

    class ServerForRemoteTaskManager
    {
        static List<User> clients = new List<User>();
        public static ObservableCollection<MyTask> Tasks { get; set; }
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 8080);
            Tasks = new ObservableCollection<MyTask>();

            TcpListener server = new TcpListener(endPoint);
            server.Start();

            Console.WriteLine("Server started...");
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                Task.Run(() =>
                {
                    using (var reader = new StreamReader(client.GetStream()))
                    {
                        var username = "";
                        var connected = true;
                        while (connected)
                        {
                            try
                            {
                                var json = reader.ReadLine();
                                var msg = JsonConvert.DeserializeObject<Message>(json);

                                switch (msg.Type)
                                {
                                    case "Connect":
                                  

                                        Console.WriteLine($"User {msg.Data} connected!");
                                        clients.Add(new User
                                        {
                                            Username = msg.Data,
                                            Client = client,
                                            Writer = new StreamWriter(client.GetStream())
                                        });
                                        username = msg.Data;

                                        BroadcastMessage();
                                        break;
                                    

                                    case "NewTask":
                                        Console.WriteLine(msg.Data);
                                        Process.Start(msg.Data);
                                        break;

                                    case "DeleteTask":
                                        var process = Process.GetProcessesByName(msg.Data);
                                        foreach (var item in process)
                                        {
                                            item.Kill();

                                        }
                                        break;

                                    default:
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                //clients.Remove(clients.FirstOrDefault(x => x.Username == username));
                                //Console.WriteLine($"User {username} disconnected!");
                                break;
                            }
                        }
                    }
                });

            }
        }
        static void BroadcastMessage()
        {
            Task.Run(() =>
            {
                while (true)
                {

                Thread.Sleep(300);
                foreach (var item in Process.GetProcesses())
                {
                    MyTask myTask = new MyTask { ID = item.Id, Name = item.ProcessName };
                    Tasks.Add(myTask);
                    //Console.WriteLine(item.ProcessName);
                }
                var json=JsonConvert.SerializeObject(Tasks);
                //Console.WriteLine(json);
                Console.WriteLine(clients[0].Username);
               
               
                clients[0].Writer.WriteLine(json);
                Tasks.Clear();
                //clients[0].Writer.Flush();
                }

            });

          

            
            
        }

    }
}
