using ScriptCoreLib;
using ScriptCoreLib.Delegates;
using ScriptCoreLib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RTCICELobby
{
    /// <summary>
    /// Methods defined in this type can be used from JavaScript. The method calls will seamlessly be proxied to the server.
    /// </summary>
    public class ApplicationWebService
    {
        public string sdp;
        public List<string> sdpCandidates = new List<string>();

        public async Task Offer()
        {
            Console.WriteLine(new { sdp });

            Memory.AllAvailableOffers.Add(sdp);
        }

        public string sdpAnwser;
        public List<string> sdpAnwserCandidates = new List<string>();

        public async Task CheckAnswer()
        {
            // copy anwser for sdp, if there is one...
            var x = Memory.AllAvailableAnwsers.FirstOrDefault(z => z.sdpOffer == this.sdp);

            this.sdpAnwser = x.sdpAnwser;
            this.sdpAnwserCandidates = x.sdpAnwserCandidates;

            // and now remove from memory? 
            // pairing complete
        }

        public string sdpOffer;

        public async Task GetOffer()
        {
            // if there is an offer available already, lets make it known...


            sdpOffer = Memory.AllAvailableOffers.FirstOrDefault();
        }

        public async Task Anwser()
        {
            foreach (var sdpAnwserCandidate in sdpAnwserCandidates)
            {
                Console.WriteLine(new { sdpAnwserCandidate });
            }
            Console.WriteLine(new { sdpAnwser });
            Console.WriteLine(new { sdpOffer });

            Memory.AllAvailableAnwsers.Add(
                new AnwserToOffer
                {
                    sdpOffer = sdpOffer,
                    sdpAnwser = sdpAnwser,

                    sdpAnwserCandidates = sdpAnwserCandidates.ToList()
                }
            );
        }
    }

    public class AnwserToOffer
    {
        public string sdpOffer;

        public string sdpAnwser;
        public List<string> sdpAnwserCandidates;
    }

    public static class Memory
    {
        public static List<string> AllAvailableOffers = new List<string>();

        public static List<AnwserToOffer> AllAvailableAnwsers = new List<AnwserToOffer>();


    }
}
