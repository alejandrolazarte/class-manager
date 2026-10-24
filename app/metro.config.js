const { getSentryExpoConfig } = require("@sentry/react-native/metro");
const { withNativeWind } = require("nativewind/metro");

const metroConfig = getSentryExpoConfig(__dirname);

module.exports = withNativeWind(metroConfig, { input: "./src/global.css" });
