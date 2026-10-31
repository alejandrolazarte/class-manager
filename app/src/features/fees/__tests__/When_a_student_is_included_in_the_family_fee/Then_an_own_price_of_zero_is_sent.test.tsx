import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { setStudentPlan } from "@/features/fees/feesApi";
import { monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import {
  clientOfStudentPlanScreen,
  renderStudentPlanScreen,
  studentPlanOfScreen,
} from "@/testing/studentPlanScreen";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

describe("When a student is included in the family fee", () => {
  beforeEach(() => {
    jest.mocked(setStudentPlan).mockResolvedValue(clientOfStudentPlanScreen.plans);
  });

  it("Then an own price of zero is sent", async () => {
    await renderStudentPlanScreen();

    await fireEvent.press(
      await screen.findByRole("switch", { name: translate("fees.studentPlan.customPrice") }),
    );
    await fireEvent.changeText(
      screen.getByLabelText(translate("fees.studentPlan.customPriceAmount")),
      "0",
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(setStudentPlan).toHaveBeenCalledWith(studentPlanOfScreen.studentId, {
        classPackId: studentPlanOfScreen.classPackId,
        customPrice: 0,
        effectiveFrom: monthOf(),
      }),
    );
  });
});
