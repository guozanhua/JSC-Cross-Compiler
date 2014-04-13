﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib.ActionScript.flash.display;

namespace ScriptCoreLib.ActionScript.Nonoba.api
{
    [Script(IsNative = true)]
    public static class NonobaAPI
    {
        #region Methods
        /// <summary>
        /// Registers an event listener object with an EventDispatcher object so that the listener receives notification of an event.
        /// </summary>
        public static Connection MakeMultiplayer(Stage stage, string s, int p)
        {
            return default(Connection);
        }

        /// <summary>
        /// Registers an event listener object with an EventDispatcher object so that the listener receives notification of an event.
        /// </summary>
        public static Connection MakeMultiplayer(Stage stage, string s)
        {
            return default(Connection);
        }

        /// <summary>
        /// Registers an event listener object with an EventDispatcher object so that the listener receives notification of an event.
        /// </summary>
        public static Connection MakeMultiplayer(Stage stage)
        {
            return default(Connection);
        }

		/// <summary>
		/// Shows the purchase process for item. 
		/// </summary>
		/// <param name="stage"></param>
		/// <param name="item"></param>
		/// <param name="callback"></param>
		public static void ShowShop(Stage stage, string item, Function callback)
		{
			// http://nonoba.com/developers/documentation/multiplayerapi/classnonobaapi
		}
        #endregion

    }
}