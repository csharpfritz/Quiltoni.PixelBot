using System;
using PixelBot.Extensibility;

namespace PixelBot.StandardFeatures.ScreenWidgets.ChatRoom
{

	[ActivatingEvents(StreamEvent.OnMessage)]
	public class ChatRoomFeature : BaseFeature
	{

		public override string Name => "ChatRoom Screen Widget";

	}

}
