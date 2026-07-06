using System.Collections.Concurrent;
using Minisplitwise.Domain.Interfaces;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;

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

    public async Task<List<Member>> GetMembersByIdsAsync(List<Guid> memberIds, CancellationToken cancellationToken = default)
    {
        var missingIds = memberIds.Where(id => !_members.ContainsKey(id)).ToList();

        if (missingIds.Count > 0)
        {
            throw new NotFoundException($"Members not found: {string.Join(", ", missingIds)}");
        }

        return await Task.FromResult(memberIds.Select(id => _members[id]).ToList());
    }

    public async Task<Member> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        if(!_members.TryGetValue(memberId, out var member))
        {
            throw new NotFoundException("Member not found.");
        }

        return await Task.FromResult(member);
    }
}