import { get, post, put } from "@/utils/fetchWrapper";

export type Mapping = {
  detFunctioneleIdentificatie?: string;
  ozUuid?: string;
};

export type MigrationStatus = {
  detFunctioneleIdentificatie: string;
  isRunning: boolean;
  startedAt?: string;
  stoppedAt?: string;
};

export const datamigratieService = {
  getMappingByDETFunctioneleIdentificatie: (
    detFunctioneleIdentificatie: string
  ): Promise<Mapping> => {
    return get<Mapping>(`/api/mapping/${detFunctioneleIdentificatie}`);
  },
  createMapping: (payload: Mapping): Promise<Mapping> => {
    return post<Mapping>(`/api/mapping`, payload);
  },
  updateMapping: (payload: Mapping): Promise<Mapping> => {
    return put<Mapping>(`/api/mapping/${payload.detFunctioneleIdentificatie}`, payload);
  },
  startMigration: (payload: MigrationStatus): Promise<MigrationStatus> => {
    return post(`/api/migration`, payload);
  },
  getMigrationStatus: (): Promise<MigrationStatus> => {
    return get(`/api/migration`);
  }
};
