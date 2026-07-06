using Minisplitwise.Application.Members;
using Minisplitwise.Application.Services;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;
using Minisplitwise.Domain.Interfaces;
using NSubstitute;

namespace MiniSplitwise.Tests.Application;

public class MemberServiceTests
{
    private readonly IMemberRepository _memberRepository = Substitute.For<IMemberRepository>();
    private readonly MemberService _service;

    public MemberServiceTests()
    {
        _service = new MemberService(_memberRepository);
    }

    [Fact]
    public async Task CreateMemberAsync_WithValidData_ReturnsMappedDto()
    {
        var request = new MemberRequestDto("John Doe", "john@example.com", new DateTime(1990, 5, 20));
        _memberRepository.CreateMemberAsync(Arg.Any<Member>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Member>());

        var response = await _service.CreateMemberAsync(request);

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("John Doe", response.Name);
        Assert.Equal("john@example.com", response.Email);
        Assert.Equal(new DateTime(1990, 5, 20), response.BirthDate);
    }

    [Fact]
    public async Task CreateMemberAsync_PassesCreatedMemberToRepository()
    {
        var request = new MemberRequestDto("John Doe", "john@example.com", new DateTime(1990, 5, 20));
        _memberRepository.CreateMemberAsync(Arg.Any<Member>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Member>());

        await _service.CreateMemberAsync(request);

        await _memberRepository.Received(1).CreateMemberAsync(
            Arg.Is<Member>(m => m.Name == "John Doe" && m.Email == "john@example.com"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateMemberAsync_WithInvalidData_ThrowsDomainExceptionAndDoesNotCallRepository()
    {
        var request = new MemberRequestDto("", "john@example.com", new DateTime(1990, 5, 20));

        await Assert.ThrowsAsync<DomainException>(() => _service.CreateMemberAsync(request));

        await _memberRepository.DidNotReceive().CreateMemberAsync(Arg.Any<Member>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllMembersAsync_ReturnsMappedDtos()
    {
        var members = new List<Member>
        {
            Member.Create("John", "john@example.com", new DateTime(1990, 1, 1)),
            Member.Create("Jane", "jane@example.com", new DateTime(1985, 6, 15))
        };
        _memberRepository.GetAllMembersAsync(Arg.Any<CancellationToken>()).Returns(members);

        var response = await _service.GetAllMembersAsync();

        Assert.Equal(2, response.Count);
        Assert.Equal(members[0].Id, response[0].Id);
        Assert.Equal("John", response[0].Name);
        Assert.Equal("john@example.com", response[0].Email);
        Assert.Equal(members[1].Id, response[1].Id);
        Assert.Equal("Jane", response[1].Name);
    }

    [Fact]
    public async Task GetAllMembersAsync_WhenNoMembers_ReturnsEmptyList()
    {
        _memberRepository.GetAllMembersAsync(Arg.Any<CancellationToken>()).Returns(new List<Member>());

        var response = await _service.GetAllMembersAsync();

        Assert.Empty(response);
    }
}
