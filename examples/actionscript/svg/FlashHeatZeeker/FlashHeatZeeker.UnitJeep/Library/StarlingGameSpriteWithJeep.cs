﻿using FlashHeatZeeker.StarlingSetup.Library;
using ScriptCoreLib.ActionScript.flash.geom;
using starling.display;
using starling.textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlashHeatZeeker.UnitJeep.Library
{
    public class StarlingGameSpriteWithJeepTextures 
    {
        public Func<Texture>
          black4,
          jeep,
          jeep_shadow;

        public StarlingGameSpriteWithJeepTextures(Texture64Constructor new_tex_crop)
        {
            // http://forum.starling-framework.org/topic/confirmation-on-optimum-quadbatch-use
            // hack, Quad should do the work, yet it drags performance!
            black4 = new_tex_crop("assets/FlashHeatZeeker.UnitJeep/jeep_shadow.svg", innersize: 4);


            jeep = new_tex_crop("assets/FlashHeatZeeker.UnitJeep/jeep.svg");
            jeep_shadow = new_tex_crop("assets/FlashHeatZeeker.UnitJeep/jeep_shadow.svg", alpha: 0.3);


        }
    }

    public sealed class StarlingGameSpriteWithJeep : StarlingGameSpriteBase
    {
        public StarlingGameSpriteWithJeep()
        {
            this.autorotate = true;

            var textures = new StarlingGameSpriteWithJeepTextures(new_tex_crop);

            this.onbeforefirstframe += delegate
            {


                //peds.Add(imgstand);

                //var count = 12;
                var count = 2;

                for (int i = 0; i < count; i++)
                    for (int yi = 0; yi < count; yi++)
                    {
                        var shadow = new Image(
                             textures.jeep_shadow()
                             )
                                {
                                }.AttachTo(
                             Content
                            //q
                         );

                        var q = new Sprite().AttachTo(Content);







                        var tire0 = new Image(
                          textures.black4()
                          )
                        {
                        }.AttachTo(
                            //Content
                            q
                        );

                        var tire1 = new Image(textures.black4()).AttachTo(q);
                        var tire2 = new Image(textures.black4()).AttachTo(q);
                        var tire3 = new Image(textures.black4()).AttachTo(q);

                        var imgstand = new Image(
                          textures.jeep()
                          )
                        {
                        }.AttachTo(
                            //Content
                            q
                              );


                        var rot = random.NextDouble() * Math.PI;

                        {
                            var cm = new Matrix();

                            cm.translate(-32, -32);

                            cm.scale(4.0, 4.0);

                            cm.rotate(rot);
                            cm.translate(i * 512, yi * 512);

                            // how high his the unit?
                            cm.translate(32, 32);

                            shadow.transformationMatrix = cm;
                        }




                        {
                            var cm = new Matrix();
                            cm.translate(-2, -2);
                            cm.scale(2, 4);
                            cm.rotate(Math.PI * 0.1);

                            cm.translate(-18, -20);

                            tire0.transformationMatrix = cm;
                        }

                        {
                            var cm = new Matrix();
                            cm.translate(-2, -2);
                            cm.scale(2, 4);
                            cm.rotate(Math.PI * 0.1);

                            cm.translate(18, -20);

                            tire1.transformationMatrix = cm;
                        }

                        {
                            var cm = new Matrix();
                            cm.translate(-2, -2);
                            cm.scale(2, 4);

                            cm.translate(-18, 20);

                            tire2.transformationMatrix = cm;
                        }


                        {
                            var cm = new Matrix();
                            cm.translate(-2, -2);
                            cm.scale(2, 4);

                            cm.translate(18, 20);

                            tire3.transformationMatrix = cm;
                        }


                        {
                            var cm = new Matrix();
                            cm.translate(-32, -32);
                            imgstand.transformationMatrix = cm;
                        }

                        {
                            var cm = new Matrix();

                            // how big shall the shadow be?
                            cm.scale(4.0, 4.0);

                            cm.rotate(rot);
                            cm.translate(i * 512, yi * 512);
                            q.transformationMatrix = cm;
                        }


                    }

                //var t0 = new Quad(32, 32, 0).AttachTo(
                //                    Content
                //    //q
                //                );

            };

        }
    }
}
