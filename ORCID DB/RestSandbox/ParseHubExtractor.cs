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
            Console.WriteLine(runToken);


            var cts = new CancellationTokenSource();

            PollForData(() => { if (GetORCIDData(runToken) == "OK") cts.Cancel(); }, 1, cts.Token);
           

            return "WIP";
        }

        private string GetORCIDData(String runToken)
        {
            var dataClient = new RestClient(" https://www.parsehub.com/api/v2/runs/" + runToken + "/data");
            var dataRequest = new RestRequest(Method.GET);
            dataRequest.AddParameter("api_key", "twWf3s2XUkKN");
            dataRequest.AddParameter("format", "json");

            IRestResponse dataResponse = dataClient.Execute(dataRequest);
            String statusCode = dataResponse.StatusCode.ToString();
            Console.WriteLine(statusCode);

            Console.WriteLine(dataResponse.Content);

            return statusCode;
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
    }
}
