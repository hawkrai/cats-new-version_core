﻿using System;
using System.IO;
using System.Text;
using LMP.PlagiarismNet.Services;

namespace LMP.PlagiarismNet.Utils
{
    public class StringUtils
    {
        public static void Convert(string fileName)
        {
            TextReader fis = null;
            try
            {
                fis = new StreamReader(fileName);

                //POITextExtractor extractor;
                //if (fileName.ToLower().EndsWith(".docx"))
                //{
                //    org.apache.poi.xwpf.usermodel.XWPFDocument
                //        doc = new org.apache.poi.xwpf.usermodel.XWPFDocument(fis);
                //    extractor = new org.apache.poi.xwpf.extractor.XWPFWordExtractor(doc);
                //}
                //else
                //{
                //    org.apache.poi.hwpf.HWPFDocument doc = new org.apache.poi.hwpf.HWPFDocument(fis);
                //    extractor = new org.apache.poi.hwpf.extractor.WordExtractor(doc);
                //}

                var text = fis.ReadToEnd();

                var fil = File.Create(MyStem.MYSTEM_IN_DIR + GetFileName(fileName) + ".txt");
                using (var output = new StreamWriter(fil, Encoding.UTF8))
                {
                    output.Write(text);
                }
            }
            catch (FileNotFoundException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(e.StackTrace);
            }
            finally
            {
                if (fis != null)
                    try
                    {
                        fis.Close();
                    }
                    catch (IOException e)
                    {
                        Console.Error.WriteLine(e.StackTrace);
                    }
            }
        }

        public static string GetFileName(string path)
        {
            var startIndex = path.LastIndexOf(Path.DirectorySeparatorChar);
            var endIndex = path.LastIndexOf(".");

            var length = endIndex - startIndex;

            return startIndex != -1 && endIndex != -1 ? path.Substring(startIndex + 1, length) : path;
        }
    }
}