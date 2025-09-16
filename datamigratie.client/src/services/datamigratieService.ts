import { get, post, put } from "@/utils/fetchWrapper";

export type ZaaktypeMapping = {
  detZaaktypeId: string;
  ozZaaktypeId: string;
};

export type UpdateZaaktypeMapping = {
  detZaaktypeId: string;
  updatedOzZaaktypeId: string;
};

export type MigrationStatus = {
  detFunctioneleIdentificatie: string;
  isRunning: boolean;
  startedAt?: string;
  stoppedAt?: string;
};

export const datamigratieService = {
  getMappingByDETZaaktypeId: (detZaaktypeId: string): Promise<ZaaktypeMapping> =>
    get<ZaaktypeMapping>(`/api/mapping/${detZaaktypeId}`),
  createMapping: (payload: ZaaktypeMapping): Promise<ZaaktypeMapping> =>
    post<ZaaktypeMapping>(`/api/mapping/${payload.detZaaktypeId}`, payload),
  updateMapping: (payload: UpdateZaaktypeMapping): Promise<ZaaktypeMapping> =>
    put<ZaaktypeMapping>(`/api/mapping/${payload.detZaaktypeId}`, payload)
};
