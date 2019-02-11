namespace Quiltoni.PixelBot.Relay.Models
{
    //public class Shipping_Address
    //{
    //    public string first_name { get; set; }
    //    public string address1 { get; set; }
    //    public string phone { get; set; }
    //    public string city { get; set; }
    //    public string zip { get; set; }
    //    public string province { get; set; }
    //    public string country { get; set; }
    //    public string last_name { get; set; }
    //    public object address2 { get; set; }
    //    public string company { get; set; }
    //    public object latitude { get; set; }
    //    public object longitude { get; set; }
    //    public string name { get; set; }
    //    public string country_code { get; set; }
    //    public string province_code { get; set; }
    //}

    public class Customer
    {
        public long id { get; set; }
        public string email { get; set; }
        public bool accepts_marketing { get; set; }
        public object created_at { get; set; }
        public object updated_at { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int orders_count { get; set; }
        public string state { get; set; }
        public string total_spent { get; set; }
        public object last_order_id { get; set; }
        public object note { get; set; }
        public bool verified_email { get; set; }
        public object multipass_identifier { get; set; }
        public bool tax_exempt { get; set; }
        public object phone { get; set; }
        public string tags { get; set; }
        public object last_order_name { get; set; }
        public string currency { get; set; }
        public Address default_address { get; set; }
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
