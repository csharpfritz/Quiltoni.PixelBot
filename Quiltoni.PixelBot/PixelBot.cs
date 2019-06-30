using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client.Enums;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource;
using System.Reflection;
using System.Diagnostics;

namespace Quiltoni.PixelBot
{
	public class PixelBot : IHostedService, IChatService
	{

		private TwitchClient _Client;
		static string ApplicationName = "PixelBot";
		private readonly ISheetProxy _GoogleSheet;

		public PixelBot(IEnumerable<IBotCommand> commands, IOptions<PixelBotConfig> configuration, ILoggerFactory loggerFactory) :
			this(commands, configuration, loggerFactory, null) { }

		public PixelBot(IEnumerable<IBotCommand> commands, IOptions<PixelBotConfig> configuration, ILoggerFactory loggerFactory, ISheetProxy sheetProxy) {

			Config = configuration.Value;
			Commands = commands.Where(c => c.Enabled);
			this.Logger = loggerFactory.CreateLogger("PixelBot");
			var sheetType = GetType().Assembly.DefinedTypes.First(t => t.Name == Config.Currency.SheetType);
			_GoogleSheet = sheetProxy ?? Activator.CreateInstance(sheetType, configuration, loggerFactory) as ISheetProxy;
			_GoogleSheet.Twitch = this;

		}

		public static PixelBotConfig Config { get; set; }
		public IEnumerable<IBotCommand> Commands { get; }
		public ILogger Logger { get; }
		private bool EnableSubPixels { get { return Config.Currency.Enabled; } }


		public string Channel { get { return Config.Twitch.Channel; } }

		public string Spreadsheet { get { return Config.Google.SheetId; } }

		public Task StartAsync(CancellationToken cancellationToken)
		{

			var creds = new ConnectionCredentials(Config.Twitch.UserName, Config.Twitch.AccessToken);
			_Client = new TwitchClient();
			_Client.Initialize(creds);

			_Client.OnConnected += (sender, args) => _Client.JoinChannel(Channel);
			_Client.OnNewSubscriber += _Client_OnNewSubscriber;
			_Client.OnReSubscriber += _Client_OnReSubscriber;
			_Client.OnGiftedSubscription += _Client_OnGiftedSubscription;
			_Client.OnRaidNotification += _Client_OnRaidNotification;
			_Client.OnChatCommandReceived += _Client_OnChatCommandReceived;
			_Client.OnMessageReceived += _Client_OnMessageReceived;

			_Client.Connect();

			return Task.CompletedTask;

		}

		private void _Client_OnMessageReceived(object sender, OnMessageReceivedArgs e) {

			Debug.WriteLine($"User entering text: {e.ChatMessage.Username}");

			Commands.Where(c => c is IBotListensToMesages)
				.ToList().ForEach(c => ((IBotListensToMesages)c).MessageReceived(this, e.ChatMessage.Username));

		}

		private void _Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs e) 
		{

			var cmd = Commands.FirstOrDefault(c => c.CommandText == e.Command.CommandText);

			// Exit if I don't know how to handle this command
			if (cmd == null) return;

			if (cmd is IRequiresSheet) ((IRequiresSheet)cmd).GoogleSheet = _GoogleSheet;

			cmd.Execute(e.Command, this);

		}

		private void _Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
		{

			if (!EnableSubPixels) return;

			// Exit if we do not meet the minimum of 3 viewers
			if (int.Parse(e.RaidNotificaiton.MsgParamViewerCount) < 3) return;

			var pixels = new int[] { 3, int.Parse(e.RaidNotificaiton.MsgParamViewerCount) }.Max();
			pixels = pixels > 200 ? 200 : pixels;

			_GoogleSheet.AddPixelsForUser(e.RaidNotificaiton.DisplayName, pixels, "PixelBot-Raid");

		}

		private static readonly Dictionary<SubscriptionPlan, int> _PixelRewards = new Dictionary<SubscriptionPlan, int> {
			{ SubscriptionPlan.Prime, 10 },
			{ SubscriptionPlan.Tier1, 10 },
			{ SubscriptionPlan.Tier2, 25 },
			{ SubscriptionPlan.Tier3, 60 },
		};

		private void _Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
		{

			if (!EnableSubPixels) return;

			_GoogleSheet.AddPixelsForUser(e.ReSubscriber.DisplayName, _PixelRewards[e.ReSubscriber.SubscriptionPlan], "PixelBot-Resub");

		}

		public void _Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
		{

			if (!EnableSubPixels) return;

			_GoogleSheet.AddPixelsForUser(e.Subscriber.DisplayName, _PixelRewards[e.Subscriber.SubscriptionPlan], "PixelBot-Sub");

		}

		private void _Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
		{

			if (!EnableSubPixels) return;

			_GoogleSheet.AddPixelsForUser(e.GiftedSubscription.DisplayName, 2, "PixelBot-SubGifter");
			_GoogleSheet.AddPixelsForUser(e.GiftedSubscription.MsgParamRecipientDisplayName, _PixelRewards[e.GiftedSubscription.MsgParamSubPlan], "PixelBot-SubGift");

		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			if (_Client != null) _Client.Disconnect();
			return Task.CompletedTask;
		}

		public void BroadcastMessageOnChannel(string message) {
			_Client.SendMessage(Channel, message);
		}

		public void WhisperMessage(string userDisplayName, string message) {
			_Client.SendWhisper(userDisplayName, message);
		}

	}

}
