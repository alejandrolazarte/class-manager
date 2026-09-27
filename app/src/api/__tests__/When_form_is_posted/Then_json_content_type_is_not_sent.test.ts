import { authenticationSession } from "@/api/authenticationSession";
import { httpClient } from "@/api/httpClient";

const accessToken = "access-token";

describe("When form is posted", () => {
  beforeEach(() => {
    authenticationSession.startSession(accessToken);
    globalThis.fetch = jest
      .fn()
      .mockResolvedValue(new Response(JSON.stringify({}), { status: 200 }));
  });

  afterEach(() => {
    authenticationSession.reset();
  });

  it("Then json content type is not sent", async () => {
    const form = new FormData();

    await httpClient.postForm("/api/import-export/students/preview", form);

    const [, requestInit] = jest.mocked(globalThis.fetch).mock.calls[0];
    expect(requestInit?.body).toBe(form);
    expect(requestInit?.headers).not.toHaveProperty("Content-Type");
    expect(requestInit?.headers).toMatchObject({ Authorization: `Bearer ${accessToken}` });
  });
});
