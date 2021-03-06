﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
[assembly: Obfuscation(Feature = "script")]

// 453
namespace Test435CoreAsDynamic
{
    public class Class1 : ScriptCoreLib.Shared.IAssemblyReferenceToken
    {
        // X:\jsc.svn\examples\rewrite\Test\Test453If\Test453If\Program.cs
        // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2015/201501/20150103/test435coreasdynamic

        public static void Invoke(object self)
        {
            // X:\jsc.svn\examples\javascript\Test\Test435CoreDynamic\Test435CoreDynamic\Class1.cs
            // X:\jsc.svn\examples\javascript\Test\Test435CoreAsDynamic\Test435CoreAsDynamic\Class1.cs

            // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2015/201501/20150103/dynamic

            // X:\jsc.svn\core\ScriptCoreLib\JavaScript\BCLImplementation\System\Threading\Tasks\Task\Task.ContinueWith.cs
            // X:\jsc.svn\core\ScriptCoreLib\JavaScript\BCLImplementation\System\Threading\Tasks\Task\Task.ctor.cs
            // X:\jsc.svn\core\ScriptCoreLib\JavaScript\BCLImplementation\System\Threading\Tasks\TaskFactory.ContinueWhenAll.cs
            // X:\jsc.svn\core\ScriptCoreLib\JavaScript\BCLImplementation\System\Type.cs
            // X:\jsc.svn\core\ScriptCoreLib\JavaScript\DOM\HTML\CanvasRenderingContext2D.cs
            // X:\jsc.svn\core\ScriptCoreLib\JavaScript\DOM\IDocument.cs
            // X:\jsc.svn\core\ScriptCoreLib\JavaScript\DOM\IStyle.cs
            // X:\jsc.svn\core\ScriptCoreLib\JavaScript\DOM\Worker.cs

            // roslyn seems to emit dynamic sites a bit differently.
            // while stack rewriter can help in merge mode
            // scriptcorelib would also lie to make use of it, with out the merge mode, only with script mode.

            //dynamic self = Native.self;
            //object value = self[item];


            // we should test all use cases for dynamic for roslyn js.

            ///* let 001b = */new Array(1);
            ///* ldloc 001b */new Array(1)[0] = twgABr0mMjmREJxf62g10A(0, null);
            //AQAABKl6eD2kr62vxldjAQ = hQkABkmHWjqHBHzPjs4Qsg(PQkABiO2_aTySKN41_aKL3ew(0, 'bar', _4R8ABtC6ljmbrk8x5kK6iA(new ctor$bR8ABhfpfj6IFLf_a4gLSZg(type$UxpR0RNEiTmJ760665b1dw)), /* 001b */new Array(1)));

            //var __001b = new Array(1);
            //__001b[0] = twgABr0mMjmREJxf62g10A(0, null);
            //AQAABKl6eD2kr62vxldjAQ = hQkABkmHWjqHBHzPjs4Qsg(PQkABiO2_aTySKN41_aKL3ew(0, 'bar', _4R8ABtC6ljmbrk8x5kK6iA(new ctor$bR8ABhfpfj6IFLf_a4gLSZg(type$UxpR0RNEiTmJ760665b1dw)), __001b));

            var bar = (self as dynamic).bar;
        }
    }
}
