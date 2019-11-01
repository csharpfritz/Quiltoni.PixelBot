namespace Quiltoni.PixelBot.Relay.Models
{
	//public class Amount_Set
	//{
	//    public Shop_Money shop_money { get; set; }
	//    public Shop_Money presentment_money { get; set; }
	//}

	//public class Shop_Money8
	//{
	//    public string amount { get; set; }
	//    public string currency_code { get; set; }
	//}

	//public class Presentment_Money8
	//{
	//    public string amount { get; set; }
	//    public string currency_code { get; set; }
	//}

	public class Shipping_Lines
	{
		public long id { get; set; }
		public string title { get; set; }
		public string price { get; set; }
		public object code { get; set; }
		public string source { get; set; }
		public object phone { get; set; }
		public object requested_fulfillment_service_id { get; set; }
		public object delivery_category { get; set; }
		public object carrier_identifier { get; set; }
		public string discounted_price { get; set; }
		public Price_Set price_set { get; set; }
		public Price_Set discounted_price_set { get; set; }
		public object[] discount_allocations { get; set; }
		public object[] tax_lines { get; set; }
	}

	//public class Price_Set1
	//{
	//    public Shop_Money shop_money { get; set; }
	//    public Shop_Money presentment_money { get; set; }
	//}

	//public class Shop_Money9
	//{
	//    public string amount { get; set; }
	//    public string currency_code { get; set; }
	//}

	//public class Presentment_Money9
	//{
	//    public string amount { get; set; }
	//    public string currency_code { get; set; }
	//}

	//public class Discounted_Price_Set
	//{
	//    public Shop_Money shop_money { get; set; }
	//    public Shop_Money presentment_money { get; set; }
	//}

	//public class Shop_Money10
	//{
	//    public string amount { get; set; }
	//    public string currency_code { get; set; }
	//}

	//public class Presentment_Money10
	//{
	//    public string amount { get; set; }
	//    public string currency_code { get; set; }
	//}


}