using Ploeh.FiftyBest.Pages;

namespace Ploeh.FiftyBest.Tests;

public sealed class YearsModelTests
{
    [Fact]
    public void PreviousYearTo2022()
    {
        var data = new FiftyBestData(new Dictionary<ushort, IReadOnlyCollection<FiftyBestEntry>>
        {
            { 2022, [] },
            { 2021, [] }
        });
        var sut = new YearsModel(data);

        sut.OnGet(2022);

        Assert.Equal((ushort)2021, sut.PreviousYear);
    }

    [Fact]
    public void PreviousToFirstYear()
    {
        var data = new FiftyBestData(new Dictionary<ushort, IReadOnlyCollection<FiftyBestEntry>>
        {
            { 2022, [] },
            { 2021, [] }
        });
        var sut = new YearsModel(data);

        sut.OnGet(2021);

        Assert.Null(sut.PreviousYear);
    }

    [Fact]
    public void PreviousToGapYear()
    {
        var data = new FiftyBestData(new Dictionary<ushort, IReadOnlyCollection<FiftyBestEntry>>
        {
            { 2021, [] },
            // No data for 2020
            { 2019, [] }
        });
        var sut = new YearsModel(data);

        sut.OnGet(2021);

        Assert.Equal((ushort)2019, sut.PreviousYear);
    }
}
