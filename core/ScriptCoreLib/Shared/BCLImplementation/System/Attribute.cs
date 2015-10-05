﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ScriptCoreLib.Shared.BCLImplementation.System
{
	// https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/Attribute.cs
	// https://github.com/dotnet/coreclr/blob/master/src/mscorlib/src/System/AttributeUsageAttribute.cs

	[Script(Implements=typeof(global::System.Attribute))]
    public class __Attribute
    {
		// for java, jsc need to keep looking for name clashing with native types it seems.

        // Z:\jsc.svn\examples\java\hybrid\Test\TestMessageContractAttribute\TestMessageContractAttribute\Program.cs

        public __Attribute()
        {

        }

 
    }
}
