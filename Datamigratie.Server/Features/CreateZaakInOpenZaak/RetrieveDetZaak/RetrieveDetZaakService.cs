using Datamigratie.Common.Services.Det;
using Datamigratie.Common.Services.Det.Models;

namespace Datamigratie.Server.Features.CreateZaakInOpenZaak.RetrieveDetZaak
{
    public interface IRetrieveDetZaakService
    {
        Task<DetZaak?> GetZaakByZaaknummer(string zaaknummer);
    }

    public class RetrieveDetZaakService : IRetrieveDetZaakService
    {
        private readonly IDetApiClient _detApiClient;
        private readonly ILogger<RetrieveDetZaakService> _logger;

        public RetrieveDetZaakService(IDetApiClient detApiClient, ILogger<RetrieveDetZaakService> logger)
        {
            _detApiClient = detApiClient;
            _logger = logger;
        }

        public async Task<DetZaak?> GetZaakByZaaknummer(string zaaknummer)
        {
            try
            {
                var zaak = await _detApiClient.GetZaakByZaaknummer(zaaknummer);

                if (zaak == null)
                {
                    return null;
                }

                return zaak;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving zaak {Zaaknummer} from DET", zaaknummer);
                throw;
            }
        }
    }
}