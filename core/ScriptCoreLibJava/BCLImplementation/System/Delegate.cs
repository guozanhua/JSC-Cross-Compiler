﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib;
using ScriptCoreLibJava.BCLImplementation.System.Reflection;
using System.Reflection;

namespace ScriptCoreLibJava.BCLImplementation.System
{
    // http://referencesource.microsoft.com/#mscorlib/system/delegate.cs
    // https://github.com/mono/mono/blob/master/mcs/class/corlib/System/Delegate.cs
    // https://github.com/dot42/api/blob/master/System/Delegate.cs
    // X:\jsc.svn\core\ScriptCoreLib\JavaScript\BCLImplementation\System\Delegate.cs
    // X:\jsc.svn\core\ScriptCoreLib\ActionScript\BCLImplementation\System\Delegate.cs
    // X:\jsc.svn\core\ScriptCoreLibJava\BCLImplementation\System\Delegate.cs
    // X:\jsc.svn\core\ScriptCoreLibNative\ScriptCoreLibNative\BCLImplementation\System\Delegate.cs

    [Script(Implements = typeof(global::System.Delegate))]
    internal abstract class __Delegate
    {
        // jsc will currently look for a field with specific type...
        public object Target { get; set; }
        public MethodInfo Method { get; set; }

        public __Delegate(object e, global::System.IntPtr p)
        {
            this.Target = e;
            this.Method = new __MethodInfo { InternalMethod = ((__IntPtr)(object)p).MethodToken };
        }


        public static __Delegate Combine(__Delegate a, __Delegate b)
        {
            if (a == null)
            {
                return b;
            }
            if (b == null)
            {
                return a;
            }

            return a.CombineImpl(b);
        }

        protected virtual __Delegate CombineImpl(__Delegate d)
        {
            return default(__Delegate);
        }

        public static __Delegate Remove(__Delegate source, __Delegate value)
        {
            if (source == null)
            {
                return null;
            }
            if (value == null)
            {
                return source;
            }
            return source.RemoveImpl(value);
        }

        protected virtual __Delegate RemoveImpl(__Delegate d)
        {
            return default(__Delegate);
        }

        public virtual __Delegate[] GetInvocationList()
        {
            return default(__Delegate[]);
        }

        public override string ToString()
        {
            // https://sites.google.com/a/jsc-solutions.net/work/knowledge-base/15-dualvr/20150721/ovroculus360photoshud

            return this.GetType().Name + " " + new { this.Target, this.Method };
        }
    }
}
