using Xunit;
using OpenQA.Selenium;

namespace BIE.E2ETests;

public class BrowserTests : BrowserTestBase
{
    [Fact]
    public void TouristAt31DaysIsRejected()
    {
        Fill("Tourist", "Tourismus", "31");
        Driver.FindElement(By.Id("submit")).Click();
        Assert.Contains("ABGELEHNT", Decision("rejected").Text);
    }

    [Fact]
    public void AllSixProfilesHavePortraitsAndTransferSecurityFields()
    {
        string[] names = ["Zorg Blubber III", "Nox Taschenuniversum", "Spora Flausch", "Lexa Vielgesicht", "Kira Lichtleiter", "Borin Schaltkreis"];
        for (int i = 0; i < names.Length; i++)
        {
            Assert.Equal(1, Driver.FindElements(By.CssSelector(".sample-card")).Count(c => c.Displayed));
            var card = Driver.FindElement(By.CssSelector(".sample-card:not([hidden])"));
            Assert.Contains(names[i], card.Text);
            Wait.Until(d => true.Equals(((IJavaScriptExecutor)d).ExecuteScript("const i=document.getElementById('sample-portrait');return i.complete && i.naturalWidth>0;")));
            Assert.Equal("Porträt von " + names[i], Driver.FindElement(By.Id("sample-portrait")).GetDomAttribute("alt"));
            card.FindElement(By.TagName("button")).Click();
            Assert.Equal(names[i], Driver.FindElement(By.Id("name")).GetDomProperty("value"));
            Assert.Equal(i == 1, Driver.FindElement(By.Id("blackHole")).Selected);
            Assert.Equal(i == 2, Driver.FindElement(By.Id("fungus")).Selected);
            Assert.Equal(i == 3, Driver.FindElement(By.Id("shapeshifter")).Selected);
            Assert.True(Driver.FindElement(By.Id("validIdentity")).Selected);
            if (i >= 4)
                Assert.Equal("Arbeit", Driver.FindElement(By.Id("purpose")).GetDomProperty("value"));
            Driver.FindElement(By.Id("sample-next")).Click();
        }

        Driver.FindElement(By.Id("sample-prev")).Click();
        Assert.Contains("6 / 6", Driver.FindElement(By.Id("sample-position")).Text);
        Driver.FindElement(By.CssSelector(".sample-card:not([hidden]) button")).Click();
        Driver.FindElement(By.Id("submit")).Click();
        Assert.Contains("GENEHMIGT", Decision("approved").Text);
    }
}
