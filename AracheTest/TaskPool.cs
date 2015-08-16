using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.XtraCharts;

namespace AracheTest
{
    internal class TaskPool
    {
        public List<TaskBase> TaskList = new List<TaskBase>();

        public delegate void UpdateUIDelegate();


        private readonly Object _objectLock = new object();

        public TaskPool()
        {
        }


        public void AddTask(TaskBase task)
        {
            TaskList.Add(task);
        }

        public int TaskCount()
        {
            return TaskList.Count;
        }

        public void RemoveTask(TaskBase task)
        {
            TaskList.Remove(task);
        }

        public void Run()
        {
            BackgroundWorker bs = new BackgroundWorker();
            ProgressForm form = new ProgressForm();
            bs.DoWork += delegate { RunTask(); };
            form.Show();
            bs.RunWorkerCompleted += delegate
            {
                for (int i = 0; i < TaskList.Count; i++)
                {
                    TaskList[i].UpdateDataByDelegate();
                }
                form.Close();
                TaskList.RemoveAll(node => node.IsFinished);
            };
            bs.RunWorkerAsync();
        }

        private void RunTask()
        {
            for (int i = 0; i < TaskList.Count; i++)
            {
                TaskList[i].Run();
            }
        }
    }
}