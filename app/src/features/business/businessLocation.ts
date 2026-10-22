const secureScheme = "https:";
const shareHost = "maps.app.goo.gl";
const shortLinkHost = "goo.gl";
const mapsHost = "maps.google.com";
const mapsPath = "/maps";
const googleHostPattern = /^(www\.)?google\.[a-z]{2,3}(\.[a-z]{2})?$/;

export const locationNameMaximumLength = 200;
export const locationMapsUrlMaximumLength = 500;

export interface BusinessLocationValues {
  locationName: string;
  locationMapsUrl: string;
}

export interface BusinessLocationRequest {
  locationName: string | null;
  locationMapsUrl: string | null;
}

function parseUrl(link: string): URL | null {
  try {
    return new URL(link);
  } catch {
    return null;
  }
}

function isMapsPath(url: URL): boolean {
  const path = url.pathname.toLowerCase();
  return path === mapsPath || path.startsWith(`${mapsPath}/`);
}

export function isGoogleMapsLink(link: string): boolean {
  const url = parseUrl(link);
  if (url === null || url.protocol !== secureScheme) {
    return false;
  }
  const host = url.hostname.toLowerCase();
  return (
    host === shareHost ||
    host === mapsHost ||
    (host === shortLinkHost && isMapsPath(url)) ||
    (googleHostPattern.test(host) && isMapsPath(url))
  );
}

export function locationValuesOf(location: BusinessLocationRequest): BusinessLocationValues {
  return {
    locationName: location.locationName ?? "",
    locationMapsUrl: location.locationMapsUrl ?? "",
  };
}

export function hasLocationMapsUrl(values: BusinessLocationValues): boolean {
  return values.locationMapsUrl.trim() !== "";
}

export function isLocationMapsUrlValid(values: BusinessLocationValues): boolean {
  return !hasLocationMapsUrl(values) || isGoogleMapsLink(values.locationMapsUrl.trim());
}

export function isLocationComplete(values: BusinessLocationValues): boolean {
  return (
    isLocationMapsUrlValid(values) &&
    (!hasLocationMapsUrl(values) || values.locationName.trim() !== "")
  );
}

export function toLocationRequest(values: BusinessLocationValues): BusinessLocationRequest {
  const locationName = values.locationName.trim();
  const locationMapsUrl = values.locationMapsUrl.trim();
  return {
    locationName: locationName === "" ? null : locationName,
    locationMapsUrl: locationMapsUrl === "" ? null : locationMapsUrl,
  };
}
