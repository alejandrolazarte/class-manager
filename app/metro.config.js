const { getDefaultConfig } = require("expo/metro-config");
const { withNativeWind } = require("nativewind/metro");

const metroConfig = getDefaultConfig(__dirname);

module.exports = withNativeWind(metroConfig, { input: "./src/global.css" });
