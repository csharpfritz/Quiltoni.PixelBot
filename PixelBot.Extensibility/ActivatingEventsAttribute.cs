using System;
using System.Collections.Generic;
using System.Text;

namespace PixelBot.Extensibility
{

	public class ActivatingEventsAttribute : Attribute
	{
		public ActivatingEventsAttribute(StreamEvent events) {

			this.EventsListeningTo = events;

		}

		public StreamEvent EventsListeningTo { get; set; }

	}

}
