const appTargetVariable = "APP_TARGET";
const backofficeTarget = "backoffice";
const backofficeName = "Laztty Admin";
const backofficeRoutesDirectory = "./backoffice";

function appConfig({ config }) {
  if (process.env[appTargetVariable] !== backofficeTarget) {
    return config;
  }
  return {
    ...config,
    name: backofficeName,
    extra: {
      ...config.extra,
      router: { ...config.extra?.router, root: backofficeRoutesDirectory },
    },
  };
}

module.exports = appConfig;
