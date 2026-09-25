import { authenticationSession } from "@/api/authenticationSession";
import { httpClient } from "@/api/httpClient";

const accessToken = "access-token";

describe("When request is sent with session", () => {
  beforeEach(() => {
    authenticationSession.startSession(accessToken);
    globalThis.fetch = jest
      .fn()
      .mockResolvedValue(new Response(JSON.stringify([]), { status: 200 }));
  });

  afterEach(() => {
    authenticationSession.reset();
  });

  it("Then bearer token is included", async () => {
    await httpClient.get("/api/employees");

    const [, requestInit] = jest.mocked(globalThis.fetch).mock.calls[0];
    expect(requestInit?.headers).toMatchObject({ Authorization: `Bearer ${accessToken}` });
    expect(requestInit?.headers).not.toHaveProperty("X-Business-Id");
  });
});
