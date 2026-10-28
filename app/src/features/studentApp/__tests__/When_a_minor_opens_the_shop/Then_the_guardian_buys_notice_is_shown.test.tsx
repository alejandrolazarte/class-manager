import { screen } from "@testing-library/react-native";
import { getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When a minor opens the shop", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop({ canBuy: false }));
  });

  it("Then the guardian buys notice is shown", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    expect(await screen.findByText(translate("student.shop.guardianBuys"))).toBeOnTheScreen();
  });
});
