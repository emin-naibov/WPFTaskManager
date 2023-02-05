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

        StreamWriter writer;
        StreamReader reader;
        private string username = "Gleb";
        public string Username { get => username; set => Set(ref username, value); }

        private string _newTask;
        public string NewTask { get => _newTask; set => Set(ref _newTask, value); }


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
                var client = new TcpClient();

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(Ip), 8080);
                client.Connect(endPoint);
                writer = new StreamWriter(client.GetStream());
                writer.AutoFlush = true;
                 Task.Run(() =>
                 {
                       
                        var msg = new Message { Type = "Connect",Data=Username};
                        var json = JsonConvert.SerializeObject(msg);
                        writer.WriteLine(json);
                       
                        reader = new StreamReader(client.GetStream());
                       
                       
                        RecieveMessages();
                 });
            
        }


        private RelayCommand _sendCommand;
        public RelayCommand SendTaskCommand => _sendCommand ??= new RelayCommand(
            () =>
            {
                Task.Run(() =>
                {
                    var msg = new Message { Type = "NewTask", Data = NewTask };
                    Sender(msg);
                });
                    
                    
            }
        );

        private RelayCommand<MyTask> _deletecommand;
        public RelayCommand<MyTask> DeleteCommand => _deletecommand ??= new RelayCommand<MyTask>(
            param =>
            {
                Task.Run(() =>
                {
                    if (param != null)
                    {
                        var msg = new Message { Type = "DeleteTask", Data = param.Name };
                        Sender(msg);
                      
                    }
                    else
                    {
                        MessageBox.Show("please select the task!");
                    }

                });
            }
        );
        private void Sender(Message message)
        {
            var json = JsonConvert.SerializeObject(message);
            writer.WriteLine(json);
            writer.AutoFlush = true;
        }
        private void RecieveMessages()
        {

            Task.Run(() =>
            {
                while (true)
              {
                  try
                   {

                          var json = reader.ReadLine();
                          //MessageBox.Show(json);
                          Application.Current.Dispatcher.Invoke(() =>
                          {
                             Tasks = JsonConvert.DeserializeObject<List<MyTask>>(json);
                             //MessageBox.Show(Tasks[0].Name);
                          });
                    }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                      throw;
                  }
               }
                
            });
        }

        
    }

}



