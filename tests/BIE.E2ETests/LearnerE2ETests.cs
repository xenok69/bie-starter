using Xunit;
using OpenQA.Selenium;

namespace BIE.E2ETests;

public class LearnerE2ETests : BrowserTestBase
{
    [Fact]
    public void TouristAt30DaysRemainsApprovedAfterReload()
    {
        Fill("Sam Browser", "Tourismus", "30");
        Driver.FindElement(By.Id("submit")).Click();
        Assert.Contains("GENEHMIGT", Decision("approved").Text);
        var caseId = Driver.FindElement(By.Id("result")).GetDomAttribute("data-application-id");
        Driver.Navigate().Refresh();
        var row = Wait.Until(d => d.FindElements(By.CssSelector("#application-list tr")).FirstOrDefault(r => r.GetDomAttribute("data-id") == caseId));
        var cells = row.FindElements(By.TagName("td"));
        Assert.Equal("Sam Browser", cells[1].Text);
        Assert.Equal("Orion", cells[2].Text);
        Assert.Equal("30 Tage", cells[3].Text);
        Assert.Equal("GENEHMIGT", cells[4].Text);
    }
}
