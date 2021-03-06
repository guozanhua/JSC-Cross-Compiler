﻿using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ScriptCoreLib.Query.Experimental;
using System.Data.SQLite;

namespace TestAndroidOrderByThenGroupBy
{
    /// <summary>
    /// Methods defined in this type can be used from JavaScript. The method calls will seamlessly be proxied to the server.
    /// </summary>
    public class ApplicationWebService
    {
        /// <summary>
        /// The static content defined in the HTML file will be update to the dynamic content once application is running.
        /// </summary>
        //public XElement Header = new XElement(@"h1", @"JSC - The .NET crosscompiler for web platforms. ready.");

        // android cookie garbles, or turnacates data?
        // I/System.Console( 1762): XDocument Parse error: { text = <h1 id="Header">JSC - The .NET crosscompiler for web platforms. ready.</h1∩┐╜∩┐╜ }
        //        I/System.Console( 1762): #4 POST /xml/WebMethod2 HTTP/1.1 error:
        //I/System.Console( 1762): #4 java.lang.RuntimeException: expected: '>' actual: '∩┐┐' (position:END_TAG </h1∩┐╜∩┐╜>@1:77 in java.io.StringReader@40669738)
        //I/System.Console( 1762): #4 java.lang.RuntimeException: expected: '>' actual: '∩┐┐' (position:END_TAG </h1∩┐╜∩┐╜>@1:77 in java.io.StringReader@40669738)

        // X:\jsc.svn\core\ScriptCoreLibJava\BCLImplementation\System\Activator.cs

        // wtf.

        //        I/System.Console( 4640): enter InternalDatabaseName { InternalConnectionString = Data Source=file:PerformanceResourceTimingData2.xlsx.sqlite; Version=0; }
        //        I/System.Console( 4640): __SQLiteCommand.ExecuteNonQuery { CommandText = insert into `PerformanceResourceTimingData2ApplicationPerformance` (
        //I/System.Console( 4640): `connectStart`, `connectEnd`, `requestStart`, `responseStart`, `responseEnd`, `domLoading`, `domComplete`, `loadEventStart`, `loadEventEnd`, `EventTime`, `z`, `Tag`, `Timestamp`) values(@connectStart, @connectEnd, @requestStart, @responseStart, @responseEnd, @domLoading, @domComplete, @loadEventStart, @loadEventEnd, @EventTime, @z, @Tag, @Timestamp), InternalReadOnly = false }
        //    I/System.Console( 4640): #4 POST /xml/WebMethod2 HTTP/1.1 error:
        //I/System.Console( 4640): #4 java.lang.RuntimeException
        //I/System.Console( 4640): #4 java.lang.RuntimeException
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodInfo.InternalInvoke(__MethodInfo.java:97)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodBase.Invoke(__MethodBase.java:71)
        //I/System.Console( 4640):        at ScriptCoreLib.Shared.BCLImplementation.System.__Action_1.Invoke(__Action_1.java:28)
        //I/System.Console( 4640):        at ScriptCoreLib.Query.Experimental.QueryExpressionBuilder.Insert(QueryExpressionBuilder.java:144)
        //I/System.Console( 4640):        at TestAndroidOrderByThenGroupBy.ApplicationWebService.WebMethod2(ApplicationWebService.java:114)
        //I/System.Console( 4640):        at TestAndroidOrderByThenGroupBy.Global.Invoke(Global.java:163)
        //I/System.Console( 4640):        at ScriptCoreLib.Ultra.WebService.InternalGlobalExtensions.InternalApplication_BeginRequest(InternalGlobalExtensions.java:345)
        //I/System.Console( 4640):        at TestAndroidOrderByThenGroupBy.Global.Application_BeginRequest(Global.java:46)
        //I/System.Console( 4640):        at TestAndroidOrderByThenGroupBy.Activities.ApplicationWebServiceActivity___c__DisplayClass25._CreateServer_b__20(ApplicationWebServiceActivity___c__DisplayClass25.java:379)
        //I/System.Console( 4640):        at java.lang.reflect.Method.invokeNative(Native Method)
        //I/System.Console( 4640):        at java.lang.reflect.Method.invoke(Method.java:507)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodInfo.InternalInvoke(__MethodInfo.java:93)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodBase.Invoke(__MethodBase.java:71)
        //I/System.Console( 4640):        at ScriptCoreLib.Shared.BCLImplementation.System.__Action_2.Invoke(__Action_2.java:28)
        //I/System.Console( 4640):        at TestAndroidOrderByThenGroupBy.Activities.ApplicationWebServiceActivity___c__DisplayClass25___c__DisplayClass2e._CreateServer_b__24(ApplicationWebServiceActivity___c__DisplayClass25___c__DisplayClass2e.java:32)
        //I/System.Console( 4640):        at java.lang.reflect.Method.invokeNative(Native Method)
        //I/System.Console( 4640):        at java.lang.reflect.Method.invoke(Method.java:507)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodInfo.InternalInvoke(__MethodInfo.java:93)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodBase.Invoke(__MethodBase.java:71)
        //I/System.Console( 4640):        at ScriptCoreLib.Shared.BCLImplementation.System.Threading.__ThreadStart.Invoke(__ThreadStart.java:28)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Threading.__Thread___c__DisplayClass3.__ctor_b__1(__Thread___c__DisplayClass3.java:20)
        //I/System.Console( 4640):        at java.lang.reflect.Method.invokeNative(Native Method)
        //I/System.Console( 4640):        at java.lang.reflect.Method.invoke(Method.java:507)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodInfo.InternalInvoke(__MethodInfo.java:93)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodBase.Invoke(__MethodBase.java:71)
        //I/System.Console( 4640):        at ScriptCoreLib.Shared.BCLImplementation.System.__Action.Invoke(__Action.java:28)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Threading.__Thread_RunnableHandler.run(__Thread_RunnableHandler.java:20)
        //I/System.Console( 4640):        at java.lang.Thread.run(Thread.java:1019)
        //I/System.Console( 4640): Caused by: java.lang.reflect.InvocationTargetException
        //I/System.Console( 4640):        at java.lang.reflect.Method.invokeNative(Native Method)
        //I/System.Console( 4640):        at java.lang.reflect.Method.invoke(Method.java:507)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodInfo.InternalInvoke(__MethodInfo.java:93)
        //I/System.Console( 4640):        ... 27 more
        //I/System.Console( 4640): Caused by: java.lang.RuntimeException
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodInfo.InternalInvoke(__MethodInfo.java:97)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodBase.Invoke(__MethodBase.java:71)
        //I/System.Console( 4640):        at ScriptCoreLib.Shared.BCLImplementation.System.__Action_1.Invoke(__Action_1.java:28)
        //I/System.Console( 4640):        at TestAndroidOrderByThenGroupBy.ApplicationWebService.__cctor_b__0(ApplicationWebService.java:154)
        //I/System.Console( 4640):        ... 30 more
        //I/System.Console( 4640): Caused by: java.lang.reflect.InvocationTargetException
        //I/System.Console( 4640):        at java.lang.reflect.Method.invokeNative(Native Method)
        //I/System.Console( 4640):        at java.lang.reflect.Method.invoke(Method.java:507)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodInfo.InternalInvoke(__MethodInfo.java:93)
        //I/System.Console( 4640):        ... 33 more
        //I/System.Console( 4640): Caused by: java.lang.RuntimeException
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodInfo.InternalInvoke(__MethodInfo.java:97)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodBase.Invoke(__MethodBase.java:71)
        //I/System.Console( 4640):        at ScriptCoreLib.
        //I/System.Console( 4640): Shared.BCLImplementation.System.__Func_1.Invoke(__Func_1.java:29)
        //I/System.Console( 4640):        at ScriptCoreLib.Android.BCLImplementation.System.Data.SQLite.__SQLiteCommand.ExecuteNonQuery(__SQLiteCommand.java:198)
        //I/System.Console( 4640):        at ScriptCoreLib.Query.Experimental.QueryExpressionBuilder.Insert(QueryExpressionBuilder.java:1023)
        //I/System.Console( 4640):        at ScriptCoreLib.Query.Experimental.QueryExpressionBuilder___c__DisplayClass36_1._Insert_b__38(QueryExpressionBuilder___c__DisplayClass36_1.java:36)
        //I/System.Console( 4640):        ... 36 more
        //I/System.Console( 4640): Caused by: java.lang.reflect.InvocationTargetException
        //I/System.Console( 4640):        at java.lang.reflect.Method.invokeNative(Native Method)
        //I/System.Console( 4640):        at java.lang.reflect.Method.invoke(Method.java:507)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Reflection.__MethodInfo.InternalInvoke(__MethodInfo.java:93)
        //I/System.Console( 4640):        ... 41 more

        //I/System.Console( 4640): Caused by: java.lang.ArrayStoreException: source[11] of type Ljava/lang/String; cannot be stored in destination array of type[Ljava / lang / Long;

        //    I/System.Console( 4640):        at java.lang.System.arraycopy(Native Method)
        //I/System.Console( 4640):        at java.util.ArrayList.toArray(ArrayList.java:526)
        //I/System.Console( 4640):        at ScriptCoreLibJava.BCLImplementation.System.Collections.Generic.__List_1.ToArray(__List_1.java:204)
        //I/System.Console( 4640):        at ScriptCoreLib.Shared.BCLImplementation.System.Linq.__Enumerable.ToArray(__Enumerable.java:615)
        //I/System.Console( 4640):        at ScriptCoreLib.Android.BCLImplementation.System.Data.SQLite.__SQLiteCommand___c__DisplayClasse._InternalCreateStatement_b__7(__SQLiteCommand___c__DisplayClasse.java:59)

        // wtf. a problem on android 2.3 only or for 4.4. too?

        static ApplicationWebService()
        {
            #region QueryExpressionBuilder.WithConnection
            QueryExpressionBuilder.WithConnection =
                y =>
            {
                var cc = new SQLiteConnection(
                    new SQLiteConnectionStringBuilder { DataSource = "file:PerformanceResourceTimingData2.xlsx.sqlite" }.ToString()
                );

                cc.Open();
                y(cc);
                cc.Dispose();
            };
            #endregion
        }



        public void WebMethod2(Action<string> yield)
        //public async Task<string> WebMethod2()
        {
            Console.WriteLine("enter WebMethod2");

            //            I / System.Console(4251): #8 POST /xml/WebMethod2 HTTP/1.1 error:
            //I / System.Console(4251): #8 java.lang.RuntimeException: System.Diagnostics.Debugger.Break
            //I / System.Console(4251): #8 java.lang.RuntimeException: System.Diagnostics.Debugger.Break
            //I / System.Console(4251):        at ScriptCoreLibJava.BCLImplementation.System.Diagnostics.__Debugger.Break(__Debugger.java:32)
            //I / System.Console(4251):        at ScriptCoreLibJava.BCLImplementation.System.Runtime.CompilerServices.__AsyncTaskMethodBuilder_1.SetException(__AsyncTaskMethodBuilder_1.java:46)

            new PerformanceResourceTimingData2ApplicationPerformance().Delete();


            // can we insert on android?

            new PerformanceResourceTimingData2ApplicationPerformance().Insert(
                new PerformanceResourceTimingData2ApplicationPerformanceRow
            {
                connectEnd = 9,
                connectStart = 5,
                Tag = "first insert"
            },

                new PerformanceResourceTimingData2ApplicationPerformanceRow
            {
                connectStart = 5,
                connectEnd = 111,
                Tag = "middle insert"
            },

                new PerformanceResourceTimingData2ApplicationPerformanceRow
            {
                connectStart = 5,
                connectEnd = 11,
                Tag = "Last insert, selected by group by"
            }
            );



            var f = (
                from x in new PerformanceResourceTimingData2ApplicationPerformance()
                    // MYSQL and SQLITE seem to behave differently? in reverse actually!
                    //orderby x.Key ascending
                orderby x.connectEnd descending
                // { f = { Tag = middle insert } }
                group x by x.connectStart into gg
                //select new
                //{
                //    //c = gg.Count(),
                //    gg.Last().Tag
                //}
                select gg.Last().Tag

            ).FirstOrDefault();

            System.Console.WriteLine(
                new { f }
                );

            //Debugger.Break();

            //return new { message = "ok" }.ToString();
            yield(new { message = "ok", f }.ToString());
            //return new { message = "ok" }.ToString();
        }

    }
}
