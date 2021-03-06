using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestSandbox
{
    class ParseHubExtractor
    {


        List<String> orcidID = new List<String>();
        String name = "";


        public ParseHubExtractor()
        {

        }

        public String GetResearcherORCIDID(String researcherUrl, String name)
        {
            this.name = name;
            var client = new RestClient(" https://www.parsehub.com/api/v2/projects/tMQrtJvJgtm-/run");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            //  request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("api_key", "twWf3s2XUkKN");
            request.AddParameter("format", "json");
            /*start_url (Optional)	The url to start running on. Defaults to the project’s start_site*/
            request.AddParameter("start_url", researcherUrl);
            IRestResponse response = client.Execute(request);

            var data = (JObject)JsonConvert.DeserializeObject(response.Content);
            string runToken = data["run_token"].Value<string>();
            Console.WriteLine("Found run token :" + runToken);

            string pureUrl = syncedPollPureURL(2, runToken);
            string orcidID =   RunORCIDIDScraper(pureUrl);


            return orcidID;
        }

        private ParseHubResponse GetPurePortalURL(String runToken)
        {
            var dataClient = new RestClient(" https://www.parsehub.com/api/v2/runs/" + runToken + "/data");
            var dataRequest = new RestRequest(Method.GET);
            dataRequest.AddParameter("api_key", "twWf3s2XUkKN");
            dataRequest.AddParameter("format", "json");

            IRestResponse dataResponse = dataClient.Execute(dataRequest);
            String statusCode = dataResponse.StatusCode.ToString();
            // Console.WriteLine(statusCode);

            if (statusCode == "OK")
                Console.WriteLine(dataResponse.Content);
            ParseHubResponse parseHubResponse = new ParseHubResponse();
            parseHubResponse.statusCode = statusCode;
            if (statusCode == "OK")
            {
                var data = (JObject)JsonConvert.DeserializeObject(dataResponse.Content);
                string pureUrl = data["pureUrl"].Value<string>();
                parseHubResponse.data = pureUrl;
            }

            return parseHubResponse;
        }

        private String RunORCIDIDScraper(String startUrl)
        {
            bool status = false;
            var client = new RestClient(" https://www.parsehub.com/api/v2/projects/t4-ULJRgyjKk/run");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            //  request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("api_key", "twWf3s2XUkKN");
            Console.WriteLine(startUrl);
            request.AddParameter("start_url", startUrl);
            IRestResponse response = client.Execute(request);

            var data = (JObject)JsonConvert.DeserializeObject(response.Content);
            string runToken = data["run_token"].Value<string>();
            Console.WriteLine(runToken);



            var cts = new CancellationTokenSource();

            String orcidID = syncedPollORCIDID(3, runToken);

            Console.WriteLine("Found orcidid :" + orcidID );
            
                       

            return orcidID;
        }

        private ParseHubResponse GetORCIDID(string runToken)
        {
            var dataClient = new RestClient(" https://www.parsehub.com/api/v2/runs/" + runToken + "/data");
            var dataRequest = new RestRequest(Method.GET);
            dataRequest.AddParameter("api_key", "twWf3s2XUkKN");
            dataRequest.AddParameter("format", "json");

            IRestResponse dataResponse = dataClient.Execute(dataRequest);
            String statusCode = dataResponse.StatusCode.ToString();
            Console.WriteLine(statusCode);
            ParseHubResponse parseHubResponse = new ParseHubResponse();
            parseHubResponse.statusCode = statusCode;

            if (statusCode == "OK")
            {
                Console.WriteLine(dataResponse.Content);
                var data = (JObject)JsonConvert.DeserializeObject(dataResponse.Content);
                string orcidId = data["orcidid"].Value<string>();
                parseHubResponse.data = orcidId;

            }
            //    Task.WaitAll(waitList.ToArray());
          
            return parseHubResponse;
        }

        #region OLD CODE
        private void PollForData(Action action, int seconds, CancellationToken token)
        {
            if (action == null)
                return;
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    action();
                    await Task.Delay(TimeSpan.FromSeconds(seconds), token);
                }
            }, token).Wait();
        }

        private void PollForORCIDID(Action action, int seconds, CancellationToken token)
        {
            if (action == null)
                return;
            Task.Run(async () =>
             {

                 while (!token.IsCancellationRequested)
                 {
                     try
                     {
                         action();
                         await Task.Delay(TimeSpan.FromSeconds(seconds), token);

                     }
                     catch (OperationCanceledException e)
                     {
                         Console.WriteLine("Poll for orcid was completed");
                     }
                 }
                 token.ThrowIfCancellationRequested();
             }

             , token);
        }

        public List<String> getParsedOrcids()
        {
            return orcidID;

        }

        #endregion

        public String syncedPollORCIDID(int seconds, string runToken)
        {
            ParseHubResponse parseHubResponse = new ParseHubResponse();
            try
            {
                var currentMs = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;
                parseHubResponse = GetORCIDID(runToken);

                while (parseHubResponse.statusCode != "OK" && parseHubResponse.data==null)
                {

                    var elapsedTime = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;

                    if (elapsedTime - currentMs >= seconds * 1000)
                    {
                        Console.WriteLine("Searching for orcidid....");
                        parseHubResponse = GetORCIDID(runToken);
                    }


                }
                Console.WriteLine("Orcid Found...");
                Console.WriteLine(parseHubResponse.data);
            } catch (Exception e)
            {

            }

            return parseHubResponse.data;

        }

        public String syncedPollPureURL(int seconds, string runToken)
        {
            ParseHubResponse parseHubResponse = new ParseHubResponse();
            try
            {
                var currentMs = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;
              parseHubResponse = GetPurePortalURL(runToken);

                while (parseHubResponse.statusCode != "OK" && parseHubResponse.data==null)
                {

                    var elapsedTime = (DateTime.Now - DateTime.MinValue).TotalMilliseconds;

                    if (elapsedTime - currentMs >= seconds * 1000)
                    {
                        parseHubResponse = GetPurePortalURL(runToken);
                        Console.WriteLine("Searching for PureUrl...");
                    }


                }
            
            Console.WriteLine("Pure Url found!!");
            Console.WriteLine(parseHubResponse.data);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            }
            return parseHubResponse.data;
        }



        internal class ParseHubResponse
        {
            public String statusCode;
            public String data = null;

            public ParseHubResponse() { }
            public ParseHubResponse(String sc, String d)
            {
                statusCode = sc; data = d;
            }
        }
    }
}
