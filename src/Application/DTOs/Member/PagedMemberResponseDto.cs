namespace Application.DTOs.Member;

public class PagedMemberResponseDto
{
    public IEnumerable<MemberDto> Items { get; set; } = new List<MemberDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

