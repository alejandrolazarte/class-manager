import { screen } from "@testing-library/react-native";
import { StudentListScreen } from "@/features/students/screens/StudentListScreen";
import { searchStudents } from "@/features/students/studentsApi";
import { translate } from "@/i18n/translate";
import { buildCurrentMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildSubscription } from "@/testing/subscriptionFactory";

jest.mock("@/features/students/studentsApi");

describe("When the student limit is near", () => {
  beforeEach(() => {
    jest.mocked(searchStudents).mockResolvedValue([]);
  });

  it("Then the usage is shown", async () => {
    const subscription = buildSubscription({
      features: [{ code: "students", limit: 30, used: 27 }],
    });

    await renderWithProviders(<StudentListScreen />, {
      member: buildCurrentMember({ subscription }),
    });

    expect(
      screen.getByText(
        translate("subscriptions.featureUsed", { feature: "Alumnos", used: 27, limit: 30 }),
      ),
    ).toBeTruthy();
  });
});
