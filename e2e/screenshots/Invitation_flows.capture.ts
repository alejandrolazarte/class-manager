import {
  APIRequestContext,
  Browser,
  BrowserContext,
  devices,
  expect,
  Page,
  test,
} from "@playwright/test";
import { randomUUID } from "node:crypto";
import { mkdirSync } from "node:fs";
import { join } from "node:path";
import { e2eEnvironment } from "../support/environment";
import { SmtpSink } from "../support/smtpSink";
import { waitForBrandWelcomeToClose } from "./brandWelcome";

const outputDirectory = join(process.env.SCREENSHOTS_DIR ?? "screenshots-output", "invitaciones");
const settleMilliseconds = 800;
const timeZoneId = "America/Argentina/Buenos_Aires";
const ownerPassword = "flows-owner-password";
const invitedPassword = "flows-invited-password";
const guardianPassword = "flows-guardian-password";
const acceptStudentInvitationPattern = /accept-student-invitation\?token=([^\s]+)/;
const authorizeStudentAppPattern = /authorize-student-app\?token=([^\s]+)/;

test.use({ locale: "es-AR", timezoneId: timeZoneId });

const smtpSink = new SmtpSink();

test.beforeAll(async () => {
  await smtpSink.start(e2eEnvironment.smtpPort);
  mkdirSync(outputDirectory, { recursive: true });
});

test.afterAll(async () => {
  await smtpSink.stop();
});

interface FlowBusiness {
  ownerEmail: string;
  ownerToken: string;
  familyClientId: string;
  guardianEmail: string;
  teenStudentId: string;
  childStudentId: string;
  existingAccountClientId: string;
}

function isoDateYearsAgo(years: number, extraDays = 0): string {
  const date = new Date();
  date.setFullYear(date.getFullYear() - years);
  date.setDate(date.getDate() - extraDays);
  return date.toISOString().slice(0, 10);
}

function randomPhoneNumber(): string {
  const digits = String(Math.floor(10_000_000 + Math.random() * 89_999_999));
  return `11 ${digits.slice(0, 4)}-${digits.slice(4)}`;
}

function typedDateOf(isoDate: string): string {
  const [year, month, day] = isoDate.split("-");
  return `${day}${month}${year}`;
}

async function capture(page: Page, fileName: string): Promise<void> {
  await page.waitForTimeout(settleMilliseconds);
  await waitForBrandWelcomeToClose(page);
  await page.screenshot({ path: join(outputDirectory, `${fileName}.png`) });
}

async function newPhone(browser: Browser): Promise<BrowserContext> {
  return browser.newContext({ ...devices["Pixel 7"], locale: "es-AR", timezoneId: timeZoneId });
}

async function linkSentTo(email: string, pattern: RegExp): Promise<string> {
  const message = await smtpSink.waitForEmailTo(email, undefined, pattern);
  const token = pattern.exec(message.body)?.[1];
  expect(token, `the email to ${email} has the link`).toBeTruthy();
  return decodeURIComponent(token!);
}

async function seedFlowBusiness(request: APIRequestContext): Promise<FlowBusiness> {
  const suffix = randomUUID().slice(0, 8);
  const ownerEmail = `duena-${suffix}@flows.local`;
  let ownerToken = "";
  let ownerTokenBeforeGuardian = "";

  async function send<TResponse>(method: string, path: string, data?: unknown): Promise<TResponse> {
    const response = await request.fetch(`${e2eEnvironment.apiUrl}${path}`, {
      method,
      data,
      headers: ownerToken ? { Authorization: `Bearer ${ownerToken}` } : {},
    });
    expect(response.ok(), `${method} ${path} answered ${response.status()}`).toBeTruthy();
    return (await response.json()) as TResponse;
  }

  ({ accessToken: ownerToken } = await send<{ accessToken: string }>("POST", "/api/auth/sign-up", {
    ownerFullName: "Laura Gómez",
    email: ownerEmail,
    password: ownerPassword,
    businessName: "Natación Olas",
    timeZoneId,
    currencyCode: "ARS",
    defaultCountryCallingCode: "54",
    ownerBirthDate: "1985-06-20",
  }));

  const guardianEmail = `ana-${suffix}@flows.local`;
  const family = await send<{ id: string; students: { id: string; fullName: string }[] }>(
    "POST",
    "/api/clients",
    {
      fullName: "Ana Pérez",
      phoneNumber: randomPhoneNumber(),
      email: guardianEmail,
      notes: null,
      students: [
        { fullName: "Lucía Pérez", birthDate: isoDateYearsAgo(14, 30), notes: null, email: null },
        { fullName: "Tomás Pérez", birthDate: isoDateYearsAgo(11, 30), notes: null, email: null },
      ],
    },
  );
  const existingAccountClient = await send<{ id: string }>("POST", "/api/clients", {
    fullName: "Marta Ruiz",
    phoneNumber: randomPhoneNumber(),
    email: null,
    notes: null,
    students: [{ fullName: "Marta Ruiz", birthDate: null, notes: null, email: null }],
  });

  await send("POST", `/api/clients/${family.id}/app-invitation`, { email: guardianEmail });
  const guardianInvitationToken = await linkSentTo(guardianEmail, acceptStudentInvitationPattern);
  ownerTokenBeforeGuardian = ownerToken;
  ownerToken = "";
  await send("POST", "/api/auth/student-app-invitations/accept", {
    token: guardianInvitationToken,
    fullName: null,
    password: guardianPassword,
    birthDate: "1984-02-11",
  });
  ownerToken = ownerTokenBeforeGuardian;

  const studentNamed = (fullName: string) =>
    family.students.find((student) => student.fullName === fullName)!.id;
  return {
    ownerEmail,
    ownerToken,
    familyClientId: family.id,
    guardianEmail,
    teenStudentId: studentNamed("Lucía Pérez"),
    childStudentId: studentNamed("Tomás Pérez"),
    existingAccountClientId: existingAccountClient.id,
  };
}

async function signInAsOwner(page: Page, business: FlowBusiness): Promise<void> {
  await page.goto("/sign-in");
  await page.getByLabel("Email").fill(business.ownerEmail);
  await page.getByLabel("Contraseña", { exact: true }).fill(ownerPassword);
  await page.getByRole("button", { name: "Ingresar" }).click();
  await expect(page).toHaveURL(/\/today/);
}

async function inviteFromCard(page: Page, studentName: string, email: string): Promise<void> {
  const studentCard = page
    .getByText(studentName, { exact: true })
    .filter({ visible: true })
    .first()
    .locator(`xpath=ancestor::*[.//*[@aria-label="Invitar a la app"]][1]`);
  await studentCard.getByRole("button", { name: "Invitar a la app" }).click();
  await page.getByLabel(`Email propio de ${studentName}`).filter({ visible: true }).fill(email);
}

test("Invitation flows", async ({ page, request, browser }) => {
  const business = await seedFlowBusiness(request);
  const teenEmail = `lucia-${randomUUID().slice(0, 8)}@flows.local`;
  const childEmail = `tomas-${randomUUID().slice(0, 8)}@flows.local`;

  await page.goto("/sign-up");
  await page.getByLabel("Nombre y apellido").fill("Laura Gómez");
  await page.getByLabel("Fecha de nacimiento").fill("20061985");
  await capture(page, "01-registro-del-dueno-con-fecha");

  await signInAsOwner(page, business);
  await page.goto(`/students/clients/${business.familyClientId}`);
  await page.getByText("Lucía Pérez", { exact: true }).filter({ visible: true }).first().waitFor();
  await capture(page, "02-ficha-de-la-familia");

  await inviteFromCard(page, "Lucía Pérez", teenEmail);
  await capture(page, "03-invitar-a-lucia-14-anos");
  await page
    .getByRole("button", { name: "Enviar invitación" })
    .filter({ visible: true })
    .first()
    .click();
  await page
    .getByText(/falta que acepte/)
    .filter({ visible: true })
    .first()
    .waitFor();
  await capture(page, "04-lucia-invitada");

  await page.getByText("Invitación enviada", { exact: true }).waitFor({ state: "hidden" });
  await inviteFromCard(page, "Tomás Pérez", childEmail);
  await page.getByRole("button", { name: "Enviar invitación" }).last().click();
  await page
    .getByText(/Falta que su responsable autorice/)
    .filter({ visible: true })
    .first()
    .waitFor();
  await page.getByText(/Como es chico/).waitFor({ state: "hidden" });
  await capture(page, "05-tomas-11-anos-espera-autorizacion");

  const guardianEmailPhone = await newPhone(browser);
  const guardianEmailPage = await guardianEmailPhone.newPage();
  await guardianEmailPage.goto(
    `/authorize-student-app?token=${encodeURIComponent(await linkSentTo(business.guardianEmail, authorizeStudentAppPattern))}`,
  );
  await guardianEmailPage.getByRole("button", { name: "Autorizar", exact: true }).waitFor();
  await capture(guardianEmailPage, "06-responsable-autoriza-desde-el-mail");
  await guardianEmailPhone.close();

  const guardianPhone = await newPhone(browser);
  const guardianPage = await guardianPhone.newPage();
  await guardianPage.goto("/sign-in");
  await guardianPage.getByLabel("Email").fill(business.guardianEmail);
  await guardianPage.getByLabel("Contraseña", { exact: true }).fill(guardianPassword);
  await guardianPage.getByRole("button", { name: "Ingresar" }).click();
  await expect(guardianPage).toHaveURL(/\/student-app/);
  await guardianPage.getByText("Tomás quiere usar la app").waitFor();
  await capture(guardianPage, "07-responsable-autoriza-desde-la-app");
  await guardianPage.getByRole("button", { name: "Autorizar", exact: true }).click();
  await guardianPage.getByText(/Le mandamos la invitación/).waitFor();
  await capture(guardianPage, "08-responsable-autorizo");
  await guardianPhone.close();

  const childPhone = await newPhone(browser);
  const childPage = await childPhone.newPage();
  await childPage.goto(
    `/accept-student-invitation?token=${encodeURIComponent(await linkSentTo(childEmail, acceptStudentInvitationPattern))}`,
  );
  await childPage.getByText(/Hola, Tomás Pérez/).waitFor();
  await capture(childPage, "09-tomas-crea-su-cuenta");
  await childPage.getByLabel("Contraseña", { exact: true }).fill(invitedPassword);
  await childPage.getByLabel("Repetí la contraseña").fill(invitedPassword);
  await childPage.getByRole("button", { name: "Crear cuenta" }).click();
  await expect(childPage).toHaveURL(/\/student-app/);
  await childPage.getByText("Próxima clase").or(childPage.getByText("Tomás")).first().waitFor();
  await capture(childPage, "10-tomas-entra-a-la-app");
  await childPhone.close();

  const teenPhone = await newPhone(browser);
  const teenPage = await teenPhone.newPage();
  await teenPage.goto(
    `/accept-student-invitation?token=${encodeURIComponent(await linkSentTo(teenEmail, acceptStudentInvitationPattern))}`,
  );
  await teenPage.getByText(/Hola, Lucía Pérez/).waitFor();
  await capture(teenPage, "11-lucia-crea-su-cuenta-sin-autorizacion");
  await teenPhone.close();

  const existingAccountInvitation = await request.post(
    `${e2eEnvironment.apiUrl}/api/clients/${business.existingAccountClientId}/app-invitation`,
    {
      data: { email: business.ownerEmail },
      headers: { Authorization: `Bearer ${business.ownerToken}` },
    },
  );
  expect(
    existingAccountInvitation.ok(),
    "Marta is invited with an email that has an account",
  ).toBeTruthy();
  const existingAccountPhone = await newPhone(browser);
  const existingAccountPage = await existingAccountPhone.newPage();
  await existingAccountPage.goto(
    `/accept-student-invitation?token=${encodeURIComponent(await linkSentTo(business.ownerEmail, acceptStudentInvitationPattern))}`,
  );
  await existingAccountPage.getByRole("button", { name: "Rechazar" }).waitFor();
  await capture(existingAccountPage, "12-ya-tiene-cuenta-aceptar-o-rechazar");
  await existingAccountPage.getByRole("button", { name: "Rechazar" }).click();
  await existingAccountPage.getByText(/Rechazaste la invitación/).waitFor();
  await capture(existingAccountPage, "13-rechazo-la-invitacion");
  await existingAccountPhone.close();

  await page.goto("/today/notifications");
  await page
    .getByText(/rechazó la invitación a la app/)
    .filter({ visible: true })
    .first()
    .waitFor();
  await capture(page, "14-aviso-al-equipo");

  await page.goto("/settings/profile");
  await page.getByRole("button", { name: "Editar mi perfil" }).click();
  await page.getByLabel("Fecha de nacimiento").waitFor();
  await capture(page, "15-editar-perfil");
  await page.getByLabel("Fecha de nacimiento").fill(typedDateOf("1985-06-20"));
});
