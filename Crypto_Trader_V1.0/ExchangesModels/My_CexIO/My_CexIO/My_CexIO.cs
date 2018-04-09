using System;
using System.Collections.Generic;
using System.Web;
using Crypto_Trader_V1.Models;


namespace Crypto_Trader_V1.ExchangesModels.My_CexIO
{

    /// <summary>
    ///     Class representing the Cex.IO API Wrapper
    /// </summary>
    public class My_CexIO : My_ExchangeAPI
    {
        System.Net.Http.HttpClient httpClient;
        System.Net.Http.HttpRequestMessage httpRequest;
        System.Net.Http.HttpResponseMessage httpResponse;
        System.Security.Cryptography.HMACSHA256 hasher;
        private string username;
        private string apiKey;
        private string apiSecret;
        string api;
        Dictionary<string, string> bodyRequest;
        Dictionary<dynamic, dynamic> responseRequest;
        string hexHash;
        string endpoint;
        long nonce;


        /// <summary>
        ///     Constructor
        /// </summary>
        public My_CexIO()
        {
            this.api = "https://cex.io/api/";
            this.hexHash = "";
            this.endpoint = "";
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds();

            this.httpClient = new System.Net.Http.HttpClient();
            this.httpRequest = new System.Net.Http.HttpRequestMessage();
            this.httpResponse = new System.Net.Http.HttpResponseMessage();

            this.bodyRequest = new Dictionary<string, string>();

            this.username = "<your_username>";
            this.apiKey = "<your_api_key>";
            this.apiSecret = "<your_api_secret>";

            this.hasher = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(this.apiSecret));
        }


        /// <summary>
        ///     Function returning the current market price for one <paramref name="currency"/>
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <returns>A <see cref="Price"/> object representing the current market price</returns>
        override public Price GetPrice(string currency)
        {
            Price cryptoPrice = new Price();
            string exchange;
            double price = 0.00;
            string jsonResponse;

            try
            {
                this.endpoint = "last_price/" + currency.ToUpper() + "/" + MainCurrency.USD.ToString();
                this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, this.api + this.endpoint);
                this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;
                jsonResponse = this.httpResponse.Content.ReadAsStringAsync().Result;
                responseRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(jsonResponse);
                exchange = Exchanges.CEXIO.ToString();
                price = Double.Parse(responseRequest["lprice"], System.Globalization.CultureInfo.InvariantCulture);
                cryptoPrice = new Price(exchange, currency, MainCurrency.USD.ToString(), price);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                exchange = Exchanges.CEXIO.ToString();
                cryptoPrice = new Price(error: true, exchange: Exchanges.CEXIO.ToString());
            }

            return cryptoPrice;
        }


        /// <summary>
        ///     Function that send an order to <paramref name="side"/>.
        ///     The order is for <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to cex.IO market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new order that has been send to CexIO market
        /// </returns>
        override public Order NewOrder(string currency, double amount, double price, string side)
        {
            OrderType oneOrderType = (OrderType)Enum.Parse(typeof(OrderType), side.ToUpper());
            OrderStyle oneOrderStyle = OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.CEXIO;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = MainCryptos.USDT;
            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, price);;

            string queryString = "place_order/" + currency + "/" + MainCurrency.USD.ToString() + "/";
            string queryJson;
            Dictionary<string, string> ThreadAlias = new Dictionary<string, string>();

            this.endpoint = "place_order/" + currency + "/" + MainCurrency.USD.ToString() + "/";
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds();

            this.hexHash = this.getSignature(this.nonce);

            this.bodyRequest.Clear();

            this.bodyRequest.Add("nonce", this.nonce.ToString());
            this.bodyRequest.Add("key", this.apiKey);
            this.bodyRequest.Add("signature", this.hexHash);
            this.bodyRequest.Add("type", oneNewOrder.side.ToString().ToLower());
            this.bodyRequest.Add("amount", oneNewOrder.amount.ToString().ToLower());
            this.bodyRequest.Add("price", oneNewOrder.orderPrice.price.ToString().ToLower());
            queryJson = Newtonsoft.Json.JsonConvert.SerializeObject(this.bodyRequest);

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, this.api + this.endpoint);
            this.httpRequest.Content = new System.Net.Http.StringContent(queryJson, System.Text.Encoding.UTF8, "application/json");

            this.httpRequest.Content.Headers.Add("key", this.apiKey);
            this.httpRequest.Content.Headers.Add("signature", this.hexHash);
            this.httpRequest.Content.Headers.Add("nonce", this.nonce.ToString());

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;
            var r = this.httpResponse.Content.ReadAsStringAsync().Result;

            var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(r);

            oneNewOrder.success = false;
            oneNewOrder.message = jsonResponse["error"];

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that send an order to buy <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to cex.IO market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new Buy order that has been send to CexIO market
        /// </returns>
        override public Order Buy(string currency, double amount, double price)
        {
            Order oneNewOrder;

            oneNewOrder = this.NewOrder(currency, amount, price, Models.OrderType.BUY.ToString());

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that send an order to sell <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to cex.IO market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new Sell order that has been send to CexIO market
        /// </returns>
        override public Order Sell(string currency, double amount, double price)
        {
            Order oneNewOrder;

            oneNewOrder = this.NewOrder(currency, amount, price, Models.OrderType.SELL.ToString());

            return oneNewOrder;
        }


        /// <summary>
        ///     Function returning the portfolio of the user 
        /// </summary>
        /// <returns>
        ///     A <see cref="Portfolio"/> object representing the portfolio of the user on CexIO
        /// </returns>
        override public Portfolio GetFolio()
        {
            Portfolio myFolio = new Portfolio();
            Balance oneBalance;
            MainCryptos ccyBase;
            MainCryptos ccyPair;
            string queryJson;
            string stringResponse;
            Dictionary<dynamic, dynamic> dicoResults = new Dictionary<dynamic, dynamic>();
            Price BTCPrice = this.GetPrice(MainCryptos.BTC.ToString());
            double BTCSpot = BTCPrice.price;
            string currency;

            this.endpoint = "balance/";
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds();
            this.bodyRequest.Clear();

            this.hexHash = this.getSignature(this.nonce);

            this.bodyRequest.Clear();

            this.bodyRequest.Add("key", this.apiKey);
            this.bodyRequest.Add("signature", this.hexHash);
            this.bodyRequest.Add("nonce", this.nonce.ToString());

            queryJson = Newtonsoft.Json.JsonConvert.SerializeObject(this.bodyRequest);

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, this.api + this.endpoint);

            this.httpRequest.Content = new System.Net.Http.StringContent(queryJson, System.Text.Encoding.UTF8, "application/json");

            this.httpRequest.Content.Headers.Add("key", this.apiKey);
            this.httpRequest.Content.Headers.Add("signature", this.hexHash);
            this.httpRequest.Content.Headers.Add("nonce", this.nonce.ToString());

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;
            stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            this.responseRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(stringResponse);

            foreach (KeyValuePair<dynamic, dynamic> row in this.responseRequest)
            {
                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), row.Key))
                {
                    currency = row.Key.ToUpper();
                    Dictionary<dynamic, dynamic> dicoRow = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(row.Value.ToString());

                    if (Double.Parse(dicoRow["available"], System.Globalization.CultureInfo.InvariantCulture) != 0.00)
                    {
                        ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
                        ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
                        oneBalance = new Balance(ccyBase, ccyPair, Double.Parse(dicoRow["available"], System.Globalization.CultureInfo.InvariantCulture));
                        myFolio.AddBalance(oneBalance);
                    }   
                }
            }

            return myFolio;
        }

        /// <summary>
        ///     Function that return the adress of the <paramref name="currency"/> portoflio
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <returns>
        ///     the adress of the porfolio, with which you can send some crypto currency to it
        /// </returns>
        override public string GetAdress(string currency)
        {
            string queryJson = "";
            string stringResponse;
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds();

            this.endpoint = "get_address/";

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, this.api + this.endpoint);

            this.hexHash = this.getSignature(this.nonce);

            this.bodyRequest.Clear();

            this.bodyRequest.Add("key", this.apiKey);
            this.bodyRequest.Add("signature", this.hexHash);
            this.bodyRequest.Add("nonce", this.nonce.ToString());
            this.bodyRequest.Add("currency", currency);

            queryJson = Newtonsoft.Json.JsonConvert.SerializeObject(this.bodyRequest);

            this.httpRequest.Content = new System.Net.Http.StringContent(queryJson, System.Text.Encoding.UTF8, "application/json");

            this.httpRequest.Content.Headers.Add("key", this.apiKey);
            this.httpRequest.Content.Headers.Add("signature", this.hexHash);
            this.httpRequest.Content.Headers.Add("nonce", this.nonce.ToString());
            this.httpRequest.Content.Headers.Add("currency", currency);

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;
            stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            try
            {
                this.responseRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(stringResponse);
                stringResponse = this.responseRequest["data"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.responseRequest = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
                stringResponse = "ERROR";
            }

            return stringResponse;
        }


        /// <summary>
        ///     Function that return the good signature for including it in http request
        ///     to the Cex.IO REST API
        ///     <paramref name="nonce"/> is the Unix Timestamp of the request
        /// </summary>
        /// <param name="nonce">Unix Timestamp of the reques.</param>
        /// <returns>
        ///     the signature to add to the header of a request to the API
        /// </returns>
        public string getSignature(long nonce)
        {
            string msg2hash = "";
            this.nonce = nonce;

            this.bodyRequest.Clear();

            this.bodyRequest.Add("nonce", this.nonce.ToString());
            this.bodyRequest.Add("id", this.username);
            this.bodyRequest.Add("key", this.apiKey);

            foreach (KeyValuePair<string, string> row in this.bodyRequest)
            {
                msg2hash += row.Value;
            }

            this.hexHash = Tools.byteToString(this.hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(msg2hash))).ToLower();

            this.bodyRequest.Clear();

            return this.hexHash;
        }


        /// <summary>
        ///     Function that return some account informations
        /// </summary>
        /// <returns>
        ///     various account informations :
        ///     - API key
        ///     - API secret
        ///     - Main cryptocurrencies adress 
        /// </returns>
        override public Dictionary<string, string> GetAccountInfos()
        {
            Dictionary<string, string> dicoResponse = new Dictionary<string, string>();
            string BTCAdress = this.GetAdress(MainCryptos.BTC.ToString());
            string ETHAdress = this.GetAdress(MainCryptos.ETH.ToString());
            string XRPAdress = this.GetAdress(MainCryptos.XRP.ToString());
            string LTCAdress = this.GetAdress(MainCryptos.LTC.ToString());

            dicoResponse.Add("key", this.apiKey);
            dicoResponse.Add("secret", this.apiSecret);
            dicoResponse.Add("BTC Adress", BTCAdress);
            dicoResponse.Add("ETH Adress", BTCAdress);
            dicoResponse.Add("XRP Adress", XRPAdress);
            dicoResponse.Add("LTC Adress", LTCAdress);

            return dicoResponse;
        }


        /// <summary>
        ///     Function that cancel an order placed on cex.IO markets,
        ///     identified with its <paramref name="orderId"/>
        /// </summary>
        /// <param name="orderId">the ID of the order to cancel.</param>
        /// <returns>
        ///     a <see cref="Order"/> object, representing the cancelled order
        /// </returns>
        override public Order CancelOrder(string currency, string orderId)
        {
            string queryJson = "";
            string stringResponse;
            OrderType oneOrderType = OrderType.CANCEL;
            OrderStyle oneOrderStyle = OrderStyle.CANCELLATION;
            Exchanges oneExchange = Exchanges.BINANCE;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper()); ;

            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, 0, 0);

            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds();

            this.endpoint = "cancel_order/";

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, this.api + this.endpoint);

            this.hexHash = this.getSignature(this.nonce);

            this.bodyRequest.Clear();

            this.bodyRequest.Add("key", this.apiKey);
            this.bodyRequest.Add("signature", this.hexHash);
            this.bodyRequest.Add("nonce", this.nonce.ToString());
            this.bodyRequest.Add("id", orderId);

            queryJson = Newtonsoft.Json.JsonConvert.SerializeObject(this.bodyRequest);

            this.httpRequest.Content = new System.Net.Http.StringContent(queryJson, System.Text.Encoding.UTF8, "application/json");

            this.httpRequest.Content.Headers.Add("key", this.apiKey);
            this.httpRequest.Content.Headers.Add("signature", this.hexHash);
            this.httpRequest.Content.Headers.Add("nonce", this.nonce.ToString());
            this.httpRequest.Content.Headers.Add("currency", currency);

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;
            stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            try
            {
                this.responseRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(stringResponse);
                oneNewOrder.message = this.responseRequest["data"];
                oneNewOrder.success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.responseRequest = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
                oneNewOrder.message = "ERROR";
                oneNewOrder.success = false;
            }

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that return order ID of the specified <paramref name="currency"/>
        ///     order, previsouly placed on cex.IO markets
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <returns>
        ///     the ID of the order, if it exists, else, returns ERROR
        /// </returns>
        override public string GetOrderId(string currency)
        {
            string orderId = "";
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds();
            List<Order> listOpenOrders = this.GetOpenOrders(currency);

            foreach (Order oneOrder in listOpenOrders)
            {
                if (oneOrder.ccyBase.ToString().ToUpper() == currency.ToUpper())
                {
                    orderId = oneOrder.ID;
                }
            }

            if (orderId == "")
            {
                orderId = "ERROR";
            }

            return orderId;
        }


        /// <summary>
        ///     Function returning all open orders (BUY or SELL) placed on any crypto currencies
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <returns>
        ///     a List of <see cref="Order"/>, representing all open orders
        /// </returns>
        override public List<Order> GetOpenOrders(string currency)
        {
            OrderType oneOrderType;
            OrderStyle oneOrderStyle = OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.CEXIO;
            MainCryptos ccyBase;
            MainCryptos ccyPair;
            double amount;
            double orderPrice;
            List<Order> orderList = new List<Order>();
            Order oneNewOrder;

            string queryJson = "";
            string stringResponse;
            string error;
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds();
            Dictionary<dynamic, dynamic> dicoResponse;

            this.endpoint = "open_position/" + currency + "/" + MainCurrency.USD.ToString();

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, this.api + this.endpoint);

            this.hexHash = this.getSignature(this.nonce);

            this.bodyRequest.Clear();

            this.bodyRequest.Add("key", this.apiKey);
            this.bodyRequest.Add("signature", this.hexHash);
            this.bodyRequest.Add("nonce", this.nonce.ToString());

            queryJson = Newtonsoft.Json.JsonConvert.SerializeObject(this.bodyRequest);

            this.httpRequest.Content = new System.Net.Http.StringContent(queryJson, System.Text.Encoding.UTF8, "application/json");

            this.httpRequest.Content.Headers.Add("key", this.apiKey);
            this.httpRequest.Content.Headers.Add("signature", this.hexHash);
            this.httpRequest.Content.Headers.Add("nonce", this.nonce.ToString());
            this.httpRequest.Content.Headers.Add("symbol", currency);
            this.httpRequest.Content.Headers.Add("msymbol", currency);

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;
            stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            try
            {
                this.responseRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(stringResponse);
                stringResponse = this.responseRequest["data"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                error = this.responseRequest["error"];
                this.responseRequest = new Dictionary<dynamic, dynamic>() { { "ERROR", error } };
                dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", error } };

                stringResponse = "ERROR";
            }

            return orderList;
        }


        /// <summary>
        ///     Function that send <paramref name="amount"/> <paramref name="currency"/>,
        ///     to the specified <paramref name="adress"/>
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="currency">the crypto currency to send.</param>
        /// <returns>
        ///     A <see cref="Order"/> object representing the sending.
        /// </returns>
        override public Order SendCryptos(string adress, string currency, double amount)
        {
            OrderType oneOrderType = OrderType.SEND;
            OrderStyle oneOrderStyle = OrderStyle.WITHDRAWAL;
            Exchanges oneExchange = Exchanges.CEXIO;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, 0);

            string queryJson = "";
            string stringResponse;
            Dictionary<dynamic, dynamic> dicoResponse;

            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds();
            this.endpoint = "withdrawal";
            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, this.api + this.endpoint);
            this.hexHash = this.getSignature(this.nonce);

            this.bodyRequest.Clear();
            this.bodyRequest.Add("key", this.apiKey);
            this.bodyRequest.Add("signature", this.hexHash);
            this.bodyRequest.Add("nonce", this.nonce.ToString());

            queryJson = Newtonsoft.Json.JsonConvert.SerializeObject(this.bodyRequest);

            this.httpRequest.Content = new System.Net.Http.StringContent(queryJson, System.Text.Encoding.UTF8, "application/json");

            this.httpRequest.Content.Headers.Add("key", this.apiKey);
            this.httpRequest.Content.Headers.Add("signature", this.hexHash);
            this.httpRequest.Content.Headers.Add("nonce", this.nonce.ToString());
            this.httpRequest.Content.Headers.Add("symbol", currency);
            this.httpRequest.Content.Headers.Add("msymbol", currency);

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;
            stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            try
            {
                this.responseRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(stringResponse);

                if (this.responseRequest["ok"].ToUpper() == "ERROR")
                {
                    oneNewOrder.success = false;    
                }
                else
                {
                    oneNewOrder.success = true;    
                }

                oneNewOrder.message = this.responseRequest["error"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.responseRequest = new Dictionary<dynamic, dynamic>() { { "ERROR", "NO OPEN ORDERS" } };
                dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "NO OPEN ORDERS" } };

                oneNewOrder.success = false;
                oneNewOrder.message = this.responseRequest["error"];
            }

            return oneNewOrder;
        }
    }
}
