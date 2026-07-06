using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;

namespace MiniSplitwise.Tests.Domain;

public class GroupTests
{
    private static Member CreateMember(string name = "John", string email = "john@example.com")
        => Member.Create(name, email, new DateTime(1990, 1, 1));

    private static List<Member> CreateMembers(int count)
        => Enumerable.Range(1, count)
            .Select(i => CreateMember($"Member {i}", $"member{i}@example.com"))
            .ToList();

    [Fact]
    public void Create_WithValidData_ReturnsGroup()
    {
        var members = CreateMembers(2);

        var group = Group.Create("Trip to Paris", members);

        Assert.NotEqual(Guid.Empty, group.Id);
        Assert.Equal("Trip to Paris", group.Name);
        Assert.Equal(members, group.Members);
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var first = Group.Create("Group A", CreateMembers(2));
        var second = Group.Create("Group B", CreateMembers(2));

        Assert.NotEqual(first.Id, second.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithInvalidName_ThrowsDomainException(string? name)
    {
        var exception = Assert.Throws<DomainException>(() =>
            Group.Create(name!, CreateMembers(2)));

        Assert.Equal("Name is required", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Create_WithFewerThanTwoMembers_ThrowsDomainException(int memberCount)
    {
        var exception = Assert.Throws<DomainException>(() =>
            Group.Create("Trip to Paris", CreateMembers(memberCount)));

        Assert.Equal("A group must have at least 2 members", exception.Message);
    }

    [Fact]
    public void Create_WithMoreThanTwoMembers_ReturnsGroup()
    {
        var members = CreateMembers(5);

        var group = Group.Create("Big Group", members);

        Assert.Equal(5, group.Members.Count);
    }
}
