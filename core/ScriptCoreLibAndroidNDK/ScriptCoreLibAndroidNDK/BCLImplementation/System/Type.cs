﻿using ScriptCoreLib;
using ScriptCoreLibAndroidNDK.BCLImplementation.System.Reflection;
using ScriptCoreLibNative.SystemHeaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptCoreLibAndroidNDK.BCLImplementation.System
{
    // z:\jsc.svn\core\ScriptCoreLibAndroidNDK\ScriptCoreLibAndroidNDK\BCLImplementation\System\Type.cs
    // z:\jsc.svn\core\ScriptCoreLib\ActionScript\BCLImplementation\System\Type.cs
    // z:\jsc.svn\core\ScriptCoreLibJava\BCLImplementation\System\Type.cs
    // z:\jsc.svn\core\ScriptCoreLib\JavaScript\BCLImplementation\System\Type.cs


    [Script(Implements = typeof(global::System.Type))]
    public class __Type //: __MemberInfo, __IReflect
    {
        // jsc owned NDK process mallocs bytearrays as le+bytes
        // this enables the use of ldlen
        // Z:\jsc.svn\examples\c\android\NDKUdpClient\xNativeActivity.cs



        // Z:\jsc.svn\examples\java\android\future\NDKHybridMockup\NDKHybridMockup\ApplicationActivity.cs
        // Z:\jsc.svn\examples\java\android\synergy\OVROculus360PhotosNDK\OVROculus360PhotosNDK\xNativeActivity.cs

        // jsc should automatically make typeof(this) available
        // my rendering them to arguments:

        public JNIEnv arg0_env;
        public jclass arg1_type;

        public jmethodID GetMethodID(string name, string sig)
        {
            return arg0_env.GetMethodID(arg0_env, arg1_type, name, sig);
        }

        public __MethodInfo GetMethod(string name, string sig)
        {
            // no gc. remember.

            // process end, gc collects
            return new __MethodInfo
            {
                InternalDeclaringType = this,

                methodName = name,
                methodSignature = sig,

                methodID = arg0_env.GetMethodID(arg0_env, arg1_type, name, sig)
            };
        }



        public static global::System.Type GetTypeFromHandle(RuntimeTypeHandle TypeHandle)
        {
            //var e = new __Type
            //{
            //    _TypeHandle = TypeHandle
            //};

            //return (global::System.Type)(object)e;

            return null;
        }

    }
}
