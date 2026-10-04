import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { translate } from "@/i18n/translate";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

describe("When student searches the shop", () => {
  beforeEach(() => {
    jest.mocked(getStudentAppShop).mockResolvedValue(buildStudentAppShop());
  });

  it("Then only matching items are shown", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.changeText(
      await screen.findByLabelText(translate("student.shop.search")),
      "gorro",
    );

    expect(screen.getByRole("button", { name: "Gorro de natación" })).toBeOnTheScreen();
    expect(screen.queryByText("8 clases")).toBeNull();
  });
});
