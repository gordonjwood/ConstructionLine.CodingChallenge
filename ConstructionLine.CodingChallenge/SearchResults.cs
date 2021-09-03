using System.Collections.Generic;

namespace ConstructionLine.CodingChallenge
{
    public class SearchResults
    {
        public List<Shirt> Shirts { get; set; }


        public List<SizeCount> SizeCounts { get; set; }


        public List<ColorCount> ColorCounts { get; set; }
    }


    [System.Diagnostics.DebuggerDisplay("{Size.Name} = {Count}")]
    public class SizeCount
    {
        public Size Size { get; set; }

        public int Count { get; set; }
    }

    [System.Diagnostics.DebuggerDisplay("{Color.Name} = {Count}")]
    public class ColorCount
    {
        public Color Color { get; set; }

        public int Count { get; set; }
    }
}