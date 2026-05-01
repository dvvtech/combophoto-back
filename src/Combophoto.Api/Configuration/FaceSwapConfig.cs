namespace Combophoto.Api.Configuration
{
    public class FaceSwapConfig
    {
        public const string SectionName = "FaceSwapConfig";

        public string BaseUrl { get; init; } = "http://faceswap-api:8080/";
        public int TimeoutSeconds { get; init; } = 120;
    }
}
