using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using LMP.PlagiarismNet.Services.Interfaces;

namespace LMP.PlagiarismNet.Services
{
    public class MyStem : IMyStem
    {
        public const string MUTEX = "mutex";

        public static readonly string MYSTEM_DIR = @"D:\LMSSystem" + Path.DirectorySeparatorChar + "mystem";

        public static readonly string MYSTEM_IN_DIR = @"D:\LMSSystem" + Path.DirectorySeparatorChar;

        public static readonly string MYSTEM_OUT_DIR = @"D:\LMSSystem" + Path.DirectorySeparatorChar;

        public List<string> Parse(string fileName)
        {
            var terms = new List<string>();

            try
            {
                terms.AddRange(PopulateTermsList(GetText(Path.GetFullPath(fileName))));
            }
            catch (IOException var4)
            {
                Console.WriteLine(var4.StackTrace);
            }

            return terms;
        }

        public static List<string> PopulateTermsList(List<string> termsList)
        {
            var terms = new List<string>();
            termsList.ForEach(x =>
            {
                var words = x.Split(' ');
                terms.AddRange(words);
            });
            terms.RemoveAll(x => x == "");

            return terms;
        }
        
        private static List<string> GetText(string filepath)
        {
            var wordDoc = WordprocessingDocument.Open(filepath, true);

            var termsList = new List<string>();

            var doc = wordDoc.MainDocumentPart.Document;
            foreach (var paragraph in doc.Body.ChildElements)
                // Iterate through all Run elements in the Paragraph element.
            {
                foreach (var run in paragraph.ChildElements)
                {
                    var text = run.InnerText;
                    if (!string.IsNullOrEmpty(text)) termsList.Add(text);
                }
            }

            wordDoc.Close();
            return termsList;
        }
    }
}