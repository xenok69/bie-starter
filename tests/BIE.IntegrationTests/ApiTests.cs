using System.Net;
using System.Net.Http.Json;
using BIE.Core;
using BIE.Web;
using Xunit;

namespace Bie.IntegrationTests;

public class ApiTests
{
    [Fact]
    public async Task WorkIsCreatedAndCanBeFetched()
    {
        using var app = new BieFactory();
        using var client = app.CreateClient();
        var response = await client.PostAsJsonAsync("/api/cases", new Entry("  Kira  ", "Kepler-186f", "Arbeit", 90, false, false, false, true));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var saved = await response.Content.ReadFromJsonAsync<Case>();
        Assert.NotNull(saved);
        Assert.Equal("Kira", saved.Applicant.Name);
        Assert.Equal("approved", saved.Decision.Status);
        Assert.True(saved.Id > 0);
        Assert.Equal(saved, await client.GetFromJsonAsync<Case>(response.Headers.Location));
    }

    [Theory]
    [InlineData("", "Orion", "Arbeit", 90)]
    [InlineData("  ", "Orion", "Arbeit", 90)]
    [InlineData("Test", "Orion", "Arbeit", 0)]
    [InlineData("Test", "Orion", "Arbeit", 366)]
    [InlineData("Test", "Mars", "Arbeit", 90)]
    [InlineData("Test", "Orion", "Besuch", 90)]
    public async Task InvalidEntryIsNotSaved(string name, string planet, string purpose, int days)
    {
        using var app = new BieFactory();
        using var client = app.CreateClient();
        var response = await client.PostAsJsonAsync("/api/cases", new Entry(name, planet, purpose, days, false, false, false, true));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var all = await client.GetFromJsonAsync<List<Case>>("/api/cases");
        Assert.NotNull(all);
        Assert.Equal(0, all.Count);
    }

    [Theory]
    [InlineData(true, false, "rejected")]
    [InlineData(false, true, "quarantine")]
    public async Task ValidEntryIsSavedForEveryDecision(bool hole, bool fungus, string expected)
    {
        using var app = new BieFactory();
        using var client = app.CreateClient();
        var response = await client.PostAsJsonAsync("/api/cases", new Entry("Test", "Orion", "Arbeit", 90, hole, fungus, false, true));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var all = await client.GetFromJsonAsync<List<Case>>("/api/cases");
        Assert.NotNull(all);
        Assert.Equal(1, all.Count);
        Assert.Equal(expected, all[0].Decision.Status);
    }

    [Fact]
    public void RegistrySurvivesRestart()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".db");
        try
        {
            var saved = new Registry(path).Add(new("Borin", "Orion", "Arbeit", 180, false, false, false, true));
            Assert.Equal(saved, new Registry(path).All().Single());
        }
        finally
        {
            File.Delete(path);
        }
    }
}
