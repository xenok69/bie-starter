using BIE.Core;
using Xunit;

namespace BIE.UnitTests;

public class PolicyTests
{
    [Theory]
    [InlineData(1, "approved")]
    [InlineData(29, "approved")]
    [InlineData(31, "rejected")]
    [InlineData(365, "rejected")]
    public void TouristBoundary(int days, string expected) => Assert.Equal(expected, EntryPolicy.Evaluate(new("Test", "Orion", "Tourismus", days, false, false, false, true)).Status);
    [Theory]
    [InlineData(90)]
    [InlineData(180)]
    [InlineData(365)]
    public void WorkIsNotLimitedToThirtyDays(int days) => Assert.Equal("approved", EntryPolicy.Evaluate(new("Arbeit", "Orion", "Arbeit", days, false, false, false, true)).Status);
    [Fact]
    public void BlackHoleOverridesFungus() => Assert.Equal("rejected", EntryPolicy.Evaluate(new("Nox", "Orion", "Diplomatie", 3, true, true, false, true)).Status);
    [Fact]
    public void FungusRequiresQuarantine() => Assert.Equal("quarantine", EntryPolicy.Evaluate(new("Spora", "Kepler-186f", "Tourismus", 12, false, true, false, true)).Status);
    [Fact]
    public void ShapeshifterNeedsIdentity() => Assert.Equal("rejected", EntryPolicy.Evaluate(new("Lexa", "Orion", "Tourismus", 10, false, false, true, false)).Status);
    [Fact]
    public void RejectionOverridesQuarantine() => Assert.Equal("rejected", EntryPolicy.Evaluate(new("Test", "Orion", "Tourismus", 31, false, true, false, true)).Status);
    [Theory]
    [InlineData(0)]
    [InlineData(366)]
    public void InvalidDurationThrows(int days) => Assert.Throws<ArgumentOutOfRangeException>(() => EntryPolicy.Evaluate(new("Test", "Orion", "Arbeit", days, false, false, false, true)));
}
