using LMP.Models;

namespace WebAPI.Controllers.Services.Models.News
{
    public class NewsViewData
    {
        public NewsViewData()
        {
        }

        public NewsViewData(SubjectNews news)
        {
            Body = news.Body;
            NewsId = news.Id;
            Title = news.Title;
            SubjectId = news.SubjectId;
            DateCreate = news.EditDate.ToShortDateString();
            Disabled = news.Disabled;
        }

        public int NewsId { get; set; }

        public int SubjectId { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public string DateCreate { get; set; }

        public bool Disabled { get; set; }
    }
}