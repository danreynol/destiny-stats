using System;
using System.Net.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DestinyHelper
{
    public static class DestinyClient
    {
        private const string ApiKey = "myApiKey";

        /// <summary>
        /// Gets the UrlBase property.
        /// </summary>
        public static string UrlBase => "https://www.bungie.net/platform/";

        /// <summary>
        /// Run a request against the Bungie servers.
        /// </summary>
        /// <param name="requestString">The request to run</param>
        /// <returns>The result of the request</returns>
        public static dynamic SendRequest(string requestString)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);
                var response = client.GetAsync(requestString).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                dynamic item = JsonConvert.DeserializeObject(content);
                CheckForErrorMessage(item);
                bool resultsWereReturned = CheckIfResultsWereReturned(item);

                if (!resultsWereReturned)
                {
                    item = null;
                }

                return item;
            }
        }

        /// <summary>
        /// Check to see if an error was generated.
        /// </summary>
        /// <param name="item">The json item</param>
        private static void CheckForErrorMessage(dynamic item)
        {
            string errorCode = item.ErrorCode;

            if (errorCode != "1")
            {
                string message = string.Format("[Bungie] {0}", item.Message);
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Check to see if results were returned.
        /// </summary>
        /// <param name="item">The json item</param>
        /// <returns>True if results were returned, false if not</returns>
        private static bool CheckIfResultsWereReturned(dynamic item)
        {
            JToken responseItem = item.Response;
            return responseItem.HasValues ? true : false;
        }
    }
}
