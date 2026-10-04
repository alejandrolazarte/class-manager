import { screen } from "@testing-library/react-native";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { listClientPayments } from "@/features/fees/feesApi";
import { translate } from "@/i18n/translate";
import { buildClient } from "@/testing/clientFactory";
import { buildViewer } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const client = buildClient();

describe("When viewer opens a client", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([]);
  });

  it("Then payment actions are hidden", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />, { member: buildViewer() });

    expect(await screen.findByText(translate("fees.client.noPayments"))).toBeOnTheScreen();
    expect(screen.queryByText(translate("fees.client.recordPayment"))).toBeNull();
    expect(
      screen.queryByRole("button", { name: translate("fees.client.changePlanAccessibility") }),
    ).toBeNull();
  });
});
