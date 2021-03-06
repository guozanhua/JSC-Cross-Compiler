﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib;
using System.ComponentModel;
using ScriptCoreLib.Shared.BCLImplementation.System.ComponentModel;

namespace ScriptCoreLibJava.BCLImplementation.System.Windows.Forms
{
	[Script(Implements = typeof(global::System.Windows.Forms.Timer))]
	internal class __Timer : __Component
	{
		public __Timer(IContainer e)
		{

		}

		public object Tag { get; set; }

        // tested by
        // / X:\jsc.svn\examples\java\forms\AppletAsyncWhenReady\AppletAsyncWhenReady\ApplicationApplet.cs
		public override void Dispose(bool e)
		{
			this.Enabled = false;
		}

		bool InternalEnabled;
		public bool Enabled
		{
			get { return InternalEnabled; }
			set
			{
				if (InternalEnabled == value)
					return;

				InternalEnabled = value;

				InternalUpdate();
			}
		}

		public void Start()
		{
			this.Enabled = true;
		}

		public void Stop()
		{
			this.Enabled = false;
		}

		int InternalInterval;
		public int Interval
		{
			get
			{
				return InternalInterval;
			}
			set
			{
				if (InternalInterval == value)
					return;

				InternalInterval = value;

				InternalUpdate();
			}
		}

		javax.swing.Timer InternalTimer;

		[Script]
		public class TimerAction : java.awt.@event.ActionListener
		{
			public Action Invoke;

			#region ActionListener Members

			public void actionPerformed(java.awt.@event.ActionEvent e)
			{
				Invoke();
			}

			#endregion
		}

		void InternalUpdate()
		{
			if (InternalTimer != null)
			{
				InternalTimer.stop();
				InternalTimer = null;
			}

			if (InternalEnabled)
			{
				InternalTimer = new javax.swing.Timer(InternalInterval,
					new TimerAction
					{
						Invoke =
							delegate
							{
								if (Tick != null)
									Tick(this, new EventArgs());
							}
					}
				);
				InternalTimer.start();
			}
		}

		public event EventHandler Tick;

	}
}
