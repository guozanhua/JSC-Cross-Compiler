﻿package Nonoba.api{	import flash.events.Event	import Nonoba.api.Message;	public class ConnectionEvent extends Event{		public static const INIT:String 		= "onInit";		public static const DISCONNECT:String 	= "onDisconnect";		function ConnectionEvent(type:String){			super(type);		}		public override function clone():Event {			 return new ConnectionEvent(type);		}	}}