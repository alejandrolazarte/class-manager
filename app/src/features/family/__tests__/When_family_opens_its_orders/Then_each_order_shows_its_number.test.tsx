import { screen } from "@testing-library/react-native";
import { getFamilyHome, listFamilyOrders } from "@/features/family/familyApi";
import { FamilyOrdersScreen } from "@/features/family/screens/FamilyOrdersScreen";
import { buildFamilyHome, buildFamilyOrder } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family opens its orders", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(buildFamilyHome());
    jest.mocked(listFamilyOrders).mockResolvedValue([buildFamilyOrder({ number: 57 })]);
  });

  it("Then each order shows its number", async () => {
    await renderFamilyScreen(<FamilyOrdersScreen />);

    expect(await screen.findByText(/Pedido n\.º 57 del/)).toBeOnTheScreen();
  });
});
