using Minisplitwise.Domain.Interfaces;
using Minisplitwise.Application.Members;
using Minisplitwise.Application.Interfaces;
using Minisplitwise.Domain.Entities;

namespace Minisplitwise.Application.Services;

public class MemberService(IMemberRepository memberRepository) : IMemberService
{
    public async Task<MemberResponseDto> CreateMemberAsync(MemberRequestDto memberRequestDto, CancellationToken cancellationToken = default)
    {
        Member member = Member.Create(memberRequestDto.Name, memberRequestDto.Email, memberRequestDto.BirthDate);

        var createdMember = await memberRepository.CreateMemberAsync(member, cancellationToken);
        
        return new MemberResponseDto(createdMember.Id, createdMember.Name, createdMember.Email, createdMember.BirthDate);
    }

    public async Task<List<MemberResponseDto>> GetAllMembersAsync(CancellationToken cancellationToken = default)
    {
        var members = await memberRepository.GetAllMembersAsync(cancellationToken);

        return members.Select(member => new MemberResponseDto(member.Id, member.Name, member.Email, member.BirthDate)).ToList();
    }
}