using FluentValidation;
using Minisplitwise.Application.Groups;

namespace Minisplitwise.Application.Validators;

public class GroupValidator : AbstractValidator<GroupRequestDto>
{
    public GroupValidator()
    {
        RuleFor(g => g.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required and must be less than 100 characters");
        
        RuleFor(g => g.MemberIds)
            .NotEmpty()
            .WithMessage("Member IDs are required");
    }
}