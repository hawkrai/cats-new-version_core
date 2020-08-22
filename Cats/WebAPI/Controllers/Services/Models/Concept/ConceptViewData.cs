using System;
using System.Collections.Generic;
using System.Linq;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Concept
{
    public class ConceptViewData
    {
        private ICollection<ConceptViewData> _childrens;

        public ConceptViewData(LMP.Models.Concept concept)
        {
            Id = concept.Id;
            Name = concept.Name;
            ShortName = concept.Name.Length <= 20 ? concept.Name : $"{concept.Name.Substring(0, 20)}...";
            Container = concept.Container;
            ParentId = concept.ParentId.GetValueOrDefault();
            IsGroup = concept.IsGroup;
            Published = concept.Published;
            //Published = (concept.IsGroup && concept.Children.Any() && concept.Children.All(c => c.Published)) || (!concept.IsGroup && concept.Published);
            ReadOnly = concept.ReadOnly;
            HasData = !string.IsNullOrEmpty(Container);
            Prev = concept.PrevConcept;
            Next = concept.NextConcept;
            SubjectName = concept.Subject.Name;
        }

        public ConceptViewData(LMP.Models.Concept concept, bool buildTree)
            : this(concept)
        {
            if (!buildTree) return;
            Children = new List<ConceptViewData>();
            InitTree(concept, concept.Children);
        }

        public ConceptViewData(LMP.Models.Concept concept, bool buildTree,
            Func<LMP.Models.Concept, bool> filterFirstLevelChildren)
            : this(concept)
        {
            if (!buildTree) return;
            Children = new List<ConceptViewData>();
            InitTree(concept,
                filterFirstLevelChildren == null
                    ? concept.Children
                    : concept.Children.Where(filterFirstLevelChildren).ToList());
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string SubjectName { get; set; }

        public string Container { get; set; }

        public int ParentId { get; set; }

        public bool IsGroup { get; set; }

        public bool Published { get; set; }

        public bool ReadOnly { get; set; }

        public bool HasData { get; set; }

        public int? Next { get; set; }

        public int? Prev { get; set; }

        public ICollection<ConceptViewData> Children
        {
            get => _childrens.SortDoubleLinkedList();
            set => _childrens = value;
        }

        private void InitTree(LMP.Models.Concept concept, ICollection<LMP.Models.Concept> ch)
        {
            if (ch != null && ch.Any())
                Children = ch.Select(c => new ConceptViewData(c, true)).ToList();
            else
                return;
        }
    }

    public class AttachViewData
    {
        public AttachViewData(int id, string name, Attachment att)
        {
            Id = id;
            Name = name;
            HasData = id > 0;
            if (att == null) return;
            FilePath = att.PathName;
            FileName = att.FileName;
            FullPath = GetFilePath();
            HasAttach = true;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public string FullPath { get; set; }

        public bool HasData { get; set; }

        public bool HasAttach { get; set; }

        private string GetFilePath()
        {
            var uploadFolder = "UploadedFiles";
            return $"/{uploadFolder}/{FilePath}/{FileName}";
        }
    }

    public static class ConceptViewDataExtension
    {
        public static List<ConceptViewData> SortDoubleLinkedList(this IEnumerable<ConceptViewData> source)
        {
            var checkCount = 0;
            var res = new List<ConceptViewData>();
            if (source == null)
                return res;
            var first = source.FirstOrDefault(s => s.Prev == null);
            if (first == null)
            {
                res.AddRange(source);
                return res;
            }

            res.Add(first);
            var next = source.FirstOrDefault(s => s.Id == first.Next.GetValueOrDefault(-1));
            if (next == null)
            {
                res.AddRange(source.Where(i => res.All(r => r.Id != i.Id)));
                return res;
            }

            res.Add(next);
            first = next;
            while (next != null && checkCount < 1000)
            {
                next = source.FirstOrDefault(s => s.Id == first.Next.GetValueOrDefault());
                if (next != null)
                    res.Add(next);
                first = next;
                checkCount++;
            }

            return res;
        }
    }
}