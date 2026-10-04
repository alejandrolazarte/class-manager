import { APIRequestContext, expect } from "@playwright/test";
import { randomUUID } from "node:crypto";
import { e2eEnvironment } from "../support/environment";

export const demoPassword = "demo-owner-password";
export const demoTimeZoneId = "America/Argentina/Buenos_Aires";

const weekOrder = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

interface Identified {
  id: string;
}

interface RegisteredClient extends Identified {
  students: { id: string; fullName: string }[];
}

export interface DemoBusiness {
  email: string;
  today: string;
  month: string;
  beginnersClassGroupId: string;
  aquagymClassGroupId: string;
  studentAppClientId: string;
  debtorClientId: string;
  classPackClientId: string;
}

function todayIn(timeZoneId: string): string {
  return new Intl.DateTimeFormat("en-CA", { timeZone: timeZoneId }).format(new Date());
}

function weekdayOf(isoDate: string): string {
  return weekOrder[new Date(`${isoDate}T12:00:00Z`).getUTCDay()] ?? "Monday";
}

function nextMonthOf(month: string): string {
  const [year, monthNumber] = month.split("-").map(Number);
  const nextMonth = new Date(Date.UTC(year ?? 2026, monthNumber ?? 1, 1));
  return nextMonth.toISOString().slice(0, 7);
}

function withToday(weekdays: string[], today: string): string[] {
  return [...new Set([...weekdays, weekdayOf(today)])];
}

export async function seedDemoBusiness(request: APIRequestContext): Promise<DemoBusiness> {
  const today = todayIn(demoTimeZoneId);
  const month = today.slice(0, 7);
  const monthStart = `${month}-01`;
  const email = `demo-${randomUUID()}@demo.local`;
  let accessToken = "";

  async function send<TResponse>(method: string, path: string, data?: unknown): Promise<TResponse> {
    const response = await request.fetch(`${e2eEnvironment.apiUrl}${path}`, {
      method,
      data,
      headers: accessToken ? { Authorization: `Bearer ${accessToken}` } : {},
    });
    expect(response.ok(), `${method} ${path} answered ${response.status()}`).toBeTruthy();
    return response.status() === 204
      ? (undefined as TResponse)
      : ((await response.json()) as TResponse);
  }

  const tokens = await send<{ accessToken: string }>("POST", "/api/auth/sign-up", {
    ownerFullName: "Laura Gómez",
    email,
    password: demoPassword,
    businessName: "Escuela de Natación Brazada",
    timeZoneId: demoTimeZoneId,
    currencyCode: "ARS",
    defaultCountryCallingCode: "54",
  });
  accessToken = tokens.accessToken;

  const [laura] = await send<Identified[]>("GET", "/api/instructors");
  const martin = await send<Identified>("POST", "/api/instructors", { fullName: "Martín Díaz" });
  await send("PUT", "/api/business/monthly-fee", { amount: 25000 });

  const createClassGroup = (
    name: string,
    instructorId: string,
    weekdays: string[],
    startTime: string,
    durationMinutes: number,
    capacity: number,
    location: string,
  ) =>
    send<Identified>("POST", "/api/class-groups", {
      name,
      instructorId,
      weekdays,
      startTime,
      durationMinutes,
      capacity,
      location,
    });
  const beginners = await createClassGroup(
    "Natación inicial",
    laura!.id,
    withToday(["Monday", "Wednesday", "Friday"], today),
    "17:00",
    45,
    8,
    "Pileta chica",
  );
  const advanced = await createClassGroup(
    "Natación avanzada",
    martin.id,
    withToday(["Monday", "Wednesday", "Friday"], today),
    "18:00",
    60,
    4,
    "Pileta grande",
  );
  const aquagym = await createClassGroup(
    "Aquagym",
    laura!.id,
    ["Tuesday", "Thursday"],
    "10:00",
    45,
    12,
    "Pileta grande",
  );
  const adults = await createClassGroup(
    "Natación adultos",
    martin.id,
    ["Tuesday", "Thursday"],
    "19:30",
    60,
    10,
    "Pileta grande",
  );

  const registerStudentApp = (
    fullName: string,
    phoneNumber: string,
    notes: string | null,
    students: { fullName: string; birthDate: string | null; notes: string | null }[],
  ) =>
    send<RegisteredClient>("POST", "/api/clients", {
      fullName,
      phoneNumber,
      email: null,
      notes,
      students,
    });
  const ana = await registerStudentApp("Ana Pérez", "11 2233-4455", "Paga por transferencia", [
    { fullName: "Tomás Pérez", birthDate: "2018-03-14", notes: "Le cuesta meter la cabeza" },
    { fullName: "Lucía Pérez", birthDate: "2020-07-02", notes: null },
  ]);
  const carla = await registerStudentApp("Carla Gómez", "11 4455-6677", null, [
    { fullName: "Carla Gómez", birthDate: null, notes: "Rehabilitación de rodilla" },
  ]);
  const diego = await registerStudentApp("Diego Fernández", "11 5566-7788", null, [
    { fullName: "Valentina Fernández", birthDate: "2014-11-20", notes: "Compite en torneos" },
  ]);
  const sofia = await registerStudentApp(
    "Sofía Martínez",
    "11 6677-8899",
    "Dos hijos, cuota especial",
    [
      { fullName: "Benjamín Martínez", birthDate: "2015-05-09", notes: null },
      { fullName: "Mateo Martínez", birthDate: "2019-01-25", notes: null },
    ],
  );
  const jorge = await registerStudentApp("Jorge Ramírez", "11 7788-9900", null, [
    { fullName: "Jorge Ramírez", birthDate: null, notes: null },
  ]);
  const paula = await registerStudentApp("Paula Suárez", "11 8899-0011", null, [
    { fullName: "Emma Suárez", birthDate: "2017-09-30", notes: null },
  ]);

  const studentId = (client: RegisteredClient, fullName: string) =>
    client.students.find((student) => student.fullName === fullName)!.id;
  const enroll = (classGroupId: string, enrolledStudentId: string) =>
    send("POST", `/api/class-groups/${classGroupId}/enrollments`, {
      studentId: enrolledStudentId,
      startDate: monthStart,
    });
  await enroll(beginners.id, studentId(ana, "Tomás Pérez"));
  await enroll(beginners.id, studentId(ana, "Lucía Pérez"));
  await enroll(beginners.id, studentId(sofia, "Mateo Martínez"));
  await enroll(beginners.id, studentId(paula, "Emma Suárez"));
  await enroll(advanced.id, studentId(diego, "Valentina Fernández"));
  await enroll(advanced.id, studentId(sofia, "Benjamín Martínez"));
  await enroll(adults.id, studentId(carla, "Carla Gómez"));
  await enroll(aquagym.id, studentId(carla, "Carla Gómez"));
  await enroll(aquagym.id, studentId(jorge, "Jorge Ramírez"));

  const mark = (classGroupId: string, markedStudentId: string, status: string) =>
    send(
      "PUT",
      `/api/class-groups/${classGroupId}/sessions/${today}/attendance/${markedStudentId}`,
      {
        status,
      },
    );
  await mark(beginners.id, studentId(ana, "Tomás Pérez"), "Present");
  await mark(beginners.id, studentId(ana, "Lucía Pérez"), "Present");
  await mark(beginners.id, studentId(sofia, "Mateo Martínez"), "Absent");
  await mark(advanced.id, studentId(diego, "Valentina Fernández"), "Present");
  await mark(beginners.id, studentId(paula, "Emma Suárez"), "Present");

  await send("PUT", `/api/clients/${sofia.id}/billing-plan`, {
    kind: "CustomFee",
    customFee: 40000,
  });
  const pay = (clientId: string, amount: number, method: string) =>
    send("POST", `/api/clients/${clientId}/payments`, {
      amount,
      month,
      paidOn: today,
      method,
      notes: null,
    });
  await pay(ana.id, 25000, "Transfer");
  await pay(carla.id, 25000, "Cash");
  await pay(sofia.id, 20000, "Transfer");

  await send("PUT", "/api/business/monthly-fee", {
    amount: 28000,
    effectiveFrom: nextMonthOf(month),
  });
  const createClassPack = (
    name: string,
    classCount: number,
    price: number,
    validityMonths: number | null,
  ) => send<Identified>("POST", "/api/class-packs", { name, classCount, price, validityMonths });
  await createClassPack("Clase suelta", 1, 7000, null);
  const fourClasses = await createClassPack("4 clases", 4, 24000, 1);
  await createClassPack("8 clases", 8, 44000, 2);
  await send("PUT", `/api/clients/${paula.id}/billing-plan`, {
    kind: "ClassPacks",
    customFee: null,
    effectiveFrom: month,
  });
  await send("POST", `/api/clients/${paula.id}/class-pack-purchases`, {
    classPackId: fourClasses.id,
    price: 22000,
    purchasedOn: monthStart,
    method: "Cash",
    notes: "Descuento por pago en efectivo",
  });

  return {
    email,
    today,
    month,
    beginnersClassGroupId: beginners.id,
    aquagymClassGroupId: aquagym.id,
    studentAppClientId: ana.id,
    debtorClientId: diego.id,
    classPackClientId: paula.id,
  };
}
