import { fireEvent, screen } from "@testing-library/react-native";
import { getFamilyHome, listFamilyOrders } from "@/features/family/familyApi";
import { FamilyOrdersScreen } from "@/features/family/screens/FamilyOrdersScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyHome, buildFamilyOrder } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family filters past orders", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(buildFamilyHome());
    jest
      .mocked(listFamilyOrders)
      .mockResolvedValue([
        buildFamilyOrder({ id: "order-open" }),
        buildFamilyOrder({ id: "order-delivered", status: "Delivered" }),
      ]);
  });

  it("Then only finished orders are shown", async () => {
    await renderFamilyScreen(<FamilyOrdersScreen />);

    expect(await screen.findByText(translate("family.orders.status.Requested"))).toBeOnTheScreen();

    await fireEvent.press(
      screen.getByRole("button", { name: new RegExp(translate("family.orders.filter.past")) }),
    );

    expect(screen.getByText(translate("family.orders.status.Delivered"))).toBeOnTheScreen();
    expect(screen.queryByText(translate("family.orders.status.Requested"))).toBeNull();
  });
});
