﻿using System;
using System.Collections.Generic;
using System.Linq;
using LMP.PlagiarismNet.Data;
using LMP.PlagiarismNet.Services.Interfaces;
using LMP.PlagiarismNet.Utils;
using LMP.PlagiarismNet.XMLDocs;

namespace LMP.PlagiarismNet.Services
{
    public class Similarity : ISimilarity
    {
        public static int TERM_COUNT = 80;

        public List<SimilarityRow> MakeSimilarityRows(List<Doc> docs, int termCount)
        {
            var rows = new List<SimilarityRow>();
            if (termCount == 0)
                termCount = TERM_COUNT;
            //получим список термов для каждого документа
            var doc2Terms = GetDocumentShingle(docs, termCount);
            var doc2Similarity = new Dictionary<string, int>();
            //на случай, если ключевых слов меньше, чем задано
            var minSize = doc2Terms.Values.Min(x => x.Count);

            //if (minSize < termCount)
            //{
            //    termCount = minSize;
            //    foreach (var e in doc2Terms)
            //    {
            //        List<String> lst = e.Value;
            //        if (lst.Count() > termCount)
            //        {
            //            lst = lst.GetRange(0, termCount);
            //            doc2Terms.Add(e.Key, lst);
            //        }
            //    }
            //}
            for (var i = 0; i < docs.Count(); i++)
            {
                var doc = docs[i];
                var row = new SimilarityRow();
                row.Doc = doc;
                //список термов для данного документа
                var terms = doc2Terms[row.Doc.DocIndex];
                var arrX = Initionalize(termCount);
                //сравнение с другими документами
                for (var j = 0; j < docs.Count(); j++)
                {
                    //ид документа
                    var doc2 = docs[j];
                    //набор термов для документа, с которым сравниваем
                    var terms2 = doc2Terms[doc2.DocIndex];
                    //схожесть одинаковых документов можно не считать. всегда = 100
                    if (row.Doc.Equals(doc2))
                    {
                        //сам с собой можно не считать. всегда 100
                        row.Similarity.Add(doc2, 100);
                        continue;
                    }

                    //уже считали раньше
                    if (doc2Similarity.ContainsKey(GetUniqueKey(i, j)))
                    {
                        row.Similarity.Add(doc2, doc2Similarity[GetUniqueKey(i, j)]);
                        continue;
                    }

                    var arrY = new int[termCount];
                    //если i-е слово из N - списка присутствует в документе,
                    //то значением i - го элемента образа документа считается 1, в противном случае — 0.
                    foreach (var term in terms)
                        arrY[terms.IndexOf(term)] = terms2.Contains(term) ? 1 : 0;
                    if (arrY.Length < termCount)
                        for (var k = arrY.Length; k < termCount; k++)
                            arrY[k] = 0;
                    //считаем коэффициент схожести
                    var coeff = GetSimilarityCoefficient(arrX, arrY);
                    //добавляем в матрицу
                    row.Similarity.Add(doc2, coeff);
                    doc2Similarity.Add(GetUniqueKey(i, j), coeff);
                }

                rows.Add(row);
            }

            return rows;
        }

        protected Dictionary<string, List<string>> GetDocumentShingle(List<Doc> docs, int termCount)
        {
            var doc2term = new Dictionary<string, List<string>>();

            //List<string> stopList = ClusteringFactory.GetMyStem().Parse(MyStem.MYSTEM_DIR + Path.DirectorySeparatorChar + "stop-list.txt");

            foreach (var doc in docs)
            {
                var docIndex = doc.DocIndex;

                var terms = ClusteringFactory.GetMyStem().Parse(doc.Path);

                var countMap = new Dictionary<string, int>();

                for (var ind = 0; ind < terms.Count; ++ind)
                {
                    var term = terms[ind];
                    if (term.Contains("??"))
                        term = term.Replace("??", "");

                    if (term.Length < 4)
                        continue;
                    //исключим стоп-слова,l
                    //if (stopList.Contains(term))
                    //    continue;
                    //подсчитать количество вхождений каждого слова

                    if (!countMap.Keys.Contains(term))
                    {
                        var count = terms.FindAll(x => x == term).Count;
                        countMap.Add(term, count);
                    }
                }

                var sortedMap = new Dictionary<string, int>();
                foreach (var entry in countMap)
                    if (entry.Value > 2)
                        sortedMap.Add(entry.Key, entry.Value);

                //сортировка по количеству выхождений
                var resultMap = new Dictionary<string, int>();

                foreach (var map in sortedMap.OrderBy(x => x.Value)) resultMap.Add(map.Key, map.Value);

                var resTerms = resultMap.Keys.Count() <= termCount
                    ? new List<string>(resultMap.Keys)
                    : new List<string>(resultMap.Keys).GetRange(0, termCount);

                if (!doc2term.ContainsKey(docIndex)) doc2term.Add(docIndex, resTerms);
            }

            return doc2term;
        }

        private string GetUniqueKey(int i, int j)
        {
            return i > j ? $"key:{j}_{i}" : $"key:{i}_{j}";
        }

        private int[] Initionalize(int termCount)
        {
            var arr = new int[termCount];
            for (var i = 0; i < arr.Length; i++)
                arr[i] = 1;
            return arr;
        }

        private int GetSimilarityCoefficient(int[] arrX, int[] arrY)
        {
            //сумма произведений эл-тов массивов X и Y
            double XY = 0;
            for (var i = 0; i < arrX.Length; i++)
                XY += arrX[i] * arrY[i];

            //сумма квадратов эл-тов массива X
            double X2 = 0;
            for (var i = 0; i < arrX.Length; i++)
                X2 += arrX[i] * arrX[i];

            //сумма квадратов эл-тов массива Y
            double Y2 = 0;
            for (var i = 0; i < arrY.Length; i++)
                Y2 += arrY[i] * arrY[i];

            if (X2 == 0 || Y2 == 0)
                return 0;
            //произведение эл-тов массивов X и Y делим на произведение корней сумм квадратов эл-тов массивов X и Y
            var rez = XY / (Math.Sqrt(X2) * Math.Sqrt(Y2));
            //приводим к целочисленному типу
            return (int) Math.Round(rez * 100);
        }
    }
}