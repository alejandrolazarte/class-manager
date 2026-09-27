import { authenticationSession } from "@/api/authenticationSession";
import { httpClient } from "@/api/httpClient";

const workbookBytes = new Uint8Array([0x50, 0x4b, 0x03, 0x04, 0xc3, 0xa9, 0xff]);

describe("When a file is downloaded", () => {
  beforeEach(() => {
    authenticationSession.startSession("access-token");
    globalThis.fetch = jest.fn().mockResolvedValue(new Response(workbookBytes, { status: 200 }));
  });

  afterEach(() => {
    authenticationSession.reset();
  });

  it("Then the bytes are returned unchanged", async () => {
    const content = await httpClient.getBytes("/api/import-export/students/export");

    expect(new Uint8Array(content)).toEqual(workbookBytes);
  });
});
