using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RestSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
             var client = new RestClient(" https://orcid.org/oauth/token");
             var request = new RestRequest(Method.POST);
             request.AddHeader("cache-control", "no-cache");
             request.AddHeader("content-type", "application/x-www-form-urlencoded");
             request.AddParameter("application/x-www-form-urlencoded", "client_id=APP-O5COE6Q0DG7GL1DW&client_secret=8385cc63-48af-4679-b556-99b7168d37cb&grant_type=client_credentials&scope=/read-public", ParameterType.RequestBody);
             IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);
            Token t = JsonConvert.DeserializeObject<Token>(response.Content);
            Console.WriteLine(t.access_token);
            Console.WriteLine(response.IsSuccessful);
            Console.WriteLine(response.StatusCode);

          var  qclient = new RestClient(" https://pub.orcid.org/v2.1/0000-0002-5018-0340/record");
            qclient.AddDefaultHeader("authorization", String.Format("bearer {0}",t.access_token));
            var qrequest = new RestRequest(Method.GET);
          // qrequest.AddHeader("cache-control", "no-cache");
           qrequest.AddHeader("content-type", "application/orcid+json");
            qrequest.AddParameter("accept", "application/json");
            //qrequest.AddParameter("application/x-www-form-urlencoded", "client_id=APP-O5COE6Q0DG7GL1DW&client_secret=8385cc63-48af-4679-b556-99b7168d37cb", ParameterType.RequestBody);
            
            IRestResponse qresponse = qclient.Execute(qrequest);

            Console.WriteLine(qresponse.ErrorMessage);
            foreach(var h in qresponse.Headers)
            {
                Console.WriteLine(h.Name + ": " + h.Value);
            }
            Console.WriteLine(qresponse.Content);
            Console.WriteLine(qresponse.IsSuccessful);
            Console.WriteLine(qresponse.StatusCode);

            Console.ReadLine();

            /*{"access_token":"c5249ccd-1666-4f00-9ec8-76095f63b0fc","token_type":"bearer","refresh_token":"b3f75d10-1bd6-4c8f-9b8c-ebba70721ad7","expires_in":631138518,"scope":"/read-public","orcid":null}*/
        }

        internal class Token
        {
          public String access_token;
            String token_type;
          public  String refresh_token;
            int expires_in;
            String scope;
            String orcid;

        }
    }
}