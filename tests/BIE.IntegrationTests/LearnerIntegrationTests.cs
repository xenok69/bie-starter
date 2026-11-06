using System.Net;
using System.Net.Http.Json;
using BIE.Core;
using Xunit;

namespace Bie.IntegrationTests;

public class LearnerIntegrationTests
{
    [Fact]
    public async Task TouristAt30DaysIsSavedAsApproved()
    {
        using var app = new BieFactory();
        using var client = app.CreateClient();
        var entry = new Entry("Sam Test", "Orion", "Tourismus", 30, false, false, false, true);
        var response = await client.PostAsJsonAsync("/api/cases", entry);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var records = await client.GetFromJsonAsync<List<Case>>("/api/cases");
        Assert.NotNull(records);
        Assert.Equal(1, records.Count);
        Assert.Equal(entry, records[0].Applicant);
        Assert.Equal("approved", records[0].Decision.Status);
    }
}
