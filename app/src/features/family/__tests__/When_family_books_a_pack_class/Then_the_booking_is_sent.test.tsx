import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { bookPackClass, getFamilyHome, getFamilyPackClasses } from "@/features/family/familyApi";
import { FamilyClassesScreen } from "@/features/family/screens/FamilyClassesScreen";
import { todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildFamilyHome,
  buildFamilyMakeupSlot,
  buildFamilyStudent,
} from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const today = todayIsoDate();

describe("When family books a pack class", () => {
  beforeEach(() => {
    jest.mocked(bookPackClass).mockResolvedValue();
    jest.mocked(getFamilyHome).mockResolvedValue(
      buildFamilyHome({
        students: [buildFamilyStudent({ id: "student-tomas", nextClasses: [] })],
      }),
    );
    jest.mocked(getFamilyPackClasses).mockResolvedValue({
      classesLeft: 4,
      slots: [buildFamilyMakeupSlot({ classGroupId: "class-group-aquagym", date: today })],
    });
  });

  it("Then the booking is sent", async () => {
    await renderFamilyScreen(<FamilyClassesScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("family.makeup.book") }),
    );

    await waitFor(() =>
      expect(bookPackClass).toHaveBeenCalledWith("student-tomas", "class-group-aquagym", today),
    );
  });
});
