using Minisplitwise.Application.Members;
using Minisplitwise.Application.Services.Interfaces;

namespace Minisplitwise.API.Endpoints;

public static class MemberEndpoints
{
    public static void MapMemberEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/members", CreateMemberAsync)
            .WithName("CreateMember")
            .WithTags("Members")
            .WithSummary("Create a new member")
            .WithDescription("Create a new member with the given name, email and birth date");
    }

    public static async Task<IResult> CreateMemberAsync(MemberRequestDto memberRequestDto, IMemberService memberService, CancellationToken cancellationToken)
    {
        var member = await memberService.CreateMemberAsync(memberRequestDto, cancellationToken);

        return Results.Ok(member);
    }
}