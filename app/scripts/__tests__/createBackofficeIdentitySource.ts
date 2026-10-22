import { createWebExport } from "./createWebExport";

export const appIndexHtml = `<head>
    <meta name="description" content="Alumnos, clases, asistencia y cuotas de tu negocio." />
    <meta name="theme-color" content="#7c3aed" />
    <meta name="apple-mobile-web-app-title" content="Laztty" />
</head>`;

export function backofficeIdentitySource(): string {
  return createWebExport({
    "manifest.webmanifest": JSON.stringify({
      name: "Laztty Admin",
      description: "Backoffice de Laztty: marcas, planes, add-ons y operadores.",
      theme_color: "#334155",
    }),
    "icons/icon-192.png": "backoffice icon",
  });
}
