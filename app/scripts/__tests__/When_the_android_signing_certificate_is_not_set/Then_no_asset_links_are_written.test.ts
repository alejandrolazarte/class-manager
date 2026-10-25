import { nodeFileSystem } from "@/testing/nodeFileSystem";
import { createWebExport } from "../createWebExport";

const { writeAndroidAssetLinks } = jest.requireActual("../../writeAndroidAssetLinks.js");

describe("When the android signing certificate is not set", () => {
  let outputDirectory: string;

  beforeEach(() => {
    outputDirectory = createWebExport({ "index.html": "<html></html>" });
  });

  afterEach(() => {
    nodeFileSystem.rmSync(outputDirectory, { recursive: true, force: true });
  });

  it("Then no asset links are written", () => {
    writeAndroidAssetLinks(outputDirectory, undefined);

    expect(nodeFileSystem.existsSync(`${outputDirectory}/.well-known`)).toBe(false);
  });
});
