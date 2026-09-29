import { sessionKindOf } from "@/features/authentication/sessionKind";
import { buildAccessTokenWithClaims } from "@/testing/familyFactory";

const tokenWithPayload = buildAccessTokenWithClaims;

describe("When access token is read", () => {
  it("Then session kind comes from the kind claim", () => {
    expect(sessionKindOf(tokenWithPayload({ kind: "family" }))).toBe("family");
    expect(sessionKindOf(tokenWithPayload({ kind: "team" }))).toBe("team");
  });

  it("Then a token without the claim, an unknown kind or garbage is a team session", () => {
    expect(sessionKindOf(tokenWithPayload({ sub: "user" }))).toBe("team");
    expect(sessionKindOf(tokenWithPayload({ kind: "other" }))).toBe("team");
    expect(sessionKindOf("not-a-token")).toBe("team");
    expect(sessionKindOf("a.%%%.c")).toBe("team");
  });
});
