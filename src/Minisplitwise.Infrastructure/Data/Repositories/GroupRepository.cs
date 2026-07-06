using System.Collections.Concurrent;
using System.ComponentModel;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;
using Minisplitwise.Domain.Interfaces;

namespace Minisplitwise.Infrastructure.Data.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ConcurrentDictionary<Guid, Group> _groups = new();

    public async Task<Group> CreateGroupAsync(Group group, CancellationToken cancellationToken = default)
    {
        await Task.FromResult(_groups.TryAdd(group.Id, group));

        return group;
    }

    public async Task<Group> AddMemberToGroupAsync(Group group, Member member, CancellationToken cancellationToken = default)
    {
        group.Members.Add(member);

        return await Task.FromResult(group);
    }

    public async Task<Group> GetGroupByIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        if(!_groups.TryGetValue(groupId, out var group))
        {
            throw new NotFoundException("Group not found.");
        }

        return await Task.FromResult(group);
    }
}