﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using ScriptCoreLib;
using ScriptCoreLib.Shared;

namespace MatrixTransformC2Example
{
	interface IAssemblyReferenceToken :
		ScriptCoreLib.Shared.Query.IAssemblyReferenceToken,
		ScriptCoreLib.Shared.IAssemblyReferenceToken
	{
	}

}
