using Minisplitwise.Application.Groups;
using Minisplitwise.Application.Interfaces;

namespace Minisplitwise.API.Endpoints;

public static class GroupEndpoints
{
    public static void MapGroupEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/groups", CreateGroupAsync)
            .WithName("CreateGroup")
            .WithTags("Groups")
            .WithSummary("Create a new group")
            .WithDescription("Create a new group with the given name and member ids");
        
        app.MapPost("/groups/add-member", AddMemberToGroupAsync)
            .WithName("AddMemberToGroup")
            .WithTags("Groups")
            .WithSummary("Add a member to a group")
            .WithDescription("Add a member to a group with the given group id and member id");
    }

    public static async Task<IResult> CreateGroupAsync(GroupRequestDto groupRequestDto, IGroupService groupService, CancellationToken cancellationToken)
    {
        var group = await groupService.CreateGroupAsync(groupRequestDto, cancellationToken);

        return Results.Created($"/groups/{group.Id}", group);
    }

    public static async Task<IResult> AddMemberToGroupAsync(AddMemberRequestDto addMemberRequestDto, IGroupService groupService, CancellationToken cancellationToken)
    {
        var group = await groupService.AddMemberToGroupAsync(addMemberRequestDto, cancellationToken);

        return Results.Created($"/groups/{group.Id}/members/{addMemberRequestDto.MemberId}", group);
    }
}