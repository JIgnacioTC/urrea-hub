export interface HcmDashboard {
  activeEmployees: number;
  costCenters: number;
  organizationUnits: number;
  locations: number;
  lastNominaSync?: string;
}

export interface HcmCatalogItem {
  id: string;
  name: string;
  code?: string;
}

export interface HcmCatalogs {
  locations: HcmCatalogItem[];
  departments: HcmCatalogItem[];
  positions: HcmCatalogItem[];
  costCenters: HcmCatalogItem[];
  contractTypes: HcmCatalogItem[];
}

export interface EmployeeListItem {
  id: string;
  employeeNumber: string;
  legalFullName: string;
  preferredName?: string;
  position: string;
  department: string;
  subarea?: string;
  area?: string;
  costCenter?: string;
  managerName?: string;
  location?: string;
  contractType: string;
  status: string;
  hireDate: string;
  tenureYears: number;
  externalSource?: string;
  syncStatus: string;
  lastSyncAt?: string;
  isManualOverride: boolean;
  esCuentaGenerica: boolean;
  puedenChecarRemotamente: boolean;
}

export interface EmployeeDetail {
  id: string;
  employeeNumber: string;
  legalFirstName: string;
  legalLastName: string;
  legalMiddleName?: string;
  preferredName?: string;
  workEmail: string;
  phone?: string;
  status: string;
  hireDate: string;
  terminationDate?: string;
  position: string;
  department: string;
  subarea?: string;
  area?: string;
  location?: string;
  costCenter?: string;
  contractType: string;
  managerName?: string;
  tenureYears: number;
  externalSource?: string;
  externalEmployeeId?: string;
  syncStatus: string;
  lastSyncAt?: string;
  isManualOverride: boolean;
  esCuentaGenerica: boolean;
  puedenChecarRemotamente: boolean;
  personal?: {
    rfc?: string;
    curp?: string;
    nss?: string;
    birthDate?: string;
    address?: string;
    isMasked: boolean;
  };
  employment?: {
    workSchedule?: string;
    shift?: string;
    payrollGroup?: string;
    unionized: boolean;
    compensationVisibility: string;
  };
  directReports: Array<{
    id: string;
    employeeNumber: string;
    fullName: string;
    position: string;
  }>;
}

export interface EmployeeOrganization {
  managerId?: string;
  managerName?: string;
  managerEmployeeNumber?: string;
  departmentId: string;
  department: string;
  subarea?: string;
  area?: string;
  locationId?: string;
  location?: string;
  costCenterId?: string;
  costCenter?: string;
  positionId: string;
  position: string;
  directReportsCount: number;
}

export interface EmployeeMovement {
  id: string;
  movementType: string;
  effectiveDate: string;
  previousValue?: string;
  newValue?: string;
  source: string;
  externalReference?: string;
  createdBy: string;
  createdAt: string;
}

export interface EmployeeAuditLog {
  occurredAt: string;
  module: string;
  action: string;
  user?: string;
  detail?: string;
}

export interface EmployeeVacationSummary {
  year: number;
  daysAssigned: number;
  daysUsed: number;
  daysPending: number;
  requestsThisYear: number;
  pendingApproval: number;
}

export interface EmployeeDocument {
  id: string;
  name: string;
  documentType: string;
  version: number;
  status: string;
  validUntil?: string;
}

export interface EmployeeModuleLink {
  module: string;
  label: string;
  recordCount: number;
  moduleAvailable: boolean;
  statusMessage: string;
}

export interface HcmDataQuality {
  withoutManager: number;
  withoutCostCenter: number;
  missingRfc: number;
  missingCurp: number;
  missingNss: number;
  pendingSync: number;
  manualOverrideConflicts: number;
  totalActive: number;
}

export interface HcmDataQualityIssue {
  code: string;
  label: string;
  count: number;
  samples: Array<{ id: string; employeeNumber: string; fullName: string; position: string }>;
}

export interface HcmDataQualityReport {
  summary: HcmDataQuality;
  issues: HcmDataQualityIssue[];
}

export interface EmployeeUpdatePayload {
  preferredName?: string;
  phone?: string;
  esCuentaGenerica?: boolean;
  puedenChecarRemotamente?: boolean;
}
