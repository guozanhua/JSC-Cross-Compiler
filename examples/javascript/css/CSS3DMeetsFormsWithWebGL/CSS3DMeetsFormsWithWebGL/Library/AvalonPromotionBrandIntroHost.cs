using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib.Shared.Avalon.Extensions;
using System.Windows.Forms.Integration;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Reflection;

namespace FormsAvalonAnimation
{
    public class AvalonPromotionBrandIntroHost : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.Integration.ElementHost elementHost1 = new ElementHost();



        public System.Windows.Controls.Canvas Child { get; set; }

        public AvalonPromotionBrandIntroHost()
        {
            // http://weblogs.asp.net/fmarguerie/archive/2005/03/23/395658.aspx
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                this.Controls.Add(
                    new Label { Text = "AvalonPromotionBrandIntroHost", AutoSize = true }
                );

                return;
            }

            //            ---------------------------
            //Microsoft Visual Studio Express 2012 for Web
            //---------------------------
            //The control FormsAvalonAnimation.AnimationControlHost has thrown an unhandled exception in the designer and has been disabled.  
            this.elementHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";

            this.Controls.Add(elementHost1);

            // http://weblogs.asp.net/fmarguerie/archive/2005/03/23/395658.aspx
            this.Load +=
                delegate
                {
                    var c = new System.Windows.Controls.Canvas();

                    if (Child == null)
                        Child = new AvalonPromotionBrandIntro.ApplicationCanvas();

                    c.Children.Add(Child);

                    c.SizeChanged +=
                         delegate
                         {
                             Child.SizeTo(c.ActualWidth, c.ActualHeight);
                         };

                    this.elementHost1.Child = c;
                };
        }





    }


}


