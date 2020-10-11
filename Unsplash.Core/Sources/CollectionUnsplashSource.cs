namespace Unsplash.Core.Sources
{
    public class CollectionUnsplashSource : UnsplashSource
    {
        private string Collectionid { get; }
        
        private bool IsCurated { get; }

        public CollectionUnsplashSource(string collectionid, bool isCurated)
        {
            Collectionid = collectionid;
            IsCurated = isCurated;
        }

        public override string BuildUrlString(Settings settings)
        {
            return !IsCurated
                ? $"https://source.unsplash.com/collection/{Collectionid}/{settings.ImageWidth}x{settings.ImageHeight}"
                : $"https://source.unsplash.com/collection/curated/{Collectionid}/{settings.ImageWidth}x{settings.ImageHeight}";
        }
    }
}