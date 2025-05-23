﻿using Business.Dtos;

namespace Business.Interface
{
    public interface IMemberService
    {
        Task<int> CountAsync();
        Task<bool> DeleteMemberAsync(string id);
        Task<List<MemberDto>> GetAllAdminsAsync();
        Task<IEnumerable<MemberDto>> GetAllMembersAsync();
        Task<MemberDto?> GetMemberByEmailAsync(string email);
        Task<MemberDto> GetMemberByIdAsync(string id);
        Task<MemberDto?> GetMemberForUpdateAsync(string id);
        Task<IEnumerable<MemberDto>> GetPagedAsync(int skip, int take);
        Task<bool> UpdateMemberAsync(MemberDto dto, string? imagePath);
    }
}