using System;
using System.Net.Http;
using Akka.TestKit.Xunit2;
using Moq;
using PixelBot.Orchestrator.Data;
using Quiltoni.PixelBot.Core.Domain;

namespace PixelBot.StandardFeaturesTests.UserActivityTrain
{
	public abstract class BaseTestKit : TestKit
	{
		private readonly Mock<IChannelConfigurationContext> _ConfigurationContext;
		public MockRepository _Mockery = new MockRepository(MockBehavior.Default);

		protected BaseTestKit() {

			_ConfigurationContext = _Mockery.Create<IChannelConfigurationContext>();

		}

		protected void AddChannelConfiguration(string channelName, ChannelConfiguration config) {

			_ConfigurationContext.Setup(c => c.GetConfigurationForChannel(channelName)).Returns(config);

		}

		protected IChannelConfigurationContext ConfigurationContext { get { return _ConfigurationContext.Object; } }

	}

}
