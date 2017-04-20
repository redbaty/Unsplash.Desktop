namespace Unsplash.Core
{
    public abstract class UnsplashSource
    {
        public abstract string BuildUrlString(Settings settings);
    }
}