using System;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public class GetFeatureFromChannel
	{

		public GetFeatureFromChannel(Type featureType) {
			this.FeatureType = featureType;
		}

		public Type FeatureType { get; }
	}


}
