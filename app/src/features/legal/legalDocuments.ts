import { legalHolder, legalTermsUpdatedOn } from "@/features/legal/legalHolder";
import { TranslationKey } from "@/i18n/translate";

export interface LegalSection {
  heading: string;
  paragraphs: readonly string[];
}

export interface LegalDocument {
  titleKey: TranslationKey;
  updatedOn: string;
  sections: readonly LegalSection[];
}

export const legalDocumentKinds = ["notice", "terms", "privacy"] as const;
export type LegalDocumentKind = (typeof legalDocumentKinds)[number];

const { productName, name, taxId, address, email, governingLaw } = legalHolder;

const notice: LegalDocument = {
  titleKey: "legal.notice.title",
  updatedOn: legalTermsUpdatedOn,
  sections: [
    {
      heading: "Titular",
      paragraphs: [
        `${productName} es un servicio de ${name}, con documento fiscal ${taxId} y domicilio en ${address}.`,
        `Contacto: ${email}.`,
      ],
    },
    {
      heading: "Qué es",
      paragraphs: [
        `${productName} es una aplicación para negocios de clases (profes, escuelas y estudios) que gestionan alumnos, clases, asistencia, cuotas y ventas, y para que sus alumnos y familias consulten sus clases y saldos.`,
      ],
    },
    {
      heading: "Condiciones",
      paragraphs: [
        `Usar ${productName} implica aceptar los términos de uso y la política de privacidad, que se pueden leer en todo momento desde la app.`,
      ],
    },
    {
      heading: "Propiedad intelectual",
      paragraphs: [
        `El software, el diseño y la marca ${productName} pertenecen a su titular. Los contenidos que carga cada negocio (nombres, logos, fotos, textos y materiales) pertenecen a ese negocio, que es responsable de tener derecho a usarlos.`,
      ],
    },
    {
      heading: "Responsabilidad",
      paragraphs: [
        `El titular no responde por los contenidos que cargan los negocios ni por los acuerdos entre un negocio y sus alumnos, como precios, cobros o cancelaciones.`,
      ],
    },
    {
      heading: "Ley aplicable",
      paragraphs: [governingLaw],
    },
  ],
};

const terms: LegalDocument = {
  titleKey: "legal.terms.title",
  updatedOn: legalTermsUpdatedOn,
  sections: [
    {
      heading: "1. El servicio",
      paragraphs: [
        `${productName} permite a un negocio de clases registrar a sus alumnos y familias, organizar clases, tomar asistencia, llevar cuotas, packs, ventas y pagos, y dar a sus alumnos una app para ver sus clases y saldos. El titular del servicio se identifica en el aviso legal.`,
      ],
    },
    {
      heading: "2. Cuentas",
      paragraphs: [
        "Cada cuenta es personal. Quien la crea se compromete a dar datos verdaderos y a cuidar su contraseña.",
        "Quien registra un negocio tiene que ser mayor de edad (18 años).",
        "Una persona por debajo de la edad mínima de su país para dar su propio consentimiento (13 años en Argentina, 14 en España y 16 en los demás países) solo puede tener cuenta propia si el adulto responsable la autoriza. La app pide esa autorización y la registra.",
      ],
    },
    {
      heading: "3. Datos que carga un negocio",
      paragraphs: [
        "Los datos que un negocio carga sobre sus alumnos, familias y equipo son de ese negocio. El negocio es responsable de esos datos y de tener una base legal para tratarlos (por ejemplo, el contrato con sus alumnos) y de informarles que usa esta app.",
        `${productName} trata esos datos por cuenta del negocio, como encargado del tratamiento, en las condiciones del punto 9.`,
      ],
    },
    {
      heading: "4. Uso permitido",
      paragraphs: [
        "No está permitido usar el servicio para fines ilegales, para enviar comunicaciones no pedidas, para intentar acceder a datos de otros negocios o cuentas, ni para cargar contenidos que no se tenga derecho a usar.",
        "Los datos de salud (lesiones, certificados médicos, alergias) son datos especialmente protegidos: solo se cargan si son necesarios para dar la clase y con el consentimiento de la persona o de su responsable.",
      ],
    },
    {
      heading: "5. Precio",
      paragraphs: [
        "Durante el período de prueba el servicio no tiene costo. Antes de cobrar cualquier plan se informarán el precio y sus condiciones, y el negocio decidirá si contratarlo.",
        `Los cobros entre un negocio y sus alumnos son responsabilidad del negocio; ${productName} solo los registra.`,
      ],
    },
    {
      heading: "6. Disponibilidad",
      paragraphs: [
        "El servicio se ofrece tal como está. Se hacen copias de seguridad y se trabaja para que funcione siempre, pero puede haber interrupciones por mantenimiento o por causas ajenas.",
      ],
    },
    {
      heading: "7. Baja",
      paragraphs: [
        `Un negocio puede exportar sus datos en cualquier momento desde Ajustes, Importar y exportar, y pedir la baja escribiendo a ${email}. Después de la baja tiene 30 días para pedir una copia; luego sus datos se borran.`,
        `Cualquier persona puede pedir que se borre su cuenta escribiendo a ${email}.`,
      ],
    },
    {
      heading: "8. Cambios",
      paragraphs: [
        "Si estos términos o la política de privacidad cambian, la app lo avisa y pide aceptarlos de nuevo antes de seguir usándola.",
      ],
    },
    {
      heading: "9. Encargo del tratamiento",
      paragraphs: [
        `Para los datos que carga un negocio, ${productName} actúa como encargado del tratamiento y se compromete a:`,
        "• Tratarlos solo para prestar el servicio y según las instrucciones del negocio, que son las acciones que hace en la app.",
        "• Mantenerlos confidenciales y protegerlos con medidas de seguridad adecuadas.",
        "• Usar solo los proveedores listados en la política de privacidad, con las mismas obligaciones, y avisar antes de sumar otros.",
        "• Ayudar al negocio a responder pedidos de acceso, corrección o borrado de sus alumnos.",
        "• Avisar al negocio sin demora si hubiera un incidente de seguridad que afecte sus datos.",
        "• Al terminar el servicio, permitir exportarlos y después borrarlos.",
      ],
    },
    {
      heading: "10. Ley aplicable",
      paragraphs: [governingLaw],
    },
  ],
};

const privacy: LegalDocument = {
  titleKey: "legal.privacy.title",
  updatedOn: legalTermsUpdatedOn,
  sections: [
    {
      heading: "1. Quién es responsable",
      paragraphs: [
        `De los datos de las cuentas (email, nombre, contraseña, fecha de nacimiento y aceptación de estos textos) es responsable ${name}, titular de ${productName}. Contacto: ${email}.`,
        `De los datos que un negocio carga sobre sus alumnos, familias y equipo (clases, asistencia, cuotas, pagos, pedidos, mediciones, comentarios y fotos) es responsable ese negocio. ${productName} los trata por su cuenta, como encargado del tratamiento.`,
      ],
    },
    {
      heading: "2. Qué datos tratamos",
      paragraphs: [
        "• De la cuenta: nombre, email, contraseña (guardada cifrada, nadie puede leerla), fecha de nacimiento y la fecha en que se aceptaron estos textos.",
        "• Los que carga el negocio: nombre de alumnos y responsables, datos de contacto, fecha de nacimiento, clases, asistencia, cuotas, packs, pedidos y pagos, mediciones, comentarios del equipo, fotos y logos.",
        "• Técnicos: registros de acceso para la seguridad, las suscripciones a notificaciones del dispositivo y la sesión guardada en el dispositivo.",
      ],
    },
    {
      heading: "3. Para qué y con qué base",
      paragraphs: [
        "• Prestar el servicio: crear la cuenta, mostrar clases y saldos, enviar invitaciones, avisos y emails del servicio (ejecución del contrato).",
        "• Proteger el servicio y las cuentas (interés legítimo).",
        "• Cumplir obligaciones legales.",
        "No usamos los datos para publicidad, no los vendemos y no armamos perfiles comerciales.",
      ],
    },
    {
      heading: "4. Menores",
      paragraphs: [
        "Para saber quién es menor, cada cuenta tiene fecha de nacimiento. Por debajo de la edad mínima del país (13 años en Argentina, 14 en España y 16 en los demás), la cuenta propia necesita la autorización del adulto responsable, que queda registrada con su fecha. El adulto puede negarla y la cuenta no se crea.",
      ],
    },
    {
      heading: "5. Dónde se guardan y quién nos ayuda",
      paragraphs: [
        "Los datos se guardan en servidores de Microsoft Azure en la Unión Europea (Francia). Para prestar el servicio usamos estos proveedores:",
        "• Microsoft Azure: servidores, base de datos y fotos.",
        "• Cloudflare: publicación de la versión web de la app.",
        "• Google (Gmail): envío de emails.",
        "• Los servicios de notificaciones del navegador o del teléfono (Google, Apple, Mozilla): envío de notificaciones.",
        "• Expo: distribución de la app para teléfonos.",
        "Si algún proveedor trata datos fuera del Espacio Económico Europeo, lo hace con las garantías que exige la normativa, como las cláusulas contractuales tipo de la Comisión Europea.",
      ],
    },
    {
      heading: "6. Cuánto tiempo",
      paragraphs: [
        "La cuenta se guarda mientras exista. Los datos de un negocio se guardan mientras use el servicio y se borran 30 días después de su baja. Lo borrado desaparece también de las copias de seguridad en un máximo de 35 días.",
      ],
    },
    {
      heading: "7. Tus derechos",
      paragraphs: [
        "Toda persona puede pedir acceder a sus datos, corregirlos, borrarlos, oponerse a un tratamiento, limitarlo o recibirlos en un archivo.",
        `El nombre, el email y la fecha de nacimiento se corrigen desde Mi perfil. Para lo demás, escribí a ${email}; respondemos en menos de un mes. Si el pedido es sobre datos que cargó un negocio, se lo hacemos llegar a ese negocio.`,
        "Si no quedás conforme, podés reclamar ante la autoridad de protección de datos: en España, la Agencia Española de Protección de Datos (aepd.es); en Argentina, la Agencia de Acceso a la Información Pública (argentina.gob.ar/aaip).",
      ],
    },
    {
      heading: "8. Seguridad",
      paragraphs: [
        "Las conexiones van cifradas, las contraseñas se guardan cifradas, los datos de cada negocio están separados de los de los demás y cada persona del equipo accede solo a lo que su rol permite.",
      ],
    },
    {
      heading: "9. Cookies y almacenamiento en el dispositivo",
      paragraphs: [
        "La app no usa cookies de publicidad ni de análisis. Guarda en el dispositivo solo lo necesario para que funcione: la sesión abierta y preferencias como el tema claro u oscuro.",
      ],
    },
    {
      heading: "10. Cambios",
      paragraphs: [
        "Si esta política cambia, la app lo avisa y pide aceptarla de nuevo antes de seguir usándola.",
      ],
    },
  ],
};

export const legalDocuments: Record<LegalDocumentKind, LegalDocument> = { notice, terms, privacy };
