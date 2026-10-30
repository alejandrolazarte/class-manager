import { screen } from "@testing-library/react-native";
import { NextClassHero } from "@/features/studentApp/components/NextClassHero";
import { renderWithProviders } from "@/testing/renderWithProviders";
import { buildAccountStudent, buildStudentAppNextClass } from "@/testing/studentAppFactory";

const photoUrl = "https://files.example.com/private/photo.webp?sig=abc";

describe("When the class instructor has a photo", () => {
  it("Then the next class shows it", async () => {
    await renderWithProviders(
      <NextClassHero
        student={buildAccountStudent({
          nextClasses: [
            buildStudentAppNextClass({ date: "2099-01-05", instructorPhotoUrl: photoUrl }),
          ],
        })}
        onSeeWeek={jest.fn()}
        classBalance={null}
        onBuyClasses={jest.fn()}
      />,
    );

    expect(screen.getByTestId("avatar-photo", { includeHiddenElements: true })).toHaveProp(
      "source",
      { uri: photoUrl },
    );
  });
});
