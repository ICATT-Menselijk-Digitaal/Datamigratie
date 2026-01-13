import { get, post, put } from "@/utils/fetchWrapper";

export type ZaaktypeMapping = {
  id: string;
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

export type MigrationStatus = typeof MigrationStatus[keyof typeof MigrationStatus];

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

export type GlobalConfiguration = {
  rsin?: string;
  updatedAt?: string;
};

export type UpdateGlobalConfiguration = {
  rsin?: string;
};

export type DetStatus = {
  naam: string;
  omschrijving: string;
  eind: boolean;
};

export type OzStatustype = {
  uuid: string;
  omschrijving: string;
  isEindstatus: boolean;
};

export type StatusMappingItem = {
  detStatusNaam: string;
  ozStatustypeId: string | null;
};

export type StatusMappingsResponse = {
  detStatusNaam: string;
  ozStatustypeId: string;
};

export type SaveStatusMappingsRequest = {
  mappings: StatusMappingItem[];
};

export type StatusMappingValidationResponse = {
  allStatusesMapped: boolean;
};

export type ResultaattypeMappingItem = {
  detResultaattypeNaam: string;
  ozResultaattypeId: string | null;
};

export type SaveResultaattypeMappingsRequest = {
  mappings: ResultaattypeMappingItem[];
};

export type ResultaattypeMappingResponse = {
  detResultaattypeNaam: string;
  ozResultaattypeId: string;
};

export const datamigratieService = {
  getMappingByDETZaaktypeId: (detZaaktypeId: string): Promise<ZaaktypeMapping> =>
    get<ZaaktypeMapping>(`/api/mapping/${detZaaktypeId}`),
  createMapping: (payload: ZaaktypeMapping): Promise<ZaaktypeMapping> =>
    post<ZaaktypeMapping>(`/api/mapping/${payload.detZaaktypeId}`, payload),
  updateMapping: (payload: UpdateZaaktypeMapping): Promise<ZaaktypeMapping> =>
    put<ZaaktypeMapping>(`/api/mapping/${payload.detZaaktypeId}`, payload),
  startMigration: (payload: StartMigration): Promise<void> => post(`/api/migration/start`, payload),
  getMigration: (): Promise<Migration> => get(`/api/migration`),
  getMigrationHistory: (detZaaktypeId: string): Promise<MigrationHistoryItem[]> =>
    get<MigrationHistoryItem[]>(`/api/migration/history/${detZaaktypeId}`),
  getMigrationRecords: (migrationId: number): Promise<MigrationRecordItem[]> =>
    get<MigrationRecordItem[]>(`/api/migration/${migrationId}/records`),
  getGlobalConfiguration: (): Promise<GlobalConfiguration> => get<GlobalConfiguration>(`/api/globalmapping`),
  updateGlobalConfiguration: (payload: UpdateGlobalConfiguration): Promise<GlobalConfiguration> =>
    put<GlobalConfiguration>(`/api/globalmapping`, payload),
  getStatusMappings: (zaaktypenMappingId: string): Promise<StatusMappingsResponse[]> =>
    get<StatusMappingsResponse[]>(`/api/mappings/${zaaktypenMappingId}/statuses`),
  saveStatusMappings: (zaaktypenMappingId: string, payload: SaveStatusMappingsRequest): Promise<void> =>
    post(`/api/mappings/${zaaktypenMappingId}/statuses`, payload),
  getResultaattypeMappings: (zaaktypenMappingId: string): Promise<ResultaattypeMappingResponse[]> =>
    get<ResultaattypeMappingResponse[]>(`/api/mapping/${zaaktypenMappingId}/resultaattypen`),
  saveResultaattypeMappings: (zaaktypenMappingId: string, payload: SaveResultaattypeMappingsRequest): Promise<void> =>
    post(`/api/mapping/${zaaktypenMappingId}/resultaattypen`, payload)
};
