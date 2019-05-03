using System;
using System.Linq;
using Akka.Actor;
using Quiltoni.PixelBot.Core.Data;
using Quiltoni.PixelBot.Core.Domain;
using TwitchLib.Client.Events;

namespace PixelBot.Orchestrator.Actors.ChannelEvents
{
	public class RaidActor : ReceiveActor
	{

		public RaidActor(ChannelConfiguration config, ICurrencyRepository currencyRepository) {

			this.Config = config;
			this.CurrencyRepository = currencyRepository;

			this.Receive<OnRaidNotificationArgs>(Raid);

		}
		
		public ChannelConfiguration Config { get; }

		public ICurrencyRepository CurrencyRepository { get; }

		private void Raid(OnRaidNotificationArgs args) {

			if (Config.Currency.Enabled) HandleCurrency(args);

		}

		private void HandleCurrency(OnRaidNotificationArgs args) {

			// Exit if we do not meet the minimum of 3 viewers
			if (int.Parse(args.RaidNotificaiton.MsgParamViewerCount) < Config.Currency.AwardForRaid_Min) return;

			var currency = new int[] { Config.Currency.AwardForRaid_Min, int.Parse(args.RaidNotificaiton.MsgParamViewerCount) }.Max();
			currency = currency > Config.Currency.AwardForRaid_Max ? Config.Currency.AwardForRaid_Max : currency;

			CurrencyRepository.AddForUser(args.RaidNotificaiton.Login, currency, "Bot-Raid");

		}
	}

}
