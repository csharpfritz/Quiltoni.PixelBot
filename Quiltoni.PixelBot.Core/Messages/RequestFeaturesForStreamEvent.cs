using System;
using System.Collections.Generic;
using System.Text;

namespace Quiltoni.PixelBot.Core.Messages
{

	[Serializable]
	public class RequestFeaturesForStreamEvent
	{

		public RequestFeaturesForStreamEvent(StreamEvent evt) {
			StreamEvent = evt;
		}

		public StreamEvent StreamEvent { get; }

	}

}
