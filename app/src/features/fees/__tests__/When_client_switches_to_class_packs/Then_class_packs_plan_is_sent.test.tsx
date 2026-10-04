import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { getClassBalance } from "@/features/classPacks/classPacksApi";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { listClientPayments, setClientBillingPlan } from "@/features/fees/feesApi";
import { monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { buildClassBalance } from "@/testing/classPackFactory";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

const client = buildClient();

describe("When client switches to class packs", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([]);
    jest.mocked(getClassBalance).mockResolvedValue(buildClassBalance({ purchases: [] }));
    jest.mocked(setClientBillingPlan).mockResolvedValue({
      billingPlan: { kind: "ClassPacks", customFee: null },
      billingPlanChanges: [{ effectiveFrom: monthOf(), kind: "ClassPacks", customFee: null }],
    });
  });

  it("Then class packs plan is sent", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />);

    await fireEvent.press(screen.getByRole("button", { name: translate("fees.client.changeFee") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("fees.plan.ClassPacks") }));
    await fireEvent.press(screen.getByRole("button", { name: translate("common.save") }));

    await waitFor(() =>
      expect(setClientBillingPlan).toHaveBeenCalledWith(client.id, {
        kind: "ClassPacks",
        customFee: null,
        effectiveFrom: monthOf(),
      }),
    );
  });
});
