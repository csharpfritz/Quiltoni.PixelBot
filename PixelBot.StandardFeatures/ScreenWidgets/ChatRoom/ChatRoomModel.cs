using System;
using System.Linq;
using System.Runtime.CompilerServices;
using PixelBot.StandardFeatures.ScreenWidgets.ChatRoom;
using PixelBot.UI;
using Quiltoni.PixelBot.Core;

namespace PixelBot.StandardFeatures.ScreenWidgets.ChatRoom
{
	public class ChatRoomModel : BaseWidgetModel<ChatRoomFeature>
	{

		public override StreamEvent TriggerEvent => StreamEvent.OnMessage;

	}

}