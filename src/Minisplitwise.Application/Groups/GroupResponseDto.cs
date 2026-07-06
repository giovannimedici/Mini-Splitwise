namespace Minisplitwise.Application.Groups;

public record GroupResponseDto(Guid Id, string Name, List<Guid> Members);