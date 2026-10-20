import { getBackofficeJson, rememberAccessToken } from "@/backoffice/api/backofficeHttpClient";
import { refreshBackofficeSession } from "@/backoffice/session/backofficeSessionApi";

jest.mock("@/backoffice/session/backofficeSessionApi");

describe("When the access token expires", () => {
  beforeEach(() => {
    rememberAccessToken("expired-token");
    jest.mocked(refreshBackofficeSession).mockResolvedValue("new-token");
    globalThis.fetch = jest
      .fn()
      .mockResolvedValueOnce(new Response(null, { status: 401 }))
      .mockResolvedValueOnce(new Response(JSON.stringify({ brands: [] }), { status: 200 }));
  });

  it("Then the request is retried with a new one", async () => {
    const response = await getBackofficeJson<{ brands: unknown[] }>("/api/brands");

    expect(response).toEqual({ brands: [] });
    expect(jest.mocked(globalThis.fetch).mock.calls[1][1]).toEqual({
      headers: { Authorization: "Bearer new-token" },
    });
  });
});
