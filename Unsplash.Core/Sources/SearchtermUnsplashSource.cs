using Colorful;
using Unsplash.Core.Extensions;

namespace Unsplash.Core.Sources
{
    public class SearchtermUnsplashSource : UnsplashSource
    {
        public string Term { get; set; }
        public bool IsFeatured { get; set; }

        public SearchtermUnsplashSource(bool ask = false)
        {
            if (!ask) return;

            Term = Questions.AskQuestion("Specify the search terms (Can be separated by a comma): ");
            IsFeatured = Questions.AskBoolQuestion("Search for featured images: ");
        }

        public override string BuildUrlString(Settings settings) =>
            $"https://source.unsplash.com/{settings.Resolution}/?{Term}";
    }
}