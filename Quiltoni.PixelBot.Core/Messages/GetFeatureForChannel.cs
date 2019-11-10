using System;
using System.Collections.Generic;
using System.Text;
using Quiltoni.PixelBot.Core.Extensibility;

namespace Quiltoni.PixelBot.Core.Messages
{

	[Serializable]
	public class GetFeatureForChannel
	{

		public GetFeatureForChannel(string channelName, Type featureType)
		{

			this.Channel = channelName;
			this.FeatureType = featureType;

		}


		public string Channel { get; }
		public Type FeatureType { get; }

	}


}