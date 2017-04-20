namespace Unsplash.Core.Sources
{
    public class RandomUnsplashSource : UnsplashSource
    {
        public override string BuildUrlString(Settings settings)
        {
            return $"https://source.unsplash.com/random/{settings.Resolution}";
        }
    }
}