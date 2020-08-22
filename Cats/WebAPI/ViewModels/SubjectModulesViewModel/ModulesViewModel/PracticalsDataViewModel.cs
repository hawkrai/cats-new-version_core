using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using Application.Core;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;

namespace WebAPI.ViewModels.SubjectModulesViewModel.ModulesViewModel
{
    public class PracticalsDataViewModel
    {
        private readonly LazyDependency<IFilesManagementService> _filesManagementService =
            new LazyDependency<IFilesManagementService>();

        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public PracticalsDataViewModel()
        {
        }

        public PracticalsDataViewModel(Practical practical)
        {
            Theme = practical.Theme;
            PracticalId = practical.Id;
            Duration = practical.Duration;
            SubjectId = practical.SubjectId;
            Order = practical.Order;
            PathFile = practical.Attachments;
            ShortName = practical.ShortName;
            Attachments = FilesManagementService.GetAttachments(practical.Attachments);
        }

        public PracticalsDataViewModel(int id, int subjectId)
        {
            SubjectId = subjectId;
            Attachments = new List<Attachment>();

            if (id == 0) return;

            var practicals = SubjectManagementService.GetPractical(id);
            Order = practicals.Order;
            Theme = practicals.Theme;
            Duration = practicals.Duration;
            PracticalId = id;
            ShortName = practicals.ShortName;
            PathFile = practicals.Attachments;
            Attachments = FilesManagementService.GetAttachments(practicals.Attachments);
        }

        public IFilesManagementService FilesManagementService => _filesManagementService.Value;

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        public int Order { get; set; }

        public int PracticalId { get; set; }

        public int SubjectId { get; set; }

        [Display(Name = "Название практического занятия")]
        public string Theme { get; set; }

        [Display(Name = "Короткое название")]
        public string ShortName { get; set; }

        [Display(Name = "Количество часов (1-99)")]
        public int Duration { get; set; }

        public string PathFile { get; set; }

        public IList<Attachment> Attachments { get; set; }

        public bool Delete()
        {
            //SubjectManagementService.DeleteNews(new SubjectNews { SubjectId = SubjectId, Id = NewsId });
            return true;
        }

        public bool Save(string attachmentsJson)
        {
            var attachments = JsonSerializer.Deserialize<List<Attachment>>(attachmentsJson).ToList();
            
            SubjectManagementService.SavePractical(new Practical
            {
                SubjectId = SubjectId,
                Duration = Duration,
                Theme = Theme,
                Order = 0,
                ShortName = ShortName,
                Attachments = PathFile,
                Id = PracticalId
            }, attachments, /*todo #auth WebSecurity.CurrentUserId*/2);
            return true;
        }
    }
}