using CandidateTesting.RodineiRiboli.Core.Interfaces;

namespace CandidateTesting.RodineiRiboli.Infrasctructure.ExternalApi
{
    public class ConsumeAwsS3 : IConsumeAwsS3
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ConsumeAwsS3(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetLogMinhaCdn(string uri)
        {
            var responseApi = string.Empty;
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(uri);

                response.EnsureSuccessStatusCode();
                responseApi = await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return responseApi;
        }
    }
}
