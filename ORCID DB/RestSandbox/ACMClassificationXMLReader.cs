using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RestSandbox
{
    class ACMClassificationXMLReader
    {
        XmlDocument mDocument = new XmlDocument();
        bool mInitialised = false;


        private void Initialise()
        {
            string filename =
            string path = Directory.GetCurrentDirectory();

            try
            {
                mDocument.Load
            }
            catch(Exception e)
            {
                mInitialised = false;
            }

            mInitialised = true;

            
        }

        public void  ()

        public void CleanUP()
        {

        }
    }
}
