﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptCoreLib.Shared.BCLImplementation.System
{
    [Script(Implements = typeof(global::System.AsyncCallback))]
    internal delegate void __AsyncCallback(IAsyncResult ar);


}
