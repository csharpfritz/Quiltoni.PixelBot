using System;

namespace Quiltoni.PixelBot.Relay.Models
{
    public class Order
    {
        public long id { get; set; }
        public string email { get; set; }
        public object closed_at { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int number { get; set; }
        public object note { get; set; }
        public string token { get; set; }
        public object gateway { get; set; }
        public bool test { get; set; }
        public string total_price { get; set; }
        public string subtotal_price { get; set; }
        public int total_weight { get; set; }
        public string total_tax { get; set; }
        public bool taxes_included { get; set; }
        public string currency { get; set; }
        public string financial_status { get; set; }
        public bool confirmed { get; set; }
        public string total_discounts { get; set; }
        public string total_line_items_price { get; set; }
        public object cart_token { get; set; }
        public bool buyer_accepts_marketing { get; set; }
        public string name { get; set; }
        public object referring_site { get; set; }
        public object landing_site { get; set; }
        public DateTime? cancelled_at { get; set; }
        public string cancel_reason { get; set; }
        public object total_price_usd { get; set; }
        public object checkout_token { get; set; }
        public object reference { get; set; }
        public object user_id { get; set; }
        public object location_id { get; set; }
        public object source_identifier { get; set; }
        public object source_url { get; set; }
        public object processed_at { get; set; }
        public object device_id { get; set; }
        public object phone { get; set; }
        public string customer_locale { get; set; }
        public object app_id { get; set; }
        public object browser_ip { get; set; }
        public object landing_site_ref { get; set; }
        public int order_number { get; set; }
        public Discount_Applications[] discount_applications { get; set; }
        public object[] discount_codes { get; set; }
        public object[] note_attributes { get; set; }
        public string[] payment_gateway_names { get; set; }
        public string processing_method { get; set; }
        public object checkout_id { get; set; }
        public string source_name { get; set; }
        public string fulfillment_status { get; set; }
        public object[] tax_lines { get; set; }
        public string tags { get; set; }
        public string contact_email { get; set; }
        public string order_status_url { get; set; }
        public string presentment_currency { get; set; }
        public Price_Set total_line_items_price_set { get; set; }
        public Price_Set total_discounts_set { get; set; }
        public Price_Set total_shipping_price_set { get; set; }
        public Price_Set subtotal_price_set { get; set; }
        public Price_Set total_price_set { get; set; }
        public Price_Set total_tax_set { get; set; }
        public Line_Items[] line_items { get; set; }
        public Shipping_Lines[] shipping_lines { get; set; }
        public Address billing_address { get; set; }
        public Address shipping_address { get; set; }
        public object[] fulfillments { get; set; }
        public object[] refunds { get; set; }
        public Customer customer { get; set; }
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
