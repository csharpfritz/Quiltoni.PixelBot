using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Quiltoni.PixelBot.Core.Data;
using Quiltoni.PixelBot.Core.Domain;
using Quiltoni.PixelBot.Core.Extensibility;
using TwitchLib.Client.Events;

namespace PixelBot.Orchestrator.Actors.ChannelEvents
{
	public class NewSubscriberActor : ReceiveActor
	{

		public NewSubscriberActor(ChannelConfiguration config, IEnumerable<IFeature> features, ICurrencyRepository currencyRepository)
		{

			this.Config = config;
			this.CurrencyRepository = currencyRepository;

			this.Receive<OnNewSubscriberArgs>(NewSubscriber);

		}

		public ChannelConfiguration Config { get; }
		public ICurrencyRepository CurrencyRepository { get; }

		private void NewSubscriber(OnNewSubscriberArgs args)
		{

			if (Config.Currency.Enabled) HandleCurrency(args);

		}

		private void HandleCurrency(OnNewSubscriberArgs args)
		{

			var currencyToAward = 0;
			switch (args.Subscriber.SubscriptionPlan)
			{
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

			CurrencyRepository.AddForUser(args.Subscriber.Login, currencyToAward, "The Bot");

		}
	}

}