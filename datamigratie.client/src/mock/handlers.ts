import { http, HttpResponse } from "msw";

export const handlers = [
  http.get("/api/migration", async () => {
    return HttpResponse.json({
      detZaaktypeId: "EVG",
      status: "inProgress"
    });
  })
];
