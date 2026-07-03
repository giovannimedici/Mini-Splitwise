using Minisplitwise.Application.Members;

namespace Minisplitwise.Application.Services.Interfaces;

public interface IMemberService
{
    Task<MemberResponseDto> CreateMemberAsync(MemberRequestDto memberRequestDto, CancellationToken cancellationToken = default);
}