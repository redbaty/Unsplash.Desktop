namespace Unsplash.Core.Sources
{
    public enum Categories
    {
        Buildings,
        Food,
        Nature,
        People,
        Technology,
        Objects
    }

    public class CategoryUnsplashSource : UnsplashSource
    {
        public string Category { get; }

        public CategoryUnsplashSource(bool ask = false)
        {
            if (!ask) return;

            Category = Unsplash.ShowEnumMenu<Categories>().ToString();
        }

        public override string BuildUrlString(Settings settings)
        {
            return $"https://source.unsplash.com/category/{Category}/{settings.Resolution}";
        }
    }
}