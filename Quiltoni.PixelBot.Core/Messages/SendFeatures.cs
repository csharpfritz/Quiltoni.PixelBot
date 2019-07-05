using System;
using Quiltoni.PixelBot.Core.Extensibility;

namespace Quiltoni.PixelBot.Core.Messages
{
	[Serializable]
	public class SendFeatures
	{

		public SendFeatures(IFeature[] features) {
			this.Features = features;
		}

		public IFeature[] Features { get; }

	}

}
