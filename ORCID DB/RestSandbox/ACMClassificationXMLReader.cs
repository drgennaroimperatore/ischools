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
        int mConceptIndex = -1;
        XmlNodeList nodeConceptList = null;




        private void Initialise()
        {
            string filename = "acm.xml";
            string path = Path.Combine(Directory.GetCurrentDirectory(), filename);

            if (!File.Exists(path))
                return;

            try
            {
                mDocument.Load(path);

            }
            catch (Exception e)
            {
                mInitialised = false;
            }

            mInitialised = true;

            nodeConceptList = mDocument.GetElementsByTagName("skos:Concept");

        }

        public String GetNextCategory()
        {
            String category = "";

            if (!mInitialised)
                Initialise();

            if (nodeConceptList.Count - 1 < mConceptIndex)
                return null;

            mConceptIndex++;

            XmlNode labelNode = nodeConceptList.Item(mConceptIndex).FirstChild;
            if (labelNode == null)
                return "";
            String label = labelNode.InnerText;
            if (label == null)
                return "";
            return label;


        }

        public void CleanUP()
        {

        }
    }
}
