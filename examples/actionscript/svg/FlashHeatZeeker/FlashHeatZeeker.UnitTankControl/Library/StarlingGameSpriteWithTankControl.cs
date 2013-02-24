﻿using Box2D.Collision.Shapes;
using Box2D.Common.Math;
using Box2D.Dynamics;
using FlashHeatZeeker.CorePhysics.Library;
using FlashHeatZeeker.UnitTank.Library;
using ScriptCoreLib.ActionScript.flash.geom;
using starling.display;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FlashHeatZeeker.UnitTankControl.Library
{
    public class StarlingGameSpriteWithTankControl : StarlingGameSpriteWithPhysics
    {
        public StarlingGameSpriteWithTankControl()
        {
            var textures = new StarlingGameSpriteWithTankTextures(new_tex_crop);


            this.onbeforefirstframe += (stage, s) =>
            {




                #region b2world



                {
                    var bodyDef = new b2BodyDef();

                    bodyDef.type = Box2D.Dynamics.b2Body.b2_dynamicBody;

                    // stop moving if legs stop walking!
                    bodyDef.linearDamping = 1.0;
                    bodyDef.angularDamping = 0.1;
                    //bodyDef.angle = 1.57079633;
                    bodyDef.fixedRotation = true;

                    var body = ground_b2world.CreateBody(bodyDef);
                    current = body;


                    var fixDef = new Box2D.Dynamics.b2FixtureDef();
                    fixDef.density = 0.1;
                    fixDef.friction = 0.01;
                    fixDef.restitution = 0;

                    var fixdef_shape = new b2PolygonShape();

                    fixDef.shape = fixdef_shape;

                    // physics unit is looking to right
                    fixdef_shape.SetAsBox(3, 2);



                    var fix = body.CreateFixture(fixDef);
                }


                #endregion







                #region visual
                var currentshadow = new Image(
                        textures.greentank_shadow()
                        )
                {
                }.AttachTo(
                         Content
                     );

                var currentvisual = new Sprite().AttachTo(Content);



                var tanktrackpattern1 = new Image(
                     textures.tanktrackpattern()
                     )
                {
                }.AttachTo(
                      currentvisual
                  );


                var tanktrackpattern0 = new Image(
                     textures.tanktrackpattern()
                     )
                {
                }.AttachTo(
                      currentvisual
                  );


                var imgstand = new Image(
                   textures.greentank()
                   )
                {
                }.AttachTo(
                    currentvisual
                );

                var guntower = new Image(
                    textures.greentank_guntower()
                    )
                {
                }.AttachTo(
                     currentvisual
                 );



                {
                    var cm = new Matrix();

                    cm.translate(-64, -64);
                    cm.scale(0.55, 0.75);

                    tanktrackpattern0.transformationMatrix = cm;
                    tanktrackpattern1.transformationMatrix = cm;
                }

                // rpeate not suppported!
                // http://forum.starling-framework.org/topic/problem-with-repeat-and-textureatlas


                Action<Image, double> offsetTexCoords =
                    (img, offset) =>
                    {
                        img.setTexCoords(0, new Point(0, offset));
                        img.setTexCoords(1, new Point(1, offset));
                        img.setTexCoords(2, new Point(0, offset + 1));
                        img.setTexCoords(3, new Point(1, offset + 1));
                    };



                {
                    var cm = new Matrix();

                    cm.translate(-64, -64);

                    imgstand.transformationMatrix = cm;
                }

                {
                    var cm = new Matrix();

                    cm.translate(-64, -64);

                    guntower.transformationMatrix = cm;
                }

                {
                    var cm = new Matrix();

                    cm.translate(-64, -64);

                    // shadow with tracks!
                    cm.scale(1.2, 1.0);
                    //cm.rotate(rot);
                    //cm.translate(i * 128, yi * 128);

                    cm.translate(8, 8);

                    currentshadow.transformationMatrix = cm;
                }

                {
                    var cm = new Matrix();

                    //cm.rotate(rot);
                    //cm.translate(i * 128, yi * 128);


                    currentvisual.transformationMatrix = cm;
                }

                #endregion








                #region __keyDown
                var __keyDown = new object[0xffffff];

                stage.keyDown +=
                   e =>
                   {
                       if (__keyDown[e.keyCode] != null)
                           return;

                       // http://circlecube.com/2008/08/actionscript-key-listener-tutorial/
                       if (e.altKey)
                           __keyDown[(int)Keys.Alt] = new object();

                       __keyDown[e.keyCode] = new object();
                   };

                stage.keyUp +=
                 e =>
                 {
                     if (!e.altKey)
                         __keyDown[(int)Keys.Alt] = null;

                     __keyDown[e.keyCode] = null;
                 };

                #endregion


                var current_speed = 40.0;



                onframe += delegate
                {



                    #region SetLinearVelocity

                    var rot = 0;
                    var dx = 0.0;
                    var dy = 0.0;

                    if (__keyDown[(int)Keys.Up] != null)
                    {
                        // we have reasone to keep walking

                        dy = 1;
                    }

                    if (__keyDown[(int)Keys.Down] != null)
                    {
                        // we have reasone to keep walking
                        // go slow backwards
                        dy = -0.5;

                    }


                    if (__keyDown[(int)Keys.Left] != null)
                    {
                        // we have reasone to keep walking

                        rot = -1;

                    }

                    if (__keyDown[(int)Keys.Right] != null)
                    {
                        // we have reasone to keep walking

                        rot = 1;

                    }


                    current.SetAngularVelocity(rot * 10);
                    current.SetLinearVelocity(
                        new b2Vec2(
                            Math.Cos(current.GetAngle()) * dy * current_speed
                            + Math.Cos(current.GetAngle() + Math.PI / 2) * dx * current_speed,
                            Math.Sin(current.GetAngle()) * dy * current_speed
                            + Math.Sin(current.GetAngle() + Math.PI / 2) * dx * current_speed

                        )
                    );


                    #endregion

                    #region animate offsetTexCoords
                    if (rot == 0 && dy == 0)
                    {
                    }
                    else
                    {
                        if (dy > 0)
                        {
                            var offset = 1.0 - (gametime.ElapsedMilliseconds % 1000) / 1000.0;
                            offsetTexCoords(tanktrackpattern0, offset);
                            offsetTexCoords(tanktrackpattern1, offset - 1);
                        }
                        else
                        {
                            var offset = (gametime.ElapsedMilliseconds % 1000) / 1000.0;
                            offsetTexCoords(tanktrackpattern0, offset);
                            offsetTexCoords(tanktrackpattern1, offset - 1);
                        }

                    }
                    #endregion




                    #region transformationMatrix, phisics updated, now update visual


                    {
                        var cm = new Matrix();


                        cm.translate(-64, -64);

                        // shadow with tracks!
                        cm.scale(1.2, 1.0);
                        //cm.rotate(rot);
                        //cm.translate(i * 128, yi * 128);



                        cm.rotate(current.GetAngle() + Math.PI / 2);
                        cm.translate(
                            current.GetPosition().x * 16,
                            current.GetPosition().y * 16
                        );


                        cm.translate(8, 8);

                        currentshadow.transformationMatrix = cm;
                    }

                    {
                        var cm = new Matrix();

                        cm.rotate(current.GetAngle() + Math.PI / 2);
                        cm.translate(
                            current.GetPosition().x * 16,
                            current.GetPosition().y * 16
                        );

                        currentvisual.transformationMatrix = cm;
                    }
                    #endregion


                    //#region DisableDefaultContentDransformation
                    //DisableDefaultContentDransformation = true;
                    //{
                    //    var cm = new Matrix();


                    //    cm.translate(
                    //        -(current.GetPosition().x * 16),
                    //        -(current.GetPosition().y * 16)
                    //    );

                    //    cm.rotate(-current.GetAngle() - Math.PI / 2);
                    //    //cm.rotate(-current.GetAngle());


                    //    stagex = stage.stageWidth * 0.5;
                    //    stagey = stage.stageHeight * 0.8;
                    //    stagescale = internalscale * (stage.stageWidth) / (800.0);


                    //    cm.scale(stagescale, stagescale);

                    //    cm.translate(
                    //        (stage.stageWidth * 0.5),
                    //        (stage.stageHeight * 0.8)
                    //    );


                    //    Content.transformationMatrix = cm;
                    //    dd.transform.matrix = cm;
                    //}
                    //#endregion
                };
            };
        }
    }
}
