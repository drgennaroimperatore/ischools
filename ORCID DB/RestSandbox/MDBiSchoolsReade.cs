using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

               foreach(var f in facultyDocuments.Find(new BsonDocument()).ToList())
                {
                    Console.WriteLine(f.ToJson());
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Error with Connection " + e.Message);
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
