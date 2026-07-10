using Minisplitwise.Application.Members;

namespace Minisplitwise.Application.Payments;

public record PaymentResponseDto(
    Guid Id,
    decimal Amount,
    MemberDto WhoPays,
    MemberDto WhoReceives
);