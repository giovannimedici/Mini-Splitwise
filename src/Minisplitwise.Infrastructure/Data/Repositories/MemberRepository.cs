using Minisplitwise.Domain.Interfaces;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Minisplitwise.Infrastructure.Data.Repositories;

public class MemberRepository(MinisplitwiseDbContext context) : IMemberRepository
{

    public async Task<Member> CreateMemberAsync(Member member, CancellationToken cancellationToken = default)
    {
        await context.Members.AddAsync(member, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return member;
    }

    public async Task<List<Member>> GetAllMembersAsync(CancellationToken cancellationToken = default)
    {
        return await context.Members.ToListAsync(cancellationToken);
    }

    public async Task<List<Member>> GetMembersByIdsAsync(List<Guid> memberIds, CancellationToken cancellationToken = default)
    {

        Console.WriteLine(memberIds.Count);
        Console.WriteLine(string.Join(", ", memberIds));
        Console.WriteLine("--------------------------------");

        var members = await context.Members
                                    .Where(m => memberIds.Contains(m.Id))
                                    .ToListAsync(cancellationToken);

        Console.WriteLine(memberIds.Count);
        Console.WriteLine(string.Join(", ", memberIds));

        if (members.Count != memberIds.Count)
        {
            var foundIds = members.Select(m => m.Id).ToList();
            var missingIds = memberIds.Where(id => !foundIds.Contains(id))
                                       .Select(id => id.ToString())
                                       .ToList();
            
            throw new NotFoundException($"Members not found: {string.Join(", ", missingIds)}");
        }

        return members;
    }

    public async Task<Member> GetMemberByIdAsync(Guid memberId, CancellationToken cancellationToken = default)
    {
        var member = await context.Members.FirstOrDefaultAsync(m => m.Id == memberId, cancellationToken)
            ?? throw new NotFoundException("Member not found.");

        return member;
    }
}