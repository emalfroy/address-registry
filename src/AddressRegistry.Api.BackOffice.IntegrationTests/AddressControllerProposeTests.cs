namespace AddressRegistry.Api.BackOffice.IntegrationTests
{
    using System;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    public class AddressControllerProposeTests
    {
        [Fact]
        public async Task List()
        {
            try
            {
                var application = new WebApplicationFactory<Program>();

                var client = application.CreateClient();

                var uri = "v2/adres/acties/voorstellen";
                
                var response = await client.PostAsync(uri, JsonContent.Create(new
                {
                    postInfoId = "https://data.vlaanderen.be/id/postinfo/9000",
                    straatNaamId = "https://data.vlaanderen.be/id/straatnaam/45041",
                    huisNummer = "11",
                    busNummer = "3A"
                }));

                if (response != null)
                {

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

