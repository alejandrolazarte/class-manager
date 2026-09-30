import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { cancelMakeup } from "@/features/family/familyApi";
import { ScheduledClassCard } from "@/features/family/components/ScheduledClassCard";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { buildFamilyNextClass } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const tomorrow = addDays(todayIsoDate(), 1);

describe("When family cancels a makeup", () => {
  beforeEach(() => {
    jest.mocked(cancelMakeup).mockResolvedValue();
  });

  it("Then the booking is withdrawn", async () => {
    await renderFamilyScreen(
      <ScheduledClassCard
        studentId="student-tomas"
        scheduledClass={buildFamilyNextClass({
          date: tomorrow,
          classGroupId: "class-group-2",
          isMakeup: true,
        })}
        isToday={false}
      />,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("family.makeup.cancel") }),
    );

    await waitFor(() =>
      expect(cancelMakeup).toHaveBeenCalledWith("student-tomas", "class-group-2", tomorrow),
    );
  });
});
