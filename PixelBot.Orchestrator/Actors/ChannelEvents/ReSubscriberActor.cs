using System;
using Akka.Actor;
using Quiltoni.PixelBot.Core.Data;
using Quiltoni.PixelBot.Core.Domain;
using TwitchLib.Client.Events;

namespace PixelBot.Orchestrator.Actors.ChannelEvents
{
	public class ReSubscriberActor : ReceiveActor
	{

		public ChannelConfiguration Config { get; }
		public ICurrencyRepository CurrencyRepository { get; }

		public ReSubscriberActor(ChannelConfiguration config, ICurrencyRepository currencyRepository) {

			this.Config = config;
			this.CurrencyRepository = currencyRepository;

			this.Receive<OnReSubscriberArgs>(ReSubscriber);

		}

		private void ReSubscriber(OnReSubscriberArgs args) {
			
			if (Config.Currency.Enabled) HandleCurrency(args);
			
		}
		
		private void HandleCurrency(OnReSubscriberArgs args) {

			var currencyToAward = 0;
			switch (args.ReSubscriber.SubscriptionPlan) {
				case TwitchLib.Client.Enums.SubscriptionPlan.Prime:
					currencyToAward = Config.Currency.AwardForSub_Prime;
					break;
				case TwitchLib.Client.Enums.SubscriptionPlan.Tier1:
					currencyToAward = Config.Currency.AwardForSub_Tier1;
					break;
				case TwitchLib.Client.Enums.SubscriptionPlan.Tier2:
					currencyToAward = Config.Currency.AwardForSub_Tier2;
					break;
				case TwitchLib.Client.Enums.SubscriptionPlan.Tier3:
					currencyToAward = Config.Currency.AwardForSub_Tier3;
					break;
			}

			CurrencyRepository.AddForUser(args.ReSubscriber.Login, currencyToAward, "The Bot");
		
		}
	}

}
