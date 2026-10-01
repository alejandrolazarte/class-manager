import { authenticationSession } from "./src/api/authenticationSession";
import { resetExpoRouterMock } from "./src/testing/expoRouterMock";
import { clearTestQueryClients } from "./src/testing/testQueryClients";

process.env.EXPO_PUBLIC_API_BASE_URL = "http://api.test";

jest.mock("expo-router", () => {
  const { routerMock, searchParametersMock, segmentsMock, RedirectMock } = jest.requireActual(
    "./src/testing/expoRouterMock",
  );
  return {
    DarkTheme: jest.requireActual("expo-router/build/react-navigation/native/theming/DarkTheme")
      .DarkTheme,
    DefaultTheme: jest.requireActual(
      "expo-router/build/react-navigation/native/theming/DefaultTheme",
    ).DefaultTheme,
    ThemeProvider: jest.requireActual(
      "expo-router/build/react-navigation/core/theming/ThemeProvider",
    ).ThemeProvider,
    Redirect: RedirectMock,
    useRouter: () => routerMock,
    router: routerMock,
    useLocalSearchParams: () => searchParametersMock.current,
    useSegments: () => segmentsMock.current,
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
