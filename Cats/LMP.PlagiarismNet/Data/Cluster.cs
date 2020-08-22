using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using LMP.PlagiarismNet.XMLDocs;

namespace LMP.PlagiarismNet.Data
{
    public class Cluster
    {
        public Cluster()
        {
            Docs = new List<Doc>();
        }

        public List<Doc> Docs { get; set; }
        public string IdTutor { get; set; }

        public string GetUniqueKey()
        {
            var sb = new StringBuilder();
            foreach (var doc in Docs) sb.Append(doc.Path);

            try
            {
                var md5 = MD5.Create();
                var data = Encoding.UTF8.GetBytes(sb.ToString());
                var byteData = md5.ComputeHash(data);
                var hash = new StringBuilder();
                foreach (var t in byteData) hash.Append(Convert.ToString((t & 0xFF) + 256, 16).Substring(1));
                return hash.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var doc in Docs) sb.Append(doc.DocIndex).Append(" ");
            return sb.ToString().Trim();
        }
    }
}