using jsc.meta.Commands.Rewrite.RewriteToUltraApplication;
using ScriptCoreLib.Desktop.Forms.Extensions;
using System;

namespace TTFCurrencyExperment
{
    /// <summary>
    /// You can debug your application by hitting F5.
    /// </summary>
    internal static class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            new ApplicationWebService().WebMethod2("", delegate { });

            DesktopFormsExtensions.Launch(
                () => new ApplicationControl()
            );
#else
            RewriteToUltraApplication.AsProgram.Launch(typeof(Application));
#endif
        }

    }
}
