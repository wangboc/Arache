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
    class TaskPool
    {
        public List<TaskBase> TaskList = new List<TaskBase>();

        public delegate void UpdateUIDelegate();

        private UpdateUIDelegate updateUI;

        public TaskPool()
        {
            
        }

        public void SetUpdateUIDelegate(UpdateUIDelegate function)
        {
            updateUI = function;
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
                foreach (TaskBase task in TaskList)
                {
                    task.UpdateDataByDelegate();
                }
                form.Close();
                updateUI();
                TaskList.RemoveAll(node=>node.IsFinished);
            };
            bs.RunWorkerAsync();
        }

        private void RunTask()
        {
            lock (TaskList)
            {
                foreach (TaskBase task in TaskList)
                {
                    task.Run();
                }}
            
        }
    }
}
