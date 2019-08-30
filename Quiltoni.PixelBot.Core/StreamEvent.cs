using System;

namespace Quiltoni.PixelBot.Core
{

	[Flags]
	public enum StreamEvent
	{

		None							= 0,
		OnCommand					= 1,
		OnGiftSubscribe		= 2,
		OnMessage					= 4,
		OnRaid						= 8,
		OnResubscribe			= 16,
		OnSubscribe				= 32,
		OnFollow					= 64,
		All								= 1023

	}

}
