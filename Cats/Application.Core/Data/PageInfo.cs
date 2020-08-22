namespace Application.Core.Data
{
    public class PageInfo : IPageInfo
    {
        public int PageSize { get; set; }

        public int PageNumber { get; set; }

	    public PageInfo()
	    {
		    PageNumber = 1;
	    }
    }
}
