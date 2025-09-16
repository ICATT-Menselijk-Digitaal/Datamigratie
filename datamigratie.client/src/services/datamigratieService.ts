import { get, post, put } from "@/utils/fetchWrapper";

export type Mapping = {
  detFunctioneleIdentificatie?: string;
  ozUuid?: string;
};

export const datamigratieService = {
  getMappingByDETFunctioneleIdentificatie: (
    detFunctioneleIdentificatie: string
  ): Promise<Mapping> => get<Mapping>(`/api/mapping/${detFunctioneleIdentificatie}`),
  createMapping: (payload: Mapping): Promise<Mapping> => post<Mapping>(`/api/mapping`, payload),
  updateMapping: (payload: Mapping): Promise<Mapping> =>
    put<Mapping>(`/api/mapping/${payload.detFunctioneleIdentificatie}`, payload)
};
