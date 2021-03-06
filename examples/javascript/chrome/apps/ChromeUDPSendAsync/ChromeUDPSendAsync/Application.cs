using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.JavaScript;
using ScriptCoreLib.JavaScript.Components;
using ScriptCoreLib.JavaScript.DOM;
using ScriptCoreLib.JavaScript.DOM.HTML;
using ScriptCoreLib.JavaScript.Extensions;
using ScriptCoreLib.JavaScript.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ChromeUDPSendAsync;
using ChromeUDPSendAsync.Design;
using ChromeUDPSendAsync.HTML.Pages;
using System.Net.Sockets;
using System.Net;

namespace ChromeUDPSendAsync
{
    /// <summary>
    /// Your client side code running inside a web browser as JavaScript.
    /// </summary>
    public sealed class Application : ApplicationWebService
    {
        // test via
        // X:\jsc.svn\examples\java\android\AndroidServiceUDPNotification\AndroidServiceUDPNotification\ApplicationActivity.cs

        // X:\jsc.svn\examples\javascript\chrome\apps\ChromeUDPSendAsync\ChromeUDPSendAsync\bin\Debug\staging\ChromeUDPSendAsync.Application\web
        // subst b: r:\jsc.svn\examples\javascript\chrome\apps\ChromeUDPSendAsync\ChromeUDPSendAsync\bin\Debug\staging\ChromeUDPSendAsync.Application\web

        // http://blog.boehme.me/2011/06/nat-holepunch-solution-in-java.html

        /// <summary>
        /// This is a javascript application.
        /// </summary>
        /// <param name="page">HTML document rendered by the web server which can now be enhanced.</param>
        public Application(IApp page)
        {
            // x:\jsc.svn\examples\javascript\chrome\apps\chromeudpsendasync\chromeudpsendasync\application.cs
            // reload on idle?
            // edit and continue over udp?

            // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2015/201503/20150306/udp

            #region += Launched chrome.app.window
            dynamic self = Native.self;
            dynamic self_chrome = self.chrome;
            object self_chrome_socket = self_chrome.socket;

            if (self_chrome_socket != null)
            {
                if (!(Native.window.opener == null && Native.window.parent == Native.window.self))
                {
                    Console.WriteLine("chrome.app.window.create, is that you?");

                    // pass thru
                }
                else
                {
                    // should jsc send a copresence udp message?
                    chrome.runtime.UpdateAvailable += delegate
                    {
                        new chrome.Notification(title: "UpdateAvailable");

                    };

                    chrome.app.runtime.Launched += async delegate
                    {
                        // 0:12094ms chrome.app.window.create {{ href = chrome-extension://aemlnmcokphbneegoefdckonejmknohh/_generated_background_page.html }}
                        Console.WriteLine("chrome.app.window.create " + new { Native.document.location.href });

                        new chrome.Notification(title: "ChromeUDPSendAsync");

                        var xappwindow = await chrome.app.window.create(
                               Native.document.location.pathname, options: null
                        );

                        //xappwindow.setAlwaysOnTop

                        xappwindow.show();

                        await xappwindow.contentWindow.async.onload;

                        Console.WriteLine("chrome.app.window loaded!");
                    };


                    return;
                }
            }
            #endregion

            // X:\jsc.svn\examples\javascript\chrome\apps\ChromeNetworkInterfaces\ChromeNetworkInterfaces\Application.cs

            //{{ Length = 4 }}
            //{{ prefixLength = 64, name = {D7020941-742E-4570-93B2-C0372D3D870F}, address = fe80::88c0:f0a:9ccf:cba0 }}
            //{{ prefixLength = 24, name = {D7020941-742E-4570-93B2-C0372D3D870F}, address = 192.168.43.28 }}
            //{{ prefixLength = 64, name = {A8657A4E-8BFA-41CC-87BE-6847E33E1A81}, address = 2001:0:9d38:6abd:20a6:2815:3f57:d4e3 }}
            //{{ prefixLength = 64, name = {A8657A4E-8BFA-41CC-87BE-6847E33E1A81}, address = fe80::20a6:2815:3f57:d4e3 }}

            new { }.With(
                async delegate
                {
                    // http://css-infos.net/property/-webkit-user-select
                    // http://caniuse.com/#feat=user-select-none
                    //(Native.body.style as dynamic).userSelect = "auto";
                    (Native.document.body.style as dynamic).webkitUserSelect = "auto";
                    Native.document.documentElement.style.overflow = IStyle.OverflowEnum.auto;


                    // https://css-tricks.com/almanac/properties/u/user-select/
                    //Native.body.style.setProperty(
                    // X:\jsc.svn\examples\java\hybrid\JVMCLRNIC\JVMCLRNIC\Program.cs
                    // clr does not have it async. 

                    #region xml

                    var nmessage = "hello world";
                    var Host = "";
                    var PublicPort = "";

                    var message =
                        new XElement("string",
                            new XAttribute("c", "" + 1),
                            new XAttribute("n", nmessage),
                            "Visit me at " + Host + ":" + PublicPort
                        ).ToString();

                    #endregion



                    new IHTMLHorizontalRule { }.AttachToDocument();


                    // Z:\jsc.svn\examples\javascript\chrome\hybrid\HybridHopToUDPChromeApp\Application.cs
                    var n = await chrome.socket.getNetworkList();

                    new IHTMLPre { new { n.Length } }.AttachToDocument();

                    foreach (var item in n)
                    {
                        new IHTMLPre { new { item.prefixLength, item.name, item.address } }.AttachToDocument();

                        new IHTMLButton { "send " + item.address }.AttachToDocument().With(
                            async refresh =>
                            {

                                // experimental until ref count 33?
                                while (await refresh.async.onclick)
                                {
                                    new IHTMLHorizontalRule { }.AttachToDocument();
                                    // X:\jsc.svn\examples\merge\TestDetectOpenFiles\TestDetectOpenFiles\Program.cs
                                    // X:\jsc.svn\examples\javascript\chrome\apps\MulticastListenExperiment\MulticastListenExperiment\Application.cs

                                    // https://code.google.com/p/chromium/issues/detail?id=455352

                                    // X:\jsc.svn\examples\merge\TestDetectOpenFiles\TestDetectOpenFiles\Program.cs

                                    // bind?

                                    var data = Encoding.UTF8.GetBytes(message);	   //creates a variable b of type byte

                                    // http://stackoverflow.com/questions/13691119/chrome-packaged-app-udp-sockets-not-working
                                    // http://www.chinabtp.com/how-to-do-udp-broadcast-using-chrome-sockets-udp-api/

                                    // chrome likes 0 too.
                                    var port = new Random().Next(16000, 40000);
                                    //var port = 0;
                                    // 
                                    new IHTMLPre { "about to bind... " + new { port } }.AttachToDocument();

                                    // where is bind async?
                                    var socket = new UdpClient();
                                    socket.Client.Bind(

                                        //new IPEndPoint(IPAddress.Any, port: 40000)
                                        new IPEndPoint(IPAddress.Parse(item.address), port)
                                    );


                                    new IHTMLPre { "about to send... " + new { data.Length } }.AttachToDocument();

                                    // X:\jsc.svn\examples\javascript\chrome\apps\ChromeUDPNotification\ChromeUDPNotification\Application.cs
                                    var s = await socket.SendAsync(
                                        data,
                                        data.Length,
                                        hostname: "239.1.2.3",
                                        port: 40804
                                    );

                                    new IHTMLPre { "sent: " + s }.AttachToDocument();


                                    //socket.ReceiveAsync
                                    //socket.Close();

                                    //new IHTMLPre { $"sent: {s}" }.AttachToDocument();

                                    // android cannot see it. why? because it needs to know which NIC to use.
                                }
                            }
                        );

                    }

                }
            );
        }

    }
}
