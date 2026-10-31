import { screen } from "@testing-library/react-native";
import { translateCount } from "@/i18n/translate";
import { buildClassBalance } from "@/testing/classPackFactory";
import { renderStudentPlanScreen } from "@/testing/studentPlanScreen";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

describe("When a student with bought classes keeps a monthly plan", () => {
  it("Then the classes are said to be kept", async () => {
    await renderStudentPlanScreen(buildClassBalance({ availableClasses: 19 }));

    expect(
      await screen.findByText(translateCount("fees.billingPlan.keepClasses", 19)),
    ).toBeOnTheScreen();
  });
});
