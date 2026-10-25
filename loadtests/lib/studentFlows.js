import { nextSessionDate, pick } from "./api.js";

export function openStudentApp(session) {
  session.get("/api/student-app");
  session.get("/api/student-app/news");
  session.get("/api/student-app/shop");
  session.get("/api/me/accounts");
  session.get("/api/student-app/push-key");
}

function studentClassGroups(session, studentId) {
  return session.business.classGroups.filter((group) => group.studentIds.includes(studentId));
}

export function checkHome(session) {
  session.get("/api/student-app");
}

export function readNews(session) {
  session.get("/api/student-app/news");
  session.request("PUT", "/api/student-app/news/seen", "/api/student-app/news/seen");
}

export function browseClasses(session) {
  const studentId = pick(session.credentials.studentIds);
  session.get(
    `/api/student-app/students/${studentId}/makeups`,
    "/api/student-app/students/{id}/makeups",
  );
  session.get(
    `/api/student-app/students/${studentId}/pack-classes`,
    "/api/student-app/students/{id}/pack-classes",
  );
}

export function checkProgress(session) {
  session.get("/api/student-app/measurements");
}

export function notifyAbsence(session) {
  const studentId = pick(session.credentials.studentIds);
  const classGroup = pick(studentClassGroups(session, studentId));
  if (!classGroup) {
    return;
  }
  const path = `/api/student-app/students/${studentId}/absences/${classGroup.id}/${nextSessionDate(classGroup.weekdays)}`;
  const name = "/api/student-app/students/{id}/absences/{classGroupId}/{date}";
  session.request("PUT", path, name);
  session.get("/api/student-app");
  session.request("DELETE", path, name);
}
