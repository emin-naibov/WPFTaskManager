using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace WpfAppTaskManager
{
    class MainViewModel:ViewModelBase
    {
        
    private ObservableCollection<MyTask> tasks;
    public ObservableCollection<MyTask> Tasks { get => tasks; set => Set(ref tasks, value); }
        private MyTask _selectedTask;
        public MyTask SelectedTask { get => _selectedTask; set => Set(ref _selectedTask, value); }


        public class MyTask
        {
            public int ID { get; set; }
            public string Name { get; set; }

        }


        public MainViewModel()
        {
            Tasks = new ObservableCollection<MyTask>();
            SelectedTask = new MyTask();
            InitMethod();
        }

    private void InitMethod()
        {
            foreach (var item in Process.GetProcesses())
            {
                MyTask myTask = new MyTask { ID = item.Id, Name = item.ProcessName };
                Tasks.Add(myTask);
            }
            

        }

        private RelayCommand<MyTask> _deletecommand;
        public RelayCommand<MyTask> DeleteCommand => _deletecommand ??= new RelayCommand<MyTask>(
            param =>
            {
                if (param != null)
                {
                    Tasks.Remove(param);
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
            }
        );

        //private RelayCommand _deleteCommand;
        //public RelayCommand DeleteCommand => _deleteCommand ??= new RelayCommand(
        //    ()=>
        //    {
        //        if(SelectedTask!=null)
        //        {
        //            Tasks.Remove(SelectedTask);
        //            MessageBox.Show(SelectedTask.Name);

        //            //var process = Process.GetProcessesByName(SelectedTask.Name);
        //            //foreach (var item in process)
        //            //{
        //            //    item.Kill();

        //            //}

        //        }

        //        else
        //        {
        //            MessageBox.Show("Please select the task!");
        //        }
        //    }
        //);
    }

}
