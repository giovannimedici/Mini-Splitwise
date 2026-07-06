using Microsoft.Extensions.Logging.Abstractions;
using Minisplitwise.Application.Groups;
using Minisplitwise.Application.Services;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;
using Minisplitwise.Domain.Interfaces;
using NSubstitute;

namespace MiniSplitwise.Tests.Application;

public class GroupServiceTests
{
    private readonly IGroupRepository _groupRepository = Substitute.For<IGroupRepository>();
    private readonly IMemberRepository _memberRepository = Substitute.For<IMemberRepository>();
    private readonly GroupService _service;

    public GroupServiceTests()
    {
        _service = new GroupService(_groupRepository, _memberRepository, NullLogger<GroupService>.Instance);
    }

    private static List<Member> CreateMembers(int count)
        => Enumerable.Range(1, count)
            .Select(i => Member.Create($"Member {i}", $"member{i}@example.com", new DateTime(1990, 1, 1)))
            .ToList();

    [Fact]
    public async Task CreateGroupAsync_WithValidData_ReturnsMappedDto()
    {
        var members = CreateMembers(2);
        var memberIds = members.Select(m => m.Id).ToList();
        var request = new GroupRequestDto("Trip to Paris", memberIds);

        _memberRepository.GetMembersByIdsAsync(memberIds, Arg.Any<CancellationToken>()).Returns(members);
        _groupRepository.CreateGroupAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Group>());

        var response = await _service.CreateGroupAsync(request);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Trip to Paris", response.Name);
        Assert.Equal(memberIds, response.Members);
    }

    [Fact]
    public async Task CreateGroupAsync_PassesCreatedGroupToRepository()
    {
        var members = CreateMembers(3);
        var request = new GroupRequestDto("Big Group", members.Select(m => m.Id).ToList());

        _memberRepository.GetMembersByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(members);
        _groupRepository.CreateGroupAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Group>());

        await _service.CreateGroupAsync(request);

        await _groupRepository.Received(1).CreateGroupAsync(
            Arg.Is<Group>(g => g.Name == "Big Group" && g.Members.Count == 3),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateGroupAsync_WhenMembersNotFound_PropagatesNotFoundException()
    {
        var request = new GroupRequestDto("Trip to Paris", new List<Guid> { Guid.NewGuid(), Guid.NewGuid() });

        _memberRepository.GetMembersByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>())
            .Returns<Task<List<Member>>>(_ => throw new NotFoundException("Members not found"));

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateGroupAsync(request));

        Assert.Equal("Members not found", exception.Message);
        await _groupRepository.DidNotReceive().CreateGroupAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateGroupAsync_WithFewerThanTwoMembers_ThrowsDomainException()
    {
        var members = CreateMembers(1);
        var request = new GroupRequestDto("Solo Group", members.Select(m => m.Id).ToList());

        _memberRepository.GetMembersByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>()).Returns(members);

        await Assert.ThrowsAsync<DomainException>(() => _service.CreateGroupAsync(request));

        await _groupRepository.DidNotReceive().CreateGroupAsync(Arg.Any<Group>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddMemberToGroupAsync_ReturnsUpdatedGroupDto()
    {
        var members = CreateMembers(2);
        var group = Group.Create("Trip to Paris", members);
        var newMember = Member.Create("New Member", "new@example.com", new DateTime(1995, 3, 10));
        var updatedGroup = Group.Create("Trip to Paris", members.Append(newMember).ToList());
        var request = new AddMemberRequestDto(group.Id, newMember.Id);

        _groupRepository.GetGroupByIdAsync(group.Id, Arg.Any<CancellationToken>()).Returns(group);
        _memberRepository.GetMemberByIdAsync(newMember.Id, Arg.Any<CancellationToken>()).Returns(newMember);
        _groupRepository.AddMemberToGroupAsync(group, newMember, Arg.Any<CancellationToken>()).Returns(updatedGroup);

        var response = await _service.AddMemberToGroupAsync(request);

        Assert.Equal(updatedGroup.Id, response.Id);
        Assert.Equal("Trip to Paris", response.Name);
        Assert.Equal(3, response.Members.Count);
        Assert.Contains(newMember.Id, response.Members);
    }

    [Fact]
    public async Task AddMemberToGroupAsync_WhenGroupNotFound_PropagatesNotFoundException()
    {
        var request = new AddMemberRequestDto(Guid.NewGuid(), Guid.NewGuid());

        _groupRepository.GetGroupByIdAsync(request.GroupId, Arg.Any<CancellationToken>())
            .Returns<Task<Group>>(_ => throw new NotFoundException("Group not found"));

        await Assert.ThrowsAsync<NotFoundException>(() => _service.AddMemberToGroupAsync(request));

        await _groupRepository.DidNotReceive().AddMemberToGroupAsync(Arg.Any<Group>(), Arg.Any<Member>(), Arg.Any<CancellationToken>());
    }
}
