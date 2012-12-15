using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using System;
using System.Linq;
using System.Xml.Linq;

namespace TestDynamicObject
{
    /// <summary>
    /// Methods defined in this type can be used from JavaScript. The method calls will seamlessly be proxied to the server.
    /// </summary>
    public sealed class ApplicationWebService
    {
        /// <summary>
        /// This Method is a javascript callable method.
        /// </summary>
        /// <param name="e">A parameter from javascript.</param>
        /// <param name="y">A callback to javascript.</param>
        public void WebMethod2(string e, Action<string> y)
        {
            var x = e;

            // cannot use e directly just yet
            X.foo(ref x);

            // Send it back to the caller.
            y(e);
        }

    }

    static class X
    {
        public static void foo(ref string e)
        {
            e = "ref " + e;

        }
    }

}
