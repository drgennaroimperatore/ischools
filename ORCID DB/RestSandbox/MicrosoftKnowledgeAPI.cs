using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestSandbox
{
    class MicrosoftKnowledgeAPI
    {
        public double CalculateSemanticSimilarityBetweenTerms(String s1, String s2)
        {
            double result = 0.0f;

            var client = new RestClient(" https://api.labs.cognitive.microsoft.com/academic/v1.0/similarity");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Ocp-Apim-Subscription-Key", "d8e352a1d2504639ad548dbbc5422668");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("s1", s1);
            request.AddParameter("s2", s2);

            IRestResponse response = client.Execute(request);


            try
            {
                result=  double.Parse(response.Content);
            }
           catch (Exception e)
            {
                return 0;
            }

        //  Console.WriteLine( response.Content);

            return result;
        }
    }
}
