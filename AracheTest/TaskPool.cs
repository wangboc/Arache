using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.XtraCharts;

namespace AracheTest
{
    internal class TaskPool
    {
        private static TaskScheduler _mainUiScheduler;

        private readonly Object _objectLock = new object();
 
        /// <summary>
        /// 调用TaskPool类之前，需要调用这个函数，设定TaskScheduler，该变量必须在主线程中赋值，TaskScheduler.FromCurrentSynchronizationContext()
        /// 这样，才能使子线程通过回调函数控制主线程UI
        /// </summary>
        /// <param name="scheduler"></param>
        public static void SetScheduler(TaskScheduler scheduler)
        {
            _mainUiScheduler = scheduler;
        }

        public static void AddTask(TaskBase task)
        {
            ProgressForm form = new ProgressForm();
            form.Show();
            TaskFactory taskFactory = new TaskFactory();
            Task A = taskFactory.StartNew(task.Run);
            Task B = A.ContinueWith(t =>
            {
                task.UpdateDataByDelegate();
                form.Close();
            }, _mainUiScheduler);
        }
    }
}