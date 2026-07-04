using Minisplitwise.Application.Interfaces;
using Minisplitwise.Application.Members;
using Minisplitwise.Application.Services.Interfaces;

namespace Minisplitwise.Application.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<MemberResponseDto> CreateMemberAsync(MemberRequestDto memberRequestDto, CancellationToken cancellationToken = default)
    {
        var member = await _memberRepository.CreateMemberAsync(memberRequestDto, cancellationToken);
        
        return new MemberResponseDto(member.Id, member.Name, member.Email, member.BirthDate);
    }

    public async Task<List<MemberResponseDto>> GetAllMembersAsync(CancellationToken cancellationToken = default)
    {
        var members = await _memberRepository.GetAllMembersAsync(cancellationToken);

        return members.Select(member => new MemberResponseDto(member.Id, member.Name, member.Email, member.BirthDate)).ToList();
    }
}