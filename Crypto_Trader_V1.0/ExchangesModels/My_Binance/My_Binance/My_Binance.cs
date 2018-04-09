using System;
using System.Collections.Generic;
using Crypto_Trader_V1.Models;
using Newtonsoft.Json;


namespace Crypto_Trader_V1.ExchangesModels.My_Binance
{


    /// <summary>
    ///     Class representing the Binance API Wrapper
    /// </summary>
    /// <remarks>
    /// It use this official Binance REST API : https://github.com/binance-exchange/binance-official-api-docs/blob/master/rest-api.md
    /// </remarks>
    public class My_Binance : My_ExchangeAPI
    {
        private string api_key;
        private string api_secret;
        System.Net.Http.HttpClient httpClient;
        System.Net.Http.HttpRequestMessage httpRequest;
        System.Net.Http.HttpResponseMessage httpResponse;
        System.Security.Cryptography.HMACSHA256 hasher;
        Dictionary<string, string> bodyRequest;
        Dictionary<dynamic, dynamic> responseRequest;
        List<Dictionary<dynamic, dynamic>> listDico;
        string hexHash, endpoint, queryString, api;


        /// <summary>
        ///     Constructor
        /// </summary>
        public My_Binance()
        {
            this.api_key = "<your_api_key>";
            this.api_secret = "<your_api_key>";

            this.api = "https://api.binance.com/";

            this.httpClient = new System.Net.Http.HttpClient();
            this.httpRequest = new System.Net.Http.HttpRequestMessage();
            this.httpResponse = new System.Net.Http.HttpResponseMessage();

            this.hasher = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(this.api_secret));

            this.bodyRequest = new Dictionary<string, string>();
            this.listDico = new List<Dictionary<dynamic, dynamic>>();
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
            string ccyPair, ccyBase, market, exchange;
            double price;
            string stringResponse = "";

            this.endpoint = "api/v3/ticker/price";

            exchange = Exchanges.BINANCE.ToString();
            ccyBase = currency;

            if (currency != MainCryptos.BTC.ToString())
            {
                market = (currency + MainCryptos.BTC.ToString()).ToUpper();
                ccyPair = MainCryptos.BTC.ToString();
            }
            else
            {
                market = (currency + MainCryptos.USDT.ToString()).ToUpper();
                ccyPair = MainCryptos.USDT.ToString();
            }
            this.queryString = "";

            this.bodyRequest.Clear();
            this.bodyRequest.Add("symbol", market);

            foreach (KeyValuePair<string, string> row in this.bodyRequest)
            {
                this.queryString += row.Key + "=" + row.Value + "&";
            }
            this.queryString = this.queryString.Substring(0, this.queryString.Length - 1);

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, this.api + this.endpoint + "?" + this.queryString);
            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;
            stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            this.responseRequest = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(stringResponse);

            if (this.responseRequest.ContainsKey("price"))
            {
                price = Double.Parse(this.responseRequest["price"], System.Globalization.CultureInfo.InvariantCulture);
                cryptoPrice = new Price(exchange, ccyBase, ccyPair, price);
            }
            else
            {
                cryptoPrice = new Price(error: true);
            }

            return cryptoPrice;
        }


        /// <summary>
        ///     Function that send an order to <paramref name="side"/>.
        ///     The order is for <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to binance market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new order that has been send to Binance market
        /// </returns>
        override public Order NewOrder(string currency, double amount, double price, string side)
        {
            string market = "";
            Dictionary<string, dynamic> jsonResponse = new Dictionary<string, dynamic>();
            List<KeyValuePair<string,string>> jsonResponse2 = new List<KeyValuePair<string, string>>();
            long yesterday = DateTimeOffset.Now.AddDays(-1).ToUnixTimeMilliseconds();
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            OrderType oneOrderType = (OrderType)Enum.Parse(typeof(OrderType), side.ToUpper());
            OrderStyle oneOrderStyle = OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.BINANCE;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair;
            Order oneNewOrder;

            this.endpoint = "api/v3/order";

            this.queryString = "";

            if (ccyBase != MainCryptos.BTC)
            {
                market = (currency + "BTC").ToUpper();
                ccyPair = MainCryptos.BTC;
            }
            else
            {
                market = (currency + "USDT").ToUpper();
                ccyPair = MainCryptos.USDT;
            }

            oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, price);

            this.bodyRequest.Clear();

            this.bodyRequest.Add("symbol", market);
            this.bodyRequest.Add("side", oneNewOrder.side.ToString());
            this.bodyRequest.Add("type", oneNewOrder.style.ToString());
            this.bodyRequest.Add("timeInForce", "gtc".ToUpper());
            this.bodyRequest.Add("quantity", oneNewOrder.amount.ToString());
            this.bodyRequest.Add("price", oneNewOrder.orderPrice.price.ToString("F5"));
            this.bodyRequest.Add("recvWindow", 1000000.ToString());
            this.bodyRequest.Add("timestamp", (DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString());

            foreach (KeyValuePair<string, string> row in this.bodyRequest)
            {
                this.queryString += row.Key + "=" + row.Value + "&";
            }
            this.queryString = this.queryString.Substring(0, this.queryString.Length - 1);

            this.hexHash = Tools.byteToString(hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(this.queryString))).ToLower();

            this.queryString += "&signature=" + this.hexHash;

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, this.api + this.endpoint + "?" + this.queryString);

            this.httpRequest.Headers.Add("X-MBX-APIKEY", this.api_key);

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;

            string stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            try
            {
                jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(stringResponse);

                if (jsonResponse.ContainsKey("msg"))
                {
                    oneNewOrder.success = false;
                    oneNewOrder.message = jsonResponse["msg"];
                }
                else if (jsonResponse.ContainsKey("symbol"))
                {
                    oneNewOrder.success = true;
                    oneNewOrder.message = "order executed at price " + jsonResponse["price"];
                }
                else
                {
                    oneNewOrder.success = false;
                    oneNewOrder.message = jsonResponse["ERROR"];
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                try
                {
                    this.listDico = JsonConvert.DeserializeObject<List<Dictionary<dynamic, dynamic>>>(stringResponse);
                    oneNewOrder.success = false;
                    oneNewOrder.message = jsonResponse["ERROR"];
                }
                catch (Exception e2)
                {
                    Console.Write(e2.Message);
                    oneNewOrder.success = false;
                    oneNewOrder.message = jsonResponse["ERROR"];
                }
            }

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that send an order to buy <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to binance market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new Buy order that has been send to Binance market
        /// </returns>
        override public Order Buy(string currency, double amount, double price)
        {
            Order oneNewOrder;

            oneNewOrder = this.NewOrder(currency, amount, price, OrderType.BUY.ToString());

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that send an order to sell <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to binance market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new Sell order that has been send to Binance market
        /// </returns>
        override public Order Sell(string currency, double amount, double price)
        {
            Order oneNewOrder;

            oneNewOrder = this.NewOrder(currency, amount, price, OrderType.SELL.ToString());

            return oneNewOrder;
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
            string stringResponse = "";

            this.endpoint = "wapi/v3/depositAddress.html";
            this.queryString = "";

            this.bodyRequest.Clear();
            this.bodyRequest.Add("asset", currency);
            this.bodyRequest.Add("recvWindow", 1000000.ToString());
            this.bodyRequest.Add("timestamp", (DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString());

            foreach (KeyValuePair<string, string> row in this.bodyRequest)
            {
                this.queryString += row.Key + "=" + row.Value + "&";
            }
            this.queryString = this.queryString.Substring(0, this.queryString.Length - 1);

            this.hexHash = Tools.byteToString(hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(queryString))).ToLower();

            this.queryString += "&signature=" + this.hexHash;

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, this.api + this.endpoint + "?" + this.queryString);

            this.httpRequest.Headers.Add("X-MBX-APIKEY", this.api_key);

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;

            stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            this.responseRequest = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(stringResponse);

            if (this.responseRequest.ContainsKey("address"))
            {
                return this.responseRequest["address"];    
            }
            else if (this.responseRequest.ContainsKey("success"))
            {
                return this.responseRequest["success"].ToString();
            }
            else
            {
                return "ERROR";
            }
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
            Exchanges oneExchange = Exchanges.BINANCE;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            Order oneNewOrder;
            string stringResponse = "";

            oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, 0);

            this.endpoint = "wapi/v3/withdraw.html";
            this.queryString = "";
            this.bodyRequest.Clear();

            this.bodyRequest.Add("asset", currency);
            this.bodyRequest.Add("address", adress);
            this.bodyRequest.Add("amount", amount.ToString());
            this.bodyRequest.Add("recvWindow", 1000000.ToString());
            this.bodyRequest.Add("timestamp", (DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString());

            foreach (KeyValuePair<string, string> row in this.bodyRequest)
            {
                this.queryString += row.Key + "=" + row.Value + "&";
            }
            this.queryString = this.queryString.Substring(0, this.queryString.Length - 1);

            this.hexHash = Tools.byteToString(hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(queryString))).ToLower();

            this.queryString += "&signature=" + this.hexHash;

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, this.api + this.endpoint + "?" + this.queryString);

            this.httpRequest.Headers.Add("X-MBX-APIKEY", this.api_key);

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;

            stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            this.responseRequest = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(stringResponse);

            if (this.responseRequest.ContainsKey("success"))
            {
                oneNewOrder.success = this.responseRequest["success"];    
            }
            else
            {
                oneNewOrder.success = false;    
            }

            if (this.responseRequest.ContainsKey("msg"))
            {
                oneNewOrder.message = this.responseRequest["msg"];
            }

            return oneNewOrder;
        }


        /// <summary>
        ///     Function returning the portfolio of the user 
        /// </summary>
        /// <returns>
        ///     A <see cref="Portfolio"/> object representing the portfolio of the user on Binance
        /// </returns>
        override public Portfolio GetFolio()
        {
            Portfolio myFolio = new Portfolio();
            Balance oneBalance;
            MainCryptos ccyBase;
            MainCryptos ccyPair;

            string stringResponse = "";
            string currency;
            Dictionary<dynamic, dynamic> dicoPortfolio = new Dictionary<dynamic, dynamic>();
            this.endpoint = "api/v3/account";

            this.queryString = "";

            this.bodyRequest.Clear();

            this.bodyRequest.Add("recvWindow", 1000000.ToString());
            this.bodyRequest.Add("timestamp", (DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString());

            foreach (KeyValuePair<string, string> row in this.bodyRequest)
            {
                this.queryString += row.Key + "=" + row.Value + "&";
            }
            this.queryString = this.queryString.Substring(0, this.queryString.Length - 1);

            this.hexHash = Tools.byteToString(hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(queryString))).ToLower();

            this.queryString += "&signature=" + this.hexHash;

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, this.api + this.endpoint + "?" + this.queryString);

            this.httpRequest.Headers.Add("X-MBX-APIKEY", this.api_key);

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;

            stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            this.responseRequest = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(stringResponse);

            if (this.responseRequest.ContainsKey("balances"))
            {
                this.listDico = JsonConvert.DeserializeObject<List<Dictionary<dynamic, dynamic>>>(this.responseRequest["balances"].ToString());    
            }
            else
            {
                this.listDico = new List<Dictionary<dynamic, dynamic>>();
            }

            foreach (Dictionary<dynamic, dynamic> row in this.listDico)
            {
                Dictionary<string, double> dico1Crypto = new Dictionary<string, double>();
                currency = row["asset"].ToUpper();

                if (Double.Parse(row["free"], System.Globalization.CultureInfo.InvariantCulture) != 0.00)
                {
                    ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
                    ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
                    oneBalance = new Balance(ccyBase, ccyPair, Double.Parse(row["free"], System.Globalization.CultureInfo.InvariantCulture));
                    myFolio.AddBalance(oneBalance);
                }
            }

            return myFolio;
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

            dicoResponse.Add("key", this.api_key);
            dicoResponse.Add("secret", this.api_secret);
            dicoResponse.Add("BTC Adress", BTCAdress);
            dicoResponse.Add("ETH Adress", BTCAdress);
            dicoResponse.Add("XRP Adress", XRPAdress);
            dicoResponse.Add("LTC Adress", LTCAdress);

            return dicoResponse;
        }


        /// <summary>
        ///     Function that cancel an order placed on binance markets,
        ///     identified with its <paramref name="orderId"/>
        /// </summary>
        /// <param name="orderId">the ID of the order to cancel.</param>
        /// <returns>
        ///     a <see cref="Order"/> object, representing the cancelled order
        /// </returns>
        override public Order CancelOrder(string currency, string orderId)
        {
            OrderType oneOrderType = OrderType.CANCEL;
            OrderStyle oneOrderStyle = OrderStyle.CANCELLATION;
            Exchanges oneExchange = Exchanges.BINANCE;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair;
            Order oneNewOrder;
            string market, stringResponse;

            this.endpoint = "api/v3/order";
            this.queryString = "";

            if (currency != MainCryptos.BTC.ToString())
            {
                market = (currency + "BTC").ToUpper();
                ccyPair = MainCryptos.BTC;
            }
            else
            {
                market = (currency + "USDT").ToUpper();
                ccyPair = MainCryptos.USDT;
            }

            oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, 0, 0);

            this.queryString = "";

            this.bodyRequest.Clear();

            this.bodyRequest.Add("symbol", market.ToString());
            this.bodyRequest.Add("orderId", orderId);
            this.bodyRequest.Add("recvWindow", 1000000.ToString());
            this.bodyRequest.Add("timestamp", (DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString());

            foreach (KeyValuePair<string, string> row in this.bodyRequest)
            {
                this.queryString += row.Key + "=" + row.Value + "&";
            }
            this.queryString = this.queryString.Substring(0, this.queryString.Length - 1);

            this.hexHash = Tools.byteToString(hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(queryString))).ToLower();

            this.queryString += "&signature=" + this.hexHash;

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Delete, this.api + this.endpoint + "?" + this.queryString);

            this.httpRequest.Headers.Add("X-MBX-APIKEY", this.api_key);

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;

            stringResponse = this.httpResponse.Content.ReadAsStringAsync().Result;

            this.responseRequest = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(stringResponse);

            if (this.responseRequest.ContainsKey("symbol"))
            {
                oneNewOrder.success = true;
            }
            else
            {
                oneNewOrder.success = false;
            }

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that return order ID of the specified <paramref name="currency"/>
        ///     order, previsouly placed on binance markets
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <returns>
        ///     the ID of the order, if it exists, else, returns ERROR
        /// </returns>
        override public string GetOrderId(string currency)
        {
            Price BTCPrice = this.GetPrice(MainCryptos.BTC.ToString());
            double BTCSpot = BTCPrice.price;
            List<Order> listOpenOrders;
            string orderId = "";

            listOpenOrders = this.GetOpenOrders(currency);

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
            Exchanges oneExchange = Exchanges.BINANCE;
            MainCryptos ccyBase;
            MainCryptos ccyPair;
            double amount, orderPrice;
            List<Order> orderList = new List<Order>();
            Order oneNewOrder;

            Price BTCPrice = this.GetPrice(MainCryptos.BTC.ToString());
            double BTCSpot = BTCPrice.price;
            string market, message, aggregateMarket, ID;

            this.endpoint = "api/v3/openOrders";
            this.queryString = "";

            if (currency != MainCryptos.BTC.ToString())
            {
                market = (currency + "BTC").ToUpper();
                aggregateMarket = ("BTC-" + currency).ToUpper();
                ccyPair = MainCryptos.BTC;
                ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
            }
            else
            {
                market = (currency + "USDT").ToUpper();
                aggregateMarket = ("USDT-" + currency).ToUpper();
                ccyPair = MainCryptos.USDT;
                ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
            }

            this.queryString = "";

            this.bodyRequest.Clear();

            this.bodyRequest.Add("symbol", market.ToString());
            this.bodyRequest.Add("recvWindow", 1000000.ToString());
            this.bodyRequest.Add("timestamp", (DateTimeOffset.Now.ToUnixTimeMilliseconds()).ToString());

            foreach (KeyValuePair<string, string> row in this.bodyRequest)
            {
                this.queryString += row.Key + "=" + row.Value + "&";
            }
            this.queryString = this.queryString.Substring(0, this.queryString.Length - 1);

            this.hexHash = Tools.byteToString(hasher.ComputeHash(System.Text.Encoding.UTF8.GetBytes(queryString))).ToLower();

            this.queryString += "&signature=" + this.hexHash;

            this.httpRequest = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, this.api + this.endpoint + "?" + this.queryString);

            this.httpRequest.Headers.Add("X-MBX-APIKEY", this.api_key);

            this.httpResponse = this.httpClient.SendAsync(this.httpRequest).Result;

            message = this.httpResponse.Content.ReadAsStringAsync().Result;

            if (message != "[]")
            {
                try
                {
                    this.responseRequest = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(message);

                    if (this.responseRequest.ContainsKey("msg"))
                    {
                        this.responseRequest = new Dictionary<dynamic, dynamic>() { { "ERROR", this.responseRequest["msg"] } };
                    }                    
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);

                    if (message.Substring(0, 1) == "[")
                    {
                        message = message.Substring(1, message.Length - 2);
                    }

                    this.responseRequest = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(message);

                    if (this.responseRequest.ContainsKey("symbol"))
                    {
                        oneOrderType = Enum.Parse(typeof(OrderType), this.responseRequest["side"]);
                        oneOrderStyle = Enum.Parse(typeof(OrderStyle), this.responseRequest["type"]);
                        amount = Double.Parse(this.responseRequest["origQty"], System.Globalization.CultureInfo.InvariantCulture);
                        orderPrice = Double.Parse(this.responseRequest["price"], System.Globalization.CultureInfo.InvariantCulture);
                        ID = this.responseRequest["orderId"].ToString();

                        oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle,true , ccyBase, ccyPair, amount, orderPrice, ID);
                        orderList.Add(oneNewOrder);
                    }
                }
            }

            return orderList;
        }
    }
}
