using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quiltoni.PixelBot.Relay.Models
{

	public class Line_Items
	{
		public long id { get; set; }
		public object variant_id { get; set; }
		public string title { get; set; }
		public int quantity { get; set; }
		public string price { get; set; }
		public string sku { get; set; }
		public object variant_title { get; set; }
		public object vendor { get; set; }
		public string fulfillment_service { get; set; }
		public long product_id { get; set; }
		public bool requires_shipping { get; set; }
		public bool taxable { get; set; }
		public bool gift_card { get; set; }
		public string name { get; set; }
		public object variant_inventory_management { get; set; }
		public object[] properties { get; set; }
		public bool product_exists { get; set; }
		public int fulfillable_quantity { get; set; }
		public int grams { get; set; }
		public string total_discount { get; set; }
		public object fulfillment_status { get; set; }
		public Price_Set price_set { get; set; }
		public Price_Set total_discount_set { get; set; }
		public Discount_Allocations[] discount_allocations { get; set; }
		public object[] tax_lines { get; set; }
	}

}