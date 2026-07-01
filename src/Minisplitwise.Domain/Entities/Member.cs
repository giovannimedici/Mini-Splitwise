using Minisplitwise.Domain.Exceptions;

namespace Minisplitwise.Domain.Entities;

public sealed class Member{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public DateTime BirthDate { get; private set; }

    private Member() {}

    public static Member Create(string name, string email, DateTime birthDate){
        if(string.IsNullOrEmpty(name)) throw new DomainException("Name is required");

        if(string.IsNullOrEmpty(email)) throw new DomainException("Email is required");

        if(birthDate > DateTime.Now) throw new DomainException("Birth date cannot be in the future");
        
        return new Member{
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            BirthDate = birthDate
        };
    }
}