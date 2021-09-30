
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

            MicrosoftKnowledgeAPI microsoftKnowledgeAPI = new MicrosoftKnowledgeAPI();
            microsoftKnowledgeAPI.CalculateSemanticSimilarityBetweenTerms("smartphones", "mobile");

            //  new ParseHubExtractor().syncedPoll(3);

            // OrcidExtractor orcidExtractor = new OrcidExtractor();
            // orcidExtractor.Extract("");
            //   NLPEngine nLPEngine = new NLPEngine();
            // nLPEngine.ExtractKeyWords();

           
            Console.ReadLine();
        }

    }
}