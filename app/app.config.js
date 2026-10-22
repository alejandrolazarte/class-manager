const appTargetVariable = "APP_TARGET";
const backofficeTarget = "backoffice";
const backofficeName = "Laztty Admin";
const backofficeRoutesDirectory = "./backoffice";
const backofficeFavicon = "./backoffice-web/favicon.png";

function appConfig({ config }) {
  if (process.env[appTargetVariable] !== backofficeTarget) {
    return config;
  }
  return {
    ...config,
    name: backofficeName,
    web: { ...config.web, favicon: backofficeFavicon },
    extra: {
      ...config.extra,
      router: { ...config.extra?.router, root: backofficeRoutesDirectory },
    },
  };
}

module.exports = appConfig;
