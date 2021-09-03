using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly Dictionary<Guid, Shirt[]> _shirtsBySize;
        private readonly Dictionary<Guid, Shirt[]> _shirtsByColour;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirtsBySize = shirts.GroupBy(x => x.Size.Id).ToDictionary(x => x.Key, x => x.ToArray());
            _shirtsByColour = shirts.GroupBy(x => x.Color.Id).ToDictionary(x => x.Key, x => x.ToArray());
        }


        public SearchResults Search(SearchOptions options)
        {
            var searchByColour = SearchByColourTask(options.Colors.Select(x => x.Id).ToArray());

            var searchBySize = SearchBySizeTask(options.Sizes.Select(x => x.Id).ToArray());

            RunInParallel(searchByColour, searchBySize);
            
            var intersectionResults = searchByColour.Result.Intersect(searchBySize.Result).ToArray();

            var summaryColour = BuildColourSummary(intersectionResults);
            var summarySizes = BuildSizesSummary(intersectionResults);
            
            RunInParallel(summaryColour, summarySizes);
            
            return new SearchResults
            {
                Shirts = intersectionResults.ToList(),
                ColorCounts = summaryColour.Result,
                SizeCounts = summarySizes.Result
            };
        }

        private void RunInParallel(params Task[] tasks)
        {
            Task.WhenAll(tasks);
            var failedTask = tasks.FirstOrDefault(x => x.IsFaulted);
            if (failedTask != null)
            {
                throw new Exception("One or more search tasks failed!", failedTask.Exception);
            }
        }
        
        private Task<Shirt[]> SearchByColourTask(Guid[] colours)
        {
            var colourFilter = colours.Any() ? colours : Color.All.Select(x => x.Id).ToArray();
            var results = new List<Shirt>();

            foreach (var item in colourFilter)
            {
                if (_shirtsByColour.TryGetValue(item, out var shirts))
                {
                    results.AddRange(shirts);
                }
            }

            return Task.FromResult(results.ToArray());
        }
        
        private Task<Shirt[]> SearchBySizeTask(Guid[] sizes)
        {
            var sizesFilter = sizes.Any() ? sizes : Size.All.Select(x => x.Id).ToArray();
            var results = new List<Shirt>();
            
            foreach (var item in sizesFilter)
            {
                if (_shirtsBySize.TryGetValue(item, out var shirts))
                {
                    results.AddRange(shirts);
                }
            }
            return Task.FromResult(results.ToArray());
        }        
        
        private Task<List<ColorCount>> BuildColourSummary(Shirt[] results)
        {
            var coloursSummary = results
                .GroupBy(x => x.Color)
                .Select(c => new ColorCount()
                {
                    Color = c.Key,
                    Count = c.Count()
                }).ToList();
            foreach (var colour in Color.All.Except(coloursSummary.Select(x => x.Color)))
            {
                coloursSummary.Add(new ColorCount() { Color = colour, Count = 0 });
            }
            return Task.FromResult(coloursSummary);
        } 
        
        
        private Task<List<SizeCount>> BuildSizesSummary(Shirt[] results)
        {
            var sizesSummary = results
                .GroupBy(x => x.Size)
                .Select(c => new SizeCount()
                {
                    Size = c.Key,
                    Count = c.Count()
                }).ToList();
            foreach (var size in Size.All.Except(sizesSummary.Select(x => x.Size)))
            {
                sizesSummary.Add(new SizeCount() { Size = size, Count = 0 });
            }
            return Task.FromResult(sizesSummary);
        }        
    }
}