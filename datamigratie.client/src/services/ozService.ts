export type OZZaaktype = {
  uuid: string;
  naam: string;
};

export const ozService = {
  getAllZaaktypes: async (): Promise<OZZaaktype[]> => [
    {
      uuid: "stringa",
      naam: "Zaaktype A"
    },
    {
      uuid: "stringb",
      naam: "Zaaktype B"
    }
  ]
};
