import { currentMonth, isoDate, businessToday, lastSessionDate, pick, thinkTime } from "./api.js";

export function openApp(session) {
  session.get("/api/me");
  session.get("/api/business");
  session.get("/api/me/accounts");
  session.get("/api/organization/subscription");
  session.get("/api/team/notifications");
}

export function takeAttendance(session) {
  const today = isoDate(businessToday());
  session.get(`/api/sessions?date=${today}`, "/api/sessions?date");
  thinkTime();
  const classGroup = pick(
    session.business.classGroups.filter((group) => group.studentIds.length > 0),
  );
  const sessionDate = lastSessionDate(classGroup.weekdays);
  session.get(
    `/api/class-groups/${classGroup.id}/sessions/${sessionDate}`,
    "/api/class-groups/{id}/sessions/{date}",
  );
  thinkTime();
  session.request(
    "PUT",
    `/api/class-groups/${classGroup.id}/sessions/${sessionDate}/attendance`,
    "/api/class-groups/{id}/sessions/{date}/attendance",
    {
      students: classGroup.studentIds.map((studentId) => ({
        studentId,
        status: Math.random() < 0.85 ? "Present" : "Absent",
      })),
    },
  );
  thinkTime();
  const studentId = pick(classGroup.studentIds);
  session.request(
    "PUT",
    `/api/class-groups/${classGroup.id}/sessions/${sessionDate}/attendance/${studentId}`,
    "/api/class-groups/{id}/sessions/{date}/attendance/{studentId}",
    { status: "Present" },
  );
}

export function browseStudents(session) {
  session.get("/api/students?limit=20", "/api/students?limit");
  thinkTime();
  session.get(
    `/api/students?search=Client%20${Math.ceil(Math.random() * 99)}&limit=20`,
    "/api/students?search",
  );
  thinkTime();
  const client = pick(session.business.clients);
  session.get(`/api/clients/${client.id}`, "/api/clients/{id}");
  session.get(`/api/clients/${client.id}/payments`, "/api/clients/{id}/payments");
  session.get(
    `/api/students/${pick(client.studentIds)}/enrollments`,
    "/api/students/{id}/enrollments",
  );
}

export function reviewFees(session) {
  session.get(`/api/fees?month=${currentMonth()}`, "/api/fees?month");
  thinkTime();
  const client = pick(session.business.clients);
  const payment = session.request(
    "POST",
    `/api/clients/${client.id}/payments`,
    "/api/clients/{id}/payments",
    {
      amount: 1000,
      month: currentMonth(),
      paidOn: isoDate(businessToday()),
      method: "Transfer",
      notes: null,
    },
  );
  thinkTime();
  session.get(`/api/fees?month=${currentMonth()}`, "/api/fees?month");
  if (payment.status === 201 || payment.status === 200) {
    session.request("DELETE", `/api/payments/${payment.json("id")}`, "/api/payments/{id}");
  }
}

export function planClasses(session) {
  session.get("/api/class-groups");
  thinkTime();
  const classGroup = pick(session.business.classGroups);
  session.get(`/api/class-groups/${classGroup.id}`, "/api/class-groups/{id}");
  session.get(
    `/api/class-groups/${classGroup.id}/enrollments`,
    "/api/class-groups/{id}/enrollments",
  );
  thinkTime();
  session.get(`/api/sessions/calendar?month=${currentMonth()}`, "/api/sessions/calendar?month");
}

const classGroupFullStatus = 409;

export function registerClient(session) {
  const suffix = `${__VU}-${__ITER}-${Date.now()}`;
  const uniquePhoneDigits = String(Date.now() * 1000 + __VU).slice(-10);
  const client = session.request("POST", "/api/clients", "/api/clients", {
    fullName: `New Client ${suffix}`,
    phoneNumber: uniquePhoneDigits,
    email: null,
    notes: null,
    students: [{ fullName: `New Student ${suffix}`, birthDate: "2013-05-05", notes: null }],
  });
  if (client.status !== 201) {
    return;
  }
  thinkTime();
  const classGroup = pick(session.business.classGroups);
  session.request(
    "POST",
    `/api/class-groups/${classGroup.id}/enrollments`,
    "/api/class-groups/{id}/enrollments",
    {
      studentId: client.json("students.0.id"),
      startDate: null,
    },
    [classGroupFullStatus],
  );
}
