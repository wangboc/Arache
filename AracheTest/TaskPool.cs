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

        /// <summary>
        /// TaskFactory类可以开启后台工作线程，当A线程工作完成后，可指定B线程为后续工作
        /// 本类中，当UI发起的任务结束后，会调用TaskBase类的委托，用以刷新数据或更新界面，即后台线程与UI通信。
        /// </summary>
        /// <param name="task">需要执行的任务，用户可定制任务类，必须继承ITask接口</param>
        /// <param name="scheduler">此处实现单例模式，当用户需要开启一个任务时，需要指定该任务的上下文，从而在线程中获取更新UI的能力</param>
        public static void AddTask(TaskBase task, TaskScheduler scheduler)
        {
            if (_mainUiScheduler == null)
                _mainUiScheduler = scheduler;

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