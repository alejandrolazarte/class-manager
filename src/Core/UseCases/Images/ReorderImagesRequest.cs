namespace ClassManager.Core.UseCases.Images;

public sealed record ReorderImagesRequest(IReadOnlyList<Guid>? DocumentIds);
