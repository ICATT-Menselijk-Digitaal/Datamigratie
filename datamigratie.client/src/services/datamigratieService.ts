import { get, post, put } from "@/utils/fetchWrapper";

export type ZaaktypeMapping = {
  detZaaktypeId: string;
  ozZaaktypeId: string;
};

export type UpdateZaaktypeMapping = {
  detZaaktypeId: string;
  updatedOzZaaktypeId: string;
};

export const MigrationStatus = Object.freeze({
  pending: "pending",
  inProgress: "inProgress",
  completed: "completed",
  failed: "failed",
  cancelled: "cancelled"
});

type MigrationStatus = keyof typeof MigrationStatus;

export type Migration = {
  detZaaktypeId: string;
  status?: MigrationStatus;
  createdAt?: string;
  startedAt?: string;
  completedAt?: string;
};

export const datamigratieService = {
  getMappingByDETZaaktypeId: (detZaaktypeId: string): Promise<ZaaktypeMapping> =>
    get<ZaaktypeMapping>(`/api/mapping/${detZaaktypeId}`),
  createMapping: (payload: ZaaktypeMapping): Promise<ZaaktypeMapping> =>
    post<ZaaktypeMapping>(`/api/mapping/${payload.detZaaktypeId}`, payload),
  updateMapping: (payload: UpdateZaaktypeMapping): Promise<ZaaktypeMapping> =>
    put<ZaaktypeMapping>(`/api/mapping/${payload.detZaaktypeId}`, payload),
  startMigration: (payload: Migration): Promise<Migration> => post(`/api/migration/start`, payload),
  getMigration: (): Promise<Migration> => get(`/api/migration`)
};
