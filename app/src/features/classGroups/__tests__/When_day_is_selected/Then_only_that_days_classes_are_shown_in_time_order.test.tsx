import { fireEvent, screen } from "@testing-library/react-native";
import { listActiveClassGroups } from "@/features/classGroups/classGroupsApi";
import { WeeklyClassesScreen } from "@/features/classGroups/screens/WeeklyClassesScreen";
import { weekdayLongLabel } from "@/features/classGroups/weekdays";
import { buildClassGroup } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");

describe("When day is selected", () => {
  beforeEach(() => {
    jest.mocked(listActiveClassGroups).mockResolvedValue([
      buildClassGroup({ id: "late", name: "Aquagym", weekdays: ["Tuesday"], startTime: "19:00" }),
      buildClassGroup({ id: "monday", name: "Yoga", weekdays: ["Monday"], startTime: "08:00" }),
      buildClassGroup({
        id: "early",
        name: "Natación inicial",
        weekdays: ["Tuesday"],
        startTime: "18:00",
      }),
    ]);
  });

  it("Then only that days classes are shown in time order", async () => {
    await renderWithProviders(<WeeklyClassesScreen />);
    await fireEvent.press(await screen.findByRole("button", { name: weekdayLongLabel("Tuesday") }));

    const classNames = (await screen.findAllByTestId("class-group-name")).map(
      (classNameText) => classNameText.props.children,
    );
    expect(classNames).toEqual(["Natación inicial", "Aquagym"]);
  });
});
