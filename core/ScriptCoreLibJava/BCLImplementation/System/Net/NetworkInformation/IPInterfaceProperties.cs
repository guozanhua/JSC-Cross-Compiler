﻿using ScriptCoreLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptCoreLib.Extensions;
using System.Net.NetworkInformation;

namespace ScriptCoreLibJava.BCLImplementation.System.Net.NetworkInformation
{
    [Script(Implements = typeof(global::System.Net.NetworkInformation.IPInterfaceProperties))]
    internal class __IPInterfaceProperties
    {
        public __NetworkInterface InternalValue;

        public virtual UnicastIPAddressInformationCollection UnicastAddresses
        {
            get
            {
                var a = new List<__UnicastIPAddressInformation>();


                var InetAddresses = this.InternalValue.InternalValue.getInetAddresses();

                while (InetAddresses.hasMoreElements())
                {
                    var xInetAddress = (java.net.InetAddress)InetAddresses.nextElement();


                    a.Add(
                        new __UnicastIPAddressInformation
                        {
                            Address = new __IPAddress { InternalAddress = xInetAddress }
                        }
                    );
                }

                return new __UnicastIPAddressInformationCollection
                {
                    InternalValue = a
                };
            }
        }

        public static implicit operator global::System.Net.NetworkInformation.IPInterfaceProperties(__IPInterfaceProperties i)
        {
            return (global::System.Net.NetworkInformation.IPInterfaceProperties)(object)i;
        }
    }
}
