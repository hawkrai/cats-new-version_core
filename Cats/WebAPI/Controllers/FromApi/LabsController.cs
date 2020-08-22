using System;
using System.Linq;
using Application.Core;
using Application.Infrastructure.SubjectManagement;
using Microsoft.AspNetCore.Mvc;
using WebAPI.ViewModels.SubjectModulesViewModel.ModulesViewModel;

namespace WebAPI.Controllers.FromApi
{
    public class LabsController : ApiRoutedController
    {
        private readonly LazyDependency<ISubjectManagementService> _subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ISubjectManagementService SubjectManagementService => _subjectManagementService.Value;

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpGet]
        public IActionResult GetLabs(int subjectId)
        {
            try
            {
                var model = SubjectManagementService.GetSubject(subjectId).Labs.Select(e => new LabsDataViewModel(e));

                return Ok(model);
            }
            catch (Exception ex)
            {
                return ServerError500(new { ex.Message });
            }
        }
    }
}