import { mkdirSync, readdirSync, readFileSync, rmSync, writeFileSync, existsSync } from "node:fs";
import { randomUUID } from "node:crypto";
import { join } from "node:path";
import { fileURLToPath } from "node:url";
import { SmtpSink } from "../e2e/support/smtpSink.ts";

const loadTestsDirectory = fileURLToPath(new URL(".", import.meta.url));
const dataDirectory = join(loadTestsDirectory, ".data");
const mailboxDirectory = join(dataDirectory, "mailbox");
const businessesFilePath = join(dataDirectory, "businesses.json");

const apiUrl = process.env.LOAD_API_URL ?? "http://localhost:5000";
const smtpPort = Number(process.env.LOAD_SMTP_PORT ?? 2525);
const businessCount = Number(process.env.LOAD_BUSINESSES ?? 20);
const clientsPerBusiness = Number(process.env.LOAD_CLIENTS_PER_BUSINESS ?? 150);
const attendanceWeeks = Number(process.env.LOAD_ATTENDANCE_WEEKS ?? 52);
const studentAccountsPerBusiness = Number(process.env.LOAD_STUDENT_ACCOUNTS_PER_BUSINESS ?? 30);
const businessConcurrency = Number(process.env.LOAD_SEED_CONCURRENCY ?? 4);
const appendToExisting = process.env.LOAD_APPEND === "true";

const ownerPassword = "load-test-owner-password";
const studentAccountPassword = "load-test-student-password";
const studentAccountBirthDate = "1980-04-12";
const timeZoneId = "America/Argentina/Buenos_Aires";
const businessUtcOffsetHours = -3;
const completeSignUpLinkPattern = /complete-sign-up\?token=([^\s&"]+)/;
const acceptStudentInvitationLinkPattern = /accept-student-invitation\?token=([^\s&"]+)/;
const emailWaitMilliseconds = 30_000;
const emailPollMilliseconds = 100;
const enrollmentHistoryWeeks = attendanceWeeks + 2;
const attendanceConcurrency = 8;
const feedbackWeeks = 4;
const feedbacksPerSession = 3;
const announcementCount = 8;
const monthlyFeeAmount = 30000;
const paidClientsRatio = 0.6;
const secondStudentRatio = 0.3;
const secondEnrollmentRatio = 0.4;
const expectedEnrollmentsPerClient = 1.9;
const classGroupCapacity = 100;
const classGroupFillRatio = 0.8;
const daysPerWeek = 7;
const millisecondsPerDay = 86_400_000;

const classSchedules = [
  { weekdays: ["Monday", "Wednesday"], startTime: "08:00" },
  { weekdays: ["Monday", "Wednesday"], startTime: "10:00" },
  { weekdays: ["Monday", "Wednesday"], startTime: "18:00" },
  { weekdays: ["Monday", "Wednesday"], startTime: "19:00" },
  { weekdays: ["Tuesday", "Thursday"], startTime: "08:00" },
  { weekdays: ["Tuesday", "Thursday"], startTime: "10:00" },
  { weekdays: ["Tuesday", "Thursday"], startTime: "18:00" },
  { weekdays: ["Tuesday", "Thursday"], startTime: "19:00" },
  { weekdays: ["Friday"], startTime: "08:00" },
  { weekdays: ["Friday"], startTime: "18:00" },
  { weekdays: ["Saturday"], startTime: "09:00" },
  { weekdays: ["Saturday"], startTime: "11:00" },
];
const weekdayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

function businessToday() {
  return new Date(Date.now() + businessUtcOffsetHours * 3_600_000);
}

function isoDate(date) {
  return date.toISOString().slice(0, 10);
}

function addDays(date, days) {
  return new Date(date.getTime() + days * millisecondsPerDay);
}

function currentMonth() {
  return isoDate(businessToday()).slice(0, 7);
}

async function call(method, path, body, accessToken) {
  const response = await fetch(`${apiUrl}${path}`, {
    method,
    headers: {
      "Content-Type": "application/json",
      ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
    },
    body: body === undefined ? undefined : JSON.stringify(body),
  });
  if (!response.ok) {
    throw new Error(`${method} ${path} answered ${response.status}: ${await response.text()}`);
  }
  const text = await response.text();
  return text ? JSON.parse(text) : undefined;
}

const receivedEmails = new Map();

function readNewEmails() {
  for (const fileName of readdirSync(mailboxDirectory)) {
    if (!receivedEmails.has(fileName)) {
      receivedEmails.set(
        fileName,
        JSON.parse(readFileSync(join(mailboxDirectory, fileName), "utf8")),
      );
    }
  }
}

async function waitForLinkToken(email, linkPattern) {
  const deadline = Date.now() + emailWaitMilliseconds;
  while (Date.now() < deadline) {
    readNewEmails();
    for (const received of receivedEmails.values()) {
      const token = linkPattern.exec(received.body)?.[1];
      if (received.to.includes(email) && token) {
        return decodeURIComponent(token);
      }
    }
    await new Promise((resolve) => setTimeout(resolve, emailPollMilliseconds));
  }
  throw new Error(`No email with ${linkPattern} reached ${email}.`);
}

async function inBatches(items, concurrency, work) {
  const results = new Array(items.length);
  let nextIndex = 0;
  const workers = Array.from({ length: Math.min(concurrency, items.length) }, async () => {
    while (nextIndex < items.length) {
      const index = nextIndex++;
      results[index] = await work(items[index], index);
    }
  });
  await Promise.all(workers);
  return results;
}

async function signUp(businessNumber) {
  const email = `load-owner-${randomUUID()}@load.local`;
  await call("POST", "/api/auth/sign-up", {
    ownerFullName: `Load Owner ${businessNumber}`,
    email,
    businessName: `Load Business ${businessNumber} ${randomUUID().slice(0, 6)}`,
    timeZoneId,
    currencyCode: "ARS",
    countryCode: "AR",
    ownerBirthDate: "1985-06-20",
    acceptsLegalTerms: true,
  });
  const token = await waitForLinkToken(email, completeSignUpLinkPattern);
  const tokens = await call("POST", "/api/auth/sign-up/complete", {
    token,
    password: ownerPassword,
  });
  return { email, accessToken: tokens.accessToken };
}

async function createStudentAccount(post, client) {
  const email = `load-family-${randomUUID()}@load.local`;
  await post(`/api/clients/${client.id}/app-invitation`, { email, confirmsContactIsAdult: true });
  const token = await waitForLinkToken(email, acceptStudentInvitationLinkPattern);
  await call("POST", "/api/auth/student-app-invitations/accept", {
    token,
    fullName: null,
    password: studentAccountPassword,
    birthDate: studentAccountBirthDate,
    acceptsLegalTerms: true,
  });
  return {
    email,
    password: studentAccountPassword,
    clientId: client.id,
    studentIds: client.studentIds,
  };
}

function pastSessionDates(weekdays, weeks) {
  const today = businessToday();
  const dates = [];
  for (let daysAgo = 1; daysAgo <= weeks * daysPerWeek; daysAgo++) {
    const day = addDays(today, -daysAgo);
    if (weekdays.includes(weekdayNames[day.getUTCDay()])) {
      dates.push(isoDate(day));
    }
  }
  return dates;
}

async function seedBusiness(businessNumber) {
  const { email, accessToken } = await signUp(businessNumber);
  const post = (path, body) => call("POST", path, body, accessToken);
  const put = (path, body) => call("PUT", path, body, accessToken);

  await put("/api/business/monthly-fee", {
    amount: monthlyFeeAmount,
    effectiveFrom: currentMonth(),
  });
  const instructors = await call("GET", "/api/instructors", undefined, accessToken);
  const instructorIds = [instructors[0].id];
  const classGroupCount = Math.max(
    classSchedules.length,
    Math.ceil(
      (clientsPerBusiness * expectedEnrollmentsPerClient) /
        (classGroupCapacity * classGroupFillRatio),
    ),
  );
  while (instructorIds.length * classSchedules.length < classGroupCount) {
    const instructor = await post("/api/instructors", {
      fullName: `Instructor ${instructorIds.length + 1}`,
    });
    instructorIds.push(instructor.id);
  }

  const classGroups = [];
  for (let index = 0; index < classGroupCount; index++) {
    const schedule = classSchedules[index % classSchedules.length];
    const classGroup = await post("/api/class-groups", {
      name: `Group ${index + 1}`,
      instructorId: instructorIds[Math.floor(index / classSchedules.length)],
      weekdays: schedule.weekdays,
      startTime: schedule.startTime,
      durationMinutes: 50,
      capacity: classGroupCapacity,
      location: "Room A",
    });
    classGroups.push({ id: classGroup.id, weekdays: schedule.weekdays, studentIds: [] });
  }

  const enrollmentStartDate = isoDate(
    addDays(businessToday(), -enrollmentHistoryWeeks * daysPerWeek),
  );
  const clientNumbers = Array.from({ length: clientsPerBusiness }, (_, index) => index + 1);
  const clients = await inBatches(clientNumbers, 4, async (clientNumber) => {
    const students = [
      { fullName: `Student ${clientNumber} A`, birthDate: "2012-03-15", notes: null },
    ];
    if (Math.random() < secondStudentRatio) {
      students.push({
        fullName: `Student ${clientNumber} B`,
        birthDate: "2014-08-01",
        notes: null,
      });
    }
    const client = await post("/api/clients", {
      fullName: `Client ${clientNumber} Surname${clientNumber % 37}`,
      phoneNumber: `11 5555-${String(clientNumber).padStart(4, "0")}`,
      email: null,
      notes: null,
      students,
    });
    const studentIds = client.students.map((student) => student.id);
    for (const [studentIndex, studentId] of studentIds.entries()) {
      const firstGroup = classGroups[(clientNumber + studentIndex) % classGroups.length];
      const enrolledGroups = [firstGroup];
      if (Math.random() < secondEnrollmentRatio) {
        enrolledGroups.push(classGroups[(clientNumber + studentIndex + 5) % classGroups.length]);
      }
      for (const group of enrolledGroups) {
        await post(`/api/class-groups/${group.id}/enrollments`, {
          studentId,
          startDate: enrollmentStartDate,
        });
        group.studentIds.push(studentId);
      }
    }
    if (Math.random() < paidClientsRatio) {
      await post(`/api/clients/${client.id}/payments`, {
        amount: monthlyFeeAmount,
        month: currentMonth(),
        paidOn: isoDate(businessToday()),
        method: "Cash",
        notes: null,
      });
    }
    return { id: client.id, studentIds };
  });

  const attendanceSessions = classGroups
    .filter((group) => group.studentIds.length > 0)
    .flatMap((group) =>
      pastSessionDates(group.weekdays, attendanceWeeks).map((sessionDate) => ({
        group,
        sessionDate,
      })),
    );
  await inBatches(attendanceSessions, attendanceConcurrency, ({ group, sessionDate }) =>
    put(`/api/class-groups/${group.id}/sessions/${sessionDate}/attendance`, {
      students: group.studentIds.map((studentId) => ({
        studentId,
        status: Math.random() < 0.85 ? "Present" : "Absent",
      })),
    }),
  );

  const feedbackSessions = attendanceSessions.filter(
    ({ sessionDate }) =>
      sessionDate >= isoDate(addDays(businessToday(), -feedbackWeeks * daysPerWeek)),
  );
  await inBatches(feedbackSessions, attendanceConcurrency, async ({ group, sessionDate }) => {
    for (const studentId of group.studentIds.slice(0, feedbacksPerSession)) {
      await put(`/api/class-groups/${group.id}/sessions/${sessionDate}/feedback/${studentId}`, {
        text: "Good progress today.",
      });
    }
  });

  for (let index = 0; index < announcementCount; index++) {
    const isForOneClass = index % 2 === 1;
    await post("/api/announcements", {
      title: `Announcement ${index + 1}`,
      body: "The schedule changes next week.",
      classGroupIds: isForOneClass ? [classGroups[index % classGroups.length].id] : null,
    });
  }

  const studentAccounts = await inBatches(
    clients.slice(0, studentAccountsPerBusiness),
    4,
    (client) => createStudentAccount(post, client),
  );

  console.log(
    `business ${businessNumber}: ${clients.length} clients, ${attendanceSessions.length} sessions with attendance, ${studentAccounts.length} student app accounts`,
  );
  return {
    email,
    password: ownerPassword,
    classGroups: classGroups.map((group) => ({
      id: group.id,
      weekdays: group.weekdays,
      studentIds: group.studentIds,
    })),
    clients,
    studentAccounts,
  };
}

mkdirSync(dataDirectory, { recursive: true });
rmSync(mailboxDirectory, { recursive: true, force: true });
mkdirSync(mailboxDirectory, { recursive: true });
const smtpSink = new SmtpSink(mailboxDirectory);
await smtpSink.start(smtpPort);

const startedAt = Date.now();
const existing =
  appendToExisting && existsSync(businessesFilePath)
    ? JSON.parse(readFileSync(businessesFilePath, "utf8"))
    : [];
const businessNumbers = Array.from(
  { length: businessCount },
  (_, index) => existing.length + index + 1,
);
try {
  const businesses = await inBatches(businessNumbers, businessConcurrency, seedBusiness);
  writeFileSync(businessesFilePath, JSON.stringify([...existing, ...businesses]));
  console.log(
    `Seeded ${businesses.length} businesses in ${Math.round((Date.now() - startedAt) / 1000)} s into ${businessesFilePath}`,
  );
} finally {
  await smtpSink.stop();
}
