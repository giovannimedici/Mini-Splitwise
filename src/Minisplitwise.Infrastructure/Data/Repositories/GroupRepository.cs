using System.Collections.Concurrent;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;
using Minisplitwise.Domain.Interfaces;

namespace Minisplitwise.Infrastructure.Data.Repositories;

public class GroupRepository(MinisplitwiseDbContext context) : IGroupRepository
{
    public async Task<Group> CreateGroupAsync(Group group, CancellationToken cancellationToken = default)
    {
        await context.Groups.AddAsync(group, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return group;
    }

    public async Task<Group> AddMemberToGroupAsync(Group group, Member member, CancellationToken cancellationToken = default)
    {
        group.Members.Add(member);
        await context.SaveChangesAsync(cancellationToken);

        return group;
    }

    public async Task<Group> GetGroupByIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await context.Groups.FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken)
            ?? throw new NotFoundException("Group not found.");

        return group;
    }
}