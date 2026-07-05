using System.Collections.Concurrent;
using Minisplitwise.Domain.Interfaces;
using Minisplitwise.Application.Members;
using Minisplitwise.Domain.Entities;

namespace Minisplitwise.Infrastructure.Data.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly ConcurrentDictionary<Guid, Member> _members = new();

    public async Task<Member> CreateMemberAsync(Member member, CancellationToken cancellationToken = default)
    {
        await Task.FromResult(_members.TryAdd(member.Id, member));

        return member;
    }

    public async Task<List<Member>> GetAllMembersAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_members.Values.ToList());
    }
}