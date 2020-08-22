using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Application.Core;
using Application.Infrastructure.ConceptManagement;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAPI.ViewModels.ComplexMaterialsViewModel
{
    public class AddOrEditConceptViewModel : AddOrEditRootConceptViewModel
    {
        private readonly LazyDependency<IFilesManagementService> _filesManagementService =
            new LazyDependency<IFilesManagementService>();

        public AddOrEditConceptViewModel(int currentAuthorId, int id)
            : base(currentAuthorId, id)
        {
            if (!IsNew())
            {
                Container = SourceConcept.Container;
                if (!string.IsNullOrEmpty(Container))
                    Attachments = FilesManagementService.GetAttachments(Container);
                ParentId = SourceConcept.ParentId.GetValueOrDefault();
                IsGroup = SourceConcept.IsGroup;
                Prev = SourceConcept.PrevConcept;
                Next = SourceConcept.NextConcept;
            }


            //Attachments = new List<Attachment>();
        }

        public AddOrEditConceptViewModel(int currentAuthorId, int id, int parentId)
            : this(currentAuthorId, id)
        {
            ParentId = parentId;

            if (!IsNew()) return;

            var c = ConceptManagementService.GetById(ParentId);
            SelectedSubjectId = c.SubjectId;
        }

        public AddOrEditConceptViewModel()
        {
        }

        public IFilesManagementService FilesManagementService => _filesManagementService.Value;


        public string Container { get; set; }
        public string FileData { get; set; }
        public bool IsGroup { get; set; }

        [DisplayName("Родительский элемент")]
        public int ParentId { get; set; }

        protected IList<Attachment> Attachments { get; set; }

        public string GetRelativePathForActiveAttachment()
        {
            var att = Attachments.FirstOrDefault();
            if (att == null)
                return string.Empty;
            var uploadFolder = "UploadedFiles";
            return $"/{uploadFolder}/{att.PathName}/{att.FileName}";
        }

        public void SetAttachments(IList<Attachment> attachments)
        {
            Attachments = attachments;
        }

        public IList<Attachment> GetAttachments()
        {
            return Attachments ?? new List<Attachment>();
        }

        public override void Save()
        {
            InitSourceConcept();
            if (!IsGroup)
                ConceptManagementService.SaveConcept(SourceConcept, GetAttachments());
            else
                ConceptManagementService.SaveConcept(SourceConcept);
        }

        private void InitSourceConcept()
        {
            SourceConcept = new Concept
            {
                Id = Id,
                Name = Name,
                Container = Container,
                IsGroup = IsGroup,
                Published = Published,
                ReadOnly = ReadOnly,
                SubjectId = SelectedSubjectId,
                UserId = Author,
                ParentId = ParentId,
                PrevConcept = Prev,
                NextConcept = Next
            };
        }
    }


    public class AddOrEditRootConceptViewModel
    {
        private readonly LazyDependency<IConceptManagementService> _conceptManagementService =
            new LazyDependency<IConceptManagementService>();

        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();


        private Concept _sourceConcept;

        public AddOrEditRootConceptViewModel(int currentAuthorId, int id)
        {
            Author = currentAuthorId;
            Id = id;
            if (!IsNew())
            {
                SelectedSubjectId = SourceConcept.SubjectId;
                Name = SourceConcept.Name;
                Published = SourceConcept.Published;
                ReadOnly = SourceConcept.ReadOnly;
                SubjectName = SourceConcept.Subject.Name;
            }
        }

        public AddOrEditRootConceptViewModel()
        {
        }

        [DisplayName("Следующий")] public int? Next { get; set; }

        [DisplayName("Предыдущий")] public int? Prev { get; set; }

        public string SubjectName { get; set; }

        public IConceptManagementService ConceptManagementService => _conceptManagementService.Value;

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Название обязательно для заполнения")]
        [StringLength(250, ErrorMessage = "Название должно быть не менее 3 символов и не более 250.",
            MinimumLength = 3)]
        [DataType(DataType.Text)]
        [DisplayName("Название")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле Предмет обязательно для заполнения")]
        [DisplayName("Предмет")]
        public int SelectedSubjectId { get; set; }

        public int Author { get; set; }

        public bool ReadOnly { get; set; }

        [DisplayName("Опубликован")] public bool Published { get; set; }

        protected Concept SourceConcept
        {
            get
            {
                if (IsNew()) return _sourceConcept;

                return _sourceConcept ??= ConceptManagementService.GetById(Id);
            }
            set => _sourceConcept = value;
        }

        public static string GetFriendlyDisplayName(string name, bool published)
        {
            return $"{name} ({(published ? "опубликован" : "неопубликован")})";
        }

        public IList<SelectListItem> GetBroElements(int author, int parentId)
        {
            var brothers = ConceptManagementService.GetElementsByParentId(author, parentId).Where(c => c.Id != Id);
            return brothers.Select(b => new SelectListItem
            {
                Text = b.Name,
                Value = b.Id.ToString(CultureInfo.InvariantCulture),
                Selected = !IsNew() && b.Id == Id
            }).ToList();
        }

        public IList<SelectListItem> GetSubjects(int currentAuthorId)
        {
            var currentSubjects = SubjectManagementService.GetUserSubjects(currentAuthorId).Where(e => !e.IsArchive);

            return currentSubjects.Select(sub => new SelectListItem
            {
                Text = sub.Name,
                Value = sub.Id.ToString(CultureInfo.InvariantCulture),
                Selected = sub.Id == SelectedSubjectId
            }).ToList();
        }

        public bool IsNew()
        {
            return Id == 0;
        }

        public virtual void Save()
        {
            if (IsNew())
                ConceptManagementService.CreateRootConcept(Name, Author, SelectedSubjectId);
            else
                ConceptManagementService.UpdateRootConcept(Id, Name, Published);
        }
    }
}