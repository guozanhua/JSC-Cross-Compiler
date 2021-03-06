﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ScriptCoreLib.JavaScript.DOM.HTML;
using ScriptCoreLib.JavaScript.BCLImplementation.System.Windows.Media;

namespace ScriptCoreLib.JavaScript.BCLImplementation.System.Windows.Controls
{
    // http://referencesource.microsoft.com/#PresentationFramework/src/Framework/System/Windows/Controls/Image.cs

    [Script(Implements = typeof(global::System.Windows.Controls.Image))]
    public class __Image : __FrameworkElement
    {
        // X:\jsc.svn\examples\javascript\io\GIFDecoderExperiment\GIFDecoderExperiment\Application.cs

        readonly IHTMLDiv InternalSprite = new IHTMLDiv();

        public IHTMLImage InternalBitmap;
        public event Action InternalBitmapChanged;

        public __Image()
        {
            InternalSprite.style.position = ScriptCoreLib.JavaScript.DOM.IStyle.PositionEnum.absolute;
            InternalSprite.style.left = "0px";
            InternalSprite.style.top = "0px";

            //InternalSprite.style.background = "red";

            InternalSprite.style.overflow = ScriptCoreLib.JavaScript.DOM.IStyle.OverflowEnum.hidden;
        }

        public override IHTMLElement InternalGetDisplayObject()
        {
            return InternalSprite;
        }

        bool InternalGetOpacityTarget_DisableContainerReset;

        public override IHTMLElement InternalGetOpacityTarget()
        {
            if (InternalBitmap != null)
            {
                if (!InternalGetOpacityTarget_DisableContainerReset)
                {
                    InternalGetDisplayObject().style.Opacity = 1;
                    InternalGetOpacityTarget_DisableContainerReset = true;
                }

                return InternalBitmap;
            }

            // beaware of recursion
            return InternalGetDisplayObject();
        }

        public ImageSource InternalSource;

        public ImageSource Source
        {
            get
            {
                return this.InternalSource;
            }
            set
            {

                this.InternalSource = value;
                __ImageSource v = value;

                var alias = v.InternalManifestResourceAlias;

                #region Apply
                global::System.Action<IHTMLImage> Apply =
                    img =>
                    {
                        InternalSprite.removeChildren();

                        InternalBitmap = img;

                        InternalBitmap.style.SetLocation(0, 0);

                        InternalSprite.appendChild(InternalBitmap);

                        if (InternalBitmapChanged != null)
                            InternalBitmapChanged();

                        if (!InternalWidthValueSpecified)
                            InternalWidthValue = img.width;

                        if (!InternalHeightValueSpecified)
                            InternalHeightValue = img.height;

                        InternalSetWidth(InternalWidthValue);
                        InternalSetHeight(InternalHeightValue);
                    };
                #endregion


                if (alias != null)
                {
                    IHTMLImage i = alias;

                    i.InvokeOnComplete(Apply);

                }
                else if (v.InternalBitmap != null)
                {
                    v.InternalBitmap.Continue(i => i.InvokeOnComplete(Apply));

                }
            }
        }

        Stretch InternalStretch;

        public Stretch Stretch
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                // emulate wpf stretch

                this.InternalStretch = value;
            }
        }

        public double InternalWidthValue = 200;
        public double InternalHeightValue = 200;

        public override double InternalGetHeight()
        {
            return InternalHeightValue;
        }

        public override double InternalGetWidth()
        {
            return InternalWidthValue;
        }

        bool InternalWidthValueSpecified;
        bool InternalHeightValueSpecified;
        public override void InternalSetHeight(double value)
        {
            InternalHeightValueSpecified = true;
            InternalHeightValue = value;

            InternalUpdateStrech();

            this.InternalSprite.style.height = Convert.ToInt32(value) + "px";
        }

        public override void InternalSetWidth(double value)
        {
            InternalWidthValueSpecified = true;
            InternalWidthValue = value;

            InternalUpdateStrech();

            this.InternalSprite.style.width = Convert.ToInt32(value) + "px";
        }

        private void InternalUpdateStrech()
        {
            if (this.InternalBitmap == null)
                return;

            this.InternalBitmap.width = Convert.ToInt32(InternalWidthValue);
            this.InternalBitmap.height = Convert.ToInt32(InternalHeightValue);
        }

    }
}
