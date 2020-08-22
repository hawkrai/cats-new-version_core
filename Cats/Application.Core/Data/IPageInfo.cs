namespace Application.Core.Data
{
    public interface IPageInfo
    {
        int PageSize { get; set; }

        int PageNumber { get; set; }
    }
}
