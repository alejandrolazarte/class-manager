import { anonymousHttpClient } from "@/api/httpClient";

const acceptedStatus = 202;

describe("When response is accepted without body", () => {
  beforeEach(() => {
    globalThis.fetch = jest.fn().mockResolvedValue(new Response(null, { status: acceptedStatus }));
  });

  it("Then request resolves without error", async () => {
    await expect(
      anonymousHttpClient.post("/api/authentication/password-reset-request", {
        email: "owner@example.com",
      }),
    ).resolves.toBeUndefined();
  });
});
