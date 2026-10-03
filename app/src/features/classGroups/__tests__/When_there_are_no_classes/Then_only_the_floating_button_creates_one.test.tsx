import { screen } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { WeeklyClassesScreen } from "@/features/classGroups/screens/WeeklyClassesScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");

describe("When there are no classes", () => {
  beforeEach(() => {
    jest.mocked(listActiveClassGroups).mockResolvedValue([]);
  });

  it("Then only the floating button creates one", async () => {
    await renderWithProviders(<WeeklyClassesScreen />);

    expect(await screen.findByText(translate("classGroups.week.emptyTitle"))).toBeOnTheScreen();
    expect(
      screen.getAllByRole("button", { name: translate("classGroups.week.newClassGroup") }),
    ).toHaveLength(1);
    expect(
      screen.getByText(
        translate("common.emptyStateHint", { action: translate("classGroups.week.newClassGroup") }),
      ),
    ).toBeOnTheScreen();
  });
});
