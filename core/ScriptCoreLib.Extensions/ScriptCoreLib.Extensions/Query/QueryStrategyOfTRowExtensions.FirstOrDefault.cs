﻿using ScriptCoreLib.Shared.BCLImplementation.System.Data.Common;
using ScriptCoreLib.Shared.Data.Diagnostics;
using ScriptCoreLib.Extensions;
using ScriptCoreLib.Shared.Lambda;
using ScriptCoreLib.Ultra.Library;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace System.Data
{
    // move to a nuget?
    // shall reimplement IQueriable for jsc data layer gen
    //[Obsolete("the first generic extension method for all jsc data layer rows")]
    public static partial class QueryStrategyOfTRowExtensions
    {


        //[Obsolete("make sure to apply Take 1")]
        [Obsolete("experimental")]
        public static TElement FirstOrDefault<TElement>(this IQueryStrategy<TElement> source)
        {
            // X:\jsc.svn\examples\javascript\LINQ\test\TestSelectFirstOrDefault\TestSelectFirstOrDefault\ApplicationWebService.cs
            // X:\jsc.svn\examples\javascript\LINQ\test\TestSelectMember\TestSelectMember\ApplicationWebService.cs


            return source.Take(1).AsGenericEnumerable().FirstOrDefault();
        }
    }
}
