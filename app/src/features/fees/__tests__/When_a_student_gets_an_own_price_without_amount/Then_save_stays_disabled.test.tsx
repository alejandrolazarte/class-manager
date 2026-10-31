import { fireEvent, screen } from "@testing-library/react-native";
import { translate } from "@/i18n/translate";
import { renderStudentPlanScreen } from "@/testing/studentPlanScreen";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

describe("When a student gets an own price without amount", () => {
  it("Then save stays disabled", async () => {
    await renderStudentPlanScreen();

    await fireEvent.press(
      await screen.findByRole("switch", { name: translate("fees.studentPlan.customPrice") }),
    );

    expect(screen.getByRole("button", { name: translate("common.save") })).toBeDisabled();
  });
});
