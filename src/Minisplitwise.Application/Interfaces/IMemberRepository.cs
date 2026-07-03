using Minisplitwise.Application.Members;
using Minisplitwise.Domain.Entities;

namespace Minisplitwise.Application.Interfaces;

public interface IMemberRepository
{
    Task<Member> CreateMemberAsync(MemberRequestDto memberRequestDto, CancellationToken cancellationToken = default);
    Task<List<Member>> GetAllMembersAsync();
}