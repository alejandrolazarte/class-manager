using ClassManager.Core.Audit;

namespace ClassManager.Api.I.Tests.Audit;

internal sealed record SampleRows(int RowCount) : ICountedRows;
