﻿using android.content;
using android.graphics;
using ScriptCoreLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace android.view
{
    // https://android.googlesource.com/platform/frameworks/base.git/+/master/core/java/android/view/Surface.java
    // http://developer.android.com/reference/android/view/Surface.html

    [Script(IsNative = true)]
    public class Surface
    {
        // X:\jsc.svn\core\ScriptCoreLibAndroidNDK\ScriptCoreLibAndroidNDK\SystemHeaders\android\native_window_jni.cs


        public Surface(SurfaceTexture tex)
        {

        }

        public Canvas lockCanvas(Rect inOutDirty) { throw null; }

        public void unlockCanvasAndPost(Canvas canvas) { throw null; }
    }
}
