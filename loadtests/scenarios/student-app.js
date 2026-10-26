import { Session, pick, studentAccountOfThisUser, thinkTime } from "../lib/api.js";
import {
  browseClasses,
  checkHome,
  checkProgress,
  notifyAbsence,
  openStudentApp,
  readNews,
} from "../lib/studentFlows.js";

const peakUsers = Number(__ENV.LOAD_PEAK_USERS || 100);
const rampSeconds = Number(__ENV.LOAD_RAMP_SECONDS || 60);
const holdSeconds = Number(__ENV.LOAD_HOLD_SECONDS || 180);

export const options = {
  scenarios: {
    families: {
      executor: "ramping-vus",
      startVUs: 0,
      stages: [
        { duration: `${rampSeconds}s`, target: peakUsers },
        { duration: `${holdSeconds}s`, target: peakUsers },
        { duration: "20s", target: 0 },
      ],
      gracefulRampDown: "10s",
      exec: "families",
    },
  },
  thresholds: {
    http_req_failed: ["rate<0.01"],
    http_req_duration: ["p(95)<500"],
  },
  summaryTrendStats: ["avg", "med", "p(90)", "p(95)", "p(99)", "max", "count"],
};

const flows = [checkHome, checkHome, readNews, browseClasses, checkProgress, notifyAbsence];

let session;

export function families() {
  if (!session) {
    const { business, account } = studentAccountOfThisUser();
    session = new Session(business, account);
    openStudentApp(session);
  }
  pick(flows)(session);
  thinkTime(3, 8);
}
