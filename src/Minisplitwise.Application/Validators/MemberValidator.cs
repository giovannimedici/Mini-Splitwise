using FluentValidation;
using Minisplitwise.Application.Members;

namespace Minisplitwise.Application.Validators;

public class MemberValidator : AbstractValidator<MemberRequestDto>
{
    public MemberValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Name is required and must be less than 100 characters");
    
        RuleFor(m => m.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100)
            .WithMessage("Email is required and must be a valid email address and less than 100 characters");
            
        RuleFor(m => m.BirthDate)
            .NotEmpty()
            .LessThan(DateTime.Now)
            .WithMessage("Birth date is required and must be in the past");
    }
}