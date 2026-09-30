import { screen } from "@testing-library/react-native";
import { getFamilyHome, getFamilyShop } from "@/features/family/familyApi";
import { FamilyHomeScreen } from "@/features/family/screens/FamilyHomeScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyHome, buildFamilyShop, buildFamilyStudent } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

const comment = "Muy buen ritmo en la serie larga.";

describe("When coach left a comment", () => {
  beforeEach(() => {
    jest.mocked(getFamilyShop).mockResolvedValue(buildFamilyShop());
    jest.mocked(getFamilyHome).mockResolvedValue(
      buildFamilyHome({
        students: [
          buildFamilyStudent({
            latestFeedback: {
              date: "2026-09-28",
              className: "Natación inicial",
              instructorFullName: "Martín Díaz",
              text: comment,
            },
          }),
        ],
      }),
    );
  });

  it("Then it is shown on home", async () => {
    await renderFamilyScreen(<FamilyHomeScreen />);

    expect(
      await screen.findByText(translate("family.feedback.title", { instructor: "Martín" })),
    ).toBeOnTheScreen();
    expect(screen.getByText(`“${comment}”`)).toBeOnTheScreen();
  });
});
