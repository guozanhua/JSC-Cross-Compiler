﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptCoreLib.CompilerServices
{
	public class GLSLElement : IObservable<object>
	{
		// Observable ?
		// like XElement, XDocument


		public string SourcePath;

		public readonly List<object> Elements = new List<object>();

		public GLSLElement()
		{
			// are we multithreaded?
			var scope = new { Thread.CurrentThread.ManagedThreadId };

			#region AppendComment
			this.AppendComment = (StringBuilder CommentStringBuilder, bool IsLineComment, bool IsBlockComment) =>
			{
				// we are null tolerant.
				if (CommentStringBuilder == null)
					return null;

				// should we add to this?

				// need a type?

				var value = new GLSLComment
				{
					IsLineComment = IsLineComment,
					IsBlockComment = IsBlockComment,

					Parent = this,

					ContentStringBuilder = CommentStringBuilder
				};

				this.Elements.Add(value);

				return value;
			};
			#endregion

			this.AppendMacroFragment =
				xGLSLMacroFragment =>
				{
					// we are null tolerant.
					if (xGLSLMacroFragment == null)
						return null;

					Console.WriteLine(new { xGLSLMacroFragment });
					this.Elements.Add(xGLSLMacroFragment);


					return xGLSLMacroFragment;
				};
		}

		public readonly Func<StringBuilder, bool, bool, GLSLComment> AppendComment;

		public readonly Func<GLSLMacroFragment, GLSLMacroFragment> AppendMacroFragment;


		public override string ToString()
		{
			// we should be able to render the parsed code back by now.

			return "";
		}



		// event ?
		IDisposable IObservable<object>.Subscribe(IObserver<object> observer)
		{
			// http://www.remondo.net/observer-pattern-example-csharp-iobservable/
			// https://msdn.microsoft.com/en-us/library/dd990377(v=vs.110).aspx
			// yield return?



			throw new NotImplementedException();
		}
	}
}
