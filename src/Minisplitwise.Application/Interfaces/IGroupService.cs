using Minisplitwise.Application.Groups;

namespace Minisplitwise.Application.Interfaces;

public interface IGroupService
{
    Task<GroupResponseDto> CreateGroupAsync(GroupRequestDto groupRequestDto, CancellationToken cancellationToken = default);
    Task<GroupResponseDto> AddMemberToGroupAsync(AddMemberRequestDto addMemberRequestDto, CancellationToken cancellationToken = default);
}