using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PixelBot.Orchestrator.Actors.Commands
{
	public interface IBotCommandActor
	{

		string CommandText { get; }

	}
}