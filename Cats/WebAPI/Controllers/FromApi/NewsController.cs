using System;
using System.Linq;
using Application.Core;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.ViewModels.SubjectModulesViewModel.ModulesViewModel;

namespace WebAPI.Controllers.FromApi
{
    public class NewsController : ApiRoutedController
    {
        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpGet]
        public IActionResult GetNews(int subjectId)
        {
            try
            {
                var model = SubjectManagementService.GetSubject(subjectId).SubjectNewses
                    .Select(e => new NewsDataViewModel(e)).ToList();

                return Ok(model);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpPost]
        public IActionResult Save(SubjectNews model)
        {
            try
            {
                model.EditDate = DateTime.Now;
                SubjectManagementService.SaveNews(model);
                return Accepted();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        /// <summary>
        ///     Not tested
        /// </summary>
        [HttpDelete]
        public IActionResult Delete(SubjectNews deleteData)
        {
            try
            {
                SubjectManagementService.DeleteNews(deleteData);
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }
    }
}