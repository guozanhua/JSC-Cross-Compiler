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
using WebGLGalaxyS;
using WebGLGalaxyS.Design;
using WebGLGalaxyS.HTML.Pages;
using System.Diagnostics;

namespace WebGLGalaxyS
{
    /// <summary>
    /// Your client side code running inside a web browser as JavaScript.
    /// </summary>
    public sealed class Application : ApplicationWebService
    {
        /// <summary>
        /// This is a javascript application.
        /// </summary>
        /// <param name="page">HTML document rendered by the web server which can now be enhanced.</param>
        public Application(IApp page)
        {
            // X:\jsc.svn\examples\actionscript\synergy\Flare3DMeetsStarlingExperiment\Flare3DMeetsStarlingExperiment\ApplicationSprite.cs
            // X:\jsc.svn\examples\javascript\WebGL\WebGLGalaxyS\WebGLGalaxyS\Application.cs

            // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2014/201408/20140801
            // jsc, when can we have compiled collada asssets?
            // svg we have why not collada..
            // https://3dwarehouse.sketchup.com/model.html?id=b3bdb20081c023accbd2ad75d6ff6a24
            // https://3dwarehouse.sketchup.com/collection.html?id=982aafab70c9aba140c287facf4f3262


            // https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2013/201311/20131110-dae

            var oo = new List<THREE.Object3D>();

            #region scene
            var window = Native.window;

            var camera = new THREE.PerspectiveCamera(
                45,
                window.aspect,
                1,
                2000
                );
            camera.position.z = 400;

            // scene

            var scene = new THREE.Scene();

            var ambient = new THREE.AmbientLight(0x303030);
            scene.add(ambient);

            var directionalLight = new THREE.DirectionalLight(0xffffff, 0.7);
            directionalLight.position.set(0, 0, 1);
            scene.add(directionalLight);

            // WebGLRenderer preserveDrawingBuffer 
            var renderer = new THREE.WebGLRenderer(

                new
            {
                antialias = true,
                alpha = true,
                preserveDrawingBuffer = true
            }
            );

            // https://github.com/mrdoob/three.js/issues/3836
            //renderer.setClearColor(0xfffff, 1);

            renderer.setSize(window.Width, window.Height);

            renderer.domElement.AttachToDocument();
            renderer.domElement.style.SetLocation(0, 0);

            var canvas = (IHTMLCanvas)renderer.domElement;


            var old = new
            {



                CursorX = 0,
                CursorY = 0
            };


            var mouseX = 0;
            var mouseY = 0;
            var st = new Stopwatch();
            st.Start();


            #region onmousedown
            canvas.onmousedown +=
                e =>
                {

                    if (e.MouseButton == IEvent.MouseButtonEnum.Middle)
                    {
                        canvas.requestFullscreen();
                    }
                    else
                    {
                        // movementX no longer works
                        old = new
                        {


                            e.CursorX,
                            e.CursorY
                        };


                        e.CaptureMouse();
                    }

                };
            #endregion

            canvas.css.active.style.cursor = IStyle.CursorEnum.move;


            // X:\jsc.svn\examples\javascript\Test\TestMouseMovement\TestMouseMovement\Application.cs
            #region onmousemove
            canvas.onmousemove +=
                e =>
                {
                    var pointerLock = canvas == Native.document.pointerLockElement;


                    //Console.WriteLine(new { e.MouseButton, pointerLock, e.movementX });

                    if (e.MouseButton == IEvent.MouseButtonEnum.Left)
                    {

                        oo.WithEach(
                            x =>
                            {
                                x.rotation.y += 0.006 * (e.CursorX - old.CursorX);
                                x.rotation.x += 0.006 * (e.CursorY - old.CursorY);
                            }
                        );

                        old = new
                        {


                            e.CursorX,
                            e.CursorY
                        };



                    }

                };
            #endregion
            var z = camera.position.z;

            #region onmousewheel
            canvas.onmousewheel +=
                e =>
                {
                    //camera.position.z = 1.5;

                    // min max. shall adjust speed also!
                    // max 4.0
                    // min 0.6
                    z -= 10.0 * e.WheelDirection;

                    //camera.position.z = 400;
                    z = z.Max(200).Min(500);

                    //Native.document.title = new { z }.ToString();

                };
            #endregion


            Native.window.onframe +=
                e =>
            {
                renderer.clear();

                camera.aspect = canvas.aspect;
                camera.updateProjectionMatrix();

                camera.position.z += (z - camera.position.z) * e.delay.ElapsedMilliseconds / 200;


                camera.lookAt(scene.position);

                renderer.render(scene, camera);


            };

            Native.window.onresize +=
                delegate
            {
                if (canvas.parentNode == Native.document.body)
                {
                    renderer.setSize(window.Width, window.Height);
                }

            };
            #endregion

            new galaxyS().Source.Task.ContinueWithResult(
                   dae =>
                    {
                        //dae.scale.x = 30;
                        //dae.scale.y = 30;
                        //dae.scale.z = 30;
                        dae.position.z = 65;

                        dae.scale.x = 0.5;
                        dae.scale.y = 0.5;
                        dae.scale.z = 0.5;

                        //dae.position.y = -80;

                        scene.add(dae);
                        oo.Add(dae);


                    }
               );



        }

    }


    [Obsolete("jsc should generate this")]
    class galaxyS : THREE_ColladaAsset
    {
        public string ID175 = "assets/WebGLGalaxyS/Samsung-Galaxy-s_Final_Samsung_Galaxy_S_Front_Side_Back.jpg_1_.jpg";


        public galaxyS()
            : base(
                "assets/WebGLGalaxyS/Samsung-Galaxy-s.dae"
                )
        {

        }
    }

    public class THREE_ColladaAsset
    {
        // X:\jsc.svn\examples\javascript\WebGL\WebGLColladaExperiment\WebGLColladaExperiment\Application.cs

        public readonly TaskCompletionSource<THREE.Object3D> Source = new TaskCompletionSource<THREE.Object3D>();

        // need to cleanup xml manually??

        //<geometry id = "ID438" >
        //    < mesh />
        //</ geometry >

        public THREE_ColladaAsset(string uri)
        {
            var loader = new THREE.ColladaLoader();

            loader.options.convertUpAxis = true;

            // this does NOT work correctly?
            //loader.options.centerGeometry = true;

            loader.load(
                //"assets/WebGLColladaExperiment/truck.dae",

                uri,

                new Action<THREE.ColladaLoaderResult>(
                    collada =>
                    {
                        var dae = collada.scene;


                        ////o.position .y = -80;
                        //scene.add(dae);
                        //oo.Add(dae);

                        //dae.scale = new THREE.Vector3(5, 5, 5);

                        this.Source.SetResult(dae);

                    }
                )
            );
        }
    }
}
