import { screen } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { WeeklyClassesScreen } from "@/features/classGroups/screens/WeeklyClassesScreen";
import { translate } from "@/i18n/translate";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");

describe("When class has enrollments", () => {
  beforeEach(() => {
    jest.mocked(listActiveClassGroups).mockResolvedValue([
      buildClassGroup({
        weekdays: ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"],
        enrolledCount: 5,
        capacity: 8,
      }),
    ]);
  });

  it("Then card shows enrolled over capacity", async () => {
    await renderWithProviders(<WeeklyClassesScreen />);

    expect(
      await screen.findByText(
        translate("classGroups.card.enrolled", { enrolled: 5, capacity: 8 }),
        {
          exact: false,
        },
      ),
    ).toBeOnTheScreen();
  });
});
