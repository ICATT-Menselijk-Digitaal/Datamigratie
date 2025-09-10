import { get } from "@/utils/fetchWrapper";
import type { Zaaktype } from "@/types/zaaktype";

export const zaaktypeService = {
  getAllZaaktypes: (): Promise<Zaaktype[]> => get<Zaaktype[]>(`/api/zaaktypen`),
  getZaaktypeByFunctioneleIdentificatie: (functioneleIdentificatie: string): Promise<Zaaktype> =>
    get<Zaaktype>(`/api/zaaktypen/info/${functioneleIdentificatie}`)
};
