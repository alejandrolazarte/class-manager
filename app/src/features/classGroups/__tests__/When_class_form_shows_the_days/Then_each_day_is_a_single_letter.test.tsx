import { fireEvent, screen } from "@testing-library/react-native";
import { ClassGroupFormScreen } from "@/features/classGroups/screens/ClassGroupFormScreen";
import { listActiveInstructors } from "@/features/instructors/instructorsApi";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classGroups/classGroupsApi");
jest.mock("@/features/instructors/instructorsApi");

describe("When class form shows the days", () => {
  beforeEach(() => {
    jest.mocked(listActiveInstructors).mockResolvedValue([buildInstructor()]);
  });

  it("Then each day is a single letter", async () => {
    await renderWithProviders(<ClassGroupFormScreen />);
    const wednesday = await screen.findByRole("button", { name: "Miércoles" });

    await fireEvent.press(wednesday);

    expect(screen.getByText("X")).toBeOnTheScreen();
    expect(wednesday).toBeSelected();
  });
});
