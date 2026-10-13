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

  it("Then email fonts can be loaded from any origin", () => {
    writeStaticWebAppConfig(outputDirectory, apiBaseUrl);

    const config = JSON.parse(readExportedFile(outputDirectory, "staticwebapp.config.json"));
    expect(config.routes).toContainEqual({
      route: "/fonts/*",
      headers: {
        "Access-Control-Allow-Origin": "*",
        "Cache-Control": "public, max-age=31536000, immutable",
      },
    });
  });
});
