import { screen } from "@testing-library/react-native";
import { InstructorPicker } from "@/features/instructors/components/InstructorPicker";
import { buildInstructor } from "@/testing/classGroupFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

const photoUrl = "https://files.example.com/private/laura.webp?sig=abc";

describe("When an instructor with a photo is in the picker", () => {
  it("Then the photo is shown", async () => {
    await renderWithProviders(
      <InstructorPicker
        instructors={[buildInstructor({ photoUrl })]}
        isPending={false}
        isError={false}
        onPick={jest.fn()}
        onClose={jest.fn()}
        emptyMessage="Sin profes"
      />,
    );

    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
  });
});
