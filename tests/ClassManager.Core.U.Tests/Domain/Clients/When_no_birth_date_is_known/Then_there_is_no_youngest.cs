using ClassManager.Core.Domain.Clients;

namespace ClassManager.Core.U.Tests.Domain.Clients.When_no_birth_date_is_known;

public sealed class Then_there_is_no_youngest
{
    [Fact]
    public void Then_there_is_no_youngest_Run()
    {
        FamilyVisibility.YoungestBirthDate([null, null]).ShouldBeNull();
    }
}
