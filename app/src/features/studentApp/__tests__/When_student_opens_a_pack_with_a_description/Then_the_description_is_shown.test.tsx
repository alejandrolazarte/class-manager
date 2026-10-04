import { fireEvent, screen } from "@testing-library/react-native";
import { getStudentAppShop } from "@/features/studentApp/studentAppApi";
import { StudentAppShopScreen } from "@/features/studentApp/screens/StudentAppShopScreen";
import { buildStudentAppShop } from "@/testing/studentAppFactory";
import { renderStudentAppScreen } from "@/testing/renderStudentAppScreen";

jest.mock("@/features/studentApp/studentAppApi");

const description = "Técnica de los cuatro estilos, virajes y resistencia.";

describe("When student opens a pack with a description", () => {
  beforeEach(() => {
    const shop = buildStudentAppShop();
    jest.mocked(getStudentAppShop).mockResolvedValue({
      ...shop,
      packs: shop.packs.map((pack) => ({ ...pack, description })),
    });
  });

  it("Then the description is shown", async () => {
    await renderStudentAppScreen(<StudentAppShopScreen />);

    await fireEvent.press(await screen.findByRole("button", { name: "8 clases" }));

    expect(screen.getByText(description)).toBeOnTheScreen();
  });
});
