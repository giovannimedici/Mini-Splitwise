using Minisplitwise.Application.Expenses;
using Minisplitwise.Application.Groups;
using Minisplitwise.Application.Interfaces;
using Minisplitwise.Application.Members;
using Minisplitwise.Application.Payments;
using Minisplitwise.Domain.Entities;
using Minisplitwise.Domain.Interfaces;

namespace Minisplitwise.Application.Services;

public class ExpenseService(
    IExpenseRepository expenseRepository,
    IGroupRepository groupRepository,
    IMemberRepository memberRepository) : IExpenseService
{
    public async Task<ExpenseResponseDto> CreateExpenseAsync(ExpenseRequestDto expenseRequestDto, CancellationToken cancellationToken = default)
    {
        var group = await groupRepository.GetGroupByIdAsync(expenseRequestDto.GroupId, cancellationToken);

        var paidBy = await memberRepository.GetMemberByIdAsync(expenseRequestDto.PaidById, cancellationToken);

        var sharedWith = await memberRepository.GetMembersByIdsAsync(expenseRequestDto.SharedWithIds, cancellationToken);

        Expense expense = Expense.Create(
            expenseRequestDto.Description,
            expenseRequestDto.Amount,
            expenseRequestDto.Date,
            group,
            paidBy,
            expenseRequestDto.ForEveryone,
            sharedWith
        );

        var createdExpense = await expenseRepository.CreateExpenseAsync(expense, cancellationToken);

        return new ExpenseResponseDto(
            createdExpense.Id,
            createdExpense.Description,
            createdExpense.Amount,
            createdExpense.Date,
            new GroupDto(createdExpense.Group.Id, createdExpense.Group.Name),
            new MemberDto(createdExpense.PaidBy.Id, createdExpense.PaidBy.Name),
            createdExpense.ForEveryone,
            createdExpense.SharedWith.Select(m => new MemberDto(m.Id, m.Name)).ToList()
        );
    }

    public async Task<List<ExpenseResponseDto>> GetExpensesByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var expenses = await expenseRepository.GetExpensesByGroupIdAsync(groupId, cancellationToken);

        return await Task.FromResult(expenses.Select(e => new ExpenseResponseDto(
            e.Id,
            e.Description,
            e.Amount,
            e.Date,
            new GroupDto(e.Group.Id, e.Group.Name),
            new MemberDto(e.PaidBy.Id, e.PaidBy.Name),
            e.ForEveryone,
            e.SharedWith.Select(m => new MemberDto(m.Id, m.Name)).ToList()
        )).ToList());
    }

    public async Task<List<ExpenseMemberResponseDto>> GetExpensesByMemberIdAsync(Guid memberId, Guid groupId, CancellationToken cancellationToken = default)
    {
        var expenses = await expenseRepository.GetExpensesByMemberIdAsync(memberId, groupId, cancellationToken);

        return await Task.FromResult(expenses.Select(e => new ExpenseMemberResponseDto(
            e.Id,
            e.Description,
            e.Amount / (e.SharedWith.Count + 1),
            e.Date,
            new MemberDto(memberId, e.SharedWith.First(m => m.Id == memberId).Name),
            new MemberDto(e.PaidBy.Id, e.PaidBy.Name)
        )).ToList());
    }

    public async Task<List<PaymentResponseDto>> CalculatePaymentsByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var expenses = await expenseRepository.GetExpensesByGroupIdAsync(groupId, cancellationToken);

        var payments = new List<PaymentResponseDto>();

        var group = await groupRepository.GetGroupByIdAsync(groupId, cancellationToken);

        var members = group.Members.ToList();

        foreach (var expense in expenses)
        {
            foreach (var sharedWith in expense.SharedWith)
            {
                payments.Add(new PaymentResponseDto(
                    expense.Id,
                    expense.Amount / (expense.SharedWith.Count + 1),
                    new MemberDto(sharedWith.Id, sharedWith.Name),
                    new MemberDto(expense.PaidBy.Id, expense.PaidBy.Name)
                ));
            }
        }

        payments = CalculatePayments(payments, members);

        return await Task.FromResult(payments);
    }

    private static List<PaymentResponseDto> CalculatePayments(List<PaymentResponseDto> payments, List<Member> members)
    {
        List<PaymentResponseDto> newPayments = new();
        Dictionary<MemberDto, decimal> memberBalances = new();

        foreach (var payment in payments)
        {
            if (!memberBalances.ContainsKey(payment.WhoPays)) memberBalances[payment.WhoPays] = 0;
            if (!memberBalances.ContainsKey(payment.WhoReceives)) memberBalances[payment.WhoReceives] = 0;

            memberBalances[payment.WhoPays] -= payment.Amount;
            memberBalances[payment.WhoReceives] += payment.Amount;
        }

        var whoPaysList = memberBalances.Where(x => x.Value < 0)
                                    .Select(x => new MemberBalance { Id = x.Key.Id, Name = x.Key.Name, Balance = x.Value })
                                    .OrderBy(x => x.Balance)
                                    .ToList();

        var whoReceivesList = memberBalances.Where(x => x.Value > 0)
                                        .Select(x => new MemberBalance { Id = x.Key.Id, Name = x.Key.Name, Balance = x.Value })
                                        .OrderBy(x => x.Balance)
                                        .ToList();

        int i = 0, j = 0;

        while (i < whoPaysList.Count && j < whoReceivesList.Count)
        {
            var whoPays = whoPaysList[i];
            var whoReceives = whoReceivesList[j];

            decimal valueTransfer = Math.Min(-whoPays.Balance, whoReceives.Balance);

            newPayments.Add(new PaymentResponseDto(
                Guid.NewGuid(),
                valueTransfer,
                new MemberDto(whoPays.Id, whoPays.Name),
                new MemberDto(whoReceives.Id, whoReceives.Name)
            ));

            whoPays.Balance += valueTransfer;
            whoReceives.Balance -= valueTransfer;

            if (Math.Abs(whoPays.Balance) < 0.01m) i++;
            if (Math.Abs(whoReceives.Balance) < 0.01m) j++;
        }

        return newPayments;
    }

    private class MemberBalance
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}