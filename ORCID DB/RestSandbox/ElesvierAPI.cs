using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public String SearchByAuthor(string firstName, string lastName, string affiliation)
        {
            String result = "";

            var client = new RestClient(" https://api.elsevier.com/content/search/author");
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-ELS-APIKey", "4a70171325e8753d35fd83ba22370741");
           // request.AddHeader("X-ELS-ReqId", "gennaroimperatore");
            //request.AddHeader("X-ELS-ResourceVersion", "new");
            //request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("query", String.Format("AUTHFIRST({0}) AND AUTHLASTNAME({1}) AND AFFIL({2})", firstName, lastName, affiliation));
            
            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);

            var t = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(response.Content);
            Console.WriteLine(t["search-results"].Children().Last().First().First()["dc:identifier"]);
            Console.WriteLine(response.IsSuccessful);
            Console.WriteLine(response.StatusCode);

               return result;
        }

        public String getAuthorID(string firstName, string lastName, string affiliation)
        {
            String result = "";

            var client = new RestClient(" https://api.elsevier.com/content/search/author");
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-ELS-APIKey", "4a70171325e8753d35fd83ba22370741");
           
            request.AddHeader("Accept", "application/json");
            request.AddParameter("query", String.Format("AUTHFIRST({0}) AND AUTHLASTNAME({1}) AND AFFIL({2})", firstName, lastName, affiliation));

            IRestResponse response = client.Execute(request);

          //  Console.WriteLine(response.Content);

            String authorID = "";
            try
            {
                var t = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(response.Content);

                authorID = (t["search-results"].Children().Last().First().First()["dc:identifier"]).ToString().Split(':')[1];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
           // Console.WriteLine(response.IsSuccessful);
           // Console.WriteLine(response.StatusCode);

            return authorID;
        }

        public HashSet<String> SearchByPublication(string eid)
        {
            HashSet<String> result = new HashSet<string>();

            var client = new RestClient(" https://api.elsevier.com/content/abstract/eid/" + eid);
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-ELS-APIKey", "4a70171325e8753d35fd83ba22370741");
            request.AddHeader("Accept", "application/json");
            // request.AddParameter("query", String.Format("eid({0})",eid));
            IRestResponse response = client.Execute(request);

            // Console.WriteLine(response.Content);

            JObject t = (Newtonsoft.Json.Linq.JObject) JsonConvert.DeserializeObject(response.Content);
            JToken citationInfo = JToken.FromObject("");
            JToken subjectAreas = JToken.FromObject("");
            try
            {
                JEnumerable<JToken> children;


                if (t.Children().Count() == 0)
                    return null;
                children = t.Children();

                while(children.Count()>0)
                {
                    foreach(var child in children)
                    {
                     
                        JsonReader reader = child.CreateReader();
                        Console.WriteLine(reader.Path);
                        if (reader.Path.Equals("abstracts-retrieval-response.authkeywords"))
                        {
                            Console.WriteLine(reader.Path);
                            citationInfo = child;
                         //   break;
                        }

                        if(reader.Path.Equals("abstracts-retrieval-response.subject-areas"))
                        {
                            subjectAreas = child;
                            Console.WriteLine(subjectAreas);
                        }
                                  
                        
                    }
                    if (children.Children().Count() == 0)
                        break;
                    children = (JEnumerable < JToken >) children.Children();
                    
                }

             
                var authorKeyWords = citationInfo["author-keyword"];
                var subjects = subjectAreas["subject-area"];

                Console.WriteLine(authorKeyWords);
                foreach (var a in authorKeyWords)
                {
                   
                    result.Add(a["$"].ToString());
                }

                foreach (var s in subjects)
                {
                    result.Add(s["$"].ToString());
                }
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
                


           return result;
        }

       
        public HashSet<String> SearchByAuthorID(String authorID)
        {
            HashSet<String> result = new HashSet<string>();

            var client = new RestClient(" https://api.elsevier.com/content/search/scopus");
            var request = new RestRequest(Method.GET);
            request.AddHeader("X-ELS-APIKey", "4a70171325e8753d35fd83ba22370741");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("query", String.Format("AU-ID({0})", authorID));

            IRestResponse response = client.Execute(request);

            // Console.WriteLine(response.Content);
            String eid = "";
            try
            {
                var t = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(response.Content);
                var publications = t["search-results"].Children().Last().First();
                //Console.WriteLine(publications);
                foreach (var pub in publications.Children())
                {
                    Console.WriteLine(pub["eid"]);
                    eid = pub["eid"].ToString();
                    SearchByPublication(eid).ToList().ForEach(x=> result.Add(x));
                  
                    


                }
             //   
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
          //  
           // Console.WriteLine(response.IsSuccessful);
          //  Console.WriteLine(response.StatusCode);

            return result;
        }



    }

    /*{"search-results":
     * {"opensearch:totalResults":"1","opensearch:startIndex":"0","opensearch:itemsPerPage":"1","opensearch:Query":{"@role": "request", "@searchTerms": "AUTHFIRST(Gennaro) AND AUTHLASTNAME(Imperatore) AND AFFIL(University of Strathclyde)", "@startPage": "0"},
     *  "link": [{"@_fa": "true", "@ref": "self", "@href": "https://api.elsevier.com/content/search/author?start=0&count=25&query=AUTHFIRST%28Gennaro%29+AND+AUTHLASTNAME%28Imperatore%29+AND+AFFIL%28University+of+Strathclyde%29",  "@type": "application/json"},
     *  {"@_fa": "true", "@ref": "first", "@href": "https://api.elsevier.com/content/search/author?start=0&count=25&query=AUTHFIRST%28Gennaro%29+AND+AUTHLASTNAME%28Imperatore%29+AND+AFFIL%28University+of+Strathclyde%29", "@type": "application/json"}],
     *  "entry": [{"@_fa": "true", "link": [{"@_fa": "true", "@ref": "self", "@href": "https://api.elsevier.com/content/author/author_id/55419718300"},
     *  {"@_fa": "true", "@ref": "search", "@href": "https://api.elsevier.com/content/search/author?query=au-id%2855419718300%29"},
     *  {"@_fa": "true", "@ref": "scopus-citedby", "@href": "https://www.scopus.com/author/citedby.uri?partnerID=HzOxMe3b&citedAuthorId=55419718300&origin=inward"},
     *  {"@_fa": "true", "@ref": "scopus-author", "@href": "https://www.scopus.com/authid/detail.uri?partnerID=HzOxMe3b&authorId=55419718300&origin=inward"}],
     *"prism:url":"https://api.elsevier.com/content/author/author_id/55419718300",
     * "dc:identifier":"AUTHOR_ID:55419718300","eid":"9-s2.0-55419718300",
     * "preferred-name":{"surname":"Imperatore","given-name":"Gennaro","initials":"G."},
     * "document-count":"4","subject-area":{"@abbrev": "COMP", "@frequency": "13", "$" :"Computer Science (all)"},
     * "affiliation-current":{"affiliation-url":"https://api.elsevier.com/content/affiliation/affiliation_id/60024724","affiliation-id":"60024724","affiliation-name":"University of Strathclyde","affiliation-city":"Glasgow","affiliation-country":"United Kingdom"}}]}}*/


    class AuthorSearchResult
    {
     [JsonProperty("search-results")]   
      public  AuthorSearchResultContent content;
    }
    class AuthorSearchResultContent
    {
        [JsonProperty("document-count")]
       public int DocumentCount;
        [JsonProperty("dc:identifier")]
        public string authorID;
        
    }
}
