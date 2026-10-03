import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { cancelPackClass } from "@/features/family/familyApi";
import { ScheduledClassCard } from "@/features/family/components/ScheduledClassCard";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import { buildFamilyNextClass } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const tomorrow = addDays(todayIsoDate(), 1);

describe("When family cancels a pack class", () => {
  beforeEach(() => {
    jest.mocked(cancelPackClass).mockResolvedValue();
  });

  it("Then the booking is withdrawn", async () => {
    await renderFamilyScreen(
      <ScheduledClassCard
        studentId="student-tomas"
        scheduledClass={buildFamilyNextClass({
          date: tomorrow,
          classGroupId: "class-group-aquagym",
          isPackBooking: true,
        })}
        isToday={false}
      />,
    );

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("family.packClasses.cancel") }),
    );

    await waitFor(() =>
      expect(cancelPackClass).toHaveBeenCalledWith(
        "student-tomas",
        "class-group-aquagym",
        tomorrow,
      ),
    );
    expect(screen.queryByRole("button", { name: translate("family.absence.notify") })).toBeNull();
  });
});
