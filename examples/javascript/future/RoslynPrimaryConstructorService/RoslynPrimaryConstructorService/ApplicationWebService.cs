using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;
using ScriptCoreLib.JavaScript.DOM.HTML;

namespace RoslynPrimaryConstructorService
{
    /// <summary>
    /// Methods defined in this type can be used from JavaScript. The method calls will seamlessly be proxied to the server.
    /// </summary>
    public class ApplicationWebService(
        // half way there. jsc does not exactly send in the ctor params just yet tho.
        public string ClientTag = "xClientTag",
        public Action<string> yield = null
        ) //: Delegate
    {

        /// <summary>
        /// This Method is a javascript callable method.
        /// </summary>
        /// <param name="e">A parameter from javascript.</param>
        /// <param name="y">A callback to javascript.</param>
        public void Invoke(string InvokeTag)
        {
            Console.WriteLine(new { ClientTag, InvokeTag });
            //Debugger.Break();


            yield(new { ClientTag, InvokeTag }.ToString());


            // if we were to talk with the DOM API. we could allow it if proxied correctly?
            //new IHTMLPre { new { x } }.AttachToDocument();

            // async continuation
            //return this;


        }

    }
}
