using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Quiltoni.PixelBot.Relay.Controllers;
using Quiltoni.PixelBot.Relay.Models;
using Xunit;

namespace Quiltoni.Test.Shopify
{

	public class GivenNewOrderNotification : BaseFixture
	{
		private readonly MockRepository _Mockery;

		public GivenNewOrderNotification() {

			_Mockery = new MockRepository(MockBehavior.Loose);

		}

		[Fact]
		public void WhenPassedNewOrder_ShouldBindProperly() {

			// Arrange
			var theJson = base.GetJsonFromResource("SampleOrder.json");
			var jr = new JsonTextReader(new StringReader(theJson));
			var theOrder = JsonSerializer.Create().Deserialize<PixelBot.Relay.Models.Order>(jr);

			var config = new StoreConfig {
				Shopify = new List<ShopifyStore> {
					new ShopifyStore { Key="testkey_notereal", Name="test shop" }
				}
			};
			var options = _Mockery.Create<IOptions<StoreConfig>>();
			options.SetupGet(o => o.Value).Returns(config);

			// Act
			var sut = new ShopifyController(options.Object, null, null);
			var result = sut.Post(theOrder);


		}
		

	}

}
