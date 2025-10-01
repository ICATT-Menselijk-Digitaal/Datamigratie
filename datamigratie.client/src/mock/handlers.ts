import { http, HttpResponse } from "msw";
import type { MigrationStatus } from "@/services/datamigratieService";

export const handlers = [
  http.get("/api/migration", async () => {
    return HttpResponse.json({
      detZaaktypeId: "EVG",
      isRunning: false
    });
  }),
  http.post("/api/migration", async ({ request }) => {
    const migrationStatus = (await request.json()) as MigrationStatus;

    return HttpResponse.json({
      detZaaktypeId: migrationStatus.detZaaktypeId,
      isRunning: migrationStatus.isRunning
    });
  })
];
