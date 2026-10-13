import { nodeFileSystem } from "@/testing/nodeFileSystem";
import { createWebExport, readExportedFile } from "../createWebExport";

const { writeStaticWebAppConfig } = jest.requireActual("../../writeStaticWebAppConfig.js");

const apiBaseUrl = "https://api.example.com";

describe("When static web app config is written", () => {
  let outputDirectory: string;

  beforeEach(() => {
    outputDirectory = createWebExport({ "index.html": "<html></html>" });
  });

  afterEach(() => {
    nodeFileSystem.rmSync(outputDirectory, { recursive: true, force: true });
  });

  it("Then the policy is report only for every path", () => {
    writeStaticWebAppConfig(outputDirectory, apiBaseUrl);

    const config = JSON.parse(readExportedFile(outputDirectory, "staticwebapp.config.json"));
    const policy: string = config.globalHeaders["Content-Security-Policy-Report-Only"];
    expect(config.globalHeaders["Content-Security-Policy"]).toBeUndefined();
    expect(policy).toContain("script-src 'self';");
    expect(policy).not.toContain("unsafe-eval");
  });
});
