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

  it("Then unknown paths load the single page app", () => {
    writeStaticWebAppConfig(outputDirectory, apiBaseUrl);

    const config = JSON.parse(readExportedFile(outputDirectory, "staticwebapp.config.json"));
    expect(config.navigationFallback.rewrite).toBe("/index.html");
    expect(config.navigationFallback.exclude).toContain("/_expo/*");
  });
});
