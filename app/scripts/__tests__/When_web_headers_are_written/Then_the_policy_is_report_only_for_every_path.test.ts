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

  it("Then the policy is report only for every path", () => {
    writeWebHeaders(outputDirectory, apiBaseUrl);

    const headers = readExportedFile(outputDirectory, "_headers");
    expect(headers.startsWith("/*\n  Content-Security-Policy-Report-Only: ")).toBe(true);
    expect(headers).not.toContain("Content-Security-Policy:");
    expect(headers).toContain("script-src 'self';");
    expect(headers).not.toContain("unsafe-eval");
  });
});
