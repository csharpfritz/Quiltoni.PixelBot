namespace PixelBot.Orchestrator.Services
{

    public interface IFollowerDedupeService
    {

        bool CheckNewFollower(string channelFollowed, string newFollowerName);

    }

}