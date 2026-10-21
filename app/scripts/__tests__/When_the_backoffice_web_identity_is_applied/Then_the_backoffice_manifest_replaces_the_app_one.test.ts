import { nodeFileSystem } from "@/testing/nodeFileSystem";
import { createWebExport, readExportedFile } from "../createWebExport";
import { appIndexHtml, backofficeIdentitySource } from "../createBackofficeIdentitySource";

const { applyBackofficeWebIdentity } = jest.requireActual("../../applyBackofficeWebIdentity.js");

describe("When the backoffice web identity is applied", () => {
  let outputDirectory: string;
  let sourceDirectory: string;

  beforeEach(() => {
    outputDirectory = createWebExport({
      "index.html": appIndexHtml,
      "manifest.webmanifest": '{"name":"Laztty"}',
    });
    sourceDirectory = backofficeIdentitySource();
  });

  afterEach(() => {
    nodeFileSystem.rmSync(outputDirectory, { recursive: true, force: true });
    nodeFileSystem.rmSync(sourceDirectory, { recursive: true, force: true });
  });

  it("Then the backoffice manifest replaces the app one", () => {
    applyBackofficeWebIdentity(outputDirectory, sourceDirectory);

    expect(JSON.parse(readExportedFile(outputDirectory, "manifest.webmanifest")).name).toBe(
      "Laztty Admin",
    );
  });
});
