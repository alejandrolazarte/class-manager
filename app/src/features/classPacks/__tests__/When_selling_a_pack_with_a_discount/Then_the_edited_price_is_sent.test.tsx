import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { listClassPacks, sellClassPack } from "@/features/classPacks/classPacksApi";
import { SellClassPackScreen } from "@/features/classPacks/screens/SellClassPackScreen";
import { translate } from "@/i18n/translate";
import { buildClassPack } from "@/testing/classPackFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/classPacks/classPacksApi");

const clientId = "0192f0c4-0000-7000-8000-000000000001";
const classPack = buildClassPack();

describe("When selling a pack with a discount", () => {
  beforeEach(() => {
    jest.mocked(listClassPacks).mockResolvedValue([classPack]);
    jest.mocked(sellClassPack).mockResolvedValue({
      id: "purchase",
      clientId,
      name: classPack.name,
      classCount: classPack.classCount,
      price: 150,
      purchasedOn: "2026-09-05",
      expiresOn: "2026-11-04",
      method: "Cash",
      notes: null,
    });
  });

  it("Then the edited price is sent", async () => {
    await renderWithProviders(<SellClassPackScreen clientId={clientId} />);

    await fireEvent.press(await screen.findByRole("button", { name: classPack.name }));
    const priceField = screen.getByLabelText(translate("classPacks.sell.price"));
    expect(priceField).toHaveDisplayValue("160");
    await fireEvent.changeText(priceField, "150");
    await fireEvent.press(
      screen.getByRole("button", { name: translate("classPacks.sell.submit") }),
    );

    await waitFor(() =>
      expect(sellClassPack).toHaveBeenCalledWith(
        clientId,
        expect.objectContaining({ classPackId: classPack.id, price: 150, method: "Cash" }),
      ),
    );
  });
});
