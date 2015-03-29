﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ScriptCoreLib.JavaScript.BCLImplementation.System.Reflection
{
	[Script]
	public class __AssemblyValue
	{
		internal string FullName;

		internal object[] Types;

		internal __AssemblyValue[] References;
		internal __AssemblyNameValue Name;
	}


	// http://referencesource.microsoft.com/#mscorlib/system/reflection/assembly.cs
	// https://github.com/Reactive-Extensions/IL2JS/blob/master/mscorlib/System/Reflection/Assembly.cs
	// https://github.com/kswoll/WootzJs/blob/master/WootzJs.Runtime/Reflection/Assembly.cs

	[Script(Implements = typeof(global::System.Reflection.Assembly))]
	public class __Assembly
	{
		public virtual AssemblyName GetName()
		{
			return (AssemblyName)(object)new __AssemblyName { __NameValue = __Value.Name };
		}

		internal __AssemblyValue __Value;

		public __AssemblyName[] GetReferencedAssemblies()
		{
			var z = __Value.References;
			var x = new __AssemblyName[z.Length];

			for (int i = 0; i < z.Length; i++)
			{
				x[i] = new __AssemblyName { __Value = z[i] };
			}

			return x;
		}

		// tested by ?
		public static __Assembly Load(AssemblyName assemblyRef)
		{
			var x = (__AssemblyName)(object)assemblyRef;

			if (x.__Value == null)
				throw new Exception("Cannot load this assembly");

			return new __Assembly { __Value = x.__Value };
		}

		// Edit And Continue, while on pause, can add a new type to the assembly.
		public virtual __Type[] GetTypes()
		{
			// how would it help if we were to add a new type during debug?
			// the server would need to notify the client of a new type!

			// X:\jsc.svn\examples\javascript\test\TestEditAndContinueWithColor\TestEditAndContinueWithColor\Application.cs

			var t = this.__Value.Types;
			var x = new __Type[t.Length];

			for (int i = 0; i < t.Length; i++)
			{
				var constructor = global::ScriptCoreLib.JavaScript.Runtime.Expando.Of(t[i]);

				var p = new __RuntimeTypeHandle
				{
					Value = (IntPtr)(object)constructor.prototype
				};

				x[i] = new __Type
				{
					TypeHandle = p
				};
			}

			return x;
		}

		public virtual string FullName { get { return this.GetName().FullName; } }



		public virtual string Location
		{
			get
			{
				// X:\jsc.svn\examples\javascript\test\TestEditAndContinueWithColor\TestEditAndContinueWithColor\Application.cs
				// likely its all view-source by now...

				return "view-source";

			}
		}

	}
}
