using Minisplitwise.Application.Members;

namespace Minisplitwise.Application.Interfaces;

public interface IMemberService
{
    Task<MemberResponseDto> CreateMemberAsync(MemberRequestDto memberRequestDto, CancellationToken cancellationToken = default);
    Task<List<MemberResponseDto>> GetAllMembersAsync(CancellationToken cancellationToken = default);
}