import { screen } from "@testing-library/react-native";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { listClientPayments } from "@/features/fees/feesApi";
import { formatMoney } from "@/formatting/money";
import { Payment } from "@/features/fees/types";
import { buildBusiness } from "@/testing/businessFactory";
import { buildClient } from "@/testing/clientFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const business = buildBusiness({ currencyCode: "USD" });
const client = buildClient();
const euroPayment: Payment = {
  id: "payment-1",
  clientId: client.id,
  amount: 50,
  currencyCode: "EUR",
  month: "2026-09",
  paidOn: "2026-09-10",
  method: "Card",
  notes: null,
  recordedByUserId: null,
  recordedByFullName: null,
};

describe("When a payment was charged in another currency", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([euroPayment]);
  });

  it("Then it shows the currency it was charged in", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />, { business });

    expect(await screen.findByText(formatMoney(euroPayment.amount, "EUR"))).toBeTruthy();
  });
});
