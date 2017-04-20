using Unsplash.Core.Extensions;

namespace Unsplash.Core.Sources
{
    public class CollectionUnsplashSource : UnsplashSource
    {
        public string Collectionid { get; set; }
        public bool IsCurated { get; set; }

        public CollectionUnsplashSource(bool b = false)
        {
            if(b)
            Collectionid = Questions.AskQuestion("Please enter a collection ID: ");
        }

        public override string BuildUrlString(Settings settings)
        {
            return !IsCurated
                ? $"https://source.unsplash.com/collection/{Collectionid}/{settings.ImageWidth}x{settings.ImageHeight}"
                : $"https://source.unsplash.com/collection/curated/{Collectionid}/{settings.ImageWidth}x{settings.ImageHeight}";
        }
    }
}