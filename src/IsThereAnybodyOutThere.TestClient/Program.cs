using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsThereAnybodyOutThere.Client;
using WebSocketSharp;
using WebSocketSharp.Net;

namespace IsThereAnybodyOutThere.TestClient
{
    class Program
    {
        static void Main(string[] args)
        {

            var applicationName = "TestClient";
            var endpoint = "ws://localhost:5001";

            if (args.Length > 0)
            {
                applicationName = args[0];
            }
            if (args.Length > 1)
            {
                endpoint = args[1];
            }


            var imRightHereClient = new ImRightHereClient(endpoint, applicationName, TimeSpan.FromSeconds(5), OnSendingHearbeat);
            imRightHereClient.Start();
            
            Console.WriteLine("Press enter to close");
            Console.ReadLine();

            imRightHereClient.Stop();
        }

        private static Dictionary<string, object> OnSendingHearbeat()
        {
            return new Dictionary<string, object>();
        }
    }
}
