using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppTaskManager
{
    public class Message
    {
        public string Type { get; set; }
        public string Data { get; set; }

    }
    class MainViewModel:ViewModelBase
    {
        
    private List<MyTask> tasks;
    public List<MyTask> Tasks { get => tasks; set => Set(ref tasks, value); }

        TcpClient client;
        StreamWriter writer;
        StreamReader reader;
        private string username = "Gleb";
        public string Username { get => username; set => Set(ref username, value); }

        private string ip = "192.168.56.1";
        public string Ip { get => ip; set => Set(ref ip, value); }
        private MyTask _selectedTask;
        public MyTask SelectedTask { get => _selectedTask; set => Set(ref _selectedTask, value); }


        public class MyTask
        {
            public int ID { get; set; }
            public string Name { get; set; }

        }


        public MainViewModel()
        {
            Tasks = new List<MyTask>();
            SelectedTask = new MyTask();
            InitMethod();
        }

    private void InitMethod()
        {
            Task.Run(() =>
            {

                var client = new TcpClient();

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(Ip), 8080);
                client.Connect(endPoint);
                //Connected = true;

                var writer = new StreamWriter(client.GetStream());
                writer.AutoFlush = true;

                var msg = new Message { Type = "Connect",Data=Username};
                var json = JsonConvert.SerializeObject(msg);
                writer.WriteLine(json);

                reader = new StreamReader(client.GetStream());

                RecieveMessages();
            });
            

        }

        private RelayCommand<MyTask> _deletecommand;
        public RelayCommand<MyTask> DeleteCommand => _deletecommand ??= new RelayCommand<MyTask>(
            param =>
            {
                Task.Run(() =>
                {
                    if (param != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                           {
                               Tasks.Remove(param);
                           });
                        //Tasks.Remove(param);
                        var process = Process.GetProcessesByName(param.Name);
                        foreach (var item in process)
                        {
                            item.Kill();

                        }
                    }
                    else
                    {
                        MessageBox.Show("please select the task!");
                    }

                });
            }
        );
        private void RecieveMessages()
        {
            Task.Run(() =>
            {
                var json = reader.ReadLine();
                Tasks = JsonConvert.DeserializeObject<List<MyTask>>(json);
                MessageBox.Show(Tasks[0].Name);
            });
        }

        
    }

}


//var msg = reader.ReadLine();
//MyTask myTask =  new MyTask {JsonConvert.DeserializeObject<MyTask>(msg)};

//Application.Current.Dispatcher.Invoke(() =>
//{
//Tasks=JsonSerializer.Deserialize<List<MyTask>>(json);
