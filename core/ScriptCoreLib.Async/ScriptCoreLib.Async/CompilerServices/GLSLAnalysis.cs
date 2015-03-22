﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScriptCoreLib.CompilerServices
{
	// first step to provide glsl rewrite services.
	// this could be forwarded into a special nuget later
	// inspired by the jsc idl parser, linq to sql 
	// intended to run in CLR and in chrome worker threads, or on android L
	// https://sites.google.com/a/jsc-solutions.net/backlog/knowledge-base/2015/201503/20150318
	// webgl2/vr should be the trigger for effort

	// ref
	// https://www.opengl.org/documentation/glsl/
	// "X:\jsc.svn\examples\glsl\future\GLSLRemote\GLSLRemote.sln"

	// https://bitbucket.org/daspork/opentkshader/src/535868e04c5efb3f101da5495c74a5dbc79320bd/GLSLShader.cs?at=default
	// http://laurent.le-brun.eu/site/index.php/2010/06/07/54-fsharp-and-fparsec-a-glsl-parser-example
	// http://laurent.le-brun.eu/fsharp/glsl_parse.fs
	// http://stackoverflow.com/questions/29048161/how-to-export-a-three-js-scene-into-a-360-texture-for-photosphere

	// we would also know how the shader intends to use the uniforms
	// like do they need random texture, cubemap, audio or keyboard

	// jsc, cant we just import the spec? GLSLangSpec.4.40.pdf and generate our analyser? its 2015!!:D
	[Obsolete("experimental")]
	public class GLSLAnalysis
	{
		// where is our glsl highlighter?

		// how to run it from commandline?
		// jsc.meta "X:\jsc.svn\examples\javascript\chrome\apps\WebGL"


		// we should not be on UI thread, nor should we switch threads ourselves
		// when will it work for js workers?
		// can we do syntax highlighting?
		public static async Task WorkerThreadAnalyzeFragmentShaders(
			string[] SourceFiles,
			Action<double> AtProgress
			)
		{
			// what if more data/streams become available mid way? add them to analysis strain?
			Console.WriteLine("enter WorkerThreadAnalyzeFragmentShaders " + new { Thread.CurrentThread.ManagedThreadId, Name = Thread.CurrentThread.Name = MethodInfo.GetCurrentMethod().Name, Environment.ProcessorCount, Debugger.IsAttached });

			// this method could split analysis per core. 

			// if the files change
			// we have to discard and rewind, restart analysis

			// first we could inspect whats the first char of the shaders
			var SourceStreams = Enumerable.ToArray(
				from f in SourceFiles
				let xGLSLElement = new GLSLElement { SourcePath = f }
				let s = File.OpenRead(f)
				select new { f, s, xGLSLElement }
			);

			var cFirstChar = Enumerable.ToArray(
				from x in SourceStreams
				let xReadByte = x.s.ReadByte()
				select new { xReadByte, x.f, x.s, x.xGLSLElement }
			);

			// [0] = { xReadByte = 239, x = { f = X:\jsc.svn\examples\javascript\chrome\apps\WebGL\ChromeShaderToyAlbertArchesByDr2\ChromeShaderToyAlbertArchesByDr2\Shaders\Program.frag, s = System.IO.FileStream } }
			// "X:\util\xvi32\XVI32.exe"
			// whats 239?
			// ah its a bom
			// 0xEF BB BF

			// The Unicode Standard permits the BOM in UTF-8,[2] but does not require or recommend its use.
			// [3] Byte order has no meaning in UTF-8,[4] so its only use in UTF-8 is to signal at the start that the text stream is encoded in UTF-8. 

			// ok. lets skip bom
			// sonds lik ZIP parsing we did a while a go


			// ---------------------------
			//Unable to set the next statement.The next statement cannot be changed until the current statement has completed.

			// should we record the links we open in tabs for future ref?



			// a sub type of chars. x marks the spot
			const char expression_char = 'e';
			const char symbol_char = 's';
			const char letter_char = 'x';
			const char WhiteSpace_char = ' ';

			#region cFirstCharAfterByteOrderMark
			var cFirstCharAfterByteOrderMark = Enumerable.ToArray(
				from c in cFirstChar
				where c.xReadByte == 0xEF
				let xBB = c.s.ReadByte()
				where xBB == 0xbb
				let xBF = c.s.ReadByte()
				where xBF == 0xBF
				let xReadByte = c.s.ReadByte()
				// we are in ascii mode. does GLSL do utf encoding? when will we need it?
				let xChar0 = (char)xReadByte
				// 3.1 Character Set and Phases of Compilation
				// https://msdn.microsoft.com/en-us/library/system.char.iswhitespace%28v=vs.110%29.aspx
				let xChar0IsWhiteSpace = char.IsWhiteSpace(xChar0)

				let z = new { xReadByte, xChar0, xChar0IsWhiteSpace, c.f, c.s, c.xGLSLElement }

				#region group for parallel diagnostics and data driven development
				//+		[0]	{ count = 1, xChar = 99 'c', g = {System.Linq.Lookup<<>f__AnonymousType9<char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				//+		[1]	{ count = 1, xChar = 102 'f', g = {System.Linq.Lookup<<>f__AnonymousType9<char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				let xChar0IsLetter = char.IsLetter(z.xChar0)



				group z by new
				{
					xChar0IsLetter,
					xChar0IsWhiteSpace,
					xChar0 =
						xChar0IsLetter ? letter_char :
						xChar0IsWhiteSpace ? WhiteSpace_char : z.xChar0
				} into g

				let count = g.Count()

				// lets look bigger volumes first
				orderby count descending

				select new { count, g.Key.xChar0, g.Key.xChar0IsWhiteSpace, g }
				#endregion

			);


			//+		[2]	{ count = 4, xChar = 13 '\r', g = {System.Linq.Lookup<<>f__AnonymousType9<char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 8, xChar = 35 '#', g = {System.Linq.Lookup<<>f__AnonymousType9<char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[4]	{ count = 11, xChar = 47 '/', g = {System.Linq.Lookup<<>f__AnonymousType9<char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>


			//+		[0]	{ count = 11, xChar = 47 '/', g = {System.Linq.Lookup<<>f__AnonymousType10<bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 8, xChar = 35 '#', g = {System.Linq.Lookup<<>f__AnonymousType10<bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 4, xChar = 13 '\r', g = {System.Linq.Lookup<<>f__AnonymousType10<bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 2, xChar = 120 'x', g = {System.Linq.Lookup<<>f__AnonymousType10<bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>

			// # means a define! either a value, or a code fragment?
			// we need more examples.
			// long path error, bypass via subst for now?
			// X:\jsc.svn\examples\javascript\chrome\apps\WebGL
			// subst w: X:\jsc.svn\examples\javascript\chrome\apps\WebGL

			//+		[0]	{ count = 104, xChar = 47 '/', g = {System.Linq.Lookup<<>f__AnonymousType10<bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 25, xChar = 120 'x', g = {System.Linq.Lookup<<>f__AnonymousType10<bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 24, xChar = 35 '#', g = {System.Linq.Lookup<<>f__AnonymousType10<bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 13, xChar = 13 '\r', g = {System.Linq.Lookup<<>f__AnonymousType10<bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[4]	{ count = 7, xChar = 32 ' ', g = {System.Linq.Lookup<<>f__AnonymousType10<bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 1, xChar = 9 '\t', g = {System.Linq.Lookup<<>f__AnonymousType10<bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>

			// we should implement / processing first.
			// and we could group whitespace. its not fsharp where tabs matter is it.
			// what can we do with comments? discard or keep as documentation? for the next element in the stream?
			// 

			//+		[0]	{ count = 104, xChar = 47 '/', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 25, xChar = 120 'x', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 24, xChar = 35 '#', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 21, xChar = 32 ' ', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			#endregion

			// the defines are scoped to the stream. otherwise clashes will occur
			// could we skip the spaces?


			// how many iterations:
			#region cNoSpace
			var cNoSpace = cFirstCharAfterByteOrderMark;
			// keep an eye on time costs, we are doing a lot of grouping..
			var cNoSpacePassIterations = new List<Stopwatch>();

			while (cNoSpace.Any(g => g.xChar0IsWhiteSpace))
			{
				var cNoSpacePass = Stopwatch.StartNew();

				cNoSpace = Enumerable.ToArray(
				   from g in cNoSpace
				   from c in g.g

					   // if we are a space lets read a new byte otherwise keep the byte we have

				   let xReadByte = char.IsWhiteSpace(c.xChar0) ? c.s.ReadByte() : c.xReadByte
				   let xChar0 = (char)xReadByte

				   // https://msdn.microsoft.com/en-us/library/system.char.iswhitespace%28v=vs.110%29.aspx
				   let xChar0IsWhiteSpace = char.IsWhiteSpace(xChar0)

				   let z = new { xReadByte, xChar0, xChar0IsWhiteSpace, c.f, c.s, c.xGLSLElement }


				   #region group for parallel diagnostics and data driven development
				   //+		[0]	{ count = 1, xChar = 99 'c', g = {System.Linq.Lookup<<>f__AnonymousType9<char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				   //+		[1]	{ count = 1, xChar = 102 'f', g = {System.Linq.Lookup<<>f__AnonymousType9<char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				   let xChar0IsLetter = char.IsLetter(z.xChar0)



				   group z by new
				   {
					   xChar0IsLetter,
					   xChar0IsWhiteSpace,
					   xChar0 =
						   xChar0IsLetter ? letter_char :
						   xChar0IsWhiteSpace ? WhiteSpace_char : z.xChar0
				   } into g

				   let count = g.Count()

				   // lets look bigger volumes first
				   orderby count descending

				   select new { count, g.Key.xChar0, g.Key.xChar0IsWhiteSpace, g }
				   #endregion
 );
				cNoSpacePass.Stop();
				cNoSpacePassIterations.Add(cNoSpacePass);

				// pass complete

				//-		cFirstCharAfterByteOrderMark	{<>f__AnonymousType13<int,char,System.Linq.IGrouping<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>>[4]}	<>f__AnonymousType13<int,char,System.Linq.IGrouping<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>>[]
				//+		[0]	{ count = 104, xChar = 47 '/', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				//+		[1]	{ count = 25, xChar = 120 'x', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				//+		[2]	{ count = 24, xChar = 35 '#', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				//+		[3]	{ count = 21, xChar = 32 ' ', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				//		cNoSpacePass.ElapsedTicks	11609	long
				//-		cNoSpace	{<>f__AnonymousType13<int,char,System.Linq.IGrouping<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>>[4]}	<>f__AnonymousType13<int,char,System.Linq.IGrouping<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>>[]
				//+		[0]	{ count = 105, xChar = 47 '/', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				//+		[1]	{ count = 25, xChar = 120 'x', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				//+		[2]	{ count = 25, xChar = 35 '#', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
				//+		[3]	{ count = 19, xChar = 32 ' ', g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>

				// on the first pass we lost 2 spaces and gained a / and a #


			}

			var cNoSpacePassIterationsElapsed = TimeSpan.FromMilliseconds(cNoSpacePassIterations.Sum(x => x.ElapsedMilliseconds));
			#endregion


			//+		[0]	{ count = 241, xChar0 = 47 '/', xChar0IsWhiteSpace = false, g = {System.Linq.Lookup<<>f__AnonymousType14<bool,bool,char>,<>f__AnonymousType12<int,char,bool,string,System.IO.FileStream,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 63, xChar0 = 120 'x', xChar0IsWhiteSpace = false, g = {System.Linq.Lookup<<>f__AnonymousType14<bool,bool,char>,<>f__AnonymousType12<int,char,bool,string,System.IO.FileStream,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 59, xChar0 = 35 '#', xChar0IsWhiteSpace = false, g = {System.Linq.Lookup<<>f__AnonymousType14<bool,bool,char>,<>f__AnonymousType12<int,char,bool,string,System.IO.FileStream,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>


			#region bugcheck
			//+		[3]	{ count = 1, xChar = 45 '-', IsWhiteSpace = false, g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[4]	{ count = 1, xChar = 59 ';', IsWhiteSpace = false, g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 1, xChar = 61 '=', IsWhiteSpace = false, g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[6]	{ count = 1, xChar = 53 '5', IsWhiteSpace = false, g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>
			//+		[7]	{ count = 1, xChar = 48 '0', IsWhiteSpace = false, g = {System.Linq.Lookup<<>f__AnonymousType11<bool,bool,char>,<>f__AnonymousType8<int,char,string,System.IO.FileStream>>.Grouping} }	<Anonymous Type>

			// -		cNoSpacePassIterations	Count = 83	System.Collections.Generic.List<System.Diagnostics.Stopwatch>

			// how is it possible to start with ; - = 5 0 ?

			// what files are they. 
			// are they valid?
			// are we to discard it as invalid state?
			// is there a bug in our code already?

			// +		[0]	{ xReadByte = 45, xChar = 45 '-', f = "W:\\ChromeShaderToyDigitsByFabrice\\ChromeShaderToyDigitsByFabrice\\Shaders\\Program.frag", s = {System.IO.FileStream} }	<Anonymous Type>
			//Error ENC0279 Modifying 'method' which contains a query expression will prevent the debug session from continuing.ScriptCoreLib.Async X:\jsc.svn\core\ScriptCoreLib.Async\ScriptCoreLib.Async\CompilerServices\GLSLAnalysis.cs    50
			// +		cNoSpacePassIterationsElapsed	{00:00:00.0050000}	System.TimeSpan
			// x:\jsc.svn\examples\javascript\chrome\apps\webgl\chromeshadertoydigitsbyfabrice\chromeshadertoydigitsbyfabrice\shaders\program.frag
			// no we must have a bug.
			// can we isolate it?
			// was a bug. rerun the async test.
			#endregion
	
			// 110 shaders seem to start with a comment.
			// shall we extract a comment from the stream?

			// GLSLComment
			// GLSLDefine
			// GLSLSymbol

			// jsc could run the analyser on assetslib run.
			// the only thing to do is the read the next char too..

			#region cNoSpaceWORD
			var cNoSpaceWORD = Enumerable.ToArray(
				   from g in cNoSpace
				   from c in g.g

				   let xReadByte1 = c.s.ReadByte()
				   let xChar1 = (char)xReadByte1

				   // placeholder for when a line comment is complete a new byte is prepared for the next pass
				   let xReadByteNext0 = -1
				   let xReadByteNext0IsWhiteSpace = false
				   let xReadByteNext0IsLetter = false

				   // x? or #?
				   let xChar0IsLetter0 = char.IsLetter(c.xChar0)
				   let xChar1IsLetter1 = char.IsLetter(xChar1)

				   let IsLineComment = c.xChar0 == '/' && xChar1 == '/'
				   let IsBlockComment = c.xChar0 == '/' && xChar1 == '*'

				   // placeholder
				   let xCommentContentByte = -1
				   let xCommentTermination = false

				   // a placeholder for all comment content until \n or */ inlined twice
				   let xCommentStringBuilder = (IsBlockComment || IsLineComment) ? new StringBuilder() : null

				   //let z = new { xReadByte0, xReadByte1, xChar0, xChar1, c.f, c.s, IsLineComment, IsBlockComment, IsLetter0, IsLetter1 }
				   //let z = new { xChar0, xChar1, xLineCommentContentByte, xCommentTermination, c.f, c.s, IsLineComment, IsBlockComment, IsLetter0, IsLetter1, xCommentStringBuilder }
				   //let z = new { IsBlockComment, IsLineComment, xCommentStringBuilder, c.xChar0, c.xChar0IsWhiteSpace, xChar1, xReadByteNext0, xReadByteNext0IsWhiteSpace, xReadByteNext0IsLetter, xCommentContentByte, xCommentTermination, c.f, c.s, xChar0IsLetter0, xChar1IsLetter1, c.xGLSLElement }
				   let zc = new { IsBlockComment, IsLineComment, xCommentTermination }
				   let z = new { c.xChar0, c.xChar0IsWhiteSpace, xChar1, xReadByteNext0, xReadByteNext0IsWhiteSpace, xReadByteNext0IsLetter, xCommentContentByte, zc, c.f, c.s, xChar0IsLetter0, xChar1IsLetter1, c.xGLSLElement, xCommentStringBuilder }
				   //let z = new { IsPreprocessorDirective, xChar0, xChar1, xGLSLToken, xGLSLMacroFragment, xNameStringBuilderComplete, c.s, c.f, c.xGLSLElement }

				   #region group WORD
				   group z by new
				   {
					   z.xChar0IsWhiteSpace,

					   //IsLetter0,
					   // lets prep the grouping for comment parsing
					   IsBlockComment,
					   IsLineComment,
					   xCommentTermination,

					   xChar0 = xChar0IsLetter0 ? letter_char : c.xChar0,
					   // how to group it?
					   xChar1 = xChar1IsLetter1 ? letter_char : xChar1,

					   // remove?
					   xReadByteNext0IsWhiteSpace,
					   xReadByteNext0IsLetter
				   } into g

				   let count = g.Count()


				   // or do we have p? if we do we should skip such groups!
				   let p = false ? new { IsPreprocessorDirective = default(bool), sGLSLToken = default(string), isGLSLMacroFragment = default(bool), xNameStringBuilderComplete = default(bool) } : null

				   let c = new { g.Key.IsBlockComment, g.Key.IsLineComment, g.Key.xCommentTermination }

				   // lets look bigger volumes first
				   orderby count descending

				   //select new { count, g.Key.xChar0, g.Key.xChar1, g.Key.IsLetter0, g }
				   //select new { count, g.Key.IsBlockComment, g.Key.IsLineComment, g.Key.xCommentTermination, g.Key.xChar0, g.Key.xChar0IsWhiteSpace, g.Key.xChar1, g.Key.xReadByteNext0IsWhiteSpace, g.Key.xReadByteNext0IsLetter, g }
				   select new { count, g.Key.xChar0, g.Key.xChar1, c, g.Key.xChar0IsWhiteSpace, g.Key.xReadByteNext0IsWhiteSpace, g.Key.xReadByteNext0IsLetter, p, g }
				   #endregion

		 );


			;

			//+		[0]	{ count = 223, xChar0 = 47 '/', xChar1 = 47 '/', c = { IsBlockComment = False, IsLineComment = True, xCommentTermination = False }, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, p = null, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 63, xChar0 = 120 'x', xChar1 = 120 'x', c = { IsBlockComment = False, IsLineComment = False, xCommentTermination = False }, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, p = null, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 59, xChar0 = 35 '#', xChar1 = 120 'x', c = { IsBlockComment = False, IsLineComment = False, xCommentTermination = False }, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, p = null, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 18, xChar0 = 47 '/', xChar1 = 42 '*', c = { IsBlockComment = True, IsLineComment = False, xCommentTermination = False }, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, p = null, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>

			// GLSLComment
			// GLSLBlockComment
			// GLSLPragmaDefine
			// GLSLPragmaIfDefined
			// GLSLSymbol

			// comment needs a newline to terminate
			// block comment needs */ to terminate
			#endregion



			//lets resolve the comments

			var cNoLineCommentPassIterations = new List<Stopwatch>();
			var cNoLineComment = cNoSpaceWORD;

			after_cNoLineComment:
			Console.WriteLine("after_cNoLineComment");

			#region cNoLineComment
			// any open line comments? or any spaces to skip?
			//while (cNoLineComment.Any(g => g.xReadByteNext0IsWhiteSpace || g.IsLineComment && !g.xCommentTermination))
			while (cNoLineComment.Any(g => g.c.IsLineComment && !g.c.xCommentTermination))
			{
				var cNoLineCommentPass = Stopwatch.StartNew();

				cNoLineComment = Enumerable.ToArray(
					from g in cNoLineComment
					from c in g.g

						// how can we collect/aggregate the current bytes?

						// applicable for IsComment
						// unless xCommentTermination, then stall and keep current state for next phase

					let xCommentContentByte =
						c.zc.IsLineComment ?
							c.zc.xCommentTermination ?
								c.xCommentContentByte : c.s.ReadByte() : -1

					// did it terminate the line yet?
					// even if we did, how can we resume on the next line?
					// http://stackoverflow.com/questions/3267311/what-is-newline-character-n

					// terminates line comment yet? check for -1?
					let xCommentTermination = xCommentContentByte == '\n'

					// 3.4 Comments
					// , a single-line comment ending in the line-continuation character ( \ ) includes the next
					//line in the comment.


					let xLineCommentStringBuilderAppend =
						c.zc.IsLineComment &&
							// was the comment already complete?
							!c.zc.xCommentTermination
							// is the comment complete now?
							//!xCommentTermination

							// what happens if the stream ends mid way?
							// is the previous byte there?
							&& c.xCommentContentByte >= 0

							// how can we skip \r ?
							&& !(xCommentTermination && c.xCommentContentByte == '\r') ?
								c.xCommentStringBuilder.Append((char)c.xCommentContentByte) : null




					// once we are done with line comment, prep the next char for the next pass
					let xReadByteNext0 =
						// are we already looking at a pending space?
						////c.xReadByteNext0IsWhiteSpace ? c.s.ReadByte() :

						// are we a comment line?
						c.zc.IsLineComment
						// and we have not yet peeked?
						&& c.xReadByteNext0 < 0
						// and we did not complete on the previous run?
						&& !c.zc.xCommentTermination
						// and we complete in the current run
						&& xCommentTermination
						// if so, lets prep a byte
						? c.s.ReadByte()
						// otherwise carry over last result?
						: c.xReadByteNext0

					let xReadByteNext0IsWhiteSpace =
						xReadByteNext0 < 0 ? false : char.IsWhiteSpace((char)xReadByteNext0)

					// what else is there?
					let xReadByteNext0IsLetter =
						xReadByteNext0 < 0 ? false : char.IsLetter((char)xReadByteNext0)

					//do  C.
					//let ref0 = 

					let zc = new { c.zc.IsBlockComment, c.zc.IsLineComment, xCommentTermination }
					let z = new { c.xChar0, c.xChar0IsWhiteSpace, c.xChar1, xReadByteNext0, xReadByteNext0IsWhiteSpace, xReadByteNext0IsLetter, xCommentContentByte, zc, c.f, c.s, c.xChar0IsLetter0, c.xChar1IsLetter1, c.xGLSLElement, c.xCommentStringBuilder }



					// will it group the manual #define s near each other?
					orderby Convert.ToString(z.xCommentStringBuilder)

					group z by new
					{
						z.xChar0IsWhiteSpace,

						// can we mark the comment to be terminated yet and load a new word?
						// or do it on the next pass once all are at the same state?

						// we are to focus on comments
						c.zc.IsBlockComment,
						c.zc.IsLineComment,
						xCommentTermination,

						xChar0 = c.xChar0IsLetter0 ? letter_char : c.xChar0,

						// how to group it?
						xChar1 = c.xChar1IsLetter1 ? letter_char : c.xChar1,

						// prep for next pass
						// we should skip the whitespace if any
						//z.xReadByteNext0,
						z.xReadByteNext0IsWhiteSpace,
						z.xReadByteNext0IsLetter,
					} into g

					let count = g.Count()


					let c = new { g.Key.IsBlockComment, g.Key.IsLineComment, g.Key.xCommentTermination }
					// or do we have it already? if so we shoud had bypassed em?
					let p = false ? new { IsPreprocessorDirective = default(bool), sGLSLToken = default(string), isGLSLMacroFragment = default(bool), xNameStringBuilderComplete = default(bool) } : null


					// lets look bigger volumes first
					orderby g.Key.xCommentTermination, count descending

					//select new { count, g.Key.IsBlockComment, g.Key.IsLineComment, g.Key.xCommentTermination, g.Key.xChar0, g.Key.xChar0IsWhiteSpace, g.Key.xChar1, g.Key.xReadByteNext0IsWhiteSpace, g.Key.xReadByteNext0IsLetter, g }
					select new { count, g.Key.xChar0, g.Key.xChar1, c, g.Key.xChar0IsWhiteSpace, g.Key.xReadByteNext0IsWhiteSpace, g.Key.xReadByteNext0IsLetter, p, g }
				);

				//block comment reading will be done by 2chars at the time
				//a comment can contain anything, including a #define

				// should we care about #defines ? like a manual switch?

				cNoLineCommentPass.Stop();
				cNoLineCommentPassIterations.Add(cNoLineCommentPass);
			}
			#endregion

			var cNoLineCommentPassIterationsElapsed = TimeSpan.FromMilliseconds(cNoLineCommentPassIterations.Sum(x => x.ElapsedMilliseconds));

			// Error ENC0280 Modifying 'method' which contains an anonymous type will prevent the debug session from continuing.ScriptCoreLib.Async X:\jsc.svn\core\ScriptCoreLib.Async\ScriptCoreLib.Async\CompilerServices\GLSLAnalysis.cs    51
			// cNoLineCommentPassIterationsElapsed = {00:00:00.3980000}

			Console.WriteLine("cNoLineCommentPassIterationsElapsed " + new { cNoLineCommentPassIterationsElapsed });

			//NLP on comments?

			// now we need to stash the comments, and read a new byte in
			// whitespace takes awhile? lets isolate whitespace pass
			// um. lets flatten this 3 char state now.
			// if we have line comments, append them to the GLSLDocumentNode or discard, minimize
			// how many iterations are there?

			var cNoSpaceWORDxPassIterations = new List<Stopwatch>();
			var cNoSpaceWORDx = cNoLineComment;


			// called from above
			//+		[0]	{ count = 63, xChar0 = 120 'x', xChar1 = 120 'x', c = { IsBlockComment = False, IsLineComment = False, xCommentTermination = False }, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, p = null, g = {System.Linq.Lookup<<>f__AnonymousType39<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType38<char,bool,char,int,bool,bool,int,<>f__AnonymousType36<bool,bool,bool>,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 59, xChar0 = 35 '#', xChar1 = 120 'x', c = { IsBlockComment = False, IsLineComment = False, xCommentTermination = False }, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, p = null, g = {System.Linq.Lookup<<>f__AnonymousType39<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType38<char,bool,char,int,bool,bool,int,<>f__AnonymousType36<bool,bool,bool>,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 18, xChar0 = 47 '/', xChar1 = 42 '*', c = { IsBlockComment = True, IsLineComment = False, xCommentTermination = False }, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, p = null, g = {System.Linq.Lookup<<>f__AnonymousType39<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType38<char,bool,char,int,bool,bool,int,<>f__AnonymousType36<bool,bool,bool>,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 165, xChar0 = 47 '/', xChar1 = 47 '/', c = { IsBlockComment = False, IsLineComment = True, xCommentTermination = True }, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, p = null, g = {System.Linq.Lookup<<>f__AnonymousType39<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType38<char,bool,char,int,bool,bool,int,<>f__AnonymousType36<bool,bool,bool>,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[4]	{ count = 51, xChar0 = 47 '/', xChar1 = 47 '/', c = { IsBlockComment = False, IsLineComment = True, xCommentTermination = True }, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = true, xReadByteNext0IsLetter = false, p = null, g = {System.Linq.Lookup<<>f__AnonymousType39<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType38<char,bool,char,int,bool,bool,int,<>f__AnonymousType36<bool,bool,bool>,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 7, xChar0 = 47 '/', xChar1 = 47 '/', c = { IsBlockComment = False, IsLineComment = True, xCommentTermination = True }, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = true, p = null, g = {System.Linq.Lookup<<>f__AnonymousType39<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType38<char,bool,char,int,bool,bool,int,<>f__AnonymousType36<bool,bool,bool>,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>

			goto after_cNoSpaceWORDx;
			jump_to_cNoSpaceWORDx_from_cNoPreprocessorDirective:;

			// called from below
			//+		[1]	{ count = 13, xChar0 = 32 ' ', xChar1 = 63 '?', IsPreprocessorDirective = false, sGLSLToken = "", isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 13, xChar0 = 35 '#', xChar1 = 120 'x', IsPreprocessorDirective = false, sGLSLToken = "", isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 4, xChar0 = 47 '/', xChar1 = 47 '/', IsPreprocessorDirective = false, sGLSLToken = "", isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[6]	{ count = 3, xChar0 = 47 '/', xChar1 = 63 '?', IsPreprocessorDirective = true, sGLSLToken = "define", isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			after_cNoSpaceWORDx:;




			Console.WriteLine("cNoSpaceWORDx ");

			#region cNoSpaceWORDx
			do
			{
				var cNoSpaceWORDxPass = Stopwatch.StartNew();

				cNoSpaceWORDx = Enumerable.ToArray(
					  from g in cNoSpaceWORDx
						  // we dont need to look at all groups, only the ones with whitespace?
					  from c in g.g

						  // discard or stash the comment?
					  let xGLSLLineComment = //c.IsLineComment &&
						  c.zc.xCommentTermination
						  ? c.xGLSLElement.AppendComment(c.xCommentStringBuilder, c.zc.IsLineComment, c.zc.IsBlockComment) : null


					  let xxChar0 = c.xReadByteNext0 < 0 ? c.xChar0 : (char)c.xReadByteNext0

					  // previous cleanup pass should have removed all whitespaces
					  let xxChar0IsWhiteSpace = char.IsWhiteSpace(xxChar0)

					  // do we have a reason to read another byte?
					  // only if there is already a new pending byte to look at
					  let xReadByteNext1 = c.xReadByteNext0 < 0 ? -1 : c.s.ReadByte()

					  let xxChar1 = c.xReadByteNext0 < 0 ? c.xChar1 : (char)xReadByteNext1

					  // do we need to queue a new byte if we have a space?
					  let xReadByte3 = xxChar0IsWhiteSpace ? c.s.ReadByte() : -1

					  // can we push a space out now?

					  let xChar0 = xxChar0IsWhiteSpace ? xxChar1 : xxChar0
					  let xChar1 = xxChar0IsWhiteSpace ? (char)xReadByte3 : xxChar1

					  // TrimLeft
					  let xChar0IsWhiteSpace = char.IsWhiteSpace(xChar0)


					  #region now repeat the prep cycle
					  // placeholder for when a line comment is complete a new byte is prepared for the next pass
					  let xReadByteNext0 = -1
					  let xReadByteNext0IsWhiteSpace = false
					  let xReadByteNext0IsLetter = false

					  // x? or #?
					  let xChar0IsLetter0 = char.IsLetter(xChar0)
					  let xChar1IsLetter1 = char.IsLetter(xChar1)

					  let IsLineComment = xChar0 == '/' && xChar1 == '/'
					  let IsBlockComment = xChar0 == '/' && xChar1 == '*'

					  // placeholder
					  let xCommentContentByte = -1
					  let xCommentTermination = false

					  // a placeholder for all comment content until \n or */ inlined twice
					  let xCommentStringBuilder = (IsBlockComment || IsLineComment) ? new StringBuilder() : null


					  //let z = new { xReadByte0, xReadByte1, xChar0, xChar1, c.f, c.s, IsLineComment, IsBlockComment, IsLetter0, IsLetter1 }
					  //let z = new { xChar0, xChar1, xCommentContentByte, xCommentTermination, c.f, c.s, IsLineComment, IsBlockComment, IsLetter0, IsLetter1, xCommentStringBuilder }
					  //let z = new { c.IsLineComment, c.xCommentStringBuilder, c.xChar0, c.xChar0IsWhiteSpace, c.xChar1, xReadByteNext0, xReadByteNext0IsWhiteSpace, xReadByteNext0IsLetter, xCommentContentByte, xCommentTermination, c.f, c.s, c.IsBlockComment, c.IsLetter0, c.IsLetter1, c.xGLSLElement }
					  //let z = new { IsBlockComment, IsLineComment, xCommentStringBuilder, xChar0, xChar0IsWhiteSpace, xChar1, xReadByteNext0, xReadByteNext0IsWhiteSpace, xReadByteNext0IsLetter, xCommentContentByte, xCommentTermination, c.f, c.s, xChar0IsLetter0, xChar1IsLetter1, c.xGLSLElement }
					  let zc = new { IsBlockComment, IsLineComment, xCommentTermination }
					  let z = new { xChar0, xChar0IsWhiteSpace, xChar1, xReadByteNext0, xReadByteNext0IsWhiteSpace, xReadByteNext0IsLetter, xCommentContentByte, zc, c.f, c.s, xChar0IsLetter0, xChar1IsLetter1, c.xGLSLElement, xCommentStringBuilder }
					  #endregion



					  // fake select, to get the compiler off my back.
					  //select c

					  orderby xChar0, xChar1

					  #region group WORD
					  group z by new
					  {
						  //IsLetter0,
						  // lets prep the grouping for comment parsing

						  z.xChar0IsWhiteSpace,

						  IsBlockComment,
						  IsLineComment,
						  xCommentTermination,

						  // by this stage we will want to see whats the symbol for..
						  xChar0 = // xChar0IsLetter0 ? letter_char :
								 xChar0IsWhiteSpace ? WhiteSpace_char : xChar0

							 ,
						  // how to group it?
						  //xChar1 = xChar1IsLetter1 ? letter_char : xChar1,
						  //xChar1 = xChar1IsLetter1 && xChar0 != '#' ? letter_char : xChar1,
						  xChar1 = xChar1,


						  // remove?
						  xReadByteNext0IsWhiteSpace,
						  xReadByteNext0IsLetter
					  } into g

					  let count = g.Count()

					  // lets look bigger volumes first
					  orderby count descending

					  let c = new { g.Key.IsBlockComment, g.Key.IsLineComment, g.Key.xCommentTermination }
					  let p = false ? new { IsPreprocessorDirective = default(bool), sGLSLToken = default(string), isGLSLMacroFragment = default(bool), xNameStringBuilderComplete = default(bool) } : null

					  //select new { count, g.Key.xChar0, g.Key.xChar1, g.Key.IsLetter0, g }
					  //select new { count, g.Key.IsBlockComment, g.Key.IsLineComment, g.Key.xCommentTermination, g.Key.xChar0, g.Key.xChar0IsWhiteSpace, g.Key.xChar1, g.Key.xReadByteNext0IsWhiteSpace, g.Key.xReadByteNext0IsLetter, g }
					  select new { count, g.Key.xChar0, g.Key.xChar1,  /* /? */c, g.Key.xChar0IsWhiteSpace, g.Key.xReadByteNext0IsWhiteSpace, g.Key.xReadByteNext0IsLetter  /* /? */, p /* # */ , g }

   //select new { count, g.Key.xChar0, g.Key.xChar1, /* # */ g.Key.IsPreprocessorDirective, g.Key.sGLSLToken, g.Key.isGLSLMacroFragment, g.Key.xNameStringBuilderComplete /* # */, g } : new[] { gg }

					  #endregion
   );

				cNoSpaceWORDxPass.Stop();
				cNoSpaceWORDxPassIterations.Add(cNoSpaceWORDxPass);

				//var cNoLineCommentPassIterationsElapsed = TimeSpan.FromMilliseconds(cNoLineCommentPassIterations.Sum(x => x.ElapsedMilliseconds));


				//var cNoSpaceWORDy = cNoLineComment;
				//cNoSpaceWORDy = cNoSpaceWORDx;

				// look another 110 comments to parse
				//+		[0]	{ count = 110, IsLineComment = true, xCommentTermination = false, xChar0 = 47 '/', xChar1 = 47 '/', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
				// keep pragmas for later
				//+		[1]	{ count = 45, IsLineComment = false, xCommentTermination = false, xChar0 = 35 '#', xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
				// keep typerefs for later
				//+		[2]	{ count = 40, IsLineComment = false, xCommentTermination = false, xChar0 = 120 'x', xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
				// how is this possible? we did not run the cleanup yet?
				//+		[3]	{ count = 28, IsLineComment = false, xCommentTermination = false, xChar0 = 13 '\r', xChar1 = 10 '\n', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
				// keep block comments for later
				//+		[4]	{ count = 9, IsLineComment = false, xCommentTermination = false, xChar0 = 47 '/', xChar1 = 42 '*', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
				// how is this possible? we did not run the cleanup yet?
				//+		[5]	{ count = 1, IsLineComment = false, xCommentTermination = false, xChar0 = 32 ' ', xChar1 = 32 ' ', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			}
			while (cNoSpaceWORDx.Any(x => x.xChar0IsWhiteSpace || x.xReadByteNext0IsWhiteSpace));

			#endregion

			var cNoSpaceWORDxPassIterationsElapsed = TimeSpan.FromMilliseconds(cNoSpaceWORDxPassIterations.Sum(x => x.ElapsedMilliseconds));

			//+		[0]	{ count = 116, IsLineComment = true, xCommentTermination = false, xChar0 = 47 '/', xChar0IsWhiteSpace = false, xChar1 = 47 '/', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 58, IsLineComment = false, xCommentTermination = false, xChar0 = 35 '#', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 46, IsLineComment = false, xCommentTermination = false, xChar0 = 120 'x', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 13, IsLineComment = false, xCommentTermination = false, xChar0 = 47 '/', xChar0IsWhiteSpace = false, xChar1 = 42 '*', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			// ?. ?
			if (cNoSpaceWORDx.Any(x => x.c.IsLineComment))
			{
				cNoLineComment = cNoSpaceWORDx;
				goto after_cNoLineComment;
			}

			//+		[0]	{ count = 110, IsLineComment = false, xCommentTermination = false, xChar0 = 35 '#', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 109, IsLineComment = false, xCommentTermination = false, xChar0 = 120 'x', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 14, IsLineComment = false, xCommentTermination = false, xChar0 = 47 '/', xChar0IsWhiteSpace = false, xChar1 = 42 '*', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			// looks like #define is to win by one?
			// shall we get more examples to decide which to implement?
			// box comment is rather unpopular at this early point in parallel parsing?

			// seems we are to load that symbol.
			//+		[0]	{ count = 119, IsLineComment = false, xCommentTermination = false, xChar0 = 120 'x', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			// pragma defines
			//+		[1]	{ count = 115, IsLineComment = false, xCommentTermination = false, xChar0 = 35 '#', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			// lets look at the block comments the latest
			//+		[2]	{ count = 17, IsLineComment = false, xCommentTermination = false, xChar0 = 47 '/', xChar0IsWhiteSpace = false, xChar1 = 42 '*', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>


			/*
			// pragmas may turn out to be inline defines?
			*/
			// should we keep statistics on language features found?

			//+		[0]	{ count = 119, IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0 = 120 'x', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 118, IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0 = 35 '#', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 18, IsBlockComment = true, IsLineComment = false, xCommentTermination = false, xChar0 = 47 '/', xChar0IsWhiteSpace = false, xChar1 = 42 '*', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			var cNoBlockCommentPassIterations = new List<Stopwatch>();
			var cNoBlockComment = cNoSpaceWORDx;

			var BlockComments = cNoBlockComment.Count(g => g.c.IsBlockComment && !g.c.xCommentTermination);

			// X:\jsc.svn\examples\merge\test\TestLINQJoinConcat\TestLINQJoinConcat\Program.cs
			Console.WriteLine(new { BlockComments, cNoBlockComment.Length });

			#region cNoBlockComment
			if (cNoBlockComment.Any(g => g.c.IsBlockComment && !g.c.xCommentTermination))
			{

				do
				{
					var cNoBlockCommentPass = Stopwatch.StartNew();

					cNoBlockComment = xEnumerable.SelectManyToArray(
					   // http://stackoverflow.com/questions/12397880/using-linq-how-to-select-conditionally-some-items-but-when-no-conditions-select
					   from gg in cNoBlockComment

						   // some groups we dont want to look at. can we jump/skip/ select it?
						   //if select goto discard

					   select gg.c.IsBlockComment && !gg.c.xCommentTermination ?

					   from c in gg.g

						   // if we are a comment, we need to read a byte unless the comment is done reading. then keep the state
					   let xCommentContentByte =
						  c.zc.IsBlockComment ?
							  c.zc.xCommentTermination ?
								  c.xCommentContentByte : c.s.ReadByte() : -1

					   // did it terminate the line yet?
					   // even if we did, how can we resume on the next line?
					   // http://stackoverflow.com/questions/3267311/what-is-newline-character-n

					   // placeholder
					   let xCommentTermination =
						   // keep state?
						   c.zc.xCommentTermination ? true : (c.xCommentContentByte == '*' && xCommentContentByte == '/')


					   let xLineCommentStringBuilderAppend =
						   c.zc.IsBlockComment &&
							   // was the comment already complete?
							   !c.zc.xCommentTermination
							   // is the comment complete now?
							   //!xCommentTermination

							   // what happens if the stream ends mid way?
							   // is the previous byte there?
							   && !xCommentTermination ?
								   c.xCommentStringBuilder.Append((char)c.xCommentContentByte) : null




					   // once we are done with line comment, prep the next char for the next pass
					   let xReadByteNext0 =
						  // are we already looking at a pending space?
						  ////c.xReadByteNext0IsWhiteSpace ? c.s.ReadByte() :

						  // are we a comment line?
						  c.zc.IsBlockComment
						  // and we have not yet peeked?
						  && c.xReadByteNext0 < 0
						  // and we did not complete on the previous run?
						  && !c.zc.xCommentTermination
						  // and we complete in the current run
						  && xCommentTermination
						  // if so, lets prep a byte
						  ? c.s.ReadByte()
						  // otherwise carry over last result?
						  : c.xReadByteNext0

					   let xReadByteNext0IsWhiteSpace =
						   xReadByteNext0 < 0 ? false : char.IsWhiteSpace((char)xReadByteNext0)

					   // what else is there?
					   let xReadByteNext0IsLetter =
						  xReadByteNext0 < 0 ? false : char.IsLetter((char)xReadByteNext0)


					   //let z = new { c.xChar0, c.xChar1, xCommentContentByte, xCommentTermination, c.f, c.s, c.IsLineComment, c.IsBlockComment }
					   //let z = new { c.xChar0, c.xChar1, xCommentContentByte, xCommentTermination, c.f, c.s, c.IsLineComment, c.IsBlockComment, c.IsLetter0, c.IsLetter1 }
					   //let z = new { c.IsBlockComment, c.IsLineComment, c.xCommentStringBuilder, c.xChar0, c.xChar0IsWhiteSpace, c.xChar1, xReadByteNext0, xReadByteNext0IsWhiteSpace, xReadByteNext0IsLetter, xCommentContentByte, xCommentTermination, c.f, c.s, c.xChar0IsLetter0, c.xChar1IsLetter1, c.xGLSLElement }
					   let zc = new { c.zc.IsBlockComment, c.zc.IsLineComment, xCommentTermination }
					   let z = new { c.xChar0, c.xChar0IsWhiteSpace, c.xChar1, xReadByteNext0, xReadByteNext0IsWhiteSpace, xReadByteNext0IsLetter, xCommentContentByte, zc, c.f, c.s, c.xChar0IsLetter0, c.xChar1IsLetter1, c.xGLSLElement, c.xCommentStringBuilder }

					   // will it group the manual #define s near each other?
					   orderby Convert.ToString(z.xCommentStringBuilder)

					   group z by new
					   {
						   z.xChar0IsWhiteSpace,

						   // can we mark the comment to be terminated yet and load a new word?
						   // or do it on the next pass once all are at the same state?

						   // we are to focus on comments
						   c.zc.IsBlockComment,
						   c.zc.IsLineComment,
						   xCommentTermination,

						   //xChar0 = c.xChar0IsLetter0 ? letter_char : c.xChar0,
						   xChar0 = c.xChar0,

						   // how to group it?
						   //xChar1 = c.xChar1IsLetter1 && c.xChar0 != '#' ? letter_char : c.xChar1,
						   xChar1 = c.xChar1,

						   // prep for next pass
						   // we should skip the whitespace if any
						   //z.xReadByteNext0,
						   z.xReadByteNext0IsWhiteSpace,
						   z.xReadByteNext0IsLetter,
					   } into g

					   let count = g.Count()

					   // lets look bigger volumes first
					   //orderby g.Key.xCommentTermination, count descending
					   orderby count descending

					   let c = new { g.Key.IsBlockComment, g.Key.IsLineComment, g.Key.xCommentTermination }
					   let p = false ? new { IsPreprocessorDirective = default(bool), sGLSLToken = default(string), isGLSLMacroFragment = default(bool), xNameStringBuilderComplete = default(bool) } : null

					   //select new { count, g.Key.IsBlockComment, g.Key.IsLineComment, g.Key.xCommentTermination, g.Key.xChar0, g.Key.xChar0IsWhiteSpace, g.Key.xChar1, g.Key.xReadByteNext0IsWhiteSpace, g.Key.xReadByteNext0IsLetter, g }
					   select new { count, g.Key.xChar0, g.Key.xChar1, c, g.Key.xChar0IsWhiteSpace, g.Key.xReadByteNext0IsWhiteSpace, g.Key.xReadByteNext0IsLetter, p, g } : new[] { gg }

				   //join g in cNoBlockComment on true equals !(g.IsBlockComment && !g.xCommentTermination)
				   //from g in cNoBlockComment
				   //where !(g.IsBlockComment && !g.xCommentTermination)
				   //select g into g2


				   //select new[] { x, g2 }

				   );

					cNoBlockCommentPass.Stop();
					cNoBlockCommentPassIterations.Add(cNoBlockCommentPass);


				}
				while (cNoBlockComment.Any(g => g.c.IsBlockComment && !g.c.xCommentTermination));

				var cNoBlockCommentPassIterationsElapsed = TimeSpan.FromMilliseconds(cNoBlockCommentPassIterations.Sum(x => x.ElapsedMilliseconds));
				Console.WriteLine("cNoBlockCommentPassIterationsElapsed " + new { cNoBlockCommentPassIterationsElapsed });

				//+		[0]	{ count = 119, IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0 = 120 'x', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
				//+		[1]	{ count = 118, IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0 = 35 '#', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
				//+		[2]	{ count = 18, IsBlockComment = true, IsLineComment = false, xCommentTermination = true, xChar0 = 47 '/', xChar0IsWhiteSpace = false, xChar1 = 42 '*', xReadByteNext0IsWhiteSpace = true, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
				//		f	"W:\\ChromeShaderToySymmetricOriginsByGood\\ChromeShaderToySymmetricOriginsByGood\\Shaders\\Program.frag"	string


				cNoSpaceWORDx = cNoBlockComment;
				goto after_cNoSpaceWORDx;
			}
			#endregion

			//+       [0] { count = 1, IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0 = 120 'x', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = { System.Linq.Lookup <<> f__AnonymousType36<bool, bool, bool, bool, char, char, bool, bool>,<> f__AnonymousType35 < bool,bool, System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string, System.IO.FileStream,bool,bool, ScriptCoreLib.CompilerServices.GLSLElement >>.Grouping} }    < Anonymous Type >


			//f = "W:\\ChromeShaderToyOculusTestByDaeken\\ChromeShaderToyOculusTestByDaeken\\Shaders\\Program.frag"

			//+		[0]	{ count = 128, IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0 = 120 'x', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 127, IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0 = 35 '#', xChar0IsWhiteSpace = false, xChar1 = 120 'x', xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType36<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType35<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			// { cNoBlockCommentPassIterationsElapsed = 00:00:10.4270000 }

			// i wonder weould we be able to parse glsl in gpu?
			// http://stackoverflow.com/questions/12057746/webgl-for-gpgpu
			// https://github.com/graphitemaster/glsl-parser
			// https://www.khronos.org/opengles/sdk/tools/Reference-Compiler/

			// http://blogs.igalia.com/itoral/2015/03/03/an-introduction-to-mesas-glsl-compiler-i/
			// http://blogs.igalia.com/itoral/2015/03/06/an-introduction-to-mesas-glsl-compiler-ii/
			// https://openclnet.codeplex.com/
			// http://stackoverflow.com/questions/5654048/complete-net-opencl-implementations
			// https://openclnet.codeplex.com/SourceControl/latest#trunk/OpenCL.Net/Cl.API.cs
			// http://www.mesa3d.org/shading.html
			// http://oss.sgi.com/projects/ogl-sample/registry/ARB/vertex_program.txt
			// http://piglit.freedesktop.org/
			// http://cgit.freedesktop.org/piglit/tree/examples/glsl_parser_test/bit-logic-assign.frag?id=622d63dcfc12c5d347c1c427915d35de96ccc74c

			//+		[0]	{ count = 137, xChar0 = 120 'x', xChar1 = 120 'x', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 137, xChar0 = 35 '#', xChar1 = 120 'x', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			//+		[0]	{ count = 137, xChar0 = 120 'x', xChar1 = 120 'x', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 121, xChar0 = 35 '#', xChar1 = 100 'd', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 16, xChar0 = 35 '#', xChar1 = 105 'i', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<bool,bool,System.Text.StringBuilder,char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			// are we to read the symbol?
			// lets make that group more readable in debugger, and order it.

			// popular seems to be
			// const or float 

			//+		[0]	{ count = 122, xChar0 = 35 '#', xChar1 = 100 'd', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 52, xChar0 = 102 'f', xChar1 = 108 'l', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 45, xChar0 = 99 'c', xChar1 = 111 'o', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 19, xChar0 = 118 'v', xChar1 = 101 'e', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[4]	{ count = 16, xChar0 = 35 '#', xChar1 = 105 'i', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 7, xChar0 = 115 's', xChar1 = 116 't', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[6]	{ count = 4, xChar0 = 109 'm', xChar1 = 97 'a', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[7]	{ count = 3, xChar0 = 118 'v', xChar1 = 111 'o', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[8]	{ count = 2, xChar0 = 98 'b', xChar1 = 111 'o', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[9]	{ count = 2, xChar0 = 105 'i', xChar1 = 110 'n', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[10]	{ count = 2, xChar0 = 112 'p', xChar1 = 114 'r', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>
			//+		[11]	{ count = 1, xChar0 = 117 'u', xChar1 = 110 'n', IsBlockComment = false, IsLineComment = false, xCommentTermination = false, xChar0IsWhiteSpace = false, xReadByteNext0IsWhiteSpace = false, xReadByteNext0IsLetter = false, g = {System.Linq.Lookup<<>f__AnonymousType37<bool,bool,bool,bool,char,char,bool,bool>,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>.Grouping} }	<Anonymous Type>

			// i think its time for a third byte!

			#region cNoPreprocessorDirective
			var cNoPreprocessorDirective = Enumerable.ToArray(
				from gg in cNoBlockComment
				from c in gg.g


					// whats it called?
					// 3.3 Preprocessor

					// https://msdn.microsoft.com/en-us/library/ed8yd1ha.aspx
					// Each directive is terminated by a new line.
				let IsPreprocessorDirective = c.xChar0 == '#'


				// is there a reason not to read a third byte yet?
				let xGLSLToken = new StringBuilder()
				let sGLSLToken = Convert.ToString(xGLSLToken)

				// we should read until token completes

				let xChar0 = IsPreprocessorDirective ? c.xChar1 : c.xChar0
				let xReadByte1 = IsPreprocessorDirective ? c.s.ReadByte() : c.xChar1
				let xChar1 = (char)xReadByte1


				let xGLSLMacroFragment = default(GLSLMacroFragment)
				let xNameStringBuilderComplete = false

				// ! once
				let z = new { IsPreprocessorDirective, xChar0, xChar1, xGLSLToken, xGLSLMacroFragment, xNameStringBuilderComplete, c.s, c.f, c.xGLSLElement }

				orderby z.IsPreprocessorDirective descending, z.xChar0, z.xChar1 //, z.xGLSLToken

				group z by new
				{
					z.IsPreprocessorDirective,

					sGLSLToken,

					// lets allow whitespace to be grouped
					xChar0 =
						char.IsWhiteSpace(z.xChar0) ? ' ' :
						// wo could aswell only use the group variable?
						!z.IsPreprocessorDirective && char.IsLetter(z.xChar0) ? letter_char : z.xChar0
						,
					xChar1 =
						// we dont care about char1 if char0 is whitespace
						char.IsWhiteSpace(z.xChar0) ? '?' :
						!z.IsPreprocessorDirective && char.IsLetter(z.xChar1) ? letter_char : z.xChar1,


					// cannot group  by or order by StringBuilder
					//xGLSLToken = z.xGLSLToken,
					isGLSLMacroFragment = false,
					z.xNameStringBuilderComplete
				} into g

				let count = g.Count()

				//  add g.Key.sGLSLToken, g.Key.isGLSLMacroFragment, g.Key.xNameStringBuilderComplete
				let c = false ? new { IsBlockComment = false, IsLineComment = false, xCommentTermination = false } : null
				let p = new { g.Key.IsPreprocessorDirective, g.Key.sGLSLToken, g.Key.isGLSLMacroFragment, g.Key.xNameStringBuilderComplete }

				orderby g.Key.IsPreprocessorDirective descending, count descending, g.Key.sGLSLToken, g.Key.xChar0, g.Key.xChar1 //, g.Key.xGLSLToken
				select new { count, g.Key.xChar0, g.Key.xChar1, c, p, g }

			);
			#endregion


			// #define?
			//+		[0]	{ count = 122, xChar0 = 35 '#', xChar1 = 100 'd', xChar2 = 101 'e', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 16, xChar0 = 35 '#', xChar1 = 105 'i', xChar2 = 102 'f', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>

			// float ?
			//+		[2]	{ count = 52, xChar0 = 102 'f', xChar1 = 108 'l', xChar2 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>

			// const ?
			//+		[3]	{ count = 45, xChar0 = 99 'c', xChar1 = 111 'o', xChar2 = 110 'n', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>

			// vec?
			//+		[4]	{ count = 19, xChar0 = 118 'v', xChar1 = 101 'e', xChar2 = 99 'c', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			// struct?
			//+		[5]	{ count = 7, xChar0 = 115 's', xChar1 = 116 't', xChar2 = 114 'r', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			// matrix?
			//+		[6]	{ count = 4, xChar0 = 109 'm', xChar1 = 97 'a', xChar2 = 116 't', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			// void?
			//+		[7]	{ count = 3, xChar0 = 118 'v', xChar1 = 111 'o', xChar2 = 105 'i', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			// bool?
			//+		[8]	{ count = 2, xChar0 = 98 'b', xChar1 = 111 'o', xChar2 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			// int?
			//+		[9]	{ count = 2, xChar0 = 105 'i', xChar1 = 110 'n', xChar2 = 116 't', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			// precise?
			//+		[10]	{ count = 2, xChar0 = 112 'p', xChar1 = 114 'r', xChar2 = 101 'e', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			// uniform?
			//+		[11]	{ count = 1, xChar0 = 117 'u', xChar1 = 110 'n', xChar2 = 105 'i', g = {System.Linq.Lookup<<>f__AnonymousType77<char,char,char>,<>f__AnonymousType76<char,char,char,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>


			//+		[0]	{ count = 132, xChar0 = 35 '#', xChar1 = 100 'd', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 17, xChar0 = 35 '#', xChar1 = 105 'i', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 53, xChar0 = 102 'f', xChar1 = 108 'l', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 50, xChar0 = 99 'c', xChar1 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[4]	{ count = 20, xChar0 = 118 'v', xChar1 = 101 'e', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 7, xChar0 = 115 's', xChar1 = 116 't', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[6]	{ count = 5, xChar0 = 109 'm', xChar1 = 97 'a', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[7]	{ count = 5, xChar0 = 118 'v', xChar1 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[8]	{ count = 3, xChar0 = 112 'p', xChar1 = 114 'r', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[9]	{ count = 2, xChar0 = 98 'b', xChar1 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[10]	{ count = 2, xChar0 = 105 'i', xChar1 = 110 'n', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[11]	{ count = 1, xChar0 = 117 'u', xChar1 = 110 'n', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>


			// feels like the first quantum program:D
			// https://www.opengl.org/sdk/docs/man/

			// keyword 
			// identifier
			// 4 Variables and Types
			// User-defined types may be defined using struct to aggregate a list of existing types into a single name.
			// 4.1.8 Structures

			// 4.3 Storage Qualifiers
			// 4.3.2 Constant Qualifier
			// 9 Shading Language Grammar for Core			Profile


			//			+		[0]	{ count = 132, IsPreprocessorDirective = true, xChar0 = 100 'd', xChar1 = 101 'e', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 17, IsPreprocessorDirective = true, xChar0 = 105 'i', xChar1 = 102 'f', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>

			//+		[2]	{ count = 53, IsPreprocessorDirective = false, xChar0 = 102 'f', xChar1 = 108 'l', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 50, IsPreprocessorDirective = false, xChar0 = 99 'c', xChar1 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[4]	{ count = 20, IsPreprocessorDirective = false, xChar0 = 118 'v', xChar1 = 101 'e', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 7, IsPreprocessorDirective = false, xChar0 = 115 's', xChar1 = 116 't', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[6]	{ count = 5, IsPreprocessorDirective = false, xChar0 = 109 'm', xChar1 = 97 'a', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[7]	{ count = 5, IsPreprocessorDirective = false, xChar0 = 118 'v', xChar1 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[8]	{ count = 3, IsPreprocessorDirective = false, xChar0 = 112 'p', xChar1 = 114 'r', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[9]	{ count = 2, IsPreprocessorDirective = false, xChar0 = 98 'b', xChar1 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[10]	{ count = 2, IsPreprocessorDirective = false, xChar0 = 105 'i', xChar1 = 110 'n', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>
			//+		[11]	{ count = 1, IsPreprocessorDirective = false, xChar0 = 117 'u', xChar1 = 110 'n', g = {System.Linq.Lookup<<>f__AnonymousType79<bool,char,char>,<>f__AnonymousType78<bool,char,char,System.Text.StringBuilder,<>f__AnonymousType36<char,bool,char,int,bool,bool,int,bool,string,System.IO.FileStream,bool,bool,ScriptCoreLib.CompilerServices.GLSLElement,bool,bool,System.Text.StringBuilder>>>.Grouping} }	<Anonymous Type>




			var cNoPreprocessorDirectivePassIterations = new List<Stopwatch>();
			#region cNoPreprocessorDirective
			while (cNoPreprocessorDirective.Any(gg => (gg.p.IsPreprocessorDirective && char.IsLetter(gg.xChar0))))
			{
				var cNoPreprocessorDirectivePass = Stopwatch.StartNew();

				cNoPreprocessorDirective = xEnumerable.SelectManyToArray(
				   from gg in cNoPreprocessorDirective
				   select (gg.p.IsPreprocessorDirective && char.IsLetter(gg.xChar0)) ?
				   from c in gg.g

					   // stash one byte and read one byte

				   let xGLSLToken = c.xGLSLToken.Append(c.xChar0)
				   let sGLSLToken = Convert.ToString(xGLSLToken)

				   let xChar0 = c.xChar1

				   let xReadByte1 = c.s.ReadByte()
				   let xChar1 = (char)xReadByte1

				   // did we just complete a read for define?

				   let xGLSLMacroFragment = c.IsPreprocessorDirective && sGLSLToken == "define" && char.IsWhiteSpace(xChar0) ?
						// next pass will need to read the proposed name of the fragment
						new GLSLMacroFragment
						{
							// await until spaces are skipped?
							//NameStringBuilder = new StringBuilder { }
						} : null

				   let xNameStringBuilderComplete = false

				   // keep
				   //let z = new { c.IsPreprocessorDirective, xChar0, xChar1, xGLSLToken, xGLSLMacroFragment, xNameStringBuilderComplete, c.s, c.f }
				   let z = new { c.IsPreprocessorDirective, xChar0, xChar1, xGLSLToken, xGLSLMacroFragment, xNameStringBuilderComplete, c.s, c.f, c.xGLSLElement }



				   orderby z.IsPreprocessorDirective descending, z.xChar0, z.xChar1	//, z.xGLSLToken

				   group z by new
				   {
					   z.IsPreprocessorDirective,

					   sGLSLToken,

					   // lets allow whitespace to be grouped
					   xChar0 =
						char.IsWhiteSpace(z.xChar0) ? ' ' :
						// wo could aswell only use the group variable?
						!z.IsPreprocessorDirective && z.xChar0.IsLetterOrDigitOrUnderscore() ? letter_char : z.xChar0
						,
					   xChar1 =
						// we dont care about char1 if char0 is whitespace
						char.IsWhiteSpace(z.xChar0) ? '?' :
						!z.IsPreprocessorDirective && z.xChar1.IsLetterOrDigitOrUnderscore() ? letter_char : z.xChar1
						,

					   // cannot group  by or order by StringBuilder
					   //xGLSLToken = z.xGLSLToken,

					   // we cannot use it in group can we? as boolean we can
					   isGLSLMacroFragment = z.xGLSLMacroFragment != null,
					   z.xNameStringBuilderComplete
				   } into g

				   let count = g.Count()

				   let c = false ? new { IsBlockComment = false, IsLineComment = false, xCommentTermination = false } : null
				   let p = new { g.Key.IsPreprocessorDirective, g.Key.sGLSLToken, g.Key.isGLSLMacroFragment, g.Key.xNameStringBuilderComplete }

				   // isGLSLPreprocessorDirective
				   orderby g.Key.IsPreprocessorDirective descending, count descending, g.Key.sGLSLToken, g.Key.xChar0, g.Key.xChar1	//, g.Key.xGLSLToken
				   select new { count, g.Key.xChar0, g.Key.xChar1, c, p, g } : new[] { gg }
				);


				cNoPreprocessorDirectivePass.Stop();
				cNoPreprocessorDirectivePassIterations.Add(cNoPreprocessorDirectivePass);
			}
			#endregion


			var cNoPreprocessorDirectivePassIterationsElapsed = TimeSpan.FromMilliseconds(cNoPreprocessorDirectivePassIterations.Sum(x => x.ElapsedMilliseconds));
			Console.WriteLine("cNoPreprocessorDirectivePassIterationsElapsed " + new { cNoPreprocessorDirectivePassIterationsElapsed });

			//+		[0]	{ count = 141, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 63 '?', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>

			//+		[1]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>

			//+		[3]	{ count = 57, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 102 'f', xChar1 = 108 'l', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[4]	{ count = 51, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 99 'c', xChar1 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 21, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 118 'v', xChar1 = 101 'e', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[6]	{ count = 7, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 115 's', xChar1 = 116 't', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[7]	{ count = 6, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 109 'm', xChar1 = 97 'a', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[8]	{ count = 5, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 118 'v', xChar1 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[9]	{ count = 3, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 112 'p', xChar1 = 114 'r', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[10]	{ count = 2, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 98 'b', xChar1 = 111 'o', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[11]	{ count = 2, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 105 'i', xChar1 = 110 'n', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[12]	{ count = 1, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 117 'u', xChar1 = 110 'n', g = {System.Linq.Lookup<<>f__AnonymousType81<bool,string,char,char>,<>f__AnonymousType80<bool,char,char,System.Text.StringBuilder,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>

			// http://www.cplusplus.com/doc/tutorial/preprocessor/
			// #define can work also with parameters to define function macros:
			// https://msdn.microsoft.com/en-us/library/teas0593.aspx
			// https://gcc.gnu.org/onlinedocs/cpp/Macros.html
			// A macro is a fragment of code which has been given a name. Whenever the name is used, it is replaced by the contents of the macro. There are two kinds of macros. They differ mostly in what they look like when they are used. Object-like macros resemble data objects when used, function-like macros resemble function calls.
			// https://gcc.gnu.org/onlinedocs/cpp/Common-Predefined-Macros.html#Common-Predefined-Macros


			//+		[0]	{ count = 141, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 155, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>


			// i think we are about to read in a GLSLMacroFragment
			// isGLSLMacroFragment  is removed once completed by a newline?
			while (cNoPreprocessorDirective.Any(gg => gg.p.isGLSLMacroFragment && !gg.p.xNameStringBuilderComplete))
			{
				var cNoPreprocessorDirectivePass = Stopwatch.StartNew();
				#region xGLSLMacroFragment.NameStringBuilder
				cNoPreprocessorDirective = xEnumerable.SelectManyToArray(
					from gg in cNoPreprocessorDirective

						// we should stop reading the name if the char0 is no longer symbol char yet NameStringBuilder is still there

					select
						gg.p.isGLSLMacroFragment && !gg.p.xNameStringBuilderComplete
						?	// could the while above be linked to this group selection?
					from c in gg.g

						// tell the upper do while we are still working!

						// read the name!.. or skip spaces actually?
						// append only if we have skipped reading whitespace
					let xAppend = c.xGLSLMacroFragment.NameStringBuilder != null ?
						// are we supposed to read this char?
						c.xGLSLMacroFragment.NameStringBuilder.Append(c.xChar0) : null

					// do we need to keep reading?
					let xChar0 = c.xChar1

					// we are in name reading mode, and the char we prepeared for the next read no longer will be part of the name
					let xNameStringBuilderComplete = c.xGLSLMacroFragment.NameStringBuilder != null && !xChar0.IsLetterOrDigitOrUnderscore()

					// https://gcc.gnu.org/onlinedocs/cpp/Swallowing-the-Semicolon.html
					let xArguments = xNameStringBuilderComplete && xChar0 == '(' ? c.xGLSLMacroFragment.Arguments = new List<StringBuilder> { } : null



					let xReadByte1 = c.s.ReadByte()
					let xChar1 = (char)xReadByte1

					// are we done reading whitespaces?
					let xNameStringBuilder = !xChar0.IsLetterOrDigitOrUnderscore() ?
						// still reading whitespace
						null :
						// are we already in name reading mode, past reading whitespace?
						// look we snook the ?? in!!:D
						c.xGLSLMacroFragment.NameStringBuilder ?? (c.xGLSLMacroFragment.NameStringBuilder = new StringBuilder { })


					//let z = new { c.IsPreprocessorDirective, xChar0, xChar1, c.xGLSLToken, c.xGLSLMacroFragment, xNameStringBuilderComplete, c.s, c.f }
					let z = new { c.IsPreprocessorDirective, xChar0, xChar1, c.xGLSLToken, c.xGLSLMacroFragment, xNameStringBuilderComplete, c.s, c.f, c.xGLSLElement }



					orderby z.IsPreprocessorDirective descending, z.xChar0, z.xChar1	//, z.xGLSLToken

					group z by new
					{
						z.IsPreprocessorDirective,

						gg.p.sGLSLToken,

						// lets allow whitespace to be grouped
						xChar0 =
						//char.IsWhiteSpace(z.xChar0) ? ' ' :
						z.xChar0.IsLetterOrDigitOrUnderscore() ? symbol_char : z.xChar0


						,
						xChar1 =
						// if char1 is no longer a letter, we know we are about to complete the name read
						//  \n will complete the macro def
						//char.IsWhiteSpace(z.xChar1) ? ' ' :
						z.xChar1.IsLetterOrDigitOrUnderscore() ? symbol_char : z.xChar1
						,

						// cannot group  by or order by StringBuilder
						//xGLSLToken = z.xGLSLToken,

						// we cannot use it in group can we? as boolean we can
						isGLSLMacroFragment = z.xGLSLMacroFragment != null,

						z.xNameStringBuilderComplete
					} into g

					let count = g.Count()

					let c = false ? new { IsBlockComment = false, IsLineComment = false, xCommentTermination = false } : null
					let p = new { g.Key.IsPreprocessorDirective, g.Key.sGLSLToken, g.Key.isGLSLMacroFragment, g.Key.xNameStringBuilderComplete }

					// isGLSLPreprocessorDirective
					orderby g.Key.IsPreprocessorDirective descending, count descending, g.Key.sGLSLToken, g.Key.xChar0, g.Key.xChar1	//, g.Key.xGLSLToken
					select new { count, g.Key.xChar0, g.Key.xChar1, c, p, g } : new[] { gg }
				);

				cNoPreprocessorDirectivePass.Stop();
				cNoPreprocessorDirectivePassIterations.Add(cNoPreprocessorDirectivePass);
				#endregion


				//+		[0]	{ count = 141, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[1]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[2]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[3]	{ count = 155, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>

				//pass1
				// keep reading the name it seems
				//+		[0]	{ count = 132, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//macro name is one symbol?
				//+		[1]	{ count = 3, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 32 ' ', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				// more whitespace
				//+		[2]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 120 'x', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//underscore is par of the name
				//+		[3]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 95 '_', xChar1 = 32 ' ', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				// function?
				//+		[4]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 40 '(', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				// numbers are also part of the name, not as a first byte tho?
				//+		[5]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 48 '0', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[6]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 51 '3', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//underscore is par of the name
				//+		[7]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 95 '_', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				// skip em in this pass
				//+		[8]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[9]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[10]	{ count = 155, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>


				//pass1
				//+		[0]	{ count = 135, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[1]	{ count = 4, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 32 ' ', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[2]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 120 'x', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[3]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 40 '(', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[4]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[5]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[6]	{ count = 155, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>

				//+		[0]	{ count = 117, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[1]	{ count = 18, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 32 ' ', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[2]	{ count = 4, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 120 'x', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[3]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//// 		f	"W:\\ChromeShaderToyMorphingTeapotByIapafoto\\ChromeShaderToyMorphingTeapotByIapafoto\\Shaders\\Program.frag"	string
				//// #define U(a,b) (a.x*b.y-b.x*a.y)
				//+		[4]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 40 '(', xChar1 = 120 'x', isGLSLMacroFragment = true, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[5]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[6]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[7]	{ count = 155, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = false, g = {System.Linq.Lookup<<>f__AnonymousType83<bool,string,char,char,bool>,<>f__AnonymousType82<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>



				//+		[0]	{ count = 135, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[1]	{ count = 4, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 32 ' ', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[2]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[3]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 40 '(', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[4]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[5]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[6]	{ count = 155, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>

				//pass 2

				//+		[0]	{ count = 117, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[1]	{ count = 18, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 32 ' ', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[2]	{ count = 4, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[3]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[4]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 40 '(', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[5]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[6]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[7]	{ count = 155, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>



				//pass 3
				//+		[0]	{ count = 98, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[1]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 32 ' ', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[2]	{ count = 2, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 40 '(', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[3]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//		f	"W:\\ChromeShaderToyLightThornByVlad\\ChromeShaderToyLightThornByVlad\\Shaders\\Program.frag"	string
				//+		[4]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 32 ' ', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//x:\jsc.svn\examples\javascript\chrome\apps\webgl\synergy\musicmariobyiq\musicmariobyiq\shaders\program.frag
				//+		[5]	{ count = 3, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 46 '.', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[6]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[7]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[8]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 40 '(', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[9]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[10]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
				//+		[11]	{ count = 155, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 120 'x', xChar1 = 120 'x', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>


				// how long we should read?
				//Debugger.Break();
			}


			cNoPreprocessorDirectivePassIterationsElapsed = TimeSpan.FromMilliseconds(cNoPreprocessorDirectivePassIterations.Sum(x => x.ElapsedMilliseconds));
			Console.WriteLine("cNoPreprocessorDirectivePassIterationsElapsed " + new { cNoPreprocessorDirectivePassIterationsElapsed });

			// parse args for #define name(x) before space trimming
			#region  regroup
			cNoPreprocessorDirective = Enumerable.ToArray(
			   from gg in cNoPreprocessorDirective
			   from c in gg.g

				   //let z = new { c.IsPreprocessorDirective, c.xChar0, c.xChar1, c.xGLSLToken, c.xGLSLMacroFragment, c.xNameStringBuilderComplete, c.s, c.f }
			   let z = new { c.IsPreprocessorDirective, c.xChar0, c.xChar1, c.xGLSLToken, c.xGLSLMacroFragment, c.xNameStringBuilderComplete, c.s, c.f, c.xGLSLElement }



			   orderby z.IsPreprocessorDirective descending, z.xChar0, z.xChar1	//, z.xGLSLToken

			   group z by new
			   {
				   z.IsPreprocessorDirective,

				   gg.p.sGLSLToken,

				   // lets allow whitespace to be grouped
				   xChar0 =
						   // the magic char to complete the directive
						   z.xChar0 != '\n' && char.IsWhiteSpace(z.xChar0) ? WhiteSpace_char :
							// wo could aswell only use the group variable?
							z.xChar0.IsLetterOrDigitOrUnderscoreOrDot() ? symbol_char : z.xChar0
							,
				   xChar1 =



							 //z.IsPreprocessorDirective && char.IsWhiteSpace(z.xChar0) ? '?' :
							 z.IsPreprocessorDirective ? '?' :
							// we dont care about char1 if char0 is whitespace
							z.xChar1.IsLetterOrDigitOrUnderscoreOrDot() ? symbol_char : z.xChar1
							,

				   // cannot group  by or order by StringBuilder
				   //xGLSLToken = z.xGLSLToken,

				   // we cannot use it in group can we? as boolean we can
				   isGLSLMacroFragment = z.xGLSLMacroFragment != null,
				   z.xNameStringBuilderComplete
			   } into g

			   let count = g.Count()

			   let c = false ? new { IsBlockComment = false, IsLineComment = false, xCommentTermination = false } : null
			   let p = new { g.Key.IsPreprocessorDirective, g.Key.sGLSLToken, g.Key.isGLSLMacroFragment, g.Key.xNameStringBuilderComplete }

			   // isGLSLPreprocessorDirective
			   orderby g.Key.IsPreprocessorDirective descending, count descending, g.Key.sGLSLToken, g.Key.xChar0, g.Key.xChar1	//, g.Key.xGLSLToken
			   select new { count, g.Key.xChar0, g.Key.xChar1, c, p, g }
			);
			#endregion




			// trim and regroup?
			// at this point we can move to next line with macros that terminated?



			//+		[0]	{ count = 99, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 25, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 13 '\r', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// are we ready to parse expressions?
			//+		[3]	{ count = 9, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 9 '\t', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// ugh, a function. lets keep it for later
			//+		[4]	{ count = 7, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 40 '(', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// optional line terminator?
			//+		[5]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 59 ';', xChar1 = 13 '\r', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// are we ready to parse expressions?
			//+		[6]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// next pass could take a token, to kick off a member def
			//+		[7]	{ count = 155, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>


			// are we raymarching? :D

			//+		[0]	{ count = 133, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>

			//+		[1]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 7, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 40 '(', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 59 ';', xChar1 = 13 '\r', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[4]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 155, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>

			// http://sourceforge.net/p/brahma-fx/code/HEAD/tree/trunk/
			// http://research.microsoft.com/en-us/projects/Accelerator/
			// http://blogs.msdn.com/b/satnam_singh/archive/2010/01/11/a-c-implementation-of-a-convolver-using-accelerator-for-gpgpu-and-multicore-targets-using-linq-operators.aspx
			// https://github.com/nessos/LinqOptimizer/
			// http://nessos.github.io/LinqOptimizer/
			// https://github.com/nessos/GpuLinq/


			//+		[0]	{ count = 137, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// are we ready to parse expressions?
			//+		[1]	{ count = 17, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// ugh, a function. lets keep it for later
			//+		[2]	{ count = 7, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 40 '(', xChar1 = 115 's', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// are we to warn, or deal with such cases?
			//+		[3]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 59 ';', xChar1 = 13 '\r', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// are we ready to parse expressions?
			//+		[4]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// ready a token for this guy
			//+		[5]	{ count = 159, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>


			#region lets get rid of the spaces
			while (cNoPreprocessorDirective.Any(gg => gg.p.IsPreprocessorDirective && gg.p.isGLSLMacroFragment && (gg.xChar0 != '\n' && char.IsWhiteSpace(gg.xChar0))))
				cNoPreprocessorDirective = xEnumerable.SelectManyToArray(
					   from gg in cNoPreprocessorDirective
						   //Error   CS0019  Operator '&&' cannot be applied to operands of type 'bool?' and 'bool'  ScriptCoreLib.Async X:\jsc.svn\core\ScriptCoreLib.Async\ScriptCoreLib.Async\CompilerServices\GLSLAnalysis.cs    1603
					   select gg.p.IsPreprocessorDirective && gg.p.isGLSLMacroFragment && (gg.xChar0 != '\n' && char.IsWhiteSpace(gg.xChar0)) ?
					   from c in gg.g

					   let xChar0 = c.xChar1


					   let xReadByte1 = c.s.ReadByte()
					   let xChar1 = (char)xReadByte1

					   //let z = new { c.IsPreprocessorDirective, xChar0, xChar1, c.xGLSLToken, c.xGLSLMacroFragment, c.xNameStringBuilderComplete, c.s, c.f }
					   let z = new { c.IsPreprocessorDirective, xChar0, xChar1, c.xGLSLToken, c.xGLSLMacroFragment, c.xNameStringBuilderComplete, c.s, c.f, c.xGLSLElement }



					   orderby z.IsPreprocessorDirective descending, z.xChar0, z.xChar1	//, z.xGLSLToken

					   group z by new
					   {
						   z.IsPreprocessorDirective,

						   gg.p.sGLSLToken,

						   // lets allow whitespace to be grouped
						   xChar0 =
						   // the magic char to complete the directive
						   z.xChar0 != '\n' && char.IsWhiteSpace(z.xChar0) ? WhiteSpace_char :
							// wo could aswell only use the group variable?
							z.xChar0.IsLetterOrDigitOrUnderscore() ? expression_char : z.xChar0
							,
						   xChar1 =



							 //z.IsPreprocessorDirective && char.IsWhiteSpace(z.xChar0) ? '?' :
							 z.IsPreprocessorDirective ? '?' :
							// we dont care about char1 if char0 is whitespace
							z.xChar1.IsLetterOrDigitOrUnderscore() ? symbol_char : z.xChar1
							,

						   // cannot group  by or order by StringBuilder
						   //xGLSLToken = z.xGLSLToken,

						   // we cannot use it in group can we? as boolean we can
						   isGLSLMacroFragment = z.xGLSLMacroFragment != null,
						   z.xNameStringBuilderComplete
					   } into g

					   let count = g.Count()

					   let c = false ? new { IsBlockComment = false, IsLineComment = false, xCommentTermination = false } : null
					   // p means we are in the #? stage
					   let p = new { g.Key.IsPreprocessorDirective, g.Key.sGLSLToken, g.Key.isGLSLMacroFragment, g.Key.xNameStringBuilderComplete }

					   // isGLSLPreprocessorDirective
					   orderby g.Key.IsPreprocessorDirective descending, count descending, g.Key.sGLSLToken, g.Key.xChar0, g.Key.xChar1	//, g.Key.xGLSLToken
					   select new { count, g.Key.xChar0, g.Key.xChar1, c, p, g } : new[] { gg }
				);
			#endregion


			#region  regroup
			cNoPreprocessorDirective = Enumerable.ToArray(
			   from gg in cNoPreprocessorDirective
			   from c in gg.g


				   //let z = new { c.IsPreprocessorDirective, c.xChar0, c.xChar1, c.xGLSLToken, c.xGLSLMacroFragment, c.xNameStringBuilderComplete, c.s, c.f }
			   let z = new { c.IsPreprocessorDirective, c.xChar0, c.xChar1, c.xGLSLToken, c.xGLSLMacroFragment, c.xNameStringBuilderComplete, c.s, c.f, c.xGLSLElement }



			   orderby z.IsPreprocessorDirective descending, z.xChar0, z.xChar1	//, z.xGLSLToken

			   group z by new
			   {
				   z.IsPreprocessorDirective,

				   gg.p.sGLSLToken,

				   // lets allow whitespace to be grouped
				   xChar0 =
						   // the magic char to complete the directive
						   z.xChar0 != '\n' && char.IsWhiteSpace(z.xChar0) ? WhiteSpace_char :
							// wo could aswell only use the group variable?
							z.xChar0.IsLetterOrDigitOrUnderscoreOrDot() ? expression_char : z.xChar0
							,
				   xChar1 =



							 //z.IsPreprocessorDirective && char.IsWhiteSpace(z.xChar0) ? '?' :
							 z.IsPreprocessorDirective ? '?' :
							// we dont care about char1 if char0 is whitespace
							z.xChar1.IsLetterOrDigitOrUnderscoreOrDot() ? expression_char : z.xChar1
							,

				   // cannot group  by or order by StringBuilder
				   //xGLSLToken = z.xGLSLToken,

				   // we cannot use it in group can we? as boolean we can
				   isGLSLMacroFragment = z.xGLSLMacroFragment != null,
				   z.xNameStringBuilderComplete
			   } into g

			   let count = g.Count()

			   // we should have no data for comment parsing until now. in the next pass we may want some
			   let c = false ? new { IsBlockComment = false, IsLineComment = false, xCommentTermination = false } : null
			   let p = new { g.Key.IsPreprocessorDirective, g.Key.sGLSLToken, g.Key.isGLSLMacroFragment, g.Key.xNameStringBuilderComplete }

			   // isGLSLPreprocessorDirective
			   orderby g.Key.IsPreprocessorDirective descending, count descending, g.Key.sGLSLToken, g.Key.xChar0, g.Key.xChar1	//, g.Key.xGLSLToken
			   select new { count, g.Key.xChar0, g.Key.xChar1, c, p, g }
			);
			#endregion



			//+		[0]	{ count = 103, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// cool we have 29 definitions we can now use with #ifdef and #ifndef
			//+		[1]	{ count = 29, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 10 '\n', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 18, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// do we have 9 functions? or expressions? depends if there was a space
			//+		[3]	{ count = 9, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 40 '(', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// 
			//+		[4]	{ count = 3, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 46 '.', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 3, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 47 '/', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[6]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 59 ';', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[7]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			//+		[8]	{ count = 164, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string>>.Grouping} }	<Anonymous Type>
			// https://gcc.gnu.org/onlinedocs/gcc/Statement-Exprs.html


			//+		[0]	{ count = 106, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 115 's', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[1]	{ count = 29, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 10 '\n', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 18, IsPreprocessorDirective = true, sGLSLToken = "ifdef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 9, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 40 '(', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[4]	{ count = 3, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 47 '/', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "define", xChar0 = 59 ';', xChar1 = 63 '?', isGLSLMacroFragment = true, xNameStringBuilderComplete = true, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[6]	{ count = 1, IsPreprocessorDirective = true, sGLSLToken = "ifndef", xChar0 = 32 ' ', xChar1 = 63 '?', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[7]	{ count = 164, IsPreprocessorDirective = false, sGLSLToken = "", xChar0 = 115 's', xChar1 = 115 's', isGLSLMacroFragment = false, xNameStringBuilderComplete = false, g = {System.Linq.Lookup<<>f__AnonymousType84<bool,string,char,char,bool,bool>,<>f__AnonymousType83<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>


			#region take the next line for the defines that just completed
			cNoPreprocessorDirective = xEnumerable.SelectManyToArray(
			   from gg in cNoPreprocessorDirective

			   select gg.p.isGLSLMacroFragment && gg.xChar0 == '\n' ?
			   from c in gg.g


				   // we need to add the macro to element..
				   // move to next pass?
			   let xAppendMacroFragment = c.xGLSLElement.AppendMacroFragment(c.xGLSLMacroFragment)


			   let xChar0 = c.xChar1

			   let xReadByte1 = c.s.ReadByte()
			   let xChar1 = (char)xReadByte1

			   // a new line was just read in? 

			   //let IsPreprocessorDirective = default(bool?)
			   let IsPreprocessorDirective = false

			   // is there a reason not to read a third byte yet?
			   let xGLSLToken = default(StringBuilder)
			   let sGLSLToken = Convert.ToString(xGLSLToken)

			   // we should read until token completes




			   let xGLSLMacroFragment = default(GLSLMacroFragment)
			   let xNameStringBuilderComplete = false

			   // ! once
			   let z = new { IsPreprocessorDirective, xChar0, xChar1, xGLSLToken, xGLSLMacroFragment, xNameStringBuilderComplete, c.s, c.f, c.xGLSLElement }
			   //let z = new { c.xChar0, c.xChar0IsWhiteSpace, xChar1, xReadByteNext0, xReadByteNext0IsWhiteSpace, xReadByteNext0IsLetter, xCommentContentByte, xCommentTermination, c.f, c.s, xChar0IsLetter0, xChar1IsLetter1, c.xGLSLElement, IsBlockComment, IsLineComment, xCommentStringBuilder }

			   orderby z.IsPreprocessorDirective descending, z.xChar0, z.xChar1	//, z.xGLSLToken

			   group z by new
			   {
				   z.IsPreprocessorDirective,

				   sGLSLToken,

				   // lets allow whitespace to be grouped
				   xChar0 =
					  char.IsWhiteSpace(z.xChar0) ? ' ' :
					  // wo could aswell only use the group variable?
					  !z.IsPreprocessorDirective && char.IsLetter(z.xChar0) ? letter_char : z.xChar0
					  ,
				   xChar1 =
					  // we dont care about char1 if char0 is whitespace
					  char.IsWhiteSpace(z.xChar0) ? '?' :
					  !z.IsPreprocessorDirective && char.IsLetter(z.xChar1) ? letter_char : z.xChar1,


				   // cannot group  by or order by StringBuilder
				   //xGLSLToken = z.xGLSLToken,
				   isGLSLMacroFragment = false,
				   z.xNameStringBuilderComplete
			   } into g

			   let count = g.Count()

			   // actually c will indicate that we need to jump back?
			   let c = true ? new { IsBlockComment = false, IsLineComment = false, xCommentTermination = false } : null
			   let p = false ? new { g.Key.IsPreprocessorDirective, g.Key.sGLSLToken, g.Key.isGLSLMacroFragment, g.Key.xNameStringBuilderComplete } : null

			   orderby g.Key.IsPreprocessorDirective descending, count descending, g.Key.sGLSLToken, g.Key.xChar0, g.Key.xChar1	//, g.Key.xGLSLToken

			   select new { count, g.Key.xChar0, g.Key.xChar1, c   /* # */, p, g } : new[] { gg }
		   //select new { count, g.Key.xChar0, g.Key.xChar1, g.Key.IsBlockComment, g.Key.IsLineComment, g.Key.xCommentTermination, g.Key.xChar0IsWhiteSpace, g.Key.xReadByteNext0IsWhiteSpace, g.Key.xReadByteNext0IsLetter, g }

		   );
			#endregion


			//+		[0]	{ count = 113, xChar0 = 101 'e', xChar1 = 63 '?', c = null, p = { IsPreprocessorDirective = True, sGLSLToken = define, isGLSLMacroFragment = True, xNameStringBuilderComplete = True }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			//+		[1]	{ count = 13, xChar0 = 32 ' ', xChar1 = 63 '?', c = { IsBlockComment = False, IsLineComment = False, xCommentTermination = False }, p = null, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 13, xChar0 = 35 '#', xChar1 = 120 'x', c = { IsBlockComment = False, IsLineComment = False, xCommentTermination = False }, p = null, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 4, xChar0 = 47 '/', xChar1 = 47 '/', c = { IsBlockComment = False, IsLineComment = False, xCommentTermination = False }, p = null, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			//+		[4]	{ count = 18, xChar0 = 32 ' ', xChar1 = 63 '?', c = null, p = { IsPreprocessorDirective = True, sGLSLToken = ifdef, isGLSLMacroFragment = False, xNameStringBuilderComplete = False }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 9, xChar0 = 40 '(', xChar1 = 63 '?', c = null, p = { IsPreprocessorDirective = True, sGLSLToken = define, isGLSLMacroFragment = True, xNameStringBuilderComplete = True }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[6]	{ count = 3, xChar0 = 47 '/', xChar1 = 63 '?', c = null, p = { IsPreprocessorDirective = True, sGLSLToken = define, isGLSLMacroFragment = True, xNameStringBuilderComplete = True }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[7]	{ count = 1, xChar0 = 59 ';', xChar1 = 63 '?', c = null, p = { IsPreprocessorDirective = True, sGLSLToken = define, isGLSLMacroFragment = True, xNameStringBuilderComplete = True }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[8]	{ count = 1, xChar0 = 32 ' ', xChar1 = 63 '?', c = null, p = { IsPreprocessorDirective = True, sGLSLToken = ifndef, isGLSLMacroFragment = False, xNameStringBuilderComplete = False }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[9]	{ count = 187, xChar0 = 101 'e', xChar1 = 101 'e', c = null, p = { IsPreprocessorDirective = False, sGLSLToken = , isGLSLMacroFragment = False, xNameStringBuilderComplete = False }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>



			//+		[0]	{ count = 113, xChar0 = 101 'e', xChar1 = 63 '?', c = null, sGLSLToken = "define", isGLSLMacroFragment = true, xNameStringBuilderComplete = true, p = { IsPreprocessorDirective = True }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			//+		[1]	{ count = 13, xChar0 = 32 ' ', xChar1 = 63 '?', c = { IsBlockComment = False, IsLineComment = False, xCommentTermination = False }, sGLSLToken = "", isGLSLMacroFragment = false, xNameStringBuilderComplete = false, p = { IsPreprocessorDirective = False }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[2]	{ count = 13, xChar0 = 35 '#', xChar1 = 120 'x', c = { IsBlockComment = False, IsLineComment = False, xCommentTermination = False }, sGLSLToken = "", isGLSLMacroFragment = false, xNameStringBuilderComplete = false, p = { IsPreprocessorDirective = False }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[3]	{ count = 4, xChar0 = 47 '/', xChar1 = 47 '/', c = { IsBlockComment = False, IsLineComment = False, xCommentTermination = False }, sGLSLToken = "", isGLSLMacroFragment = false, xNameStringBuilderComplete = false, p = { IsPreprocessorDirective = False }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>

			//+		[4]	{ count = 18, xChar0 = 32 ' ', xChar1 = 63 '?', c = null, sGLSLToken = "ifdef", isGLSLMacroFragment = false, xNameStringBuilderComplete = false, p = { IsPreprocessorDirective = True }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[5]	{ count = 9, xChar0 = 40 '(', xChar1 = 63 '?', c = null, sGLSLToken = "define", isGLSLMacroFragment = true, xNameStringBuilderComplete = true, p = { IsPreprocessorDirective = True }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[6]	{ count = 3, xChar0 = 47 '/', xChar1 = 63 '?', c = null, sGLSLToken = "define", isGLSLMacroFragment = true, xNameStringBuilderComplete = true, p = { IsPreprocessorDirective = True }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[7]	{ count = 1, xChar0 = 59 ';', xChar1 = 63 '?', c = null, sGLSLToken = "define", isGLSLMacroFragment = true, xNameStringBuilderComplete = true, p = { IsPreprocessorDirective = True }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[8]	{ count = 1, xChar0 = 32 ' ', xChar1 = 63 '?', c = null, sGLSLToken = "ifndef", isGLSLMacroFragment = false, xNameStringBuilderComplete = false, p = { IsPreprocessorDirective = True }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>
			//+		[9]	{ count = 187, xChar0 = 101 'e', xChar1 = 101 'e', c = null, sGLSLToken = "", isGLSLMacroFragment = false, xNameStringBuilderComplete = false, p = { IsPreprocessorDirective = False }, g = {System.Linq.Lookup<<>f__AnonymousType94<bool,string,char,char,bool,bool>,<>f__AnonymousType93<bool,char,char,System.Text.StringBuilder,ScriptCoreLib.CompilerServices.GLSLMacroFragment,bool,System.IO.FileStream,string,ScriptCoreLib.CompilerServices.GLSLElement>>.Grouping} }	<Anonymous Type>


			// we have 30 reasons to go back a step.
			// we should not however be processing spaces in IsPreprocessorDirective up there.
			// the onces with c should be sent back!

			// on case of white space, comments or # ? different goto jumps..
			if (cNoPreprocessorDirective.Any(x => char.IsWhiteSpace(x.xChar0)))
			{
				// need to go back and resolve those spaces
				// can we keep the current state?

				// the states seem to be incompatible
				// can we manage it?


				// managing state is hard.
				// we are doing a tail call here arent we, a jump

				cNoPreprocessorDirective = cNoPreprocessorDirective;
				cNoSpaceWORDx = cNoSpaceWORDx;
				//cNoSpaceWORDx = cNoPreprocessorDirective;
				Debugger.Break();

				// http://stackoverflow.com/questions/6545720/does-anyone-still-use-goto-in-c-sharp-and-if-so-why
				goto jump_to_cNoSpaceWORDx_from_cNoPreprocessorDirective;
			}

			// http://referencesource.microsoft.com/#System.Web/WebSockets/SubprotocolUtil.cs



			Debugger.Break();
		}


	}

	internal static class xEnumerable
	{
		// X:\jsc.svn\examples\merge\test\TestLINQJoinConcat\TestLINQJoinConcat\Program.cs
		internal static T[] SelectManyToArray<T>(this IEnumerable<IEnumerable<T>> source)
		{
			return source.SelectMany(x => x).ToArray();
		}

		internal static bool IsLetterOrDigitOrUnderscore(this char c)
		{
			return (c == '_' || char.IsLetterOrDigit(c));
		}

		internal static bool IsLetterOrDigitOrUnderscoreOrDot(this char c)
		{
			return (c == '_' || c == '.' || char.IsLetterOrDigit(c));
		}
	}
}
