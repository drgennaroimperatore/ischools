using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpNL;
using SharpNL.Tokenize;
using SharpNL.Stemmer;
using SharpNL.Lemmatizer;
using SharpNL.Utility;
using TokenUtilies = SharpNL.Tokenize.TokenUtilities;
using PorterStemmer = SharpNL.Stemmer.Porter.PorterStemmer;
using System.Text.RegularExpressions;

namespace RestSandbox
{
    class NLPEngine
    {
        const string json_filename = "orcidworktemplate.txt";
        

        private List<String> loadStopWords()
        {
            StreamReader reader = null;
            List<String> stopWords = new List<String>();

            try
            {

                reader = new StreamReader("stopwords.txt");
                String line = "";
                while((line =reader.ReadLine())!=null)
                {
                    stopWords.Add(line);
                }
                
            } catch (Exception e)
            {
                Console.WriteLine("Error loading stopwords :"+ e.Message);
            }

            return stopWords;


        }

       public void ExtractKeyWords()
        {
            StreamReader sreader = new StreamReader(json_filename);

            SimpleTokenizer tokeniser = SimpleTokenizer.Instance;

            var stopWords = loadStopWords();
            int totalTitles = 0;


            try
            {
                String workData = sreader.ReadToEnd();
               // Console.WriteLine(workData);

                JsonTextReader reader = new JsonTextReader(new StringReader(workData));
                StringBuilder corpus = new StringBuilder();
               

                String prevTitle = "";
                while (reader.Read())
                {
           
                    if (reader.Value != null)
                    {
                        if (reader.TokenType.ToString().Equals("PropertyName") && reader.Value.Equals("title"))
                        {
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            reader.Read();
                            if (reader.Value.ToString().Equals(prevTitle))
                                continue;
                          prevTitle = reader.Value.ToString();
                            totalTitles++;
                            Console.WriteLine("Title: {0}", reader.Value);
                            corpus.Append(reader.Value);
                            //"title":{"title":{"value":"Validation of a sea lice dispersal model"}
                           
                        }

                        
                    }
                    else
                    {
                       // Console.WriteLine("Token: {0}", reader.TokenType);
                    }
                }
                // remove punctuation from corpus
              

                String[] t = tokeniser.Tokenize(Regex.Replace(corpus.ToString(), @"[^\w\s]", "").ToLower());
                var tokens = TokenUtilies.GetTokenFrequency(t, false);

                foreach (var s in stopWords)
                {
                    tokens.Remove(s);
                }

                foreach (var token in tokens)
                {
                    if(token.Value>= (int)totalTitles/5)
                        Console.WriteLine(String.Format("Word : {0} Frequency : {1}", token.Key, token.Value));
                }


            }
            catch (Exception e) 
            
            {
                Console.WriteLine("Error : " +e.Message);
                sreader.Close();
            }
            sreader.Close();

        }
    }
}
