using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEngineTests : SearchEngineTestsBase
    {

        private readonly List<Shirt> _shirts = new List<Shirt>()
        {
            new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
            new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
            new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            new Shirt(Guid.NewGuid(), "Black - Small", Size.Small, Color.Black),
            new Shirt(Guid.NewGuid(), "Black - Large", Size.Large, Color.Black),
            new Shirt(Guid.NewGuid(), "Red - Large", Size.Large, Color.Red),
            new Shirt(Guid.NewGuid(), "Yellow - Small", Size.Small, Color.Yellow),
            new Shirt(Guid.NewGuid(), "Yellow - Small", Size.Small, Color.Yellow),
        };

        [Test]
        [TestCase("Yellow", "Small", 2)]
        [TestCase("Black", "Small", 1)]
        [TestCase("Black", "Medium", 1)]
        [TestCase("White", "Small", 0)]
        public void TestSizeAndColour(string color, string size, int expectedResultCount)
        {
            var searchEngine = new SearchEngine(_shirts);

            RunTest(new SearchOptions
            {
                Colors = new List<Color> { Color.All.First(x => x.Name == color) },
                Sizes = new List<Size> { Size.All.First(x => x.Name == size) }
            }, expectedResultCount);
        }


        [Test]
        [TestCase("Blue", 1)]
        [TestCase("Black", 3)]
        [TestCase("White", 0)]
        [TestCase("Yellow", 2)]
        public void TestColourOnly(string color, int expectedResultCount) => RunTest(new SearchOptions
        {
            Colors = new List<Color> { Color.All.First(x => x.Name == color) },
            Sizes = new List<Size> { }
        }, expectedResultCount);


        [Test]
        [TestCase("Medium", 1)]
        [TestCase("Small", 4)]
        [TestCase("Large", 3)]
        public void TestSizeOnly(string size, int expectedResultCount) => RunTest(new SearchOptions
            {
            Colors = new List<Color> { },
            Sizes = new List<Size> { Size.All.First(x => x.Name == size) }
            }, expectedResultCount);

        private void RunTest(SearchOptions searchOptions, int expectedResultCount)
        {
            var searchEngine = new SearchEngine(_shirts);
            var results = searchEngine.Search(searchOptions);
            Assert.AreEqual(expectedResultCount, results.Shirts.Count);

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(_shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(_shirts, searchOptions, results.ColorCounts);
        }
    }
}
