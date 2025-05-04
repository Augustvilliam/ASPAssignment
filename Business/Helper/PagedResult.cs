

namespace Business.Helper;
//generisk pagedresult för pagering av data
public class PagedResult<T> 
{
    public IEnumerable<T> Itemn { get; set; } = Enumerable.Empty<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }   
    public int TotalItems { get; set; } 
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
}
