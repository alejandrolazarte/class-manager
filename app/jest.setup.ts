import { authenticationSession } from "./src/api/authenticationSession";
import { resetExpoRouterMock } from "./src/testing/expoRouterMock";
import { clearTestQueryClients } from "./src/testing/testQueryClients";

process.env.EXPO_PUBLIC_API_BASE_URL = "http://api.test";

jest.mock("expo-router", () => {
  const { routerMock, searchParametersMock, RedirectMock } = jest.requireActual(
    "./src/testing/expoRouterMock",
  );
  return {
    Redirect: RedirectMock,
    useRouter: () => routerMock,
    router: routerMock,
    useLocalSearchParams: () => searchParametersMock.current,
    useFocusEffect: jest.fn(),
    Stack: { Screen: () => null },
  };
});

beforeEach(() => {
  resetExpoRouterMock();
});

afterEach(() => {
  authenticationSession.reset();
  clearTestQueryClients();
});
