import { fireEvent, screen, waitFor } from "@testing-library/react-native";
import { ClassDeliveriesCard } from "@/features/orders/components/ClassDeliveriesCard";
import { deliverInClass, listClassDeliveries } from "@/features/orders/ordersApi";
import { translate } from "@/i18n/translate";
import { buildInstructorMember } from "@/testing/memberFactory";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/orders/ordersApi");

const delivery = {
  orderId: "order-1",
  clientFullName: "Ana Pérez",
  isReady: true,
  lines: [{ name: "Gorro de natación · S", quantity: 1 }],
};

describe("When instructor opens a class with a delivery", () => {
  beforeEach(() => {
    jest.mocked(listClassDeliveries).mockResolvedValue([delivery]);
    jest.mocked(deliverInClass).mockResolvedValue(delivery);
  });

  it("Then it can be handed over", async () => {
    await renderWithProviders(<ClassDeliveriesCard classGroupId="class-group-1" />, {
      member: buildInstructorMember(),
    });

    await fireEvent.press(
      await screen.findByRole("button", {
        name: `${translate("sessions.deliveries.deliver")} Ana Pérez`,
      }),
    );

    await waitFor(() => expect(deliverInClass).toHaveBeenCalledWith("class-group-1", "order-1"));
  });
});
