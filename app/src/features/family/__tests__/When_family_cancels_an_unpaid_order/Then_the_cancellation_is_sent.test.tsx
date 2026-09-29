import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { cancelFamilyOrder, getFamilyHome, listFamilyOrders } from "@/features/family/familyApi";
import { FamilyOrdersScreen } from "@/features/family/screens/FamilyOrdersScreen";
import { translate } from "@/i18n/translate";
import { buildFamilyHome, buildFamilyOrder } from "@/testing/familyFactory";
import { renderFamilyScreen } from "@/testing/renderFamilyScreen";

jest.mock("@/features/family/familyApi");

describe("When family cancels an unpaid order", () => {
  beforeEach(() => {
    jest.mocked(getFamilyHome).mockResolvedValue(buildFamilyHome());
    jest.mocked(listFamilyOrders).mockResolvedValue([buildFamilyOrder()]);
    jest.mocked(cancelFamilyOrder).mockResolvedValue(buildFamilyOrder({ status: "Cancelled" }));
  });

  it("Then the cancellation is sent", async () => {
    await renderFamilyScreen(<FamilyOrdersScreen />);

    await fireEvent.press(
      await screen.findByRole("button", { name: translate("family.orders.cancel") }),
    );

    await waitFor(() => expect(cancelFamilyOrder).toHaveBeenCalledWith("order-1"));
  });
});
