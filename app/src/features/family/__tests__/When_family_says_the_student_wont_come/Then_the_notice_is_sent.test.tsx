import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getFamilyHome, getFamilyShop, notifyAbsence } from "@/features/family/familyApi";
import { FamilyHomeScreen } from "@/features/family/screens/FamilyHomeScreen";
import { addDays, todayIsoDate } from "@/features/sessions/dates";
import { translate } from "@/i18n/translate";
import {
  buildFamilyHome,
  buildFamilyNextClass,
  buildFamilyShop,
  buildFamilyStudent,
} from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const tomorrow = addDays(todayIsoDate(), 1);

describe("When family says the student won't come", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest.mocked(notifyAbsence).mockResolvedValue();
    jest.mocked(getFamilyHome).mockResolvedValue(
      buildFamilyHome({
        students: [
          buildFamilyStudent({
            id: "student-tomas",
            nextClasses: [buildFamilyNextClass({ date: tomorrow, classGroupId: "class-group-1" })],
          }),
        ],
      }),
    );
  });

  it("Then the notice is sent", async () => {
    await renderFamilyScreen(<FamilyHomeScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("family.absence.notGoing") }),
    );

    await waitFor(() =>
      expect(notifyAbsence).toHaveBeenCalledWith("student-tomas", "class-group-1", tomorrow),
    );
  });
});
