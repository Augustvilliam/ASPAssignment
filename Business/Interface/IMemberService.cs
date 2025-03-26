using Domain.Models;

namespace Business.Interface
{
    public interface IMemberService
    {
        Task<bool> DeleteMemberAsync(string id);
        Task<IEnumerable<Member>> GetAllMembers();
        Task<Member?> GetMemberByIdAsync(string id);
        Task<MemberUpdateForm?> GetMemberForUpdateAsync(string id);
        Task<bool> UpdateMemberAsync(MemberUpdateForm form);

    }
}