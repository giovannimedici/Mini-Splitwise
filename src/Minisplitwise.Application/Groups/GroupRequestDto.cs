namespace Minisplitwise.Application.Groups;

public record GroupRequestDto(string Name, List<Guid> MemberIds);