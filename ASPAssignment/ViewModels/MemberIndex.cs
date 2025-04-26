using Business.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace ASPAssignment.ViewModels;

public class MemberIndex
{
    public IEnumerable<MemberDto> Items { get; set; } = Enumerable.Empty<MemberDto>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages =>
        (int)Math.Ceiling((double)TotalItems / PageSize);

}
