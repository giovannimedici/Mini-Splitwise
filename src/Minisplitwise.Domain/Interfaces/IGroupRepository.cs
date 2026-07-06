using Minisplitwise.Domain.Entities;

namespace Minisplitwise.Domain.Interfaces;

public interface IGroupRepository
{
    Task<Group> CreateGroupAsync(Group group, CancellationToken cancellationToken = default);
    Task<Group> AddMemberToGroupAsync(Group group, Member member, CancellationToken cancellationToken = default);
    Task<Group> GetGroupByIdAsync(Guid groupId, CancellationToken cancellationToken = default);
}