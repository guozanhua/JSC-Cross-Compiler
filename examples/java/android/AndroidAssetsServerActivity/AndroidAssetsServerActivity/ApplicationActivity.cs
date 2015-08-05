using System;
using System.Collections.Generic;
using System.Linq;
using android.app;
using android.os;
using android.view;
using android.widget;
using ScriptCoreLib;
using java.lang;
using android.content;
using java.net;
using java.io;
using java.util;
using android.webkit;
using android.util;

namespace AndroidAssetsServerActivity.Activities
{
    public class ApplicationActivity : Activity
    {
        // https://sites.google.com/a/jsc-solutions.net/work/knowledge-base/15-dualvr/20150723
        // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2015/201505/20150513

        // see also: y:\jsc.svn\examples\java\android\AndroidLacasCameraServerActivity\AndroidLacasCameraServerActivity\com\lacas\testsocket\TestSocketActivity.java

        ScriptCoreLib.Android.IAssemblyReferenceToken ref1;
        Inet6Address __hack;


        public WebView webview;
        public ProgressDialog progressBar;
        public AlertDialog alertDialog;

        public override bool onKeyDown(int keyCode, KeyEvent e)
        {
            // http://android-coding.blogspot.com/2011/08/handle-back-button-in-webview-to-back.html

            if (keyCode == KeyEvent.KEYCODE_BACK)
            {
                if (webview.canGoBack())
                {
                    webview.goBack();
                    return true;
                }
            }
            return base.onKeyDown(keyCode, e);
        }

        protected override void onCreate(Bundle savedInstanceState)
        {
            base.onCreate(savedInstanceState);


            var r = new System.Random();
            var port = r.Next(1024, 32000);


            var uri = "http://" + getLocalIpAddress();

            uri += ":";
            uri += ((object)(port)).ToString();

            this.setTitle((java.lang.CharSequence)(object)uri);

            serverThread = new ServerThread { mycontext = this, port = port };

            new Thread(serverThread).start();



            // http://stackoverflow.com/questions/8955228/webview-with-an-iframe-android
            // http://www.chrisdanielson.com/tag/webviewclient/

            this.alertDialog = new AlertDialog.Builder(this).create();

            this.progressBar = ProgressDialog.show(this, (CharSequence)(object)"look here!", (CharSequence)(object)"Loading...");


            this.webview = new WebView(this);


            setContentView(webview);



            //webview.getSettings().setSupportZoom(true); 
            webview.getSettings().setLoadsImagesAutomatically(true);
            webview.getSettings().setJavaScriptEnabled(true);
            webview.getSettings().setBuiltInZoomControls(true);
            //webview.setInitialScale(1);

            webview.setWebViewClient(new MyWebViewClient { __this = this });
            webview.getSettings().setSupportZoom(false);
            webview.setScrollBarStyle(WebView.SCROLLBARS_INSIDE_OVERLAY);

            //webview.getSettings().setJavaScriptEnabled(true);

            // no flash in emulator?
            // works on my phone!
            // no Flash since android 4.1.0!!!
            //webview.getSettings().setPluginsEnabled(true);
            //webview.getSettings().setPluginState(android.webkit.WebSettings.PluginState.ON);



            // OR, you can also load from an HTML string:
            //var summary = "<html><body>You scored <b>192</b> points.</body></html>";
            //webview.loadData(summary, "text/html", null);
            //Log.i(TAG, "loadUrl");
            webview.loadUrl(uri);

            //this.setContentView(sv);




        }



        class MyWebViewClient : WebViewClient
        {
            public ApplicationActivity __this;

            public override bool shouldOverrideUrlLoading(WebView view, string url)
            {
                //Log.i(TAG, "Processing webview url click...");
                view.loadUrl(url);
                return true;
            }

            public override void onPageFinished(WebView view, string url)
            {
                //Log.i(TAG, "Finished loading URL: " + url);
                if (__this.progressBar.isShowing())
                {
                    __this.progressBar.dismiss();
                }
            }

            public override void onReceivedError(WebView view, int errorCode, string description, string failingUrl)
            {
                //Log.e(TAG, "Error: " + description);

                //__this.ShowToast("Oh no! " + description);

                //Toast.makeText(__this, "Oh no! " + description, Toast.LENGTH_SHORT).show();
                //__this.alertDialog.setTitle((CharSequence)(object)"Error");
                //__this.alertDialog.setMessage(description);
                //__this.alertDialog.setButton((CharSequence)(object)"OK", new DialogInterface.OnClickListener() {
                //    public void onClick(DialogInterface dialog, int which) {
                //        return;
                //    }
                //});
                //__this.alertDialog.show();
            }
        }


        public string getLocalIpAddress()
        {
            var value = "";

            try
            {
                for (Enumeration en = NetworkInterface.getNetworkInterfaces(); en.hasMoreElements(); )
                {
                    NetworkInterface intf = (NetworkInterface)en.nextElement();
                    for (Enumeration enumIpAddr = intf.getInetAddresses(); enumIpAddr.hasMoreElements(); )
                    {
                        InetAddress inetAddress = (InetAddress)enumIpAddr.nextElement();

                        Log.wtf("getLocalIpAddress", inetAddress.getHostAddress().ToString());

                        var v6 = inetAddress is Inet6Address;

                        if (v6)
                        {
                        }
                        else if (!inetAddress.isLoopbackAddress())
                        {
                            if (value == "")
                                value = inetAddress.getHostAddress().ToString();
                        }
                    }
                }
            }
            catch
            {
            }

            return value;
        }


        ServerThread serverThread;

        //protected override void onDestroy()
        //{
        //    //  protected virtual void onDestory();
        //    //base.onDestroy();

        //    if (serverThread.serversocket != null)
        //        serverThread.closeConnections();
        //}
        public class ServerThread : Runnable
        {
            public int port = 1111;

            public Context mycontext;

            public bool isRunning = true;

            public ServerSocket serversocket;
            public Socket clientsocket;

            public BufferedReader input;
            public OutputStream output;

            public void run()
            {
                try
                {

                    serversocket = new ServerSocket(port);
                    serversocket.setReuseAddress(true);

                    while (isRunning)
                    {

                        clientsocket = serversocket.accept();
                        input = new BufferedReader(new InputStreamReader(clientsocket.getInputStream(), "ISO-8859-2"));
                        output = clientsocket.getOutputStream();

                        string sAll = getStringFromInput(input);

                        var i0 = sAll.IndexOf(" ", 0);
                        var i1 = sAll.IndexOf(" ", i0 + 1);


                        var path = ((java.lang.String)(object)sAll).substring(i0, i1);

                        if (path.Length > 0)
                            path = ((java.lang.String)(object)path).substring(2, path.Length);

                        var asset = openFileFromAssets(path, mycontext);

                        if (asset != null)
                        {
                            send(asset);
                        }
                        else
                        {
                            #region firstpage
                            string firstpage = "<body>";

                            firstpage += "<link rel=\"stylesheet\" type=\"text/css\" ";
                            firstpage += "href=\"/foo.css\" />";

                            firstpage += "<h1>";
                            firstpage += path;
                            firstpage += "</h1>";

                            firstpage += "<pre>";
                            firstpage += sAll;
                            firstpage += "</pre>";

                            firstpage += "First page! jsc! <a href='/foo.htm'>Next</a>";
                            firstpage += "Other page! jsc! <a href='/bar.htm'>Other</a>";

                            var assets = mycontext.getResources().getAssets();

                            var collection = assets.list(path);

                            foreach (var item in collection)
                            {

                                firstpage += "<pre>";
                                firstpage += item;
                                firstpage += "</pre>";
                                firstpage += "<hr />";

                            }

                            firstpage += "</body>";

                            send(firstpage);
                            #endregion

                        }

                        input.close();
                        output.close();

                    }
                }
                catch
                {

                }
            }

            public static InputStream openFileFromAssets(string spath, Context mycontext)
            {
                InputStream value = null;
                try
                {
                    value = mycontext.getResources().getAssets().open(spath);
                }
                catch
                {

                }
                return value;

            }

            void send(string s)
            {
                string header =
                        "HTTP/1.1 200 OK\n" +
                        "Connection: close\n" +
                        "Content-type: text/html; charset=utf-8\n" +
                    //"Content-Length: " + s.Length + "\n" +
                        "\n";

                var w = new OutputStreamWriter(output);

                try
                {
                    w.write(header + s);
                    w.flush();
                }
                catch
                {

                }


            }

            void send(InputStream fis, string contenttype = "application/octet-stream")
            {
                string header =
                     "HTTP/1.1 200 OK\n";

                header += "Content-type: ";
                header += contenttype;
                header += "\n";

                //header += "Content-Length: " + fis.available() + "\n" +
                header += "\n";
                try
                {
                    var w = new OutputStreamWriter(output);
                    w.write(header);
                    w.flush();

                    sbyte[] buffer = new sbyte[1024];
                    int bytes = 0;

                    bytes = fis.read(buffer);
                    while (bytes != -1)
                    {
                        output.write(buffer, 0, bytes);
                        bytes = fis.read(buffer);
                    }

                }
                catch
                {
                }
            }

            string getStringFromInput(BufferedReader input)
            {
                StringBuilder sb = new StringBuilder();
                string sTemp;

                try
                {
                    sTemp = input.readLine();

                    while (!(sTemp == ""))
                    {
                        sb.append(sTemp + "\n");
                        sTemp = input.readLine();
                    }
                }
                catch
                {
                    sb = new StringBuilder();
                }

                return sb.ToString();
            }

            public void closeConnections()
            {
                try
                {
                    serversocket.close();
                }
                catch
                {

                }
            }
        }
    }

    //[Script(Implements = typeof(global::System.Random))]
    //internal class __Random
    //{
    //    public virtual int Next()
    //    {
    //        return Next(0, int.MaxValue);
    //    }

    //    public virtual int Next(int min, int max)
    //    {
    //        var len = max - min;
    //        var r = global::java.lang.Math.floor(java.lang.Math.random() * len);

    //        int ri = (int)r;
    //        return ri + min;
    //    }

    //    public virtual double NextDouble()
    //    {
    //        return java.lang.Math.random();
    //    }


    //}



}
