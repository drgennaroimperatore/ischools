﻿
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
            ParseHubExtractor parseHubExtractor = new ParseHubExtractor();
            // Console.WriteLine(parseHubExtractor.GetResearcherORCIDID());

            MDBiSchoolsReade dBiSchoolsReade = new MDBiSchoolsReade();
            dBiSchoolsReade.GetDataBaseObject();
            Console.ReadLine();
        }

    }
}