﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using java.applet;
using jsc.meta.Library;
using ScriptCoreLib;
using ScriptCoreLib.ActionScript.flash.display;
using ScriptCoreLib.JavaScript;
using ScriptCoreLib.JavaScript.DOM.HTML;
using ScriptCoreLib.JavaScript.Extensions;
using jsc.Languages.IL;
using System.Reflection.Emit;
using ScriptCoreLib.JavaScript.DOM;
using ScriptCoreLib.ActionScript.flash.external;
using ScriptCoreLib.ActionScript.Extensions;

namespace jsc.meta.Commands.Rewrite
{
	partial class RewriteToJavaScriptDocument
	{

		private void WriteInitialization_JavaInternalElement(ILGenerator il, Type proxy, Type context, ScriptApplicationEntryPointAttribute entry, FieldBuilder __InternalElement)
		{
			const string archive = "assets/Ultra1.UltraApplication/UltraApplet.jar";
			const string code = "Ultra1.UltraApplet";

			Action Implementation1 =
				delegate
				{
					var o = new IHTMLApplet();

					// see: http://java.sun.com/j2se/1.4.2/docs/guide/plugin/developer_guide/java_js.html

					o.archive = archive;
					o.mayscript = true;
					o.code = code;
					o.width = 4;
					o.height = 5;
				};

			var il_a = new ILTranslationExtensions.EmitToArguments();

			il_a[OpCodes.Ldc_I4_4] =
				e =>
				{
					il.Emit(OpCodes.Ldc_I4, entry.Width);
				};

			il_a[OpCodes.Ldc_I4_5] =
				e =>
				{
					il.Emit(OpCodes.Ldc_I4, entry.Height);
				};

			il_a[OpCodes.Ldstr] =
				e =>
				{
					if (e.i.TargetLiteral == archive)
					{
						il.Emit(OpCodes.Ldstr, "assets/" + context.FullName + "/" + proxy.Name + ".jar");
						return;
					}

					if (e.i.TargetLiteral == code)
					{
						il.Emit(OpCodes.Ldstr, proxy.FullName);
						return;
					}

					e.Default();
				};
			il_a[OpCodes.Ret] =
				e =>
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldloc_0);
					il.Emit(OpCodes.Stfld, __InternalElement);
					il.Emit(OpCodes.Ret);
				};

			Implementation1.Method.EmitTo(il, il_a);
		}

		private void WriteInitialization_ActionScriptInternalElement(ILGenerator il, Type proxy, Type context, ScriptApplicationEntryPointAttribute entry, FieldInfo __InternalElement)
		{
			const string src = @"assets/Ultra1.UltraApplication/UltraSprite.swf";
			const string type = "application/x-shockwave-flash";

			// http://kb2.adobe.com/cps/164/tn_16494.html
			// http://stackoverflow.com/questions/2154931/how-to-call-dynamically-created-flash-external-interface-in-ie-from-javascript

			Action Implementation1 =
				delegate
				{
					var o = default(IHTMLEmbed);
					//var id = "__embed_" + new Random().Next();

					// good luck getting it to work with ID :)
					// there probably is a way to do it!

					//o = (IHTMLEmbed)new IFunction("e", "/*@cc_on return this.createElement(e); @*/ return null;").apply(Native.Document,
					//    "<embed id='accc' name='accc' width='500' height='400'  src='assets/Ultra2.UltraApplication/UltraSprite.swf' allowfullscreen='true' allownetworking='all' allowscriptaccess='always'  />"
					//);


					if (o == null)
					{
						o = new IHTMLEmbed();

						// http://www.bobbyvandersluis.com/ufo/index.html
						// for IE we might need to consider setting innerHTML
						// as we do already in scriptcorelib for some elements IHTMLInput

						o.type = type;

						// http://perishablepress.com/press/2007/04/17/embed-flash-or-die-trying/
						// http://curtismorley.com/2008/11/01/actionscript-security-error-2060-security-sandbox-violation/
						// http://developer.yahoo.com/ylive/flash_js_api/
						// http://www.extremefx.com.ar/blog/fixing-flash-external-interface-inside-form-on-internet-explorer
						// http://code.google.com/p/swfobject/source/browse/trunk/swfobject/src/swfobject.js


						// do we need id and names? for IE?
						//o.id = id;
						//o.name = id;



						o.setAttribute("allowFullScreen", "true");
						o.setAttribute("allowNetworking", "all");
						o.setAttribute("allowScriptAccess", "always");

						// we need Ldc_I4
						o.width = 4001;
						o.height = 4002;

						o.src = src;
					}
				};

			var il_a = new ILTranslationExtensions.EmitToArguments();

			//il_a.TranslateTargetType = t => t == typeof(Implementation1) ? a.Type : t;
			//il_a.TranslateTargetMethod = m => m == Implementation4.Method ? __cctor_1 : m;

			il_a[OpCodes.Ldc_I4] =
				e =>
				{
					if (e.i.TargetInteger == 4001)
					{
						il.Emit(OpCodes.Ldc_I4, entry.Width);
						return;
					}

					if (e.i.TargetInteger == 4002)
					{
						il.Emit(OpCodes.Ldc_I4, entry.Height);
						return;
					}

					e.Default();
				};



			il_a[OpCodes.Ldstr] =
				e =>
				{
					if (e.i.TargetLiteral == src)
					{
						il.Emit(OpCodes.Ldstr, "assets/" + context.FullName + "/" + proxy.Name + ".swf");
						return;
					}

					e.Default();
				};
			il_a[OpCodes.Ret] =
				e =>
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldloc_0);
					il.Emit(OpCodes.Stfld, __InternalElement);
					il.Emit(OpCodes.Ret);
				};

			Implementation1.Method.EmitTo(il, il_a);

		}


		private void WriteInitialization_ActionScriptExterenalInterface(
			RewriteToAssembly r,
			ILTranslationExtensions.EmitToArguments.ILRewriteContext c,
			TypeBuilder DeclaringType,
			Type TargetType)
		{
			var __InitializeExternalInterface = DeclaringType.DefineMethod("__InitializeExternalInterface", MethodAttributes.Private, CallingConventions.Standard, typeof(void), null);

			c.il.Emit(OpCodes.Ldarg_0);
			c.il.Emit(OpCodes.Call, __InitializeExternalInterface);

			var il = __InitializeExternalInterface.GetILGenerator();

			Action Implementation1 =
				delegate
				{
					ExternalInterface.addCallback("event", new Action(Console.WriteLine).ToFunction());
				};

			foreach (var kk in TargetType.GetMethods(
				BindingFlags.DeclaredOnly |
				BindingFlags.Public | BindingFlags.Instance))
			{

				// http://olondono.blogspot.com/2008/02/creating-code-at-runtime-part-2.html

				var IsEventMethod = kk.ReturnType == typeof(void) && kk.GetParameterTypes().Length == 1 && typeof(Delegate).IsAssignableFrom(kk.GetParameterTypes()[0]);


				#region __Delegate
				var __Delegate = DeclaringType.DefineNestedType("__Delegate_" + kk.Name, TypeAttributes.NestedFamily | TypeAttributes.AutoClass | TypeAttributes.Sealed, typeof(MulticastDelegate));

				var __Delegate_ctor = __Delegate.DefineConstructor(
					MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
					CallingConventions.Standard,
					new Type[] { typeof(object), typeof(IntPtr) }
				);

				__Delegate_ctor.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);
				// mxmlc will complain that jsc didnt create parameter names... jsc should generate them :)
				__Delegate_ctor.DefineParameter(1, ParameterAttributes.None, "object");
				__Delegate_ctor.DefineParameter(2, ParameterAttributes.None, "method");

				// Method attributes flags
				MethodAttributes maDelegate = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;
				MethodImplAttributes mia = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;

				var __Delegate_Invoke = __Delegate.DefineMethod(
					"Invoke", maDelegate,
					CallingConventions.Standard, kk.ReturnType,
					Enumerable.ToArray(
						from ParameterType in kk.GetParameterTypes()
						// add and remove events
						// must first save the delegate in global object like window
						select typeof(Delegate).IsAssignableFrom(ParameterType) ? typeof(string) : ParameterType
					)
				);

	
				__Delegate_Invoke.SetImplementationFlags(mia);
				kk.GetParameters().CopyTo(__Delegate_Invoke);

				#region BeginInvoke, EndInvoke
				{
					// BeginInvoke for Asynchronous call
					var methodBuilder = __Delegate.DefineMethod("BeginInvoke", maDelegate, typeof(IAsyncResult), new Type[] { typeof(AsyncCallback), typeof(object) });
					methodBuilder.SetImplementationFlags(mia);
				}
				{
					// EndInvoke for Asynchronous call
					var methodBuilder = __Delegate.DefineMethod("EndInvoke", maDelegate, typeof(void), new Type[] { typeof(IAsyncResult) });
					methodBuilder.SetImplementationFlags(mia);
				}
				#endregion

				__Delegate.CreateType();
				#endregion

				if (IsEventMethod)
				{
					// we need some additional code
				}
				else
				{
					#region addCallback
					var il_a = new ILTranslationExtensions.EmitToArguments();

					il_a[OpCodes.Newobj] =
						e =>
						{
							e.il.Emit(OpCodes.Newobj, __Delegate_ctor);
						};

					il_a[OpCodes.Ldftn] =
						e =>
						{
							e.il.Emit(OpCodes.Ldftn, r.RewriteArguments.context.MethodCache[kk]);
						};


					il_a[OpCodes.Ldnull] =
						e =>
						{
							e.il.Emit(OpCodes.Ldarg_0);
						};

					il_a[OpCodes.Ldstr] =
						e =>
						{
							e.il.Emit(OpCodes.Ldstr, kk.Name);
						};

					il_a[OpCodes.Ret] =
						e =>
						{

						};

					Implementation1.Method.EmitTo(il, il_a);
					#endregion

				}
			}

			il.Emit(OpCodes.Ret);
		}
	}
}
