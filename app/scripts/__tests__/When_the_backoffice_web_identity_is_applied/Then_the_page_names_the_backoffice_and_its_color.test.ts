import { nodeFileSystem } from "@/testing/nodeFileSystem";
import { createWebExport, readExportedFile } from "../createWebExport";
import { appIndexHtml, backofficeIdentitySource } from "../createBackofficeIdentitySource";

const { applyBackofficeWebIdentity } = jest.requireActual("../../applyBackofficeWebIdentity.js");

describe("When the backoffice web identity is applied", () => {
  let outputDirectory: string;
  let sourceDirectory: string;

  beforeEach(() => {
    outputDirectory = createWebExport({ "index.html": appIndexHtml });
    sourceDirectory = backofficeIdentitySource();
  });

  afterEach(() => {
    nodeFileSystem.rmSync(outputDirectory, { recursive: true, force: true });
    nodeFileSystem.rmSync(sourceDirectory, { recursive: true, force: true });
  });

  it("Then the page names the backoffice and its color", () => {
    applyBackofficeWebIdentity(outputDirectory, sourceDirectory);

    const indexHtml = readExportedFile(outputDirectory, "index.html");
    expect(indexHtml).toContain('<meta name="theme-color" content="#334155" />');
    expect(indexHtml).toContain(
      '<meta name="apple-mobile-web-app-title" content="Laztty Admin" />',
    );
    expect(indexHtml).toContain(
      '<meta name="description" content="Backoffice de Laztty: marcas, planes, add-ons y operadores." />',
    );
  });
});
