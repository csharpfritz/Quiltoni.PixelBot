using System;
using System.Collections.Generic;
using System.Text;

namespace Quiltoni.PixelBot.Core.Domain
{
	public class ChatCommand
	{

		public IList<string> ArgumentsAsList { get; set; }

		public bool IsBroadcaster { get; set; }

		public bool IsModerator { get; set; }

		public string Username { get; set; }

		public string DisplayName { get; set; }

	}
}