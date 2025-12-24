import { get, post, put } from "@/utils/fetchWrapper";

export type ZaaktypeMapping = {
  detZaaktypeId: string;
  ozZaaktypeId: string;
};

export type UpdateZaaktypeMapping = {
  detZaaktypeId: string;
  updatedOzZaaktypeId: string;
};

export type ResultaattypeMapping = {
  ozZaaktypeId: string;
  ozResultaattypeId: string;
};

export type UpdateResultaattypeMapping = {
  ozZaaktypeId: string;
  updatedOzResultaattypeId: string;
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
  totalRecords: number;
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

export const datamigratieService = {
  getMappingByDETZaaktypeId: (detZaaktypeId: string): Promise<ZaaktypeMapping> =>
    get<ZaaktypeMapping>(`/api/mapping/${detZaaktypeId}`),
  createMapping: (payload: ZaaktypeMapping): Promise<ZaaktypeMapping> =>
    post<ZaaktypeMapping>(`/api/mapping/${payload.detZaaktypeId}`, payload),
  updateMapping: (payload: UpdateZaaktypeMapping): Promise<ZaaktypeMapping> =>
    put<ZaaktypeMapping>(`/api/mapping/${payload.detZaaktypeId}`, payload),
  getResultaattypeMapping: (detZaaktypeId: string, detResultaattypeId: string): Promise<ResultaattypeMapping> =>
    get<ResultaattypeMapping>(`/api/mapping/resultaattype/${detZaaktypeId}/${detResultaattypeId}`),
  getAllResultaattypeMappingsForZaaktype: (detZaaktypeId: string): Promise<ResultaattypeMapping[]> =>
    get<ResultaattypeMapping[]>(`/api/mapping/resultaattype/${detZaaktypeId}`),
  createResultaattypeMapping: (detZaaktypeId: string, detResultaattypeId: string, payload: ResultaattypeMapping): Promise<void> =>
    post(`/api/mapping/resultaattype/${detZaaktypeId}/${detResultaattypeId}`, payload),
  updateResultaattypeMapping: (detZaaktypeId: string, detResultaattypeId: string, payload: UpdateResultaattypeMapping): Promise<void> =>
    put(`/api/mapping/resultaattype/${detZaaktypeId}/${detResultaattypeId}`, payload),
  startMigration: (payload: StartMigration): Promise<void> => post(`/api/migration/start`, payload),
  getMigration: (): Promise<Migration> => get(`/api/migration`),
  getMigrationHistory: (detZaaktypeId: string): Promise<MigrationHistoryItem[]> =>
    get<MigrationHistoryItem[]>(`/api/migration/history/${detZaaktypeId}`),
  getMigrationRecords: (migrationId: number): Promise<MigrationRecordItem[]> =>
    get<MigrationRecordItem[]>(`/api/migration/${migrationId}/records`)
};
