using Minisplitwise.Domain.Exceptions;

namespace Minisplitwise.Domain.Entities;

public sealed class Group
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public List<Member> Members { get; private set; } = new();
    private Group() { }
    public static Group Create(string name, List<Member> members)
    {
        if(string.IsNullOrEmpty(name)) throw new DomainException("Name is required");

        if(members.Count < 2) throw new DomainException("A group must have at least 2 members");
        
        return new Group
        {
            Id = Guid.NewGuid(),
            Name = name,
            Members = members
        };
    }
}