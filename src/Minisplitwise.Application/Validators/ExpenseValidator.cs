using FluentValidation;
using Minisplitwise.Application.Expenses;

namespace Minisplitwise.Application.Validators;

public class ExpenseValidator : AbstractValidator<ExpenseRequestDto>
{
    public ExpenseValidator()
    {
        RuleFor(e => e.Amount)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("Amount is required and must be greater than 0");
        
        RuleFor(e => e.Description)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Description is required and must be less than 100 characters");
        
        RuleFor(e => e.Date)
            .NotEmpty()
            .LessThan(DateTime.Now)
            .WithMessage("Date is required and must be in the past");
        
        RuleFor(e => e.GroupId)
            .NotEmpty()
            .WithMessage("Group ID is required");
        
        RuleFor(e => e.PaidById)
            .NotEmpty()
            .WithMessage("PaidBy ID is required");
    }
}