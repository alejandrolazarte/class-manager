import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { bookMakeup, getFamilyHome, getFamilyMakeups } from "@/features/family/familyApi";
import { FamilyClassesScreen } from "@/features/family/screens/FamilyClassesScreen";
import { todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildFamilyHome,
  buildFamilyMakeups,
  buildFamilyMakeupSlot,
  buildFamilyStudent,
} from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const today = todayIsoDate();

describe("When family books a makeup", () => {
  beforeEach(() => {
    jest.mocked(bookMakeup).mockResolvedValue();
    jest.mocked(getFamilyHome).mockResolvedValue(
      buildFamilyHome({
        students: [buildFamilyStudent({ id: "student-tomas", nextClasses: [] })],
      }),
    );
    jest.mocked(getFamilyMakeups).mockResolvedValue(
      buildFamilyMakeups({
        slots: [buildFamilyMakeupSlot({ classGroupId: "class-group-2", date: today })],
      }),
    );
  });

  it("Then the booking is sent", async () => {
    await renderFamilyScreen(<FamilyClassesScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("family.makeup.book") }),
    );

    await waitFor(() =>
      expect(bookMakeup).toHaveBeenCalledWith("student-tomas", "class-group-2", today),
    );
  });
});
