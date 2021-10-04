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

        private String[] FormatNameStyle(String name)
        {
            String[] formattedName = new String[2];

            String[] titles = {"Mr","Ms","Miss","Mrs","Dr","Prof","Doctor","Professor"};
            name = name.Replace(".", ""); //remove dots after title
           

            String[] splittedName = { };
            

            //remove titles from the name
            if(name.Contains(","))  //format 1 if name contains a comma the first part is the surname
            {
              splittedName  = name.Split(',');
                /// splittedName.ToList().ForEach(x => x.Trim());         
                if (splittedName.Count() > 1)
                {
                    splittedName = splittedName.Select(x=>x.Trim()).ToArray();
                    foreach(String t in titles)
                    {
                        for (int i= 0; i<splittedName.Count(); i++)
                        splittedName[i] = splittedName[i].Replace(t, "").Trim();
                        
                    }
                    
                    formattedName = new string [] { splittedName[1], splittedName[0]};
                }
            }
            else
            {
                 splittedName = name.Split(' ');

                if (splittedName.Count() > 1)
                {
                    splittedName = splittedName.Where(x => !titles.Contains(x)).ToArray();
                    // formattedName = splittedName[0] + " " + splittedName[1];
                   
                   for (int i=0; i<splittedName.Count()-1; i++)
                    {

                        formattedName[0] += splittedName[i] + " ";
                    }
                    formattedName[1] = splittedName.Last();


            
                }

            }
       

            return formattedName;
        }

        private String GetAffiliation(string email)
        {
            String affil = "";

            if (email.EndsWith("arizona.edu"))
                return "University of Arizona";

            return affil;
        }

        private String FindClosestACMClassification(string keyword)
        {
            String classification = null;

            ACMClassificationXMLReader aCMClassificationXMLReader = new ACMClassificationXMLReader();

            //some preprocessing on the keyword
            keyword = keyword.Replace("-", " ");

            string ACMLabel = "";
            double currentMax = double.MinValue;
            while((ACMLabel = aCMClassificationXMLReader.GetNextCategory())!=null)
            {
                 MicrosoftKnowledgeAPI microsoftKnowledgeAPI = new MicrosoftKnowledgeAPI();
                 double similarity = 
                    microsoftKnowledgeAPI.CalculateSemanticSimilarityBetweenTerms(ACMLabel, keyword);
                if (similarity > currentMax)
                {
                    currentMax = similarity;
                    classification = ACMLabel;
                    if (similarity == 1.0)
                        return classification;
                }

            }
            if (currentMax < 0.50)
                return null;

            return classification;
        }

        public void GetDataBaseObject()
        {
            try
            {
                MongoClient client = new MongoClient(uri);
               /* foreach (String name in client.ListDatabaseNames().ToList())
                    Console.WriteLine(name);*/

                IMongoDatabase ischoolsDB = client.GetDatabase("ischdb_experimental");
               
              var facultyDocuments = ischoolsDB.GetCollection<BsonDocument>("faculty",null);
                var institutions = ischoolsDB.GetCollection<BsonDocument>("institutions", null);
                
               // parseHubExtractor.GetResearcherORCIDID();

                float index = 0;
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
                        BsonObjectId schoolID = f.GetValue("ischool_id").AsObjectId;
                      var val =  BsonValue.Create("");

                        if (!f.TryGetValue("unclassifiedKeyWords", out val))
                        {
                            index++;
                            continue;
                        }
                            

                        int nKeywords = f.GetValue("unclassifiedKeyWords").AsBsonArray.Count;
                        long facultyTotal = facultyDocuments.Count(new BsonDocument());
                        if (nKeywords == 0)
                            index++;
                        Console.WriteLine(100-(index/(float)facultyTotal)*100);

                        continue;

                        if (schoolID == null)
                            continue;
                        String orcidID = "";

                        String universityName = institutions.Find(Builders<BsonDocument>.Filter.Eq("_id", schoolID)).ToList().First()["name"].AsString;
                        if (universityName.Equals("University of Strathclyde"))
                            continue;
                        

                        String[] formattedName = FormatNameStyle(name);

                        ElesvierAPI elesvier = new ElesvierAPI();
                       
                           
                            var keywordsForAuthor = elesvier.SearchByAuthorID
                                (elesvier.getAuthorID(formattedName[0], formattedName[1], universityName));
                            if (keywordsForAuthor == null)
                                continue;
                            Console.WriteLine("Adding Keywords for " + name);

                            // add the keywords  to our database
                            var filter = Builders<BsonDocument>.Filter.Eq("name", f["name"]);
                              var update = Builders<BsonDocument>.Update.Set("unclassifiedKeyWords", keywordsForAuthor);
                              facultyDocuments.UpdateOne(filter, update);


                            HashSet<KeyValuePair<string, string>> acmKeyWords = new HashSet<KeyValuePair<string, string>>();
                            /*foreach (var keyword in keywordsForAuthor)
                             {
                                 Console.WriteLine("Looking for classification for word " + keyword);
                                var classification = FindClosestACMClassification(keyword);
                               
                                 if(classification==null)// not found
                                 {
                                     //Console.WriteLine("Not found");
                                 }
                                 else
                                 {
                                     Console.WriteLine("Found : " + classification);
                                    acmKeyWords.Add(new KeyValuePair<string, string>(keyword, classification));

                                 }

                            }
                            if (keywordsForAuthor.Count > 0)
                            {
                                var update = Builders<BsonDocument>.Update.Set("ACMKeywords", acmKeyWords);
                                facultyDocuments.UpdateOne(filter, update);
                            }*/


                            //   orcidID = parseHubExtractor.GetResearcherORCIDID(url, name);

                            //  listWriter.WriteLine(name + ":" + orcidID);

                            /*  if (orcidID != null)

                              {
                                 
                              }*/
                          //  Console.WriteLine(keywordsForAuthor);
                        
                           index++;
                       }
                      // listWriter.Close();
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
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.ToString());
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
