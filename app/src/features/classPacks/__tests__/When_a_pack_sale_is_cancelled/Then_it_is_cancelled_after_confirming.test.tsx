import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { deleteClassPackPurchase, getClassBalance } from "@/features/classPacks/classPacksApi";
import { ClassBalanceSection } from "@/features/classPacks/components/ClassBalanceSection";
import { translate } from "@/i18n/translate";
import { buildClassBalance } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");

const clientId = "0192f0c4-0000-7000-8000-000000000001";
const balance = buildClassBalance();
const purchase = balance.purchases[0]!;

describe("When a pack sale is cancelled", () => {
  beforeEach(() => {
    jest.mocked(getClassBalance).mockResolvedValue(balance);
    jest.mocked(deleteClassPackPurchase).mockResolvedValue(undefined);
  });

  it("Then it is cancelled after confirming", async () => {
    await renderWithProviders(<ClassBalanceSection clientId={clientId} />);

    await fireEvent.press(
      await screen.findByRole("button", {
        name: `${translate("classPacks.balance.deletePurchase")} ${purchase.name}`,
      }),
    );
    expect(deleteClassPackPurchase).not.toHaveBeenCalled();
    await fireEvent.press(
      screen.getByRole("button", { name: translate("classPacks.balance.confirmDeletePurchase") }),
    );

    await waitFor(() => expect(deleteClassPackPurchase).toHaveBeenCalledWith(purchase.id));
  });
});
