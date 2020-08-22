using System;
using System.Linq;
using Application.Core;
using Application.Infrastructure.SubjectManagement;
using LMP.Models;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Services.Models.News;

namespace WebAPI.Controllers.Services.News
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class NewsServiceController : ApiRoutedController
    {
        private readonly LazyDependency<ISubjectManagementService> subjectManagementService =
            new LazyDependency<ISubjectManagementService>();

        public ISubjectManagementService SubjectManagementService => subjectManagementService.Value;

        [HttpGet("GetNews/{subjectId}")]
        public IActionResult GetNews(string subjectId)
        {
            try
            {
                var model = SubjectManagementService.GetSubject(int.Parse(subjectId)).SubjectNewses
                    .OrderByDescending(e => e.EditDate).Select(e => new NewsViewData(e)).ToList();

                return Ok(model);
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("DisableNews")]
        public IActionResult DisableNews(string subjectId)
        {
            try
            {
                SubjectManagementService.DisableNews(int.Parse(subjectId), true);

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("EnableNews")]
        public IActionResult EnableNews(string subjectId)
        {
            try
            {
                SubjectManagementService.DisableNews(int.Parse(subjectId), false);

                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpPost("Save")]
        public IActionResult Save(string subjectId, string id, string title, string body, bool disabled, bool isOldDate)
        {
            try
            {
                var newsIds = string.IsNullOrEmpty(id) ? 0 : int.Parse(id);
                var date = DateTime.Now;

                if (newsIds != 0 && isOldDate || newsIds != 0 && disabled)
                    date = SubjectManagementService.GetNews(newsIds, int.Parse(subjectId)).EditDate;
                else if (newsIds != 0 && !disabled)
                    if (SubjectManagementService.GetNews(newsIds, int.Parse(subjectId)).Disabled)
                        date = DateTime.Now;

                var model = new SubjectNews
                {
                    Id = newsIds,
                    SubjectId = int.Parse(subjectId),
                    Body = body,
                    EditDate = date,
                    Title = title,
                    Disabled = disabled
                };
                SubjectManagementService.SaveNews(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(string id, string subjectId)
        {
            try
            {
                var model = new SubjectNews
                {
                    Id = string.IsNullOrEmpty(id) ? 0 : int.Parse(id),
                    SubjectId = int.Parse(subjectId)
                };
                SubjectManagementService.DeleteNews(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }
    }
}