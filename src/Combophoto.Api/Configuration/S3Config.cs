namespace Combophoto.Api.Configuration
{
    public class S3Config
    {
        public const string SectionName = "S3Config";

        public string Endpoint { get; init; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
    }
}
