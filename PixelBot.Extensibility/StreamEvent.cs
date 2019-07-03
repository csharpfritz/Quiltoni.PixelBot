using System;

namespace PixelBot.Extensibility
{

	[Flags]
	public enum StreamEvent
	{

		OnCommand					= 1,
		OnGiftSubscribe		= 2,
		OnMessage					= 4,
		OnRaid						= 8,
		OnResubscribe			= 16,
		OnSubscribe				= 32,

	}

}
