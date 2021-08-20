using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSandbox
{
    class ElesvierAPI
    {
        public String GetAuthorisationToken()
        {
            String authToken = "";

            var client = new RestClient(" https://api.elsevier.com/content/search/author");
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-ELS-APIKey", "4a70171325e8753d35fd83ba22370741");
           // request.AddHeader("X-ELS-ReqId", "gennaroimperatore");
            //request.AddHeader("X-ELS-ResourceVersion", "new");
            //request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("field", "given-name(crawford)");
            request.AddParameter("field", "surname(revie)");
            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);
             
                String t = JsonConvert.DeserializeObject<String>(response.Content);
           // Console.WriteLine(t.access_token);
            Console.WriteLine(response.IsSuccessful);
            Console.WriteLine(response.StatusCode);

               return authToken;
        }
    }
}
