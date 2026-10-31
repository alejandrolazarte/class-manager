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

describe("When a student is left without a monthly plan", () => {
  beforeEach(() => {
    jest.mocked(setStudentPlan).mockResolvedValue(clientOfStudentPlanScreen.plans);
  });

  it("Then no plan is sent", async () => {
    await renderStudentPlanScreen();

    await fireEvent.press(
      await screen.findByRole("radio", { name: translate("fees.studentPlan.none") }),
    );
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(setStudentPlan).toHaveBeenCalledWith(studentPlanOfScreen.studentId, {
        classPackId: null,
        customPrice: null,
        effectiveFrom: monthOf(),
      }),
    );
  });
});
