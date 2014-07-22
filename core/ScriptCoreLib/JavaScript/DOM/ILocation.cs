using ScriptCoreLib.JavaScript;
using ScriptCoreLib.JavaScript.DOM;

namespace ScriptCoreLib.JavaScript.DOM
{
    using StringArray = IArray<string>;


    // http://mxr.mozilla.org/mozilla-central/source/dom/webidl/Location.webidl
    // http://developer.mozilla.org/en/docs/DOM:window.location

    [Script(HasNoPrototype=true)]
    public class ILocation
    {
        // http://stackoverflow.com/questions/4505798/difference-between-window-location-assign-and-window-location-replace

        public string protocol;
        public string host;
        public string hash;
        public string href;
        public string search;
        public string pathname;

        public bool IsHTTP
        {
            [Script(DefineAsStatic=true)]
            get { return protocol == "http:"; }
        }

        public void reload()
        {

        }

        public string this[string e]
        {
            [Script(DefineAsStatic = true)]
            get
            {
                string z = null;

                string k = StringArray.Split(this.search, "?")[1];

                if (k != null)
                {
                    StringArray u = StringArray.Split(k, "&");


                    foreach (string x in u.ToArray())
                    {
                        StringArray n = StringArray.Split(x, "=");

                        if (n.length > 1)
                        {
                            if (Native.window.unescape(n[0]) == e)
                            {
                                z = Native.window.unescape(n[1]);

                                break;
                            }
                        }
                    }

                }
                    return z;
            }
        }

        public void replace(string e)
        {
        }

        public void assign(string e)
        {
        }
    }
}
