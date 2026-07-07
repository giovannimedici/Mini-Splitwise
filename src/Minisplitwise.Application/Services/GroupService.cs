using Microsoft.Extensions.Logging;
using Minisplitwise.Application.Groups;
using Minisplitwise.Application.Interfaces;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;
using Minisplitwise.Domain.Interfaces;

namespace Minisplitwise.Application.Services;

public class GroupService(
    IGroupRepository groupRepository,
    IMemberRepository memberRepository,
    ILogger<GroupService> logger) : IGroupService
{
    public async Task<GroupResponseDto> CreateGroupAsync(GroupRequestDto groupRequestDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var members = await memberRepository.GetMembersByIdsAsync(groupRequestDto.MemberIds, cancellationToken);

            Group group = Group.Create(groupRequestDto.Name, members);

            var createdGroup = await groupRepository.CreateGroupAsync(group, cancellationToken);

            return new GroupResponseDto(createdGroup.Id, createdGroup.Name, members.Select(m => m.Id).ToList());
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex, "Error creating group. Message={Message}", ex.Message);
            throw;
        }
    }

    public async Task<GroupResponseDto> AddMemberToGroupAsync(AddMemberRequestDto addMemberRequestDto, CancellationToken cancellationToken = default)
    {
        var group = await groupRepository.GetGroupByIdAsync(addMemberRequestDto.GroupId, cancellationToken);

        var member = await memberRepository.GetMemberByIdAsync(addMemberRequestDto.MemberId, cancellationToken);

        group = await groupRepository.AddMemberToGroupAsync(group, member, cancellationToken);

        return new GroupResponseDto(group.Id, group.Name, group.Members.Select(m => m.Id).ToList());
    }
}
