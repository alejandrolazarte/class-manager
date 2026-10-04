import { sessionKindOf } from "@/features/authentication/sessionKind";
import { buildAccessTokenWithClaims } from "@/testing/studentAppFactory";

const tokenWithPayload = buildAccessTokenWithClaims;

describe("When access token is read", () => {
  it("Then session kind comes from the kind claim", () => {
    expect(sessionKindOf(tokenWithPayload({ kind: "student" }))).toBe("student");
    expect(sessionKindOf(tokenWithPayload({ kind: "team" }))).toBe("team");
  });

  it("Then a token without the claim, an unknown kind or garbage is a team session", () => {
    expect(sessionKindOf(tokenWithPayload({ sub: "user" }))).toBe("team");
    expect(sessionKindOf(tokenWithPayload({ kind: "other" }))).toBe("team");
    expect(sessionKindOf("not-a-token")).toBe("team");
    expect(sessionKindOf("a.%%%.c")).toBe("team");
  });
});
