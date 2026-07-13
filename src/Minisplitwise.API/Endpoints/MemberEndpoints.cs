using Minisplitwise.Application.Members;
using Minisplitwise.Application.Interfaces;
using FluentValidation;

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
            
        app.MapGet("/members", GetAllMembersAsync)
            .WithName("GetAllMembers")
            .WithTags("Members")
            .WithSummary("Get all members")
            .WithDescription("Get all members");
    }

    public static async Task<IResult> CreateMemberAsync(
        MemberRequestDto memberRequestDto, 
        IMemberService memberService,
        IValidator<MemberRequestDto> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(memberRequestDto, cancellationToken);
        
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary(), "Validation error");
        }

        var member = await memberService.CreateMemberAsync(memberRequestDto, cancellationToken);

        return Results.Created($"/members/{member.Id}", member);
    }
    public static async Task<IResult> GetAllMembersAsync(IMemberService memberService, CancellationToken cancellationToken)
    {
        var members = await memberService.GetAllMembersAsync(cancellationToken);

        return Results.Ok(members);
    }
}