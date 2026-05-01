namespace Combophoto.Api.Models
{
    public sealed record FaceSwapResult(
        byte[] ImageBytes,
        string ContentType,
        double SimilarityScore);
}
