using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using android.app;
using android.os;
using android.view;
using android.widget;
using ScriptCoreLib;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.Android.Extensions;
using ScriptCoreLib.Android.Manifest;
using android.content;
using android.content.res;

namespace TestChromeAsAsset.Activities
{


    public class LocalApplication : 
        //Application

        global::org.chromium.chrome.shell.ChromeShellApplication
    {
        // https://developer.android.com/reference/android/support/multidex/MultiDexApplication.html
        // can we get AIR via it?
        // https://android.googlesource.com/platform/frameworks/multidex/+/master/library/src/android/support/multidex/MultiDexApplication.java

        public override void onCreate()
        {
            // nested type optimized out?

            // https://stackoverflow.com/questions/30093998/java-lang-nosuchfielderror-android-support-v7-appcompat-rstyleable-theme-windo
            //var appcw = android.R.style.the;

            var appc = android.support.v7.appcompat.R.style.Theme_AppCompat;

            { android.support.v4.widget.DrawerLayout.ViewDragCallback ref0; }
            //{ android.support.v7.widget.ActionMenuPresenter.ActionMenuPopupCallback ref0; }

            // https://github.com/android/platform_frameworks_base/blob/master/core/java/android/widget/ActionMenuPresenter.java
            { android.support.v7.widget.ActionMenuPresenter ref0; }

            Console.WriteLine("enter LocalApplication onCreate, first time? "
                // chrome java
                // U:\chromium\src\out\Release\lib.java\chrome_java.jar\org\chromium\chrome\browser\invalidation\
                + " " + typeof(global::org.chromium.chrome.browser.invalidation.UniqueIdInvalidationClientNameGenerator)
                );

            // https://stackoverflow.com/questions/7686482/when-does-applications-oncreate-method-is-called-on-android
            Toast.makeText(this, "LocalApplication", Toast.LENGTH_LONG).show();

            base.onCreate();
        }

        //static ApplicationActivity()
        //{
        //    Console.WriteLine("should we prefetch our .so for JNI_OnLoad?");
        //    // U:\chromium\src\chrome\android\shell\chrome_shell_entry_point.cc

        //    // couldn't find "liblibchromeshell.so"
        //    java.lang.System.loadLibrary("chromeshell");
        //}
    }

    [ScriptCoreLib.Android.Manifest.ApplicationMetaData(name = "android:minSdkVersion", value = "18")]
    //[ScriptCoreLib.Android.Manifest.ApplicationMetaData(name = "android:targetSdkVersion", value = "8")]

    [ScriptCoreLib.Android.Manifest.ApplicationMetaData(name = "android:targetSdkVersion", value = "22")]
    //[ScriptCoreLib.Android.Manifest.ApplicationMetaData(name = "android:theme", value = "@android:style/Theme.Translucent")]
    [ScriptCoreLib.Android.Manifest.ApplicationMetaData(name = "android:theme", value = "@style/Theme.AppCompat")]
    //[ScriptCoreLib.Android.Manifest.ApplicationMetaData(name = "android:theme", value = "@android:style/Theme.Holo.Dialog")]
    public class ApplicationActivity : 
        // Activity

        // public class ChromeShellActivity extends ActionBarActivity
        global::org.chromium.chrome.shell.ChromeShellActivity

    {
        // https://groups.google.com/forum/#!topic/android-developers/Y5wnstMT5Lo




        protected override void onCreate(Bundle savedInstanceState)
        {
            // can we make chrome into a nuget?

            var activity = this;
            // http://stackoverflow.com/questions/11425020/actionbar-in-a-dialogfragment
            //To show activity as dialog and dim the background, you need to declare android:theme="@style/PopupTheme" on for the chosen activity on the manifest
            //activity.requestWindowFeature(Window.FEATURE_ACTION_BAR);
            //activity.getWindow().setFlags(WindowManager_LayoutParams.FLAG_DIM_BEHIND, WindowManager_LayoutParams.FLAG_DIM_BEHIND);
            //activity.getWindow().setFlags(WindowManager_LayoutParams.FLAG_TRANSLUCENT_STATUS, WindowManager_LayoutParams.FLAG_TRANSLUCENT_STATUS);
            //var @params = activity.getWindow().getAttributes();
            ////@params.height = WindowManager_LayoutParams.FILL_PARENT;
            ////@params.width = 850; //fixed width
            ////@params.height = 450; //fixed width
            //@params.alpha = 1.0f;
            //@params.dimAmount = 0.5f;
            //activity.getWindow().setAttributes(@params);
            //activity.getWindow().setLayout(850, 850);
            base.onCreate(savedInstanceState);

            //var sv = new ScrollView(this);
            //var ll = new LinearLayout(this);
            //ll.setOrientation(LinearLayout.VERTICAL);
            //sv.addView(ll);

            //var b = new Button(this).AttachTo(ll);



            //b.WithText("base: "
            //    + typeof(global::org.chromium.@base.BaseChromiumApplication)
            //    + " " + typeof(global::org.chromium.content.app.ContentApplication)
            //    + " " + typeof(global::org.chromium.chrome.browser.ChromiumApplication)

            //    // this one wont be from the jar files...
            //    + " " + typeof(global::org.chromium.chrome.shell.ChromeShellApplication)

            //    //+ " " + typeof(global::org.chromium.ui.gfx.DeviceDisplayInfo)
            //    //+ " " + typeof(global::org.chromium.net.GURLUtils)
            //    //+ " " + typeof(global::org.chromium.content.browser.LocationProviderAdapter)

            //     //[javac] W:\src\TestChromeAsAsset\Activities\ApplicationActivity.java:22: error: AudioManagerAndroid is not public in org.chromium.media; cannot be accessed from outside package
            //    //+ " " + typeof(global::org.chromium.media.AudioManagerAndroid)
            //    //+ " " + typeof(global::org.chromium.mojo.system.impl.CoreImpl)

                
            //    //+ " " + global::org.chromium.@base.BaseChromiumApplication.__hello()
            //    //+ " nativeGetCoreCount: " + org.chromium.@base.CpuFeatures.getCount()
            //    );


            //b.AtClick(
            //    v =>
            //    {
            //        b.setText("AtClick");
            //    }
            //);


          
        }

      

    }


}



//E/DID     (28806): enter ChromeShellActivity onCreate
//E/DID     (28806): enter ChromeShellApplication initCommandLine
//W/System.err(28806): stat failed: ENOENT (No such file or directory) : /data/local/tmp/chrome-shell-command-line
//I/BrowserStartupController(28806): Initializing chromium process, singleProcess=false
//W/chromium_android_linker(28806): Couldn't load libchromium_android_linker.so, trying libchromium_android_linker.cr.so
//E/ChromeShellActivity(28806): Unable to load native library.
//E/ChromeShellActivity(28806): org.chromium.base.library_loader.ProcessInitException
//E/ChromeShellActivity(28806):   at org.chromium.base.library_loader.LibraryLoader.loadAlreadyLocked(LibraryLoader.java:410)
//E/ChromeShellActivity(28806):   at org.chromium.base.library_loader.LibraryLoader.ensureInitialized(LibraryLoader.java:154)
//E/ChromeShellActivity(28806):   at org.chromium.content.browser.BrowserStartupController.prepareToStartBrowserProcess(BrowserStartupController.java:285)
//E/ChromeShellActivity(28806):   at org.chromium.content.browser.BrowserStartupController.startBrowserProcessesAsync(BrowserStartupController.java:170)
//E/ChromeShellActivity(28806):   at org.chromium.chrome.shell.ChromeShellActivity.onCreate(ChromeShellActivity.java:156)
//E/ChromeShellActivity(28806):   at TestChromeAsAsset.Activities.ApplicationActivity.onCreate(ApplicationActivity.java:37)
//E/ChromeShellActivity(28806):   at android.app.Activity.performCreate(Activity.java:6374)
//E/ChromeShellActivity(28806):   at android.app.Instrumentation.callActivityOnCreate(Instrumentation.java:1119)
//E/ChromeShellActivity(28806):   at android.app.ActivityThread.performLaunchActivity(ActivityThread.java:2767)
//E/ChromeShellActivity(28806):   at android.app.ActivityThread.handleLaunchActivity(ActivityThread.java:2879)
//E/ChromeShellActivity(28806):   at android.app.ActivityThread.access$900(ActivityThread.java:182)
//E/ChromeShellActivity(28806):   at android.app.ActivityThread$H.handleMessage(ActivityThread.java:1475)
//E/ChromeShellActivity(28806):   at android.os.Handler.dispatchMessage(Handler.java:102)
//E/ChromeShellActivity(28806):   at android.os.Looper.loop(Looper.java:145)
//E/ChromeShellActivity(28806):   at android.app.ActivityThread.main(ActivityThread.java:6141)
//E/ChromeShellActivity(28806):   at java.lang.reflect.Method.invoke(Native Method)
//E/ChromeShellActivity(28806):   at java.lang.reflect.Method.invoke(Method.java:372)
//E/ChromeShellActivity(28806):   at com.android.internal.os.ZygoteInit$MethodAndArgsCaller.run(ZygoteInit.java:1399)
//E/ChromeShellActivity(28806):   at com.android.internal.os.ZygoteInit.main(ZygoteInit.java:1194)
//E/ChromeShellActivity(28806): Caused by: java.lang.UnsatisfiedLinkError: dalvik.system.PathClassLoader[DexPathList[[zip file "/system/framework/multiwindow.jar", zip f
//E/ChromeShellActivity(28806):   at java.lang.Runtime.loadLibrary(Runtime.java:366)
//E/ChromeShellActivity(28806):   at java.lang.System.loadLibrary(System.java:989)
//E/ChromeShellActivity(28806):   at org.chromium.base.library_loader.Linker.ensureInitializedLocked(Linker.java:237)
//E/ChromeShellActivity(28806):   at org.chromium.base.library_loader.Linker.isUsed(Linker.java:371)
//E/ChromeShellActivity(28806):   at org.chromium.base.library_loader.LibraryLoader.loadAlreadyLocked(LibraryLoader.java:278)
//E/ChromeShellActivity(28806):   ... 18 more
//I/art     (28806): System.exit called, status: -1