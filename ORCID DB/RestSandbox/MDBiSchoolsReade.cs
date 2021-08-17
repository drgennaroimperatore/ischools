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
                foreach (String name in client.ListDatabaseNames().ToList())
                    Console.WriteLine(name);

                IMongoDatabase ischoolsDB = client.GetDatabase("ischdb_experimental");
               
              var facultyDocuments = ischoolsDB.GetCollection<BsonDocument>("faculty",null);
                
               // parseHubExtractor.GetResearcherORCIDID();

                int index = 0;
                ParseHubExtractor parseHubExtractor = new ParseHubExtractor();
                String path = Directory.GetCurrentDirectory();
                String filename = "orcid.txt";
                path = Path.Combine(path, filename);

                bool fileCreated = File.Exists(path);

                if (!fileCreated)
                {
                    StreamWriter listWriter = new StreamWriter(path, false);
                    foreach (var f in facultyDocuments.Find(new BsonDocument()).ToList())
                    {
                        // Console.WriteLine(f.ToJson());
                        String name = f.GetValue("name").AsString;
                        String url = f.GetValue("url").AsString;
                        String orcidID = "";
                        Console.WriteLine();
                        if (url.Contains("strath.ac.uk"))
                        {

                            orcidID = parseHubExtractor.GetResearcherORCIDID(url, name);

                            listWriter.WriteLine(name + ":" + orcidID);
                        }
                        index++;
                    }
                    listWriter.Close();
                }
                else
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

                        Console.WriteLine(orcidID[2]);

                        string pattern = "orcid.org(?[a-z]*)";
                       Console.WriteLine( Regex.Match(orcidID[2], pattern).ToString());
                            


                      

                        OrcidExtractor orcidExtractor = new OrcidExtractor();


                       

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
