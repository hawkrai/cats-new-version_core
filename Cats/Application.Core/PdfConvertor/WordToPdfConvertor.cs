using System;
using Spire.Doc;
using System.IO;

namespace Application.Core.PdfConvertor
{
    public class WordToPdfConvertor
    {
        private readonly string _storageRootTemp = /*todo #  ConfigurationManager.AppSettings["FileUploadPathTemp"]*/String.Empty;

        public string Convert(string sourceFile)
        {
            var document = new Document();
            document.LoadFromFile(sourceFile);

            var fileName = $"{Path.GetFileNameWithoutExtension(sourceFile)}.pdf";
            var fullPath = $"{_storageRootTemp}{fileName}";

            document.SaveToFile(fullPath, FileFormat.PDF);

            return fileName;
        }
    }
}
