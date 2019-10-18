using System.Collections.Generic;

namespace PixelBot.Orchestrator.Services
{

    public class InMemoryFollowerDedupeService : IFollowerDedupeService
    {

        private static readonly Dictionary<string, HashSet<string>> _PreviousFollowers = new Dictionary<string, HashSet<string>>();

        public bool CheckNewFollower(string channelFollowed, string newFollowerName)
        {
            
            if (!_PreviousFollowers.ContainsKey(channelFollowed)) {
                _PreviousFollowers.Add(channelFollowed, new HashSet<string>() {newFollowerName});
                return true;
            }
            
            var outValue = _PreviousFollowers[channelFollowed].Contains(newFollowerName);
            return _PreviousFollowers[channelFollowed].Add(newFollowerName);

        }
    }

}