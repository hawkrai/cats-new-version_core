using System.Collections.Generic;

namespace WebAPI.Controllers.Services.Models.News
{
    public class NewsResult : ResultViewData
    {
        public List<NewsViewData> News { get; set; }
    }
}