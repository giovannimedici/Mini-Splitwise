using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Exceptions;

namespace MiniSplitwise.Tests.Domain;

public class MemberTests
{
    [Fact]
    public void Create_WithValidData_ReturnsMember()
    {
        var birthDate = new DateTime(1990, 5, 20);

        var member = Member.Create("John Doe", "john@example.com", birthDate);

        Assert.NotEqual(Guid.Empty, member.Id);
        Assert.Equal("John Doe", member.Name);
        Assert.Equal("john@example.com", member.Email);
        Assert.Equal(birthDate, member.BirthDate);
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var birthDate = new DateTime(1990, 5, 20);

        var first = Member.Create("John", "john@example.com", birthDate);
        var second = Member.Create("Jane", "jane@example.com", birthDate);

        Assert.NotEqual(first.Id, second.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithInvalidName_ThrowsDomainException(string? name)
    {
        var exception = Assert.Throws<DomainException>(() =>
            Member.Create(name!, "john@example.com", new DateTime(1990, 5, 20)));

        Assert.Equal("Name is required", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithInvalidEmail_ThrowsDomainException(string? email)
    {
        var exception = Assert.Throws<DomainException>(() =>
            Member.Create("John Doe", email!, new DateTime(1990, 5, 20)));

        Assert.Equal("Email is required", exception.Message);
    }

    [Fact]
    public void Create_WithBirthDateInTheFuture_ThrowsDomainException()
    {
        var futureDate = DateTime.Now.AddDays(1);

        var exception = Assert.Throws<DomainException>(() =>
            Member.Create("John Doe", "john@example.com", futureDate));

        Assert.Equal("Birth date cannot be in the future", exception.Message);
    }

    [Fact]
    public void Create_WithBirthDateInThePast_DoesNotThrow()
    {
        var pastDate = DateTime.Now.AddYears(-30);

        var member = Member.Create("John Doe", "john@example.com", pastDate);

        Assert.Equal(pastDate, member.BirthDate);
    }
}
