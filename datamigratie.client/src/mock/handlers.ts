import { http, HttpResponse } from "msw";

export const handlers = [
  http.get("/api/oz/zaaktypen", () =>
    HttpResponse.json([
      {
        uuid: "f47ac10b-58cc-4372-a567-0e02b2c3d479",
        naam: "Zaaktype A"
      },
      {
        uuid: "6ba7b810-9dad-11d1-80b4-00c04fd430c8",
        naam: "Zaaktype B"
      }
    ])
  ),
//   http.get("/api/mapping/:detFunctioneleIdentificatie", async ({ params }) => {
//     return HttpResponse.json({
//       detFunctioneleIdentificatie: params.detFunctioneleIdentificatie,
//       ozUuid: "6ba7b810-9dad-11d1-80b4-00c04fd430c8"
//     });
//   }),
  http.post("/api/mapping", async ({ params }) => {
    return HttpResponse.json({
      detFunctioneleIdentificatie: params.detFunctioneleIdentificatie,
      ozUuid: "6ba7b810-9dad-11d1-80b4-00c04fd430c8"
    });
  }),
  http.put("/api/mapping/:detFunctioneleIdentificatie", async ({ params }) => {
    return HttpResponse.json({
      detFunctioneleIdentificatie: params.detFunctioneleIdentificatie,
      ozUuid: "6ba7b810-9dad-11d1-80b4-00c04fd430c8"
    });
  })
];
