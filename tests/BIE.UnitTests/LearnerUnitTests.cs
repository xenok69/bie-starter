using BIE.Core;
using Xunit;

namespace BIE.UnitTests;

public class LearnerUnitTests
{
    [Fact]
    public void TouristAt30DaysIsApproved()
    {
        var entry = new Entry("Sam Test", "Orion", "Tourismus", 30, false, false, false, true);
        var decision = EntryPolicy.Evaluate(entry);
        Assert.Equal("approved", decision.Status);
    }
}
