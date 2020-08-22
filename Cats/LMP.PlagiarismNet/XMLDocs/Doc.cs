namespace LMP.PlagiarismNet.XMLDocs
{
    public class Doc
    {
        public string Path { get; set; }

        public string FileName { get; set; }

        public string DocIndex { get; set; }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var doc = (Doc) obj;
            return DocIndex == null || (DocIndex?.Equals(doc.DocIndex) ?? false);
        }

        public override int GetHashCode()
        {
            var result = DocIndex != null ? DocIndex.GetHashCode() : 0;
            return result;
        }
    }
}