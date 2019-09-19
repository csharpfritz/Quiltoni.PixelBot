using Akka.Actor;
using PixelBot.StandardFeatures.ScreenWidgets.UserActivityTrain;
using Quiltoni.PixelBot.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using CORE = PixelBot.Orchestrator.Actors;
using MSG = Quiltoni.PixelBot.Core.Messages;

namespace PixelBot.StandardFeaturesTests.UserActivityTrain
{

	public class WhenActivating : BaseTestKit
	{

		[Fact]
		public void ShouldGetFeatureConfiguration() {

			// arrange
			const string CHANNELNAME = "TestChannel";
			var config = new ChannelConfiguration();
			config.FeatureConfigurations.Add("UserActivityConfiguration", new UserActivityConfiguration
			{
				ChannelName = CHANNELNAME,
				IsEnabled = true,
				Type = UserActivityConfiguration.UserActivityTrainType.Follow
			});
			base.AddChannelConfiguration(CHANNELNAME, config);

			var configActor = this.Sys.ActorOf(Props.Create<CORE.ChannelConfigurationActor>(base.ConfigurationContext), "TestConfig");
			BotConfiguration.ChannelConfigurationInstancePath = configActor.Path.ToStringWithAddress();

			// act
			var sut = new UserActivityTrainModel();
			sut.ChannelName = CHANNELNAME;
			sut.ActorSystem = this.Sys;
			sut.GetWidgetConfiguration();

			// assert
			Assert.NotNull(sut.Configuration);


		}


	}

}
