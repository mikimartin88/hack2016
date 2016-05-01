using com.hotelbeds.distribution.hotel_api_model.auto.messages;
using com.hotelbeds.distribution.hotel_api_sdk;
using com.hotelbeds.distribution.hotel_api_sdk.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebAppHack16.Connection
{
    public class ConnectionManager
    {
        /*
        API KEY: 5k6edkfb4uynp4wg3w4frhbk
    Shared secret: tfNeMWWvCA
    Status: active
            */
        public ConnectionManager()
        { }
        public AvailabilityRS doDisponibilidad(AvailabilityRQ oAvail)
        {
            Availability oAvails = new Availability();
            String sAuth = getAuth();
            HotelApiClient client = new HotelApiClient();
            StatusRS status = client.status();

            if (status != null && status.error == null)
                Console.WriteLine("StatusRS: " + status.status);
            else if (status != null && status.error != null)
            {
                Console.WriteLine("StatusRS: " + status.status + " " + status.error.code + ": " + status.error.message);
                return null;
            }
            else if (status == null)
            {
                Console.WriteLine("StatusRS: Is not available.");
                return null;
            }
            AvailabilityRS oAvailRS = client.doAvailability(oAvail);
            return oAvailRS;
        }


        public string getAuth()
        {

            const string apiKey = "5k6edkfb4uynp4wg3w4frhbk";
            const string sharedSecret = "tfNeMWWvCA";

            const string endpoint = "https://api.test.hotelbeds.com/hotel-api/1.0/status";

            // Compute the signature to be used in the API call (combined key + secret + timestamp in seconds)
            string signature;
            using (var sha = SHA256.Create())
            {
                long ts = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds / 1000;
                Console.WriteLine("Timestamp: " + ts);
                var computedHash = sha.ComputeHash(Encoding.UTF8.GetBytes(apiKey + sharedSecret + ts));
                signature = BitConverter.ToString(computedHash).Replace("-", "");
            }

            Console.WriteLine("Signature: " + signature);

            using (var client = new WebClient())
            {
                // Request configuration            
                client.Headers.Add("X-Signature", signature);
                client.Headers.Add("Api-Key", apiKey);
                client.Headers.Add("Accept", "application/xml");

                // Request execution
                string response = client.DownloadString(endpoint);
                return response;
            }

        }
    }
}