using System.Collections.Generic;
using Application.Core;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;

namespace WebAPI.Controllers.Services.Models.Labs
{
    public class LabsViewData
    {
        private readonly LazyDependency<IFilesManagementService> filesManagementService =
            new LazyDependency<IFilesManagementService>();

        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public LabsViewData()
        {
        }

        public LabsViewData(LMP.Models.Labs labs)
        {
            Theme = labs.Theme;
            LabId = labs.Id;
            Duration = labs.Duration;
            SubjectId = labs.SubjectId;
            Order = labs.Order;
            PathFile = labs.Attachments;
            ShortName = labs.ShortName;
            Attachments = FilesManagementService.GetAttachments(labs.Attachments);
        }

        public IFilesManagementService FilesManagementService => filesManagementService.Value;

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        public int SubGroup { get; set; }

        public int Order { get; set; }

        public string ShortName { get; set; }

        public int LabId { get; set; }

        public int SubjectId { get; set; }

        public string Theme { get; set; }

        public int Duration { get; set; }

        public string PathFile { get; set; }
        
        public IList<Attachment> Attachments { get; set; }

        public List<ScheduleProtectionLab> ScheduleProtectionLabsRecomend { get; set; }
    }
}