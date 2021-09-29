using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestSandbox
{
    class MDBiSchoolsReade
    {
        private const string uri = "mongodb+srv://gennaro:sha9tTer@ischdb-experimental.hmxwz.mongodb.net/ischdb_experimental?authSource=admin";

        public void GetDataBaseObject()
        {
            try
            {
                MongoClient client = new MongoClient(uri);
               /* foreach (String name in client.ListDatabaseNames().ToList())
                    Console.WriteLine(name);*/

                IMongoDatabase ischoolsDB = client.GetDatabase("ischdb_experimental");
               
              var facultyDocuments = ischoolsDB.GetCollection<BsonDocument>("faculty",null);
                
               // parseHubExtractor.GetResearcherORCIDID();

                int index = 0;
                ParseHubExtractor parseHubExtractor = new ParseHubExtractor();
                String path = Directory.GetCurrentDirectory();
                String filename = "orcid.txt";
                path = Path.Combine(path, filename);

                bool fileCreated = File.Exists(path);

                if (true)
                {
                    // StreamWriter listWriter = new StreamWriter(path, false);
                    foreach (var f in facultyDocuments.Find(new BsonDocument()).ToList())
                    {
                        // Console.WriteLine(f.ToJson());
                        String name = f.GetValue("name").AsString;
                        String url = f.GetValue("url").AsString;
                        String orcidID = "";
                        

                        ElesvierAPI elesvier = new ElesvierAPI();
                        //elesvier.SearchByAuthor("Gennaro", "Imperatore", "University of Strathclyde");
                        //elesvier.SearchByPublication("Imperatore", "University of Strathclyde");


                     var keywordsForAuthor =   elesvier.SearchByAuthorID(elesvier.getAuthorID("Gennaro", "Imperatore", "University of Strathclyde"));
                        Console.WriteLine();
                        /*   if (url.Contains("strath.ac.uk"))
                           {
                          
                               orcidID = parseHubExtractor.GetResearcherORCIDID(url, name);

                               listWriter.WriteLine(name + ":" + orcidID);

                               if (orcidID != null)

                               {
                                   // add the orcid id to our database
                                   var filter = Builders<BsonDocument>.Filter.Eq("name", f["name"] );
                                   var update = Builders<BsonDocument>.Update.Set("orcidid", orcidID);
                                   facultyDocuments.UpdateOne(filter, update);
                               }
                           }
                           index++;
                       }
                       listWriter.Close();*/
                    }
                }
                else // we have a list of orcid ids already....
                {
                    var workCollection = ischoolsDB.GetCollection<BsonDocument>("works");
                    if (workCollection == null)
                    {

                        Console.WriteLine("works collection does not exist. Creating...");
                        ischoolsDB.CreateCollection("works");
                    }

                    Console.WriteLine("Reading orcid list file... ");
                    StreamReader streamReader = new StreamReader(path);
                    String line = "";
                    while ((line =streamReader.ReadLine() )!= null)
                    {

                        String[] orcidID = line.Split(':');
                        Console.WriteLine("Extracting data for " + orcidID[0]);
                        if(orcidID[1] =="")
                        {
                            Console.WriteLine("Missing ORCID ID. Skipping");
                            continue;
                        }
                        else
                        {
                           



                        }

                        Console.WriteLine(orcidID[2]);

                        string pattern = "orcid.org.(.+)";
                        String numericID = Regex.Match(orcidID[2], pattern).Groups[1].ToString();
                       Console.WriteLine(numericID );
                            


                        OrcidExtractor orcidExtractor = new OrcidExtractor();
                        orcidExtractor.Extract(numericID);

                        return; // stop at the first person... 

                        //workCollection.InsertOne
                        }
                        
                }

               


            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

           
        }

        internal class FacultyDocument
        {
            /*{"_id":
             * {"$oid":"6022d9e6f01f6d21e6ad5316"},
             * "ischool_id":{"$oid":"601ca66eb09f554622e9c2d9"},
             * "name":"Mr Alasdair Lambert",
             * "role":"Teaching Assistant",
             * "url":"https://www.strath.ac.uk/staff/lambertalasdairmr/",
             * "updatedAt":{"$date":{"$numberLong":"1620373312922"}},
             * "email":"alasdair.lambert@strath.ac.uk"}*/

            public String name;
            public String role;
            public String url;
            public String email;


        }
    }
}
