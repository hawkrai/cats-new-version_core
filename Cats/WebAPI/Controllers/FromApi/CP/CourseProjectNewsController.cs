using System;
using Application.Core;
using Application.Infrastructure.CPManagement;
using LMP.Models.CP;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.FromApi.CP
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CourseProjectNewsController : ApiRoutedController
    {
        private readonly LazyDependency<ICPManagementService> _cpManagementService =
            new LazyDependency<ICPManagementService>();

        private ICPManagementService CpManagementService => _cpManagementService.Value;

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var result = CpManagementService.GetNewses( /*todo #auth WebSecurity.CurrentUserId*/2, id);
            return Ok(result);
        }

        //[System.Web.Http.HttpPost]
        //public System.Web.Mvc.JsonResult Save(string subjectId, string id, string title, string body, string disabled,
        //    string isOldDate, string pathFile, string attachments)
        //{
        //    var attachmentsModel = JsonConvert.DeserializeObject<List<Attachment>>(attachments).ToList();
        //    var subject = int.Parse(subjectId);
        //    try
        //    {
        //        CpManagementService.SaveNews(new Models.CourseProjectNews
        //        {
        //            SubjectId = subject,
        //            Id = int.Parse(id),
        //            Attachments = pathFile,
        //            Title = title,
        //            Body = body,
        //            Disabled = bool.Parse(disabled),
        //            EditDate = DateTime.Now,
        //        }, attachmentsModel, WebSecurity.CurrentUserId);
        //        return new System.Web.Mvc.JsonResult()
        //        {
        //            Data = new
        //            {
        //                Message = "Новость успешно сохранена",
        //                Error = false
        //            }
        //        };
        //    }
        //    catch (Exception)
        //    {
        //        return new System.Web.Mvc.JsonResult()
        //        {
        //            Data = new
        //            {
        //                Message = "Произошла ошибка при сохранении новости",
        //                Error = true
        //            }
        //        };
        //    }
        //}

        [HttpDelete]
        public IActionResult Delete([FromBody] CourseProjectNews deleteData)
        {
            try
            {
                CpManagementService.DeleteNews(deleteData);
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError500(ex.Message);
            }
        }
    }
}