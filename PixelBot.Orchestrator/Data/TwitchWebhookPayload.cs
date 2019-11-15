using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PixelBot.Orchestrator.Data
{
	public partial class TwitchWebhookPayload
	{
		[JsonProperty("data")]
		public Datum[] Data { get; set; }
	}

	public partial class Datum
	{
		[JsonProperty("from_id")]
		public long FromId { get; set; }

		[JsonProperty("from_name")]
		public string FromName { get; set; }

		[JsonProperty("to_id")]
		public long ToId { get; set; }

		[JsonProperty("to_name")]
		public string ToName { get; set; }

		[JsonProperty("followed_at")]
		public DateTimeOffset FollowedAt { get; set; }
	}

}