using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PixelBot.Workers
{
	interface IOrchestratorProxy
	{

		Task ConnectAsync();

		Task DisconnectAsync();

		Action<string> ConnectToRoom { get; set; }

	}

}