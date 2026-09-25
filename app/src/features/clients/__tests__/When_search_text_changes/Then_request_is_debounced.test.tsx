import { act, fireEvent, screen } from "@testing-library/react-native";
import { searchClients } from "@/features/clients/clientsApi";
import { clientSearchDebounceMilliseconds } from "@/features/clients/useClientSearch";
import { ClientListScreen } from "@/features/clients/screens/ClientListScreen";
import { translate } from "@/i18n/translate";
import { renderWithProviders } from "@/testing/renderWithProviders";

jest.mock("@/features/clients/clientsApi");

describe("When search text changes", () => {
  beforeEach(() => {
    jest.useFakeTimers();
    jest.mocked(searchClients).mockResolvedValue([]);
  });

  afterEach(() => {
    jest.useRealTimers();
  });

  it("Then request is debounced", async () => {
    await renderWithProviders(<ClientListScreen />);
    const searchInput = screen.getByLabelText(translate("clients.list.searchPlaceholder"));

    await fireEvent.changeText(searchInput, "a");
    await fireEvent.changeText(searchInput, "an");
    await fireEvent.changeText(searchInput, "ana");
    await act(async () => {
      jest.advanceTimersByTime(clientSearchDebounceMilliseconds);
    });

    const searchedTexts = jest
      .mocked(searchClients)
      .mock.calls.map(([searchRequest]) => searchRequest.search);
    expect(searchedTexts).toEqual(["", "ana"]);
  });
});
