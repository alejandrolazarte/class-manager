import {
  Session,
  anyRegularBusinessCredentials,
  largeBusinessOfThisUser,
  pick,
  regularBusinessOfThisUser,
  signIn,
  thinkTime,
} from "../lib/api.js";
import {
  browseStudents,
  openApp,
  planClasses,
  registerClient,
  reviewFees,
  takeAttendance,
} from "../lib/flows.js";

const peakUsers = Number(__ENV.LOAD_PEAK_USERS || 100);
const rampSeconds = Number(__ENV.LOAD_RAMP_SECONDS || 60);
const holdSeconds = Number(__ENV.LOAD_HOLD_SECONDS || 180);
const signInsPerSecond = Number(__ENV.LOAD_SIGN_INS_PER_SECOND || 1);

function rampingUsers(share) {
  const target = Math.max(1, Math.round(peakUsers * share));
  return {
    executor: "ramping-vus",
    startVUs: 0,
    stages: [
      { duration: `${rampSeconds}s`, target },
      { duration: `${holdSeconds}s`, target },
      { duration: "20s", target: 0 },
    ],
    gracefulRampDown: "10s",
  };
}

export const options = {
  scenarios: {
    attendance: { ...rampingUsers(0.35), exec: "attendance" },
    students: { ...rampingUsers(0.25), exec: "students" },
    fees: { ...rampingUsers(0.15), exec: "fees" },
    classes: { ...rampingUsers(0.15), exec: "classes" },
    newClients: { ...rampingUsers(0.05), exec: "newClients" },
    largeBusiness: { ...rampingUsers(0.05), exec: "largeBusiness" },
    signIns: {
      executor: "constant-arrival-rate",
      rate: signInsPerSecond,
      timeUnit: "1s",
      duration: `${rampSeconds + holdSeconds}s`,
      preAllocatedVUs: 10,
      maxVUs: 50,
      exec: "signIns",
    },
  },
  thresholds: {
    http_req_failed: ["rate<0.01"],
    http_req_duration: ["p(95)<500"],
  },
  summaryTrendStats: ["avg", "med", "p(90)", "p(95)", "p(99)", "max", "count"],
};

let session;

function sessionFor(businessOfThisUser) {
  if (!session) {
    session = new Session(businessOfThisUser());
    openApp(session);
  }
  return session;
}

export function attendance() {
  takeAttendance(sessionFor(regularBusinessOfThisUser));
  thinkTime(2, 5);
}

export function students() {
  browseStudents(sessionFor(regularBusinessOfThisUser));
  thinkTime(2, 5);
}

export function fees() {
  reviewFees(sessionFor(regularBusinessOfThisUser));
  thinkTime(2, 5);
}

export function classes() {
  planClasses(sessionFor(regularBusinessOfThisUser));
  thinkTime(2, 5);
}

export function newClients() {
  registerClient(sessionFor(regularBusinessOfThisUser));
  thinkTime(5, 10);
}

export function largeBusiness() {
  const current = sessionFor(largeBusinessOfThisUser);
  pick([browseStudents, reviewFees, planClasses, takeAttendance])(current);
  thinkTime(2, 5);
}

export function signIns() {
  signIn(anyRegularBusinessCredentials());
}
