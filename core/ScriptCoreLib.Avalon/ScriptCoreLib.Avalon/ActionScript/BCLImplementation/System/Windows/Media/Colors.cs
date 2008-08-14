﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ScriptCoreLib.ActionScript.BCLImplementation.System.Windows.Media
{
	[Script(Implements = typeof(global::System.Windows.Media.Colors))]
	internal static class __Colors
	{

		public static Color Transparent { get { return (__Color)0x00ffffff; } }
		public static Color Red { get { return (__Color)0xffff0000; } }
		public static Color GreenYellow { get { return (__Color)0xFFADFF2F; } }
	}
}
