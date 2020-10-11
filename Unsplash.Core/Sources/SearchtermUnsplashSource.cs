namespace Unsplash.Core.Sources
{
    public class SearchtermUnsplashSource : UnsplashSource
    {
        private string Term { get; }

        public SearchtermUnsplashSource(string term)
        {
            Term = term;
        }

        public override string BuildUrlString(Settings settings) =>
            $"https://source.unsplash.com/{settings.GetResolution()}/?{Term}";
    }
}