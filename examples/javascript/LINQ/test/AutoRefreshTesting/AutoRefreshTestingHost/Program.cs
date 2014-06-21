﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoRefreshTestingHost
{
    class Program
    {
        static Action wDispose = delegate { };
        static Process review;

        // called by post build event
        static void InternalMain(string[] args)
        {
            // we managed to get the signal from post build event into our zombie console.
            // congratz.

            Console.WriteLine("enter InternalMain");

            wDispose();
            Console.WriteLine("enter InternalMain wDispose.");

            // are we already watching the filesystem?
            // reset monitoring..

            #region security rebuild done
            foreach (var exe in args.Where(x => x.EndsWith(".exe")))
            {
                if (review != null)
                {
                    //Unhandled Exception: System.InvalidOperationException: Process has exited, so the requested information is not available.
                    //   at System.Diagnostics.Process.EnsureState(State state)
                    //   at System.Diagnostics.Process.get_MainWindowHandle()
                    //   at System.Diagnostics.Process.CloseMainWindow()

                    if (!review.HasExited)
                        review.CloseMainWindow();
                }

                // are we supposed to run this thing?
                // do we have a whitelist we can do a security scan for atleast?

                Console.WriteLine(new { exe });
                // { exe = X:\jsc.svn\examples\javascript\LINQ\test\AutoRefreshTesting\AutoRefreshTesting\bin\Debug\AutoRefreshTesting.exe }

                // can we do a security rewrite?
                //   // jsc.meta.exe RewriteToAssembly /EntryPointAssembly:"jsc.meta.exe" /AssemblyMerge:"jsc.meta.exe" /AssemblyMerge:"jsc.exe"  /Output:"jscc.exe"

                //var count = Directory.GetFiles(Path.GetDirectoryName(exe), "*.exe").Count();
                //var Output = exe + "." + count + ".exe";
                //var ww = Stopwatch.StartNew();

                //{
                //    var p = Process.Start(
                //         new ProcessStartInfo(@"c:/util/jsc/bin/jsc.meta.exe")
                //         {
                //             Arguments = "RewriteToAssembly /EntryPointAssembly:" + exe + " /AssemblyMerge:" + exe + " /Output:" + Output,

                //             // can we share our console with that sub process?
                //             UseShellExecute = false,
                //         }
                //     );

                //    p.WaitForExit();
                //}
                //Console.WriteLine("security rebuild done! " + new { ww.ElapsedMilliseconds });
                // we will loose pdb data. line numbers will be missing! can jsc keep them?

                var old = new { Console.ForegroundColor, Console.BackgroundColor };
                Console.Clear();

                {
                    // run the new version
                    var p = Process.Start(
                        //new ProcessStartInfo(Output)
                         new ProcessStartInfo(exe)
                         {
                             WorkingDirectory = Path.GetDirectoryName(exe),

                             // can we share our console with that sub process?
                             UseShellExecute = false,

                         }
                     );



                    p.WaitForExit();
                }
                Console.ForegroundColor = old.ForegroundColor;
                Console.BackgroundColor = old.BackgroundColor;

                //{
                //    Console.WriteLine();

                //    // review the IL
                //    // when can jsc start highlighting diff?
                //    review = Process.Start(
                //         new ProcessStartInfo(@"c:/util/jsc/bin/jsc.meta.exe")
                //         {
                //             Arguments = Output,

                //             // can we share our console with that sub process?
                //             UseShellExecute = false,
                //         }
                //     );

                //    // when to close it?

                //    //p.WaitForExit();
                //}
            }
            #endregion

            var rebuilding = false;

            #region yFileSystemWatcher
            Action<string> yFileSystemWatcher =
                path =>
                {
                    //Console.WriteLine(new { path });

                    //var w = new FileSystemWatcher(path, "*.cs");
                    var w = new FileSystemWatcher(path);

                    wDispose +=
                        delegate
                        {
                            if (w == null)
                                return;

                            var ww = w;
                            w = null;
                            Thread.Yield();
                            ww.Dispose();
                        };
                    // if there is a change, we could rerun msbuild?
                    // that would call post build event and we would be called again would we not?
                    w.IncludeSubdirectories = true;
                    w.EnableRaisingEvents = true;

                    FileSystemEventHandler yChanged =


                        (xsender, xargs) =>
                        {
                            if (xargs.FullPath.Contains(@"\obj\"))
                                return;
                            //if (!w.EnableRaisingEvents)
                            //    return;
                            if (rebuilding)
                                return;

                            rebuilding = true;
                            Console.Title += " rebuilding...";
                            //w.EnableRaisingEvents = false;

                            var sln = args.FirstOrDefault(x => x.EndsWith(".sln"));

                            // { ChangeType = Changed, FullPath = X:\jsc.svn\examples\javascript\LINQ\test\AutoRefreshTesting\AutoRefreshTesting\obj\Debug }

                            Console.WriteLine(
                                new { Thread.CurrentThread.ManagedThreadId, xargs.ChangeType, xargs.FullPath }
                                );

                            Thread.Sleep(100);

                            if (w == null)
                                return;



                            // are we supposed to udp broadcast now?
                            // are we supposed to have roslyn look at comments?
                            // unless there are no other changes to happen lets do a backgroun build.

                            //w.Dispose();

                            var msbuild = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";
                            //  { msbuild = C:\Windows\system32\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe }

                            var ww = Stopwatch.StartNew();
                            //Console.WriteLine("lets rebuild! " + new { msbuild });


                            //Thread.Sleep(1000);

                            // %SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

                            var p = Process.Start(
                                new ProcessStartInfo(msbuild)
                                {
                                    Arguments = "/nologo /verbosity:q " + sln,

                                    // can we share our console with that sub process?
                                    UseShellExecute = false,
                                }
                            );


                            p.WaitForExit();

                            //Console.WriteLine("rebuild done! " + new { ww.ElapsedMilliseconds });

                            // if there were errors we need to wait the next ctrl s otheriwse we will be reloaded.

                            //nhandled Exception: System.ObjectDisposedException: Cannot access a disposed object.
                            //bject name: 'FileSystemWatcher'.
                            //  at System.IO.FileSystemWatcher.StartRaisingEvents()
                            //  at System.IO.FileSystemWatcher.set_EnableRaisingEvents(Boolean value)

                            // unless we were reset!
                            //if (w != null)
                            //w.EnableRaisingEvents = true;

                            rebuilding = false;
                        };

                    w.Created += yChanged;
                    w.Changed += yChanged;

                };
            #endregion


            foreach (var sln0 in args.Where(x => x.EndsWith(".sln")))
            {
                // well start monitoring the directory.
                var path = Path.GetDirectoryName(sln0);

                Console.Title = Path.GetFileNameWithoutExtension(sln0);

                yFileSystemWatcher(path);
            }

            // since we are not really parsing .sln files we need to be told what other projects to keep an eye on for updates..
            //foreach (var sln0 in args.Where(x => x.EndsWith(".csproj")))
            foreach (var sln0 in args.Where(x => x.EndsWith(".dll")))
            {
                // well start monitoring the directory.
                var path = Path.GetDirectoryName(sln0);

                //             Unhandled Exception: System.ArgumentException: The directory name C:\util\jsc\bin\ScriptCoreLib.Extensions.dll is invalid.
                //at System.IO.FileSystemWatcher..ctor(String path, String filter)

                yFileSystemWatcher(path);
            }

        }


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            int uFlags);

        private const int HWND_TOPMOST = -1;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        // called by post build event
        static void Main(string[] args)
        {
            var AsServerSignal = new EventWaitHandle(
                false,
                EventResetMode.AutoReset,
                "AsServerSignal"
            );


            // if its a trap then start a server
            var AsServer = AsServerSignal.WaitOne(1);

            Console.WriteLine(new { AsServer } + " continue?");
            if (AsServer)
            {
                //IntPtr hWnd = Process.GetCurrentProcess().MainWindowHandle;
                IntPtr hWnd = GetConsoleWindow();

                Console.WriteLine("time to create a server?" + new { hWnd });

                // are we top most?


                SetWindowPos(hWnd,
                    new IntPtr(HWND_TOPMOST),
                    0, 0, 0, 0,
                    SWP_NOMOVE | SWP_NOSIZE);

                var pipeServer = new NamedPipeServerStream(
                    "foo", PipeDirection.In,
                    1,
                    PipeTransmissionMode.Message,
                    PipeOptions.WriteThrough);

                while (true)
                {
                    //Console.WriteLine("awaiting for a new connection");

                    pipeServer.WaitForConnection();

                    Console.WriteLine("client connected to server!");

                    var buffer = new byte[0xffff];

                    var c = pipeServer.Read(buffer, 0, buffer.Length);

                    var xml = XElement.Parse(
                        Encoding.UTF8.GetString(buffer, 0, c)
                    );

                    //var old = new { Console.ForegroundColor };
                    //Console.ForegroundColor = ConsoleColor.Cyan;
                    //Console.WriteLine(new { xml });
                    //Console.ForegroundColor = old.ForegroundColor;

                    // do something with incoming params?

                    pipeServer.Disconnect();

                    // server inactive for a moment, update the state and reconnect to next client
                    InternalMain(
                        xml.Elements().Select(x => x.Value).ToArray()
                    );
                }
            }




            var wConnected = false;

            new Thread(
                delegate()
                {
                    // allow 100 until we need to take action
                    Thread.Sleep(100);

                    if (wConnected)
                    {
                        // server already active!
                        return;
                    }

                    AsServerSignal.Set();

                    // make us a new server
                    // cmd.exe allows us to survive a process crash.
                    Process.Start("cmd.exe", "/K " + typeof(Program).Assembly.Location);





                }
            )
            {
                IsBackground = true
            }.Start();


            Console.WriteLine("connecting to the server...");
            var w = new NamedPipeClientStream(".", "foo", PipeDirection.Out, PipeOptions.WriteThrough);
            Thread.Yield();
            w.Connect();
            wConnected = true;

            Console.WriteLine("connecting to the server... done");

            // http://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes.jmp(v=vs.110).aspx

            var bytes = Encoding.UTF8.GetBytes(
                new XElement("xml",

                    from x in args
                    select new XElement("arg", x)
                    ).ToString()
            );


            w.Write(bytes, 0, bytes.Length);
            w.Flush();
            w.Dispose();


            //Thread.Sleep(100);


            //Unhandled Exception: System.InvalidOperationException: Cannot read keys when either application does not have a console or when console input has been redirected from a file. Try Console.Read.
            //1>     at System.Console.ReadKey(Boolean intercept)
            //1>     at System.Console.ReadKey()


            //Debugger.Break();
            //Console.ReadKey();
        }
    }




}
