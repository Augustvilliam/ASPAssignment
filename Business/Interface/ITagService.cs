using Business.Dtos;

namespace Business.Interface
{
    public interface ITagService
    {
        Task<IEnumerable<MemberDto>> SearchMembersAsync(string term);
        Task<IEnumerable<ProjectDto>> SearchProjectsAsync(string term);
    }
}