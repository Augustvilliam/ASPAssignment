using Business.Dtos;

namespace Business.Interface
{
    public interface IMemberService
    {
        Task<bool> DeleteMemberAsync(string id);
        Task<IEnumerable<MemberDto>> GetAllMembersAsync();
        Task<MemberDto?> GetMemberByEmailAsync(string email);
        Task<MemberDto> GetMemberByIdAsync(string id);
        Task<MemberDto?> GetMemberForUpdateAsync(string id);
        Task<bool> UpdateMemberAsync(MemberDto dto, string? imagePath);
    }
}