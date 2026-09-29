import { screen } from "@testing-library/react-native";
import { getFamilyHome } from "@/features/family/familyApi";
import { FamilyHomeScreen } from "@/features/family/screens/FamilyHomeScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyHome, buildFamilyStudent } from "@/testing/familyFactory";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/family/familyApi");

describe("When family has no classes", () => {
  beforeEach(() => {
    jest
      .mocked(getFamilyHome)
      .mockResolvedValue(buildFamilyHome({ students: [buildFamilyStudent({ nextClasses: [] })] }));
  });

  it("Then empty message is shown", async () => {
    await renderWithSession(<FamilyHomeScreen />);

    expect(await screen.findByText(translate("family.student.noClasses"))).toBeOnTheScreen();
  });
});
