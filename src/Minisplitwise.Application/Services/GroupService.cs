using Microsoft.Extensions.Logging;
using Minisplitwise.Application.Groups;
using Minisplitwise.Application.Interfaces;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;
using Minisplitwise.Domain.Interfaces;

namespace Minisplitwise.Application.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<GroupService> _logger;

    public GroupService(IGroupRepository groupRepository, IMemberRepository memberRepository, ILogger<GroupService> logger)
    {
        _groupRepository = groupRepository;
        _memberRepository = memberRepository;   
        _logger = logger;
    }

    public async Task<GroupResponseDto> CreateGroupAsync(GroupRequestDto groupRequestDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var members = await _memberRepository.GetMembersByIdsAsync(groupRequestDto.MemberIds, cancellationToken);

            Group group = Group.Create(groupRequestDto.Name, members);

            var createdGroup = await _groupRepository.CreateGroupAsync(group, cancellationToken);

            return new GroupResponseDto(createdGroup.Id, createdGroup.Name, members.Select(m => m.Id).ToList());
        }
        catch (NotFoundException ex)
        {
            _logger.LogError(ex, "Error creating group. Message={Message}", ex.Message);
            throw;
        }
    }

    public async Task<GroupResponseDto> AddMemberToGroupAsync(AddMemberRequestDto addMemberRequestDto, CancellationToken cancellationToken = default)
    {
        var group = await _groupRepository.GetGroupByIdAsync(addMemberRequestDto.GroupId, cancellationToken);

        var member = await _memberRepository.GetMemberByIdAsync(addMemberRequestDto.MemberId, cancellationToken);

        group = await _groupRepository.AddMemberToGroupAsync(group, member, cancellationToken);

        return new GroupResponseDto(group.Id, group.Name, group.Members.Select(m => m.Id).ToList());
    }
}