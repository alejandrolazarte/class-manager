import { fireEvent, screen } from "@testing-library/react-native";
import { getClient } from "@/features/clients/clientsApi";
import { ClientDetailScreen } from "@/features/clients/screens/ClientDetailScreen";
import { listStudentEnrollments } from "@/features/enrollments/enrollmentsApi";
import { translate } from "@/i18n/translate";
import { routes } from "@/navigation/routes";
import { buildClient } from "@/testing/clientFactory";
import { routerMock } from "@/testing/expoRouterMock";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");
jest.mock("@/features/enrollments/enrollmentsApi");
jest.mock("@/features/fees/feesApi");
jest.mock("@/features/classPacks/classPacksApi");

const client = { ...buildClient({ fullName: "Ana Gómez" }), isOnTheTeam: true };

describe("When a client is also on the team", () => {
  beforeEach(() => {
    jest.mocked(getClient).mockResolvedValue(client);
    jest.mocked(listStudentEnrollments).mockResolvedValue([]);
  });

  it("Then separate opens the separation", async () => {
    await renderWithProviders(<ClientDetailScreen clientId={client.id} />);
    expect(
      await screen.findByText(translate("people.alsoOnTheTeam", { name: "Ana Gómez" })),
    ).toBeOnTheScreen();

    await fireEvent.press(screen.getByRole("button", { name: translate("people.separate.open") }));

    expect(routerMock.push).toHaveBeenCalledWith(routes.separateClient("students", client.id));
  });
});
