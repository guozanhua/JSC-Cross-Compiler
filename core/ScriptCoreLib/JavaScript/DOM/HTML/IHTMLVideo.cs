using ScriptCoreLib.JavaScript.Runtime;
using ScriptCoreLib.JavaScript.DOM;
using ScriptCoreLib.JavaScript;

using ScriptCoreLib.JavaScript.DOM.HTML;

namespace ScriptCoreLib.JavaScript.DOM.HTML
{
	// http://mxr.mozilla.org/mozilla-central/source/dom/webidl/HTMLVideoElement.webidl
	// http://src.chromium.org/viewvc/blink/trunk/Source/core/html/HTMLVideoElement.idl
	// https://src.chromium.org/viewvc/blink/trunk/Source/core/html/HTMLVideoElement.cpp
	// https://chromium.googlesource.com/experimental/chromium/blink/+/refs/wip/bajones/webvr/Source/core/html/HTMLVideoElement.cpp

	[Script(InternalConstructor = true)]
	public class IHTMLVideo : IHTMLMedia
	{
        // web content could be archived into a DVD like experience with a DVD MENU
        // can a data stream be extracted from such a video?

		// when can a jsc app store video in db?

		// X:\jsc.svn\examples\javascript\async\test\TestGetUserMedia\TestGetUserMedia\Application.cs
		public int videoHeight;
		public int videoWidth;



		#region Constructor

		public IHTMLVideo()
		{
			// InternalConstructor
		}

		static IHTMLVideo InternalConstructor()
		{
			return (IHTMLVideo)IHTMLElement.InternalConstructor(HTMLElementEnum.video);
		}

		#endregion


		public byte[] bytes
		{
			// tested by?

			[Script(DefineAsStatic = true)]
			get
			{
				var c = new CanvasRenderingContext2D(this.clientWidth, this.clientHeight);

				c.drawImage(
					this, 0, 0, c.canvas.width, c.canvas.height
				);

				return c.bytes;
			}

		}
	}
}
