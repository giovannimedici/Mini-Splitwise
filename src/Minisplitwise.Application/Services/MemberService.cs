using Minisplitwise.Domain.Interfaces;
using Minisplitwise.Application.Members;
using Minisplitwise.Application.Services.Interfaces;
using Minisplitwise.Domain.Entities;

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
        Member member = Member.Create(memberRequestDto.Name, memberRequestDto.Email, memberRequestDto.BirthDate);

        var createdMember = await _memberRepository.CreateMemberAsync(member, cancellationToken);
        
        return new MemberResponseDto(createdMember.Id, createdMember.Name, createdMember.Email, createdMember.BirthDate);
    }

    public async Task<List<MemberResponseDto>> GetAllMembersAsync(CancellationToken cancellationToken = default)
    {
        var members = await _memberRepository.GetAllMembersAsync(cancellationToken);

        return members.Select(member => new MemberResponseDto(member.Id, member.Name, member.Email, member.BirthDate)).ToList();
    }
}