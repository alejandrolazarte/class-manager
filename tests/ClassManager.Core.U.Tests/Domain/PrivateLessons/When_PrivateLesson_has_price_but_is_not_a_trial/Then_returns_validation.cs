using ClassManager.Core.Domain.ClassGroups;
using ClassManager.Core.Domain.PrivateLessons;

namespace ClassManager.Core.U.Tests.Domain.PrivateLessons.When_PrivateLesson_has_price_but_is_not_a_trial;

public sealed class Then_returns_validation
{
    [Fact]
    public void Then_returns_validation_Run()
    {
        var schedule = ClassSchedule.Create([TestData.Today.DayOfWeek], "18:00", 45).Value!;
        var lesson = PrivateLesson.Create(TestIds.Next(), TestData.Today, schedule, [TestIds.Next()], null, null, null, TestData.Now).Value!.WithId();

        var trial = lesson.SetTrial(isTrial: false, trialPrice: 25m);

        trial.Error!.FieldName.ShouldBe(nameof(PrivateLesson.TrialPrice));
    }
}
