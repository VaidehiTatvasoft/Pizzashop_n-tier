public class PaginationViewModel
{
    public int CurrentOffset { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public List<int> PageSizeList { get; set; }
}