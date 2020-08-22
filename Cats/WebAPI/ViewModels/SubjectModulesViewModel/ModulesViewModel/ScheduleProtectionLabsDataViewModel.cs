using System.Collections.Generic;
using Application.Core;
using Application.Infrastructure.FilesManagement;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;

namespace WebAPI.ViewModels.SubjectModulesViewModel.ModulesViewModel
{
    public class ScheduleProtectionLabsDataViewModel
    {
        private readonly LazyDependency<IFilesManagementService> _filesManagementService =
            new LazyDependency<IFilesManagementService>();

        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ScheduleProtectionLabsDataViewModel()
        {
        }

        public ScheduleProtectionLabsDataViewModel(Labs lab, int subGroupId)
        {
            SuGroupId = subGroupId;
            LabId = lab.Id;
            Order = lab.Order;
            Name = lab.Theme;
        }

        public ScheduleProtectionLabsDataViewModel(int id, int subGroupId)
        {
        }

        public IFilesManagementService FilesManagementService => _filesManagementService.Value;

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        public string Name { get; set; }

        public int Order { get; set; }

        public List<ScheduleProtectionLabs> ScheduleProtection { get; set; }

        public int SuGroupId { get; set; }

        public int LabId { get; set; }
    }
}