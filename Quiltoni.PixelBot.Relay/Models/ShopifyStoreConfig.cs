using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quiltoni.PixelBot.Relay.Models
{
	public class StoreConfig {

		public List<ShopifyStore> Shopify { get; set; }

	}

	public class ShopifyStore
	{

		public string Name { get; set; }

		public string Key { get; set; }

	}
}
