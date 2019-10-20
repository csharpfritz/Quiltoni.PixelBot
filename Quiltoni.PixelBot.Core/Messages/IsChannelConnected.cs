namespace Quiltoni.PixelBot.Core.Messages {

    public class IsChannelConnected {

        public IsChannelConnected(string channelName)
        {
            this.ChannelName = channelName;            
        }

        public string ChannelName { get; }

    }

    public class IsChannelConnectedResponse {

        public IsChannelConnectedResponse(bool isConnected)
        {
            IsConnected = isConnected;
        }

        public bool IsConnected { get; }
    }

}