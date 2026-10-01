import { fireEvent, screen } from "@testing-library/react-native";
import { ClientFeeSection } from "@/features/fees/components/ClientFeeSection";
import { listClientPayments } from "@/features/fees/feesApi";
import { monthOf } from "@/features/fees/months";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildClient } from "@/testing/clientFactory";
import { routerMock, segmentsMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/fees/feesApi");

const client = buildClient();

describe("When a family opened from fees records a payment", () => {
  beforeEach(() => {
    jest.mocked(listClientPayments).mockResolvedValue([]);
    segmentsMock.current = ["(tabs)", "fees", "clients", "[clientId]"];
  });

  it("Then the payment opens inside fees", async () => {
    await renderWithProviders(<ClientFeeSection client={client} />);

    fireEvent.press(await screen.findByText(translate("fees.client.recordPayment")));

    expect(routerMock.push).toHaveBeenCalledWith(
      routes.recordPayment("fees", client.id, monthOf()),
    );
  });
});
