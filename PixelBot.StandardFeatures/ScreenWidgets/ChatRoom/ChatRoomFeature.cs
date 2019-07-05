using System;
using Quiltoni.PixelBot.Core;
using Quiltoni.PixelBot.Core.Extensibility;

namespace PixelBot.StandardFeatures.ScreenWidgets.ChatRoom
{

	[ActivatingEvents(StreamEvent.OnMessage)]
	public class ChatRoomFeature : BaseFeature
	{

		public override string Name => "ChatRoom Screen Widget";

	}

}
