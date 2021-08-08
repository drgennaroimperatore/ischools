using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestSandbox
{
    class ParseHubExtractor
    { 
        public String GetResearcherORCIDID()
        {
            var client = new RestClient(" https://www.parsehub.com/api/v2/projects/tMQrtJvJgtm-/run");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
          //  request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("api_key", "twWf3s2XUkKN");
            request.AddParameter("format", "json");
            IRestResponse response = client.Execute(request);
            
            var data = (JObject)JsonConvert.DeserializeObject(response.Content);
            string runToken = data["run_token"].Value<string>();
            Console.WriteLine("Found run token :" + runToken);


            var cts = new CancellationTokenSource();

            PollForData(() => 
             { ParseHubResponse parseHubResponse = GetPurePortalURL(runToken);
                 
                 if (parseHubResponse.statusCode == "OK")
                {
                    cts.Cancel(); // cancel the timer task
                                  // and call the other scraper to get the orciddata
                     Console.WriteLine(parseHubResponse.data);
                     //Console.ReadLine();
                    RunORCIDIDScraper(parseHubResponse.data);
                }
            }, 1, cts.Token);

          
           

            return "WIP";
        }

        private ParseHubResponse GetPurePortalURL(String runToken)
        {
            var dataClient = new RestClient(" https://www.parsehub.com/api/v2/runs/" + runToken + "/data");
            var dataRequest = new RestRequest(Method.GET);
            dataRequest.AddParameter("api_key", "twWf3s2XUkKN");
            dataRequest.AddParameter("format", "json");

            IRestResponse dataResponse = dataClient.Execute(dataRequest);
            String statusCode = dataResponse.StatusCode.ToString();
            Console.WriteLine(statusCode);

            if(statusCode=="OK")
                Console.WriteLine(dataResponse.Content);
            ParseHubResponse parseHubResponse = new ParseHubResponse();
            parseHubResponse.statusCode = statusCode;
            if(statusCode=="OK")
            {
                var data = (JObject)JsonConvert.DeserializeObject(dataResponse.Content);
                string pureUrl = data["pureUrl"].Value<string>();
                parseHubResponse.data = pureUrl;
            }

            return parseHubResponse; 
        }

        private String RunORCIDIDScraper(String startUrl)
        {
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

            PollForORCIDID(() =>
            {
                ParseHubResponse parseHubResponse = GetORCIDID(runToken);
                if (parseHubResponse.statusCode  == "OK")
                {
                    cts.Cancel(); // cancel the timer task
                    Console.WriteLine("OrcidID Found: " + parseHubResponse.data);
                }
            }, 1, cts.Token);

            return "id";
        }

        private ParseHubResponse GetORCIDID (string runToken)
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

            return parseHubResponse;
        }

        private void PollForData (Action action, int seconds, CancellationToken token)
        {
            if (action == null)
                return;
            Task.Run(async () => {
                while (!token.IsCancellationRequested)
                {
                    action();
                    await Task.Delay(TimeSpan.FromSeconds(seconds), token);
                }
            }, token);
        }

        private void PollForORCIDID(Action action, int seconds, CancellationToken token)
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
            }, token);
        }

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
