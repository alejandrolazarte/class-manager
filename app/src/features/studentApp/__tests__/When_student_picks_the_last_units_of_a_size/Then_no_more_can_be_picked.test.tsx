import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student picks the last units of a size", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
  });

  it("Then no more can be picked", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "Gorro de natación" }));
    await fireEvent.press(screen.getByRole("button", { name: translate("student.shop.increase") }));

    expect(screen.getByRole("button", { name: translate("student.shop.increase") })).toBeDisabled();
  });
});
