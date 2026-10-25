const { mkdirSync, writeFileSync } = require("node:fs");
const { join } = require("node:path");
const appConfig = require("../app.json");

const wellKnownDirectory = ".well-known";
const assetLinksFileName = "assetlinks.json";
const getLoginCredentialsRelation = "delegate_permission/common.get_login_creds";
const androidAppNamespace = "android_app";
const certificateFingerprintPattern = /^([0-9A-F]{2}:){31}[0-9A-F]{2}$/;
const defaultOutputDirectory = "dist";
const jsonIndentation = 2;

function androidAssetLinks(signingCertificateFingerprint) {
  return [
    {
      relation: [getLoginCredentialsRelation],
      target: {
        namespace: androidAppNamespace,
        package_name: appConfig.expo.android.package,
        sha256_cert_fingerprints: [signingCertificateFingerprint],
      },
    },
  ];
}

function writeAndroidAssetLinks(outputDirectory, signingCertificateFingerprint) {
  if (!signingCertificateFingerprint) {
    return false;
  }
  const fingerprint = signingCertificateFingerprint.trim().toUpperCase();
  if (!certificateFingerprintPattern.test(fingerprint)) {
    throw new Error(
      "ANDROID_SIGNING_CERT_SHA256 must be the SHA-256 fingerprint of the signing certificate, 32 bytes as AA:BB:…",
    );
  }
  const directory = join(outputDirectory, wellKnownDirectory);
  mkdirSync(directory, { recursive: true });
  writeFileSync(
    join(directory, assetLinksFileName),
    `${JSON.stringify(androidAssetLinks(fingerprint), null, jsonIndentation)}\n`,
  );
  return true;
}

if (require.main === module) {
  const outputDirectory = process.argv[2] ?? defaultOutputDirectory;
  const isWritten = writeAndroidAssetLinks(
    outputDirectory,
    process.env.ANDROID_SIGNING_CERT_SHA256,
  );
  console.log(
    isWritten
      ? `Wrote ${wellKnownDirectory}/${assetLinksFileName} in ${outputDirectory}`
      : `Skipped ${wellKnownDirectory}/${assetLinksFileName}: ANDROID_SIGNING_CERT_SHA256 is not set`,
  );
}

module.exports = { writeAndroidAssetLinks };
