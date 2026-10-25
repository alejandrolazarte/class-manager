import { nodeFileSystem } from "@/testing/nodeFileSystem";
import { createWebExport, readExportedFile } from "../createWebExport";

const { writeAndroidAssetLinks } = jest.requireActual("../../writeAndroidAssetLinks.js");

const signingCertificateFingerprint =
  "14:6d:e9:83:c5:73:06:50:d8:ee:b9:95:2f:34:fc:64:16:a0:83:42:e6:1d:be:a8:8a:04:96:b2:3f:cf:44:e5";

describe("When the android signing certificate is set", () => {
  let outputDirectory: string;

  beforeEach(() => {
    outputDirectory = createWebExport({ "index.html": "<html></html>" });
  });

  afterEach(() => {
    nodeFileSystem.rmSync(outputDirectory, { recursive: true, force: true });
  });

  it("Then the asset links let the app use passkeys", () => {
    writeAndroidAssetLinks(outputDirectory, signingCertificateFingerprint);

    const assetLinks = JSON.parse(readExportedFile(outputDirectory, ".well-known/assetlinks.json"));
    expect(assetLinks).toEqual([
      {
        relation: ["delegate_permission/common.get_login_creds"],
        target: {
          namespace: "android_app",
          package_name: "com.alejandrolazarte.classmanager",
          sha256_cert_fingerprints: [signingCertificateFingerprint.toUpperCase()],
        },
      },
    ]);
  });
});
