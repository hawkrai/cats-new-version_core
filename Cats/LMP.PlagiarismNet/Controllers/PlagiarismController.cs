using System.Collections.Generic;
using System.IO;
using System.Linq;
using LMP.PlagiarismNet.Extensions;
using LMP.PlagiarismNet.Utils;
using LMP.PlagiarismNet.XMLDocs;

namespace LMP.PlagiarismNet.Controllers
{
    public class PlagiarismController
    {
        public List<ClusterDoc> Check(List<string> paths, int threshold, int termCount, int mode)
        {
            var docs = GetDocs(paths);
            var rows = ClusteringFactory.GetSimilarity(mode).MakeSimilarityRows(docs, termCount);

            #region debug

            //if (Boolean.valueOf(ApplicationUtil.getProperty("debug.mode"))) {
            //	try {
            //		FileWriter fw = new FileWriter(new File(MyStem.MYSTEM_DIR + File.separator + "result.txt"));
            //		Iterator var7 = rows.iterator();

            //		while(var7.hasNext()) {
            //			SimilarityRow row = (SimilarityRow)var7.next();
            //			String str = row.getDoc().getDocIndex() + ": ";
            //			TreeMap<Doc, Integer> map = new TreeMap(new 1(this));
            //			map.putAll(row.getSimilarity());

            //			Entry entry;
            //			for(Iterator var11 = map.entrySet().iterator(); var11.hasNext(); str = str + entry.getValue() + " ") {
            //				entry = (Entry) var11.next();
            //			}

            //			fw.write(str + System.lineSeparator());
            //		}

            //		fw.close();
            //	} catch (IOException var13) {
            //		var13.printStackTrace();
            //	}
            //}

            #endregion

            var clusters = ClusteringFactory.GetGreedy().Clustering(rows, threshold);
            var cDocs = new List<ClusterDoc>();
            clusters.ForEach(cluster =>
            {
                if (cluster.Docs.Count > 1) cDocs.Add(new ClusterDoc(cluster));
            });
            return cDocs;
        }

        public List<ClusterDoc> CheckByDirectory(List<string> folders, int threshold, int termCount, int mode)
        {
            return Check(GetDocsByDirectoty(folders), threshold, termCount, mode);
        }

        public List<PlagiateDoc> CheckBySingleDoc(string checkDoc, List<string> folders, int threshold, int termCount)
        {
            var paths = GetDocsByDirectoty(folders);
            var docs = GetDocs(paths);
            var doc = GetDoc(checkDoc);
            docs.Add(doc);
            var rows = ClusteringFactory.GetSimilarity(0).MakeSimilarityRows(docs, termCount);
            var row = rows.FirstOrDefault(x => x.Doc.Equals(doc));

            var plagiats = new List<PlagiateDoc>();
            if (row != null)
            {
                var var11 = row.Similarity.GetEnumerator();

                while (var11.MoveNext())
                {
                    var entry = var11.Current;
                    if (!entry.Key.Equals(doc) && entry.Value >= threshold)
                        plagiats.Add(new PlagiateDoc(entry.Key.Path, entry.Value));
                }

                var11.Dispose();
            }

            return plagiats;
        }

        private List<string> GetDocsByDirectoty(List<string> folders)
        {
            var docs = new List<string>();
            var var3 = folders.GetEnumerator();

            while (var3.MoveNext())
            {
                var folder = var3.Current;
                if (!string.IsNullOrEmpty(folder)) docs.AddRange(ProcessFilesFromFolder(folder));
            }

            var3.Dispose();
            return docs;
        }

        public List<string> ProcessFilesFromFolder(string folder)
        {
            return CustomDirectory.GetFiles(folder, "*.doc|*.docx", SearchOption.AllDirectories);
        }

        private List<Doc> GetDocs(List<string> paths)
        {
            var docs = new List<Doc>();
            var var3 = paths.GetEnumerator();

            while (var3.MoveNext())
            {
                var path = var3.Current;
                docs.Add(GetDoc(path));
            }

            var3.Dispose();
            return docs;
        }

        private Doc GetDoc(string path)
        {
            var doc = new Doc();
            doc.Path = path;
            doc.FileName = StringUtils.GetFileName(path);
            doc.DocIndex = doc.FileName;
            return doc;
        }
    }
}