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

    [Fact]
    public void NextYearTo2021()
    {
        var data = new FiftyBestData(new Dictionary<ushort, IReadOnlyCollection<FiftyBestEntry>>
        {
            { 2021, [] },
            { 2022, [] }
        });
        var sut = new YearsModel(data);

        sut.OnGet(2021);

        Assert.Equal((ushort)2022, sut.NextYear);
    }

    [Fact]
    public void NextYearToLastYear()
    {
        var data = new FiftyBestData(new Dictionary<ushort, IReadOnlyCollection<FiftyBestEntry>>
        {
            { 2021, [] },
            { 2022, [] }
        });
        var sut = new YearsModel(data);

        sut.OnGet(2022);

        Assert.Null(sut.NextYear);
    }

    [Fact]
    public void NextToGapYear()
    {
        var data = new FiftyBestData(new Dictionary<ushort, IReadOnlyCollection<FiftyBestEntry>>
        {
            { 2021, [] },
            // No data for 2020
            { 2019, [] }
        });
        var sut = new YearsModel(data);

        sut.OnGet(2019);

        Assert.Equal((ushort)2021, sut.NextYear);
    }
}
