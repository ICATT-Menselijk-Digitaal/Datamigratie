// Shared types for the Datamigratie application

export type ZaaktypeOptionItem = {
  id: string;
  name: string;
};

export type ZaaktypeMapping = {
  id: string;
  detZaaktypeId: string;
  ozZaaktypeId: string;
};

export type CreateZaaktypeMapping = {
  detZaaktypeId: string;
  ozZaaktypeId: string;
};

export type UpdateZaaktypeMapping = {
  detZaaktypeId: string;
  updatedOzZaaktypeId: string;
};

export const MigrationStatus = Object.freeze({
  none: "None",
  inProgress: "InProgress"
});

export type MigrationStatus = (typeof MigrationStatus)[keyof typeof MigrationStatus];

export type Migration = {
  detZaaktypeId?: string;
  status: MigrationStatus;
};

export type StartMigration = {
  detZaaktypeId: string;
};

export type MigrationHistoryItem = {
  id: number;
  status: string;
  startedAt?: string;
  completedAt?: string;
  errorMessage?: string;
  totalRecords?: number;
  processedRecords: number;
  successfulRecords: number;
  failedRecords: number;
};

export type MigrationRecordItem = {
  id: number;
  detZaaknummer: string;
  ozZaaknummer?: string;
  isSuccessful: boolean;
  errorTitle?: string;
  errorDetails?: string;
  statusCode?: number;
  processedAt: string;
};

export type RsinConfiguration = {
  rsin?: string;
};

export type UpdateRsinConfiguration = {
  rsin?: string;
};

export type DocumentstatusMappingItem = {
  detDocumentstatus: string;
  ozDocumentstatus: string | null;
};

export type DocumentstatusMappingResponse = {
  detDocumentstatus: string;
  ozDocumentstatus: string;
};

export type SaveDocumentstatusMappingsRequest = {
  mappings: DocumentstatusMappingItem[];
};
