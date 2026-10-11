import { nodeFileSystem } from "@/testing/nodeFileSystem";
import { createWebExport, readExportedFile } from "../createWebExport";

const { writeWebHeaders } = jest.requireActual("../../writeWebHeaders.js");

const apiBaseUrl = "https://api.example.com";

describe("When web headers are written", () => {
  let outputDirectory: string;

  beforeEach(() => {
    outputDirectory = createWebExport({ "index.html": "<html></html>" });
  });

  afterEach(() => {
    nodeFileSystem.rmSync(outputDirectory, { recursive: true, force: true });
  });

  it("Then email fonts can be loaded from any origin", () => {
    writeWebHeaders(outputDirectory, apiBaseUrl);

    const headers = readExportedFile(outputDirectory, "_headers");
    expect(headers).toContain(
      "/fonts/*\n  Access-Control-Allow-Origin: *\n  Cache-Control: public, max-age=31536000, immutable\n",
    );
  });
});
