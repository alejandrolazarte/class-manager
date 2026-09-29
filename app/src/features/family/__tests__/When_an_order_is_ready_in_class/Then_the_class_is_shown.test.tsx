import { screen } from "@testing-library/react-native";
import { getFamilyHome, listFamilyOrders } from "@/features/family/familyApi";
import { FamilyOrdersScreen } from "@/features/family/screens/FamilyOrdersScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyHome, buildFamilyOrder } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When an order is ready in class", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(buildFamilyHome());
    jest.mocked(listFamilyOrders).mockResolvedValue([
      buildFamilyOrder({
        status: "Paid",
        awaitsPickup: true,
        isReady: true,
        delivery: "InClass",
        deliveryClassGroupName: "Natación inicial",
        lines: [
          {
            id: "line-cap",
            kind: "Product",
            name: "Gorro de natación · S",
            quantity: 1,
            total: 12,
            refundedQuantity: 0,
          },
        ],
      }),
    ]);
  });

  it("Then the class is shown", async () => {
    await renderFamilyScreen(<FamilyOrdersScreen />);

    expect(await screen.findByText(translate("family.orders.status.ready"))).toBeOnTheScreen();
    expect(
      screen.getByText(
        `${translate("family.orders.deliveryInClass", { className: "Natación inicial" })}.`,
      ),
    ).toBeOnTheScreen();
  });
});
