using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FormsWebServiceViaInheritance
{
    public partial class ApplicationWebService : UserControl
#if ReleaseAndroid
, ScriptCoreLib.Android.Windows.Forms.IAssemblyReferenceToken_Forms
#endif
    {
        // only code and components!
        public string Foo = "Foo";

        public Task<string> GetString(string e)
        {
            return Task.FromResult(
                new { e, Foo }.ToString()
            );
        }

    }
}
