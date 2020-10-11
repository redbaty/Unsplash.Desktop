namespace Unsplash.Core.Sources
{
    public class CategoryUnsplashSource : UnsplashSource
    {
        private string Category { get; }

        public CategoryUnsplashSource(string category)
        {
            Category = category;
        }

        public override string BuildUrlString(Settings settings)
        {
            return $"https://source.unsplash.com/category/{Category}/{settings.GetResolution()}";
        }
    }
}