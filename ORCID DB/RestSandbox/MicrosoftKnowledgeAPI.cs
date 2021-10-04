using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fastenshtein;

namespace RestSandbox
{
    class MicrosoftKnowledgeAPI
    {
        public double CalculateSemanticSimilarityBetweenTerms(String s1, String s2)
        {
            double result = 0.0f;

            String cleanS1 = s1.ToLower().Replace("-"," ");
            String cleanS2 = s2.ToLower().Replace("-", " ");

            if (cleanS1.Equals(cleanS2))
                return 1.0;
          int distance =  Fastenshtein.Levenshtein.Distance(cleanS1, cleanS2);
            if (distance == 0)
                return 1.0;
            else
               if (distance < 5)
                return 1.0;
            

            var client = new RestClient(" https://api.labs.cognitive.microsoft.com/academic/v1.0/similarity");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Ocp-Apim-Subscription-Key", "d8e352a1d2504639ad548dbbc5422668");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("s1", cleanS1);
            request.AddParameter("s2", cleanS2);

            IRestResponse response = client.Execute(request);


            try
            {
                result = double.Parse(response.Content);
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
