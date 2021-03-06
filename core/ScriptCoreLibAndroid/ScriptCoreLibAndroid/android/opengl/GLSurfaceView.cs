﻿using android.content;
using android.view;
using javax.microedition.khronos.egl;
using javax.microedition.khronos.opengles;
using ScriptCoreLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace android.opengl
{
    // http://developer.android.com/reference/android/opengl/GLSurfaceView.html
    // https://android.googlesource.com/platform/frameworks/base.git/+/master/opengl/java/com/google/android/gles_jni/EGLSurfaceImpl.java
    // https://android.googlesource.com/platform/frameworks/base.git/+/master/opengl/java/android/opengl/GLSurfaceView.java

    [Script(IsNative = true)]
    public class GLSurfaceView : SurfaceView
    {
        // X:\jsc.svn\examples\c\android\hybrid\HybridGLES3JNIActivity\HybridGLES3JNIActivity\ApplicationActivity.cs

        // X:\jsc.svn\core\ScriptCoreLib.Ultra\ScriptCoreLib.Ultra\Android\Extensions\GLSurfaceViewExtensions.cs
        public void queueEvent(java.lang.Runnable y)
        { 
        }
        // http://developer.android.com/reference/android/opengl/GLSurfaceView.Renderer.html
        [Script(IsNative = true)]
        public interface Renderer
        {
            void onDrawFrame(GL10 gl);


            void onSurfaceChanged(GL10 gl, int width, int height);

            // X:\jsc.svn\core\ScriptCoreLib.Ultra\ScriptCoreLib.Ultra\Android\RenderingContextView.cs
            void onSurfaceCreated(GL10 gl, javax.microedition.khronos.egl.EGLConfig config);
        }

        public GLSurfaceView(Context context)
            : base(context)
        {

        }

        public void setRenderer(GLSurfaceView.Renderer renderer)
        {

        }

        public void setDebugFlags(int debugFlags)
        {
        }
        public void setEGLConfigChooser(int redSize, int greenSize, int blueSize,
            int alphaSize, int depthSize, int stencilSize)
        {
        }
        public void setEGLContextClientVersion(int version)
        {

        }
        public virtual bool onTouchEvent(MotionEvent _event)
        {
            return default(bool);
        }
    }
}
