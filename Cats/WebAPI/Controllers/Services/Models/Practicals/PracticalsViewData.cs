using System.Collections.Generic;
using Application.Core;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Practicals
{
    public class PracticalsViewData
    {
        private readonly LazyDependency<IFilesManagementService> filesManagementService =
            new LazyDependency<IFilesManagementService>();

        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public PracticalsViewData(Practical practical)
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

        public IFilesManagementService FilesManagementService => filesManagementService.Value;

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        public int Order { get; set; }

        public string ShortName { get; set; }

        public int PracticalId { get; set; }

        public int SubjectId { get; set; }

        public string Theme { get; set; }

        public int Duration { get; set; }

        public string PathFile { get; set; }

        public IList<Attachment> Attachments { get; set; }
    }
}