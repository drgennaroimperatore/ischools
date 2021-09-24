
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace RestSandbox
{
    class Program
    {
        static void Main(string[] args)
        {

            // Console.WriteLine(parseHubExtractor.GetResearcherORCIDID());

            // MDBiSchoolsReade dBiSchoolsReade = new MDBiSchoolsReade();
            //  dBiSchoolsReade.GetDataBaseObject();

            //  new ParseHubExtractor().syncedPoll(3);

            // OrcidExtractor orcidExtractor = new OrcidExtractor();
            // orcidExtractor.Extract("");
            //   NLPEngine nLPEngine = new NLPEngine();
            // nLPEngine.ExtractKeyWords();

            ElesvierAPI elesvier = new ElesvierAPI();
            //elesvier.SearchByAuthor("Gennaro", "Imperatore", "University of Strathclyde");
            //elesvier.SearchByPublication("Imperatore", "University of Strathclyde");
            elesvier.SearchByAuthorID(elesvier.getAuthorID("Gennaro", "Imperatore", "University of Strathclyde"));
            Console.ReadLine();
        }

    }
}