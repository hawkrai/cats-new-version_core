using Application.Core;
using Application.Infrastructure.SubjectManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models.Parental;

namespace WebAPI.Controllers.Services.Subjects
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = "lector")]
    public class SubjectsServiceController : ApiRoutedController
    {
        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        [HttpPatch("Subjects")]
        public IActionResult Update(SubjectViewData subject)
        {
            var loadedSubject = SubjectManagementService.GetSubject(subject.Id);
            loadedSubject.IsNeededCopyToBts = subject.IsNeededCopyToBts;
            SubjectManagementService.SaveSubject(loadedSubject);
            var subjectResult = new SubjectViewData(loadedSubject);
            return Ok(subjectResult);
        }
    }
}