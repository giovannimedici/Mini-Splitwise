using Minisplitwise.Domain.Entities;

namespace Minisplitwise.Domain.Interfaces;

public interface IMemberRepository
{
    Task<Member> CreateMemberAsync(Member member, CancellationToken cancellationToken = default);
    Task<List<Member>> GetAllMembersAsync(CancellationToken cancellationToken = default);
}