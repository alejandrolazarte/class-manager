import { screen } from "@testing-library/react-native";
import { listInstructorsIncludingInactive } from "@/features/instructors/instructorsApi";
import { InstructorListScreen } from "@/features/instructors/screens/InstructorListScreen";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/instructors/instructorsApi");

describe("When listing instructors", () => {
  beforeEach(() => {
    jest
      .mocked(listInstructorsIncludingInactive)
      .mockResolvedValue([
        buildInstructor({ id: "inactive", fullName: "Ana Ruiz", isActive: false }),
        buildInstructor({ id: "active", fullName: "Laura Gómez" }),
      ]);
  });

  it("Then inactive ones are last", async () => {
    await renderWithProviders(<InstructorListScreen />);

    const instructorButtons = await screen.findAllByRole("button", { name: /Gómez|Ruiz/ });
    expect(instructorButtons.map((button) => button.props.accessibilityLabel)).toEqual([
      "Laura Gómez",
      "Ana Ruiz",
    ]);
  });
});
