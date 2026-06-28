import type {
  BeneficioSolicitud,
  Candidato,
  ColaboradorHcm,
  CursoLms,
  DenunciaCaso,
  DocumentoExpediente,
  EncuestaDh,
  EventoTimeline,
  FeedPublicacion,
  IncidenciaAsistencia,
  LogIntegracion,
  ObjetivoOkr,
  OnboardingProceso,
  SolicitudAusenciaDh,
  TicketServicio,
  Vacante,
} from "./types";

export const AREAS = [
  "Desarrollo Humano",
  "Nómina",
  "Comercial",
  "Operaciones",
  "Finanzas",
  "Tecnología",
  "Manufactura",
  "Legal",
  "Seguridad e Higiene",
] as const;

export const COLABORADORES: ColaboradorHcm[] = [
  { id: "c01", numeroEmpleado: "1001", nombreLegal: "Ana Lucía García Mendoza", nombrePreferido: "Ana García", rfc: "GAME850315AB1", curp: "GAME850315MDFRNN02", nss: "12345678901", email: "ana.garcia@urrea.com.mx", telefono: "+52 81 8000 1001", puesto: "Directora General", area: "Finanzas", centroCosto: "CC-1000", jefeId: null, jefeNombre: "—", estatus: "activo", antiguedadAnios: 12, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 185000, fechaIngreso: "2014-03-01", domicilio: "San Pedro Garza García, NL", confidencial: true },
  { id: "c02", numeroEmpleado: "1002", nombreLegal: "Luis Alberto Martínez Torres", nombrePreferido: "Luis Martínez", rfc: "MATL880722CD2", curp: "MATL880722HDFRRS08", nss: "23456789012", email: "luis.martinez@urrea.com.mx", telefono: "+52 81 8000 1002", puesto: "Gerente de Operaciones", area: "Operaciones", centroCosto: "CC-2100", jefeId: "c01", jefeNombre: "Ana García", estatus: "activo", antiguedadAnios: 9, ubicacion: "Monterrey Planta", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 78000, fechaIngreso: "2017-06-15", domicilio: "Guadalupe, NL" },
  { id: "c03", numeroEmpleado: "1003", nombreLegal: "María Fernanda López Ruiz", nombrePreferido: "María López", rfc: "LORM920510EF3", curp: "LORM920510MDFPZR04", nss: "34567890123", email: "maria.lopez@urrea.com.mx", telefono: "+52 81 8000 1003", puesto: "Analista de Nómina Sr.", area: "Nómina", centroCosto: "CC-1200", jefeId: "c05", jefeNombre: "Patricia Ruiz", estatus: "activo", antiguedadAnios: 5, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 42000, fechaIngreso: "2021-02-01", domicilio: "Monterrey, NL" },
  { id: "c04", numeroEmpleado: "1004", nombreLegal: "Carlos Eduardo Hernández Vega", nombrePreferido: "Carlos Hernández", rfc: "HEVC900818GH4", curp: "HEVC900818HDFRGL01", nss: "45678901234", email: "carlos.hernandez@urrea.com.mx", telefono: "+52 81 8000 1004", puesto: "Supervisor de Producción", area: "Manufactura", centroCosto: "CC-3100", jefeId: "c02", jefeNombre: "Luis Martínez", estatus: "activo", antiguedadAnios: 7, ubicacion: "Monterrey Planta", tipoContrato: "Indeterminado", jornada: "Mixta", salarioMensual: 38000, fechaIngreso: "2019-01-10", domicilio: "Apodaca, NL" },
  { id: "c05", numeroEmpleado: "1005", nombreLegal: "Patricia Isabel Ruiz Campos", nombrePreferido: "Patricia Ruiz", rfc: "RUCP870305IJ5", curp: "RUCP870305MDFZMT09", nss: "56789012345", email: "patricia.ruiz@urrea.com.mx", telefono: "+52 81 8000 1005", puesto: "Directora de Desarrollo Humano", area: "Desarrollo Humano", centroCosto: "CC-1100", jefeId: "c01", jefeNombre: "Ana García", estatus: "activo", antiguedadAnios: 11, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 95000, fechaIngreso: "2015-08-20", domicilio: "San Pedro Garza García, NL" },
  { id: "c06", numeroEmpleado: "1006", nombreLegal: "Roberto Sánchez Delgado", nombrePreferido: "Roberto Sánchez", rfc: "SADR910625KL6", curp: "SADR910625HDFNLB03", nss: "67890123456", email: "roberto.sanchez@urrea.com.mx", telefono: "+52 81 8000 1006", puesto: "Ejecutivo Comercial", area: "Comercial", centroCosto: "CC-4100", jefeId: "c12", jefeNombre: "Sofía Ramírez", estatus: "activo", antiguedadAnios: 4, ubicacion: "CDMX", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 35000, fechaIngreso: "2022-04-01", domicilio: "Ciudad de México", confidencial: true },
  { id: "c07", numeroEmpleado: "1007", nombreLegal: "Daniela Morales Ortiz", nombrePreferido: "Daniela Morales", rfc: "MOOD930712MN7", curp: "MOOD930712MDFRLN07", nss: "78901234567", email: "daniela.morales@urrea.com.mx", telefono: "+52 81 8000 1007", puesto: "Especialista Legal", area: "Legal", centroCosto: "CC-5100", jefeId: "c01", jefeNombre: "Ana García", estatus: "activo", antiguedadAnios: 3, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 48000, fechaIngreso: "2023-01-15", domicilio: "Monterrey, NL" },
  { id: "c08", numeroEmpleado: "1008", nombreLegal: "Jorge Iván Castillo Núñez", nombrePreferido: "Jorge Castillo", rfc: "CANJ890420OP8", curp: "CANJ890420HDFSTG05", nss: "89012345678", email: "jorge.castillo@urrea.com.mx", telefono: "+52 81 8000 1008", puesto: "Ingeniero de TI", area: "Tecnología", centroCosto: "CC-6100", jefeId: "c14", jefeNombre: "Miguel Ángel Fuentes", estatus: "activo", antiguedadAnios: 6, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 52000, fechaIngreso: "2020-09-01", domicilio: "San Nicolás, NL" },
  { id: "c09", numeroEmpleado: "1009", nombreLegal: "Gabriela Pérez Salinas", nombrePreferido: "Gabriela Pérez", rfc: "PESG940815QR9", curp: "PESG940815MDFRLB02", nss: "90123456789", email: "gabriela.perez@urrea.com.mx", telefono: "+52 81 8000 1009", puesto: "Coordinadora de Capacitación", area: "Desarrollo Humano", centroCosto: "CC-1100", jefeId: "c05", jefeNombre: "Patricia Ruiz", estatus: "activo", antiguedadAnios: 4, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 36000, fechaIngreso: "2022-07-01", domicilio: "Guadalupe, NL" },
  { id: "c10", numeroEmpleado: "1010", nombreLegal: "Fernando Reyes Aguilar", nombrePreferido: "Fernando Reyes", rfc: "REAF860103ST0", curp: "REAF860103HDFYGN01", nss: "01234567890", email: "fernando.reyes@urrea.com.mx", telefono: "+52 81 8000 1010", puesto: "Técnico de Seguridad", area: "Seguridad e Higiene", centroCosto: "CC-7100", jefeId: "c02", jefeNombre: "Luis Martínez", estatus: "activo", antiguedadAnios: 8, ubicacion: "Monterrey Planta", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 28000, fechaIngreso: "2018-03-12", domicilio: "Escobedo, NL" },
  { id: "c11", numeroEmpleado: "1011", nombreLegal: "Laura Edith Vargas Luna", nombrePreferido: "Laura Vargas", rfc: "VALE910228UV1", curp: "VALE910228MDFRLR06", nss: "11234567890", email: "laura.vargas@urrea.com.mx", telefono: "+52 81 8000 1011", puesto: "Analista Financiero", area: "Finanzas", centroCosto: "CC-1000", jefeId: "c01", jefeNombre: "Ana García", estatus: "activo", antiguedadAnios: 5, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 40000, fechaIngreso: "2021-05-01", domicilio: "Monterrey, NL" },
  { id: "c12", numeroEmpleado: "1012", nombreLegal: "Sofía Alejandra Ramírez Cruz", nombrePreferido: "Sofía Ramírez", rfc: "RACS880715WX2", curp: "RACS880715MDFMRF03", nss: "21234567890", email: "sofia.ramirez@urrea.com.mx", telefono: "+52 55 8000 1012", puesto: "Directora Comercial", area: "Comercial", centroCosto: "CC-4000", jefeId: "c01", jefeNombre: "Ana García", estatus: "activo", antiguedadAnios: 10, ubicacion: "CDMX", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 88000, fechaIngreso: "2016-02-01", domicilio: "Ciudad de México" },
  { id: "c13", numeroEmpleado: "1013", nombreLegal: "Héctor Manuel Díaz Ríos", nombrePreferido: "Héctor Díaz", rfc: "DIRH900901YZ3", curp: "DIRH900901HDFZSC08", nss: "31234567890", email: "hector.diaz@urrea.com.mx", telefono: "+52 81 8000 1013", puesto: "Operador de Línea", area: "Manufactura", centroCosto: "CC-3100", jefeId: "c04", jefeNombre: "Carlos Hernández", estatus: "activo", antiguedadAnios: 2, ubicacion: "Monterrey Planta", tipoContrato: "Indeterminado", jornada: "Rotativa", salarioMensual: 18500, fechaIngreso: "2024-01-08", domicilio: "Apodaca, NL" },
  { id: "c14", numeroEmpleado: "1014", nombreLegal: "Miguel Ángel Fuentes Lara", nombrePreferido: "Miguel Fuentes", rfc: "FULM870612AB4", curp: "FULM870612HDFNTR04", nss: "41234567890", email: "miguel.fuentes@urrea.com.mx", telefono: "+52 81 8000 1014", puesto: "Director de TI", area: "Tecnología", centroCosto: "CC-6000", jefeId: "c01", jefeNombre: "Ana García", estatus: "activo", antiguedadAnios: 9, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 92000, fechaIngreso: "2017-01-15", domicilio: "San Pedro Garza García, NL" },
  { id: "c15", numeroEmpleado: "1015", nombreLegal: "Valentina Cruz Mejía", nombrePreferido: "Valentina Cruz", rfc: "CUMV950303CD5", curp: "CUMV950303MDFRJL01", nss: "51234567890", email: "valentina.cruz@urrea.com.mx", telefono: "+52 81 8000 1015", puesto: "Reclutadora", area: "Desarrollo Humano", centroCosto: "CC-1100", jefeId: "c05", jefeNombre: "Patricia Ruiz", estatus: "activo", antiguedadAnios: 3, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 32000, fechaIngreso: "2023-03-01", domicilio: "Monterrey, NL" },
  { id: "c16", numeroEmpleado: "1016", nombreLegal: "Ricardo Ortega Pineda", nombrePreferido: "Ricardo Ortega", rfc: "ORPR880811EF6", curp: "ORPR880811HDFRND07", nss: "61234567890", email: "ricardo.ortega@urrea.com.mx", telefono: "+52 55 8000 1016", puesto: "Ejecutivo Comercial", area: "Comercial", centroCosto: "CC-4100", jefeId: "c12", jefeNombre: "Sofía Ramírez", estatus: "activo", antiguedadAnios: 6, ubicacion: "Guadalajara", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 33000, fechaIngreso: "2020-06-01", domicilio: "Guadalajara, JAL" },
  { id: "c17", numeroEmpleado: "1017", nombreLegal: "Isabel Torres Mendoza", nombrePreferido: "Isabel Torres", rfc: "TOMI920425GH7", curp: "TOMI920425MDFRNS05", nss: "71234567890", email: "isabel.torres@urrea.com.mx", telefono: "+52 81 8000 1017", puesto: "Auxiliar Administrativo", area: "Operaciones", centroCosto: "CC-2100", jefeId: "c02", jefeNombre: "Luis Martínez", estatus: "reingreso", antiguedadAnios: 1, ubicacion: "Monterrey Planta", tipoContrato: "Temporal", jornada: "Diurna", salarioMensual: 16000, fechaIngreso: "2025-11-01", domicilio: "Guadalupe, NL" },
  { id: "c18", numeroEmpleado: "1018", nombreLegal: "Arturo Jiménez Solís", nombrePreferido: "Arturo Jiménez", rfc: "JISA850720IJ8", curp: "JISA850720HDFMRT02", nss: "81234567890", email: "arturo.jimenez@urrea.com.mx", telefono: "+52 81 8000 1018", puesto: "Contador General", area: "Finanzas", centroCosto: "CC-1000", jefeId: "c01", jefeNombre: "Ana García", estatus: "activo", antiguedadAnios: 14, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 72000, fechaIngreso: "2012-04-01", domicilio: "San Pedro Garza García, NL", confidencial: true },
  { id: "c19", numeroEmpleado: "1019", nombreLegal: "Paola Hernández Rivas", nombrePreferido: "Paola Hernández", rfc: "HERP960118KL9", curp: "HERP960118MDFRVL08", nss: "91234567890", email: "paola.hernandez@urrea.com.mx", telefono: "+52 81 8000 1019", puesto: "Practicante DH", area: "Desarrollo Humano", centroCosto: "CC-1100", jefeId: "c05", jefeNombre: "Patricia Ruiz", estatus: "pendiente", antiguedadAnios: 0, ubicacion: "Monterrey HQ", tipoContrato: "Prácticas", jornada: "Diurna", salarioMensual: 12000, fechaIngreso: "2026-07-01", domicilio: "Monterrey, NL" },
  { id: "c20", numeroEmpleado: "1020", nombreLegal: "Oscar Delgado Campos", nombrePreferido: "Oscar Delgado", rfc: "DECO830505MN0", curp: "DECO830505HDFLMS01", nss: "10234567890", email: "oscar.delgado@urrea.com.mx", telefono: "+52 81 8000 1020", puesto: "Jefe de Nómina", area: "Nómina", centroCosto: "CC-1200", jefeId: "c05", jefeNombre: "Patricia Ruiz", estatus: "baja", antiguedadAnios: 8, ubicacion: "Monterrey HQ", tipoContrato: "Indeterminado", jornada: "Diurna", salarioMensual: 55000, fechaIngreso: "2018-02-01", domicilio: "Monterrey, NL" },
];

export const VACANTES: Vacante[] = [
  { id: "v1", titulo: "Analista de Compensaciones", area: "Desarrollo Humano", puesto: "Analista de Compensaciones", ubicacion: "Monterrey HQ", vacantes: 1, estatus: "abierta", fechaApertura: "2026-05-10", candidatosCount: 4 },
  { id: "v2", titulo: "Ingeniero de Manufactura", area: "Manufactura", puesto: "Ingeniero de Procesos", ubicacion: "Monterrey Planta", vacantes: 2, estatus: "abierta", fechaApertura: "2026-04-22", candidatosCount: 6 },
  { id: "v3", titulo: "Ejecutivo Comercial Zona Norte", area: "Comercial", puesto: "Ejecutivo Comercial", ubicacion: "Monterrey", vacantes: 3, estatus: "abierta", fechaApertura: "2026-06-01", candidatosCount: 5 },
  { id: "v4", titulo: "Especialista SAP HCM", area: "Tecnología", puesto: "Consultor SAP", ubicacion: "Monterrey HQ", vacantes: 1, estatus: "abierta", fechaApertura: "2026-05-28", candidatosCount: 3 },
  { id: "v5", titulo: "Coordinador de Seguridad", area: "Seguridad e Higiene", puesto: "Coordinador", ubicacion: "Monterrey Planta", vacantes: 1, estatus: "pausada", fechaApertura: "2026-03-15", candidatosCount: 2 },
  { id: "v6", titulo: "Abogado Corporativo", area: "Legal", puesto: "Abogado Sr.", ubicacion: "Monterrey HQ", vacantes: 1, estatus: "cerrada", fechaApertura: "2026-01-10", candidatosCount: 8 },
  { id: "v7", titulo: "Técnico de Mantenimiento", area: "Operaciones", puesto: "Técnico", ubicacion: "Monterrey Planta", vacantes: 2, estatus: "abierta", fechaApertura: "2026-06-05", candidatosCount: 2 },
  { id: "v8", titulo: "Data Analyst BI", area: "Finanzas", puesto: "Analista de Datos", ubicacion: "Monterrey HQ", vacantes: 1, estatus: "abierta", fechaApertura: "2026-06-12", candidatosCount: 1 },
];

export const CANDIDATOS: Candidato[] = [
  { id: "ca1", vacanteId: "v1", nombre: "Elena Ríos Montoya", email: "elena.rios@gmail.com", etapa: "entrevista", score: 82, altaSap: "na", fechaPostulacion: "2026-05-15" },
  { id: "ca2", vacanteId: "v1", nombre: "Pedro Núñez Lara", email: "pedro.nunez@outlook.com", etapa: "filtrado", score: 71, altaSap: "na", fechaPostulacion: "2026-05-18" },
  { id: "ca3", vacanteId: "v2", nombre: "Andrea Silva Ponce", email: "andrea.silva@gmail.com", etapa: "oferta", score: 91, altaSap: "pendiente", fechaPostulacion: "2026-04-25" },
  { id: "ca4", vacanteId: "v2", nombre: "Marco Antonio León", email: "marco.leon@hotmail.com", etapa: "entrevista", score: 78, altaSap: "na", fechaPostulacion: "2026-05-02" },
  { id: "ca5", vacanteId: "v3", nombre: "Diana Flores Vega", email: "diana.flores@gmail.com", etapa: "nuevo", score: 65, altaSap: "na", fechaPostulacion: "2026-06-10" },
  { id: "ca6", vacanteId: "v3", nombre: "Javier Ortiz Ruiz", email: "javier.ortiz@gmail.com", etapa: "aceptado", score: 88, altaSap: "completada", fechaPostulacion: "2026-05-20" },
  { id: "ca7", vacanteId: "v4", nombre: "Camila Herrera Díaz", email: "camila.herrera@gmail.com", etapa: "entrevista", score: 85, altaSap: "na", fechaPostulacion: "2026-06-01" },
  { id: "ca8", vacanteId: "v6", nombre: "Rodrigo Méndez Solís", email: "rodrigo.mendez@gmail.com", etapa: "aceptado", score: 93, altaSap: "completada", fechaPostulacion: "2026-02-01" },
  { id: "ca9", vacanteId: "v2", nombre: "Lucía Campos Reyes", email: "lucia.campos@gmail.com", etapa: "rechazado", score: 52, altaSap: "na", fechaPostulacion: "2026-04-28" },
  { id: "ca10", vacanteId: "v5", nombre: "Gerardo Luna Pérez", email: "gerardo.luna@gmail.com", etapa: "filtrado", score: 74, altaSap: "na", fechaPostulacion: "2026-03-20" },
  { id: "ca11", vacanteId: "v7", nombre: "Norma Esquivel", email: "norma.esquivel@gmail.com", etapa: "nuevo", score: 60, altaSap: "na", fechaPostulacion: "2026-06-08" },
  { id: "ca12", vacanteId: "v3", nombre: "Alberto Cruz", email: "alberto.cruz@gmail.com", etapa: "filtrado", score: 69, altaSap: "na", fechaPostulacion: "2026-06-03" },
  { id: "ca13", vacanteId: "v1", nombre: "Silvia Mendoza", email: "silvia.mendoza@gmail.com", etapa: "rechazado", score: 48, altaSap: "na", fechaPostulacion: "2026-05-12" },
  { id: "ca14", vacanteId: "v8", nombre: "Tomás Aguilar", email: "tomas.aguilar@gmail.com", etapa: "nuevo", score: 77, altaSap: "na", fechaPostulacion: "2026-06-14" },
  { id: "ca15", vacanteId: "v4", nombre: "Verónica Soto", email: "veronica.soto@gmail.com", etapa: "oferta", score: 90, altaSap: "pendiente", fechaPostulacion: "2026-06-05" },
];

export const ONBOARDINGS: OnboardingProceso[] = [
  {
    id: "ob1", colaboradorNombre: "Javier Ortiz Ruiz", puesto: "Ejecutivo Comercial", area: "Comercial", fechaInicio: "2026-06-16", avance: 72,
    tareas: [
      { id: "t1", nombre: "Documentos de ingreso", responsable: "Valentina Cruz", fechaLimite: "2026-06-18", estado: "completado" },
      { id: "t2", nombre: "Firma de contrato", responsable: "Daniela Morales", fechaLimite: "2026-06-20", estado: "completado" },
      { id: "t3", nombre: "Alta de accesos TI", responsable: "Jorge Castillo", fechaLimite: "2026-06-22", estado: "en_proceso" },
      { id: "t4", nombre: "Inducción corporativa", responsable: "Gabriela Pérez", fechaLimite: "2026-06-25", estado: "pendiente" },
      { id: "t5", nombre: "Examen médico", responsable: "Clínica URREA", fechaLimite: "2026-06-19", estado: "vencido" },
    ],
  },
  {
    id: "ob2", colaboradorNombre: "Paola Hernández Rivas", puesto: "Practicante DH", area: "Desarrollo Humano", fechaInicio: "2026-07-01", avance: 15,
    tareas: [
      { id: "t6", nombre: "Carta de prácticas", responsable: "Patricia Ruiz", fechaLimite: "2026-06-28", estado: "en_proceso" },
      { id: "t7", nombre: "Buddy asignado", responsable: "Gabriela Pérez", fechaLimite: "2026-06-30", estado: "pendiente" },
      { id: "t8", nombre: "Encuesta 30 días", responsable: "Sistema", fechaLimite: "2026-07-31", estado: "pendiente" },
    ],
  },
];

export const DOCUMENTOS: DocumentoExpediente[] = [
  { id: "d1", colaboradorId: "c03", tipo: "Contrato", nombre: "Contrato laboral indeterminado", version: "v3", estado: "firmado", vigencia: "2027-02-01", confidencial: true },
  { id: "d2", colaboradorId: "c03", tipo: "Aviso privacidad", nombre: "Aviso de privacidad ARCO", version: "v2", estado: "firmado", confidencial: false },
  { id: "d3", colaboradorId: "c06", tipo: "Recibo nómina", nombre: "Recibo quincena 12-2026", version: "v1", estado: "firmado", confidencial: true },
  { id: "d4", colaboradorId: "c13", tipo: "Descripción puesto", nombre: "Descripción Operador Línea", version: "v1", estado: "pendiente_firma", confidencial: false },
  { id: "d5", colaboradorId: "c08", tipo: "Política", nombre: "Política de seguridad TI", version: "v4", estado: "vencido", vigencia: "2026-01-01", confidencial: false },
];

export const SOLICITUDES_AUSENCIA: SolicitudAusenciaDh[] = [
  { id: "s1", colaboradorId: "c03", colaboradorNombre: "María López", tipo: "Vacaciones", fechaInicio: "2026-07-07", fechaFin: "2026-07-11", dias: 5, estado: "pendiente" },
  { id: "s2", colaboradorId: "c06", colaboradorNombre: "Roberto Sánchez", tipo: "Teletrabajo por fuerza mayor", fechaInicio: "2026-06-26", fechaFin: "2026-06-26", dias: 1, estado: "pendiente" },
  { id: "s3", colaboradorId: "c04", colaboradorNombre: "Carlos Hernández", tipo: "Permiso personal tiempo por tiempo", fechaInicio: "2026-06-27", fechaFin: "2026-06-27", dias: 0.5, estado: "aprobada" },
  { id: "s4", colaboradorId: "c16", colaboradorNombre: "Ricardo Ortega", tipo: "Llegada tarde", fechaInicio: "2026-06-24", fechaFin: "2026-06-24", dias: 0, estado: "aprobada", comentario: "Tráfico en Periférico" },
  { id: "s5", colaboradorId: "c11", colaboradorNombre: "Laura Vargas", tipo: "Día flotante", fechaInicio: "2026-07-03", fechaFin: "2026-07-03", dias: 1, estado: "pendiente" },
  { id: "s6", colaboradorId: "c13", colaboradorNombre: "Héctor Díaz", tipo: "Vacaciones", fechaInicio: "2026-08-04", fechaFin: "2026-08-15", dias: 10, estado: "pendiente" },
  { id: "s7", colaboradorId: "c09", colaboradorNombre: "Gabriela Pérez", tipo: "Capacitación", fechaInicio: "2026-06-30", fechaFin: "2026-07-01", dias: 2, estado: "aprobada" },
  { id: "s8", colaboradorId: "c08", colaboradorNombre: "Jorge Castillo", tipo: "Salida temprano", fechaInicio: "2026-06-25", fechaFin: "2026-06-25", dias: 0, estado: "ajuste" },
  { id: "s9", colaboradorId: "c17", colaboradorNombre: "Isabel Torres", tipo: "Permiso sin goce", fechaInicio: "2026-07-14", fechaFin: "2026-07-14", dias: 1, estado: "pendiente" },
  { id: "s10", colaboradorId: "c10", colaboradorNombre: "Fernando Reyes", tipo: "Defunción familiar", fechaInicio: "2026-06-23", fechaFin: "2026-06-25", dias: 3, estado: "aprobada" },
];

export const TIPOS_PERMISO = [
  "Olvido de gafete", "Cambio de horario", "Llegada tarde", "Salida temprano", "Día flotante",
  "Vacaciones", "Laboral", "Personal tiempo por tiempo", "Nacimiento o adopción", "Defunción",
  "Matrimonio", "Permiso sin goce de sueldo", "Teletrabajo por fuerza mayor", "Salida intermedia",
];

export const INCIDENCIAS_ASISTENCIA: IncidenciaAsistencia[] = [
  { id: "a1", colaboradorId: "c13", colaboradorNombre: "Héctor Díaz", fecha: "2026-06-24", tipo: "retardo", minutos: 18, origen: "biometrico", estado: "detectada", area: "Manufactura" },
  { id: "a2", colaboradorId: "c16", colaboradorNombre: "Ricardo Ortega", fecha: "2026-06-24", tipo: "retardo", minutos: 25, origen: "app_movil", estado: "correccion_pendiente", area: "Comercial" },
  { id: "a3", colaboradorId: "c17", colaboradorNombre: "Isabel Torres", fecha: "2026-06-23", tipo: "ausencia", origen: "biometrico", estado: "validada", area: "Operaciones" },
  { id: "a4", colaboradorId: "c04", colaboradorNombre: "Carlos Hernández", fecha: "2026-06-22", tipo: "hora_extra", minutos: 90, origen: "manual", estado: "reportada_nomina", area: "Manufactura" },
  { id: "a5", colaboradorId: "c06", colaboradorNombre: "Roberto Sánchez", fecha: "2026-06-24", tipo: "salida_temprano", minutos: 40, origen: "app_movil", estado: "detectada", area: "Comercial" },
  { id: "a6", colaboradorId: "c03", colaboradorNombre: "María López", fecha: "2026-06-21", tipo: "correccion", origen: "manual", estado: "correccion_pendiente", area: "Nómina" },
  { id: "a7", colaboradorId: "c10", colaboradorNombre: "Fernando Reyes", fecha: "2026-06-20", tipo: "retardo", minutos: 8, origen: "biometrico", estado: "validada", area: "Seguridad e Higiene" },
  { id: "a8", colaboradorId: "c06", colaboradorNombre: "Roberto Sánchez", fecha: "2026-06-19", tipo: "ausencia", origen: "biometrico", estado: "validada", area: "Comercial" },
  { id: "a9", colaboradorId: "c13", colaboradorNombre: "Héctor Díaz", fecha: "2026-06-18", tipo: "hora_extra", minutos: 120, origen: "biometrico", estado: "reportada_nomina", area: "Manufactura" },
  { id: "a10", colaboradorId: "c16", colaboradorNombre: "Ricardo Ortega", fecha: "2026-06-17", tipo: "correccion", origen: "manual", estado: "detectada", area: "Comercial" },
];

export const CURSOS: CursoLms[] = [
  { id: "cu1", nombre: "Seguridad industrial NOM-026", categoria: "Seguridad", duracionHoras: 8, modalidad: "Presencial", estatus: "completado", fechaLimite: "2026-03-01", calificacion: 92, obligatorio: true, colaboradorId: "c13" },
  { id: "cu2", nombre: "Inducción URREA", categoria: "Corporativo", duracionHoras: 4, modalidad: "En línea", estatus: "vencido", fechaLimite: "2026-01-15", obligatorio: true, colaboradorId: "c17" },
  { id: "cu3", nombre: "Excel avanzado", categoria: "Tecnología", duracionHoras: 12, modalidad: "Híbrido", estatus: "en_progreso", fechaLimite: "2026-07-30", calificacion: 78, obligatorio: false, colaboradorId: "c11" },
  { id: "cu4", nombre: "Liderazgo colaborativo", categoria: "Desarrollo", duracionHoras: 16, modalidad: "Presencial", estatus: "asignado", fechaLimite: "2026-08-15", obligatorio: false, colaboradorId: "c04" },
  { id: "cu5", nombre: "Código de ética", categoria: "Compliance", duracionHoras: 2, modalidad: "SCORM", estatus: "completado", fechaLimite: "2026-02-01", calificacion: 100, obligatorio: true },
  { id: "cu6", nombre: "DC-3 Soldadura básica", categoria: "Técnico", duracionHoras: 40, modalidad: "Presencial", estatus: "en_progreso", fechaLimite: "2026-09-01", obligatorio: true, colaboradorId: "c13" },
  { id: "cu7", nombre: "Ventas consultivas", categoria: "Comercial", duracionHoras: 8, modalidad: "En línea", estatus: "asignado", fechaLimite: "2026-07-10", obligatorio: false, colaboradorId: "c06" },
  { id: "cu8", nombre: "Protección de datos", categoria: "Compliance", duracionHoras: 3, modalidad: "xAPI", estatus: "vencido", fechaLimite: "2026-05-01", obligatorio: true, colaboradorId: "c08" },
];

export const OBJETIVOS_OKR: ObjetivoOkr[] = [
  { id: "o1", colaboradorId: "c02", colaboradorNombre: "Luis Martínez", objetivo: "Reducir merma productiva 8%", kpi: "Merma %", avance: 65, periodo: "Q2 2026", estado: "en_curso", alineacion: "Eficiencia operativa" },
  { id: "o2", colaboradorId: "c05", colaboradorNombre: "Patricia Ruiz", objetivo: "Implementar portal DH", kpi: "Módulos activos", avance: 78, periodo: "Q2 2026", estado: "en_curso", alineacion: "Transformación digital" },
  { id: "o3", colaboradorId: "c12", colaboradorNombre: "Sofía Ramírez", objetivo: "Incrementar ventas zona norte 12%", kpi: "Ventas MXN", avance: 42, periodo: "Q2 2026", estado: "en_riesgo", alineacion: "Crecimiento comercial" },
  { id: "o4", colaboradorId: "c14", colaboradorNombre: "Miguel Fuentes", objetivo: "Integración SAP-CDM estable", kpi: "Uptime sync", avance: 88, periodo: "Q2 2026", estado: "en_curso", alineacion: "Infraestructura" },
  { id: "o5", colaboradorId: "c03", colaboradorNombre: "María López", objetivo: "Cerrar nómina en T+2", kpi: "Días cierre", avance: 95, periodo: "Q2 2026", estado: "completado", alineacion: "Excelencia nómina" },
  { id: "o6", colaboradorId: "c09", colaboradorNombre: "Gabriela Pérez", objetivo: "95% cumplimiento capacitación obligatoria", kpi: "% cumplimiento", avance: 71, periodo: "Q2 2026", estado: "en_curso", alineacion: "Desarrollo talento" },
];

export const ENCUESTAS: EncuestaDh[] = [
  { id: "e1", titulo: "Clima laboral 2026", tipo: "clima", audiencia: "Toda la empresa", anonima: true, tasaRespuesta: 68, estatus: "activa", fechaCierre: "2026-07-15" },
  { id: "e2", titulo: "Pulse junio — Bienestar", tipo: "pulse", audiencia: "Monterrey HQ", anonima: true, tasaRespuesta: 54, estatus: "activa", fechaCierre: "2026-06-30" },
  { id: "e3", titulo: "Onboarding 30 días — Javier O.", tipo: "onboarding", audiencia: "Individual", anonima: false, tasaRespuesta: 0, estatus: "borrador", fechaCierre: "2026-07-16" },
  { id: "e4", titulo: "Encuesta de salida — Oscar D.", tipo: "salida", audiencia: "Individual", anonima: true, tasaRespuesta: 100, estatus: "cerrada", fechaCierre: "2026-05-20" },
  { id: "e5", titulo: "Servicio interno RH", tipo: "libre", audiencia: "Desarrollo Humano", anonima: true, tasaRespuesta: 82, estatus: "cerrada", fechaCierre: "2026-04-01" },
];

export const TICKETS: TicketServicio[] = [
  { id: "tk1", folio: "DH-2026-0142", categoria: "Nómina", asunto: "Aclaración deducción ISR", solicitante: "Carlos Hernández", responsable: "María López", slaHoras: 24, estado: "en_proceso", prioridad: "media", createdAt: "2026-06-24T09:00:00" },
  { id: "tk2", folio: "DH-2026-0143", categoria: "Vacaciones", asunto: "Saldo no actualizado", solicitante: "Laura Vargas", responsable: "Patricia Ruiz", slaHoras: 48, estado: "asignado", prioridad: "alta", createdAt: "2026-06-24T11:30:00" },
  { id: "tk3", folio: "DH-2026-0138", categoria: "Accesos", asunto: "Restablecer VPN", solicitante: "Jorge Castillo", responsable: "Mesa TI", slaHoras: 8, estado: "resuelto", prioridad: "alta", createdAt: "2026-06-23T14:00:00" },
  { id: "tk4", folio: "DH-2026-0135", categoria: "Beneficios", asunto: "Alta dependiente GMM", solicitante: "Roberto Sánchez", responsable: "Patricia Ruiz", slaHoras: 72, estado: "en_espera", prioridad: "baja", createdAt: "2026-06-22T10:00:00" },
  { id: "tk5", folio: "DH-2026-0130", categoria: "Documentos", asunto: "Constancia laboral", solicitante: "Héctor Díaz", responsable: "Valentina Cruz", slaHoras: 24, estado: "cerrado", prioridad: "media", createdAt: "2026-06-20T08:00:00" },
  { id: "tk6", folio: "DH-2026-0144", categoria: "Capacitación", asunto: "No puedo acceder curso SCORM", solicitante: "Gabriela Pérez", responsable: "Mesa TI", slaHoras: 24, estado: "nuevo", prioridad: "media", createdAt: "2026-06-24T16:00:00" },
];

export const DENUNCIAS: DenunciaCaso[] = [
  { id: "dn1", token: "URR-7X4K-2026", tipo: "conducta", estado: "investigacion", fecha: "2026-06-10", responsable: "Comité de Ética", confidencial: true },
  { id: "dn2", token: "URR-9M2P-2026", tipo: "conflicto_interes", estado: "triage", fecha: "2026-06-18", confidencial: true },
  { id: "dn3", token: "URR-3L8Q-2026", tipo: "seguridad", estado: "recibido", fecha: "2026-06-23", confidencial: true },
  { id: "dn4", token: "URR-1A5R-2026", tipo: "fraude", estado: "cerrado", fecha: "2026-05-05", responsable: "Auditoría Interna", confidencial: true },
];

export const LOGS_INTEGRACION: LogIntegracion[] = [
  { id: "l1", proceso: "Alta colaborador", origen: "SAP", destino: "CDM", registrosOk: 3, registrosError: 0, estado: "exitoso", fecha: "2026-06-24T02:00:00" },
  { id: "l2", proceso: "Actualización colaborador", origen: "CDM", destino: "Portal DH", registrosOk: 847, registrosError: 2, estado: "parcial", fecha: "2026-06-24T02:15:00", mensaje: "2 registros sin jefe inmediato" },
  { id: "l3", proceso: "Cambio centro de costo", origen: "SAP", destino: "CDM", registrosOk: 12, registrosError: 0, estado: "exitoso", fecha: "2026-06-24T02:05:00" },
  { id: "l4", proceso: "Permisos → Nómina", origen: "Portal DH", destino: "Nómina", registrosOk: 28, registrosError: 1, estado: "parcial", fecha: "2026-06-24T06:00:00", mensaje: "Incidencia sin centro de costo" },
  { id: "l5", proceso: "Asistencias → Nómina", origen: "Portal DH", destino: "Nómina", registrosOk: 1520, registrosError: 0, estado: "exitoso", fecha: "2026-06-24T06:30:00" },
  { id: "l6", proceso: "Baja colaborador", origen: "SAP", destino: "CDM", registrosOk: 1, registrosError: 0, estado: "exitoso", fecha: "2026-06-23T02:00:00" },
  { id: "l7", proceso: "Reingreso", origen: "SAP", destino: "CDM", registrosOk: 1, registrosError: 0, estado: "exitoso", fecha: "2026-06-20T02:00:00" },
  { id: "l8", proceso: "Headcount → Data Warehouse", origen: "Portal DH", destino: "DW", registrosOk: 847, registrosError: 0, estado: "exitoso", fecha: "2026-06-24T07:00:00" },
  { id: "l9", proceso: "Cambio puesto", origen: "SAP", destino: "CDM", registrosOk: 5, registrosError: 1, estado: "parcial", fecha: "2026-06-22T02:00:00", mensaje: "Puesto no mapeado en catálogo" },
  { id: "l10", proceso: "Prenómina incidencias", origen: "Portal DH", destino: "Nómina", registrosOk: 0, registrosError: 15, estado: "fallido", fecha: "2026-06-21T06:00:00", mensaje: "Timeout conexión nómina" },
];

export const FEED: FeedPublicacion[] = [
  { id: "f1", autor: "Patricia Ruiz", area: "Desarrollo Humano", contenido: "Bienvenidos al nuevo Portal DH URREA. Centralizamos procesos de personas en una sola plataforma.", tipo: "anuncio", reacciones: 86, comentarios: 24, fecha: "2026-06-24T09:00:00" },
  { id: "f2", autor: "Luis Martínez", area: "Operaciones", contenido: "Reconocemos al turno matutino por 120 días sin accidentes.", tipo: "reconocimiento", reacciones: 142, comentarios: 31, fecha: "2026-06-23T14:00:00" },
  { id: "f3", autor: "Comité Cultura", area: "Corporativo", contenido: "Registro abierto: Día de la Familia URREA 2026 — 12 de julio.", tipo: "evento", reacciones: 58, comentarios: 19, fecha: "2026-06-22T08:00:00" },
];

export const BENEFICIOS_SOLICITUDES: BeneficioSolicitud[] = [
  { id: "b1", colaboradorNombre: "Roberto Sánchez", beneficio: "Alta dependiente GMM", estatus: "en_proceso", fecha: "2026-06-20" },
  { id: "b2", colaboradorNombre: "María López", beneficio: "Reembolso telemedicina", estatus: "aprobado", fecha: "2026-06-18" },
  { id: "b3", colaboradorNombre: "Carlos Hernández", beneficio: "Vales despensa", estatus: "solicitado", fecha: "2026-06-24" },
];

export const TIMELINE: EventoTimeline[] = [
  { id: "ev1", modulo: "Integraciones", descripcion: "Sync nocturna SAP→CDM completada con 2 errores", usuario: "Sistema", fecha: "2026-06-24T02:15:00", tipo: "integracion" },
  { id: "ev2", modulo: "Vacaciones", descripcion: "María López solicitó 5 días de vacaciones", usuario: "María López", fecha: "2026-06-24T10:30:00", tipo: "info" },
  { id: "ev3", modulo: "Onboarding", descripcion: "Examen médico vencido — Javier Ortiz", usuario: "Sistema", fecha: "2026-06-24T08:00:00", tipo: "alerta" },
  { id: "ev4", modulo: "Capacitación", descripcion: "3 cursos obligatorios vencen esta semana", usuario: "Sistema", fecha: "2026-06-24T07:00:00", tipo: "alerta" },
  { id: "ev5", modulo: "Integraciones", descripcion: "Fallo prenómina — reintentar manualmente", usuario: "Sistema", fecha: "2026-06-21T06:00:00", tipo: "critico" },
];

export const HEADCOUNT_POR_AREA = AREAS.map((area) => ({
  area,
  count: COLABORADORES.filter((c) => c.area === area && c.estatus === "activo").length,
}));

export const SOLICITUDES_POR_MODULO = [
  { modulo: "Vacaciones/Permisos", count: 10 },
  { modulo: "Beneficios", count: 3 },
  { modulo: "Servicios", count: 6 },
  { modulo: "Asistencia", count: 10 },
  { modulo: "Capacitación", count: 4 },
];

export function getColaborador(id: string) {
  return COLABORADORES.find((c) => c.id === id);
}

export function getSubordinados(jefeId: string) {
  return COLABORADORES.filter((c) => c.jefeId === jefeId);
}
