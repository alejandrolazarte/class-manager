import http from "k6/http";
import { check, fail, sleep } from "k6";
import { SharedArray } from "k6/data";

export const apiUrl = __ENV.LOAD_API_URL || "http://localhost:5000";

const accessTokenRefreshMilliseconds = 10 * 60 * 1000;
const businessUtcOffsetHours = -3;
const millisecondsPerDay = 86_400_000;
const weekdayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

const largeBusinessMinimumClients = 500;

const businesses = new SharedArray("businesses", () =>
  JSON.parse(open("../.data/businesses.json")),
);
const businessClientCounts = new SharedArray("businessClientCounts", () =>
  JSON.parse(open("../.data/businesses.json")).map((business) => [business.clients.length]),
);

function businessIndexes(isWanted) {
  return Array.from({ length: businessClientCounts.length }, (_, index) => index).filter((index) =>
    isWanted(businessClientCounts[index][0]),
  );
}

const regularBusinessIndexes = businessIndexes(
  (clientCount) => clientCount < largeBusinessMinimumClients,
);
const largeBusinessIndexes = businessIndexes(
  (clientCount) => clientCount >= largeBusinessMinimumClients,
);

export function regularBusinessOfThisUser() {
  return businesses[regularBusinessIndexes[(__VU - 1) % regularBusinessIndexes.length]];
}

export function largeBusinessOfThisUser() {
  return businesses[largeBusinessIndexes[(__VU - 1) % largeBusinessIndexes.length]];
}

export function anyRegularBusinessCredentials() {
  const business = businesses[pick(regularBusinessIndexes)];
  return { email: business.email, password: business.password };
}

export function pick(items) {
  return items[Math.floor(Math.random() * items.length)];
}

export function thinkTime(minimumSeconds = 1, maximumSeconds = 3) {
  sleep(minimumSeconds + Math.random() * (maximumSeconds - minimumSeconds));
}

export function businessToday() {
  return new Date(Date.now() + businessUtcOffsetHours * 3_600_000);
}

export function isoDate(date) {
  return date.toISOString().slice(0, 10);
}

export function currentMonth() {
  return isoDate(businessToday()).slice(0, 7);
}

export function lastSessionDate(weekdays) {
  const today = businessToday();
  for (let daysAgo = 0; daysAgo < 7; daysAgo++) {
    const day = new Date(today.getTime() - daysAgo * millisecondsPerDay);
    if (weekdays.includes(weekdayNames[day.getUTCDay()])) {
      return isoDate(day);
    }
  }
  return isoDate(today);
}

export function signIn(business) {
  const response = http.post(
    `${apiUrl}/api/auth/sign-in`,
    JSON.stringify({ email: business.email, password: business.password }),
    { headers: { "Content-Type": "application/json" }, tags: { name: "POST /api/auth/sign-in" } },
  );
  if (!check(response, { "sign-in 200": (r) => r.status === 200 })) {
    fail(`sign-in answered ${response.status}`);
  }
  return response.json("accessToken");
}

export class Session {
  constructor(business) {
    this.business = business;
    this.signIn();
  }

  signIn() {
    this.accessToken = signIn(this.business);
    this.signedInAt = Date.now();
  }

  request(method, path, name, body, acceptedStatuses = []) {
    if (Date.now() - this.signedInAt > accessTokenRefreshMilliseconds) {
      this.signIn();
    }
    const response = http.request(
      method,
      `${apiUrl}${path}`,
      body === undefined ? null : JSON.stringify(body),
      {
        headers: {
          Authorization: `Bearer ${this.accessToken}`,
          "Content-Type": "application/json",
        },
        tags: {
          name: `${method} ${name}`,
          businessSize:
            this.business.clients.length >= largeBusinessMinimumClients ? "large" : "regular",
        },
        responseCallback: http.expectedStatuses({ min: 200, max: 299 }, ...acceptedStatuses),
      },
    );
    check(response, {
      [`${method} ${name} ok`]: (r) =>
        (r.status >= 200 && r.status < 300) || acceptedStatuses.includes(r.status),
    });
    return response;
  }

  get(path, name) {
    return this.request("GET", path, name ?? path);
  }
}
