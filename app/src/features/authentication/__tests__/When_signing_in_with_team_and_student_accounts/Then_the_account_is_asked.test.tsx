import { fireEvent, screen } from "@testing-library/react-native";
import { Pressable, Text } from "react-native";
import { listAccounts, signIn } from "@/features/authentication/authenticationApi";
import { useSession } from "@/features/authentication/useSession";
import { buildAccount } from "@/testing/accountFactory";
import { buildTokenResponse } from "@/testing/authenticationFactory";
import { mockRefreshTokenStorage } from "@/testing/refreshTokenStorageMock";
import { renderWithSession } from "@/testing/renderWithSession";

jest.mock("@/features/authentication/authenticationApi");
jest.mock("@/features/authentication/sessionStorage");

const signInProbeLabel = "sign in probe";
const mustChooseText = "must choose";

function SignInProbe() {
  const { session, signIn: startSignIn } = useSession();
  return (
    <>
      {session.status === "signedIn" && session.mustChooseAccount ? (
        <Text>{mustChooseText}</Text>
      ) : null}
      <Pressable
        accessibilityRole="button"
        accessibilityLabel={signInProbeLabel}
        onPress={() => startSignIn({ email: "ana@example.com", password: "a long passphrase" })}
      />
    </>
  );
}

describe("When signing in with team and student accounts", () => {
  beforeEach(() => {
    mockRefreshTokenStorage(null);
    jest.mocked(signIn).mockResolvedValue(buildTokenResponse());
    jest.mocked(listAccounts).mockResolvedValue([
      buildAccount(),
      buildAccount({
        businessId: "business-school",
        businessName: "Escuela Brazada",
        kind: "student",
        isCurrent: false,
      }),
    ]);
  });

  it("Then the account is asked", async () => {
    await renderWithSession(<SignInProbe />);

    await fireEvent.press(screen.getByRole("button", { name: signInProbeLabel }));

    expect(await screen.findByText(mustChooseText)).toBeOnTheScreen();
  });
});
