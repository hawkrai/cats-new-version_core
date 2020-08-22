using System.Collections.Generic;
using Application.Core;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Lectures
{
    public class LecturesViewData
    {
        private readonly LazyDependency<IFilesManagementService> filesManagementService =
            new LazyDependency<IFilesManagementService>();

        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public LecturesViewData(LMP.Models.Lectures lectures)
        {
            Theme = lectures.Theme;
            LecturesId = lectures.Id;
            Duration = lectures.Duration;
            SubjectId = lectures.SubjectId;
            Order = lectures.Order;
            PathFile = lectures.Attachments;
            Attachments = FilesManagementService.GetAttachments(lectures.Attachments);
        }

        public IFilesManagementService FilesManagementService => filesManagementService.Value;

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        public int Order { get; set; }

        public int LecturesId { get; set; }

        public int SubjectId { get; set; }

        public string Theme { get; set; }

        public int Duration { get; set; }

        public string PathFile { get; set; }

        public IList<Attachment> Attachments { get; set; }
    }
}