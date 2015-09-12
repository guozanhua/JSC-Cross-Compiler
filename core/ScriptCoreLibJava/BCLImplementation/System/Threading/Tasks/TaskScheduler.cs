﻿using ScriptCoreLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptCoreLibJava.BCLImplementation.System.Threading.Tasks
{
    // http://referencesource.microsoft.com/#mscorlib/system/threading/Tasks/TaskScheduler.cs
    // https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Threading/Tasks/TaskScheduler.cs
    // https://github.com/mono/mono/blob/master/mcs/class/corlib/System.Threading.Tasks/TaskScheduler.cs
    // https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/threading/Tasks/TaskScheduler.cs

    // Z:\jsc.svn\core\ScriptCoreLib\JavaScript\BCLImplementation\System\Threading\Tasks\TaskScheduler.cs
    // Z:\jsc.svn\core\ScriptCoreLibJava\BCLImplementation\System\Threading\Tasks\TaskScheduler.cs

    [Script(Implements = typeof(global::System.Threading.Tasks.TaskScheduler))]
    internal class __TaskScheduler
    {
        // tested by?

        public static TaskScheduler Current { get; set; }

        // to add: WorkerTaskScheduler with url
        public static TaskScheduler Default { get; set; }

        public static TaskScheduler FromCurrentSynchronizationContext()
        {
            // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2013/201308/20130828-thread-run

            return new __TaskScheduler();
        }


        public static implicit operator TaskScheduler(__TaskScheduler e)
        {
            return (TaskScheduler)(object)e;
        }
    }

}
