namespace CandidateTesting.RodineiRiboli.Core.Interfaces
{
    public interface IConsumeAwsS3
    {
        Task<string> GetLogMinhaCdn(string uri);
    }
}
