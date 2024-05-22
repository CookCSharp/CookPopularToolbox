/*
 *Description: ThreadPerTask
 *Author: Chance.zheng
 *Creat Time: 2023/11/28 14:18:23
 *.Net Version: 8.0
 *CLR Version: 4.0.30319.42000
 *Copyright © CookCSharp 2023 All Rights Reserved.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressTool
{
    public class ThreadPerTask : TaskScheduler
    {
        protected override void QueueTask(Task task)
        {
            new Thread(() => this.TryExecuteTask(task))
            {
                IsBackground = true
            }.Start();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

        protected override IEnumerable<Task> GetScheduledTasks() { yield break; }
    }
}
