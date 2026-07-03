using System.ComponentModel;
using System.Runtime.CompilerServices;
using Minisplitwise.Application.Interfaces;
using Minisplitwise.Application.Members;
using Minisplitwise.Domain.Entities;

namespace Minisplitwise.Infrastructure.Data.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly Dictionary<Guid, Member> _members = new();
    public async Task<Member> CreateMemberAsync(MemberRequestDto memberRequestDto, CancellationToken cancellationToken = default)
    {
        Member member = Member.Create(memberRequestDto.Name, memberRequestDto.Email, memberRequestDto.BirthDate);

        _members[member.Id] = member;

        return await Task.FromResult(member);
    }
    public async Task<List<Member>> GetAllMembersAsync()
    {
        return await Task.FromResult(_members.Values.ToList());
    }
}