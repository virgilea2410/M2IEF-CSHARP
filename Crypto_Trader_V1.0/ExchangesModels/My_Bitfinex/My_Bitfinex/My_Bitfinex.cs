using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Collections.Generic;
using Newtonsoft.Json;
using Crypto_Trader_V1.Models;


namespace Crypto_Trader_V1.ExchangesModels.My_Bitfinex
{


    /// <summary>
    ///     Class representing the Bitfinex API Wrapper
    /// </summary>
    /// <remarks>
    ///     It use the offical Bitfinex REST API : https://docs.bitfinex.com/docs/public-endpoints
    /// </remarks>
    public class My_Bitfinex : My_ExchangeAPI
    {
        private string key;
        private string secret;

        char[] cSecret, cKey;
        byte[] bKey, hash, bSecret, bPayload, bBody;
        string shash;

        HttpClient client;
        HttpRequestMessage request;
        HttpResponseMessage response;

        Dictionary<dynamic, dynamic> body;
        Dictionary<dynamic, dynamic> requestResponse;
        List<Dictionary<dynamic, dynamic>> listDico;

        HMACSHA384 hasher;

        string payload, jStringResponse, sBody, apiV1, nonce, endpoint;


        /// <summary>
        ///     Constructor
        /// </summary>
        public My_Bitfinex()
        {
            this.key = "XdUb4vbjpJUx1KLFqGqmItuN9YG6Bl8eRiiDLOHzOP9";
            this.secret = "H9vmIrk2it9TqNidtZK0EyvI1Sy4iS3CfhOzpBSn12P";

            this.cSecret = this.secret.ToCharArray();
            this.bSecret = System.Text.Encoding.UTF8.GetBytes(this.cSecret);

            this.cKey = this.key.ToCharArray();
            this.bKey = System.Text.Encoding.UTF8.GetBytes(this.cKey);

            this.client = new HttpClient();
            this.request = new HttpRequestMessage();
            this.response = new HttpResponseMessage();

            this.body = new Dictionary<dynamic, dynamic>();
            this.listDico = new List<Dictionary<dynamic, dynamic>>();

            this.hasher = new HMACSHA384(this.bSecret);

            this.apiV1 = "https://api.bitfinex.com/v1/";
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        }


        /// <summary>
        ///     Function returning the current market price for one <paramref name="currency"/>
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <returns>A <see cref="Price"/> object representing the current market price</returns>
        override public Price GetPrice(string currency)
        {
            double price = 0.00;
            Price cryptoPrice = new Price();
            string exchange;
            string ccyBase;
            string ccyPair;
            string market = "";
            string completeUrl;
            Dictionary<dynamic, dynamic> newBody = new Dictionary<dynamic, dynamic>();

            if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), currency))
            {
                if (currency != MainCryptos.BTC.ToString())
                {
                    if (currency == MainCryptos.DASH.ToString())
                    {
                        currency = "DSH";
                    }

                    market = currency + MainCryptos.BTC.ToString();
                    ccyBase = currency;
                    ccyPair = MainCryptos.BTC.ToString();
                    exchange = Exchanges.BITFINEX.ToString();
                }
                else
                {
                    market = currency + MainCurrency.USD.ToString();
                    ccyBase = currency;
                    ccyPair = MainCurrency.USD.ToString();
                    exchange = Exchanges.BITFINEX.ToString();
                }
            }
            else
            {
                Console.WriteLine("currency not found");
                cryptoPrice = new Price(error: true);
                return cryptoPrice;
            }

            this.endpoint = "pubticker/" + market.ToLower();
            completeUrl = this.apiV1 + this.endpoint;

            this.request = new HttpRequestMessage(HttpMethod.Get, completeUrl);
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            this.response = this.client.SendAsync(this.request).Result;
            this.jStringResponse = this.response.Content.ReadAsStringAsync().Result;

            try
            {
                this.requestResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.jStringResponse);
                price = Double.Parse(this.requestResponse["mid"], System.Globalization.CultureInfo.InvariantCulture);
                cryptoPrice = new Price(exchange, ccyBase, ccyPair, price);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                price = 0;
                ccyBase = "ERROR";
                ccyPair = "ERROR";
                cryptoPrice = new Price(error: true);
            }

            return cryptoPrice;
        }


        /// <summary>
        ///     Function that send an order to <paramref name="side"/>.
        ///     The order is for <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to bitfinex market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new order that has been send to Bitfinex market
        /// </returns>
        override public Order NewOrder(string currency, double amount, double price, string side)
        {
            OrderType oneOrderType = (OrderType)Enum.Parse(typeof(OrderType), side.ToUpper());
            OrderStyle oneOrderStyle = OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.BITFINEX;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair;
            Order oneNewOrder;
            string message = "";
            string completeUrl;
            string market;

            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            this.endpoint ="order/new";
            completeUrl = this.apiV1 + this.endpoint;
            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);

            if (ccyBase != MainCryptos.BTC)
            {
                market = currency.ToUpper() + "BTC";
                ccyPair = MainCryptos.BTC;
            }
            else
            {
                market = currency.ToUpper() + "USD";
                ccyPair = MainCryptos.USDT;
            }

            oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, price);

            this.body.Clear();
            this.body.Add("request", "/v1/order/new");
            this.body.Add("nonce", nonce.ToString());
            this.body.Add("symbol", market);
            this.body.Add("amount", oneNewOrder.amount.ToString());
            this.body.Add("price", oneNewOrder.orderPrice.price.ToString("F5"));
            this.body.Add("exchange", oneNewOrder.exchange.ToString().ToLower());
            this.body.Add("side", oneNewOrder.side.ToString().ToLower());
            this.body.Add("type", "exchange market");

            this.setRequestHeaders(this.body);

            this.response = this.client.SendAsync(this.request).Result;
            this.jStringResponse = this.response.Content.ReadAsStringAsync().Result;

            this.requestResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.jStringResponse);

            if (this.requestResponse.ContainsKey("message"))
            {
                message = this.requestResponse["message"];
                oneNewOrder.success = false;
                oneNewOrder.message = message;
            }
            else
            {
                oneNewOrder.success = true;
            }

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that send an order to buy <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to bitfinex market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new Buy order that has been send to Bitfinex market
        /// </returns>
        override public Order Buy(string currency, double amount, double price)
        {
            Order oneNewOrder;

            oneNewOrder = this.NewOrder(currency, amount, price, OrderType.BUY.ToString());

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that send an order to sell <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to bitfinex market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new Sell order that has been send to Bitfinex market
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
            string message = "";
            string method;
            string walletName = "exchange";
            int renew = 1;
            Dictionary<dynamic, dynamic> newBody = new Dictionary<dynamic, dynamic>();

            this.endpoint = "deposit/new";
            string completeUrl = this.apiV1 + this.endpoint;
            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            if (System.Linq.Enumerable.Contains(MyConstants.MainCryptosDico.Keys, currency))
            {
                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), currency))
                {
                    method = MyConstants.MainCryptosDico[currency.ToUpper()].ToLower();
                }
                else
                {
                    Console.WriteLine("currency not found for new address");
                    method = MyConstants.MainCryptosDico[MainCryptos.BTC.ToString()].ToLower();
                }
            }
            else
            {
                Console.WriteLine("currency not found for new address");
                method = MyConstants.MainCryptosDico[MainCryptos.BTC.ToString()].ToLower();
            }

            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);

            this.body.Clear();
            this.body.Add("request", "/v1/" + this.endpoint);
            this.body.Add("nonce", nonce.ToString());
            this.body.Add("method", method);
            this.body.Add("wallet_name", walletName);
            this.body.Add("renew", renew);

            this.setRequestHeaders(this.body);

            this.response = this.client.SendAsync(this.request).Result;
            this.jStringResponse = this.response.Content.ReadAsStringAsync().Result;

            try
            {
                this.requestResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.jStringResponse);
                message = this.requestResponse["address"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.requestResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
                message = "ERROR";
            }

            return message;
        }


        /// <summary>
        ///     Function returning the portfolio of the user 
        /// </summary>
        /// <returns>
        ///     A <see cref="Portfolio"/> object representing the portfolio of the user on Bitfinex
        /// </returns>
        override public Portfolio GetFolio()
        {
            Portfolio myFolio = new Portfolio();
            Balance oneBalance;
            MainCryptos ccyBase;
            MainCryptos ccyPair;

            this.endpoint = "balances";
            string completeUrl = this.apiV1 + this.endpoint;

            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);

            this.body.Clear();
            this.body.Add("request", "/v1/" + this.endpoint);
            this.body.Add("nonce", nonce.ToString());

            this.setRequestHeaders(this.body);

            this.response = this.client.SendAsync(this.request).Result;
            this.jStringResponse = this.response.Content.ReadAsStringAsync().Result;

            try
            {
                this.listDico = JsonConvert.DeserializeObject<List<Dictionary<dynamic, dynamic>>>(this.jStringResponse);

                this.requestResponse.Clear();

                foreach (Dictionary<dynamic, dynamic> row in this.listDico)
                {
                    string currency = row["currency"].ToUpper();

                    if (Double.Parse(row["available"], System.Globalization.CultureInfo.InvariantCulture) != 0.00)
                    {

                        ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
                        ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
                        oneBalance = new Balance(ccyBase, ccyPair, Double.Parse(row["available"], System.Globalization.CultureInfo.InvariantCulture));
                        myFolio.AddBalance(oneBalance);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.requestResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
            }

            return myFolio;
        }


        /// <summary>
        ///     Function that set the right Headers (mostly the signature) for http request
        ///     to the bitfinex REST API 
        /// </summary>
        /// <param name="requestBody">A dictionary containing all the request key and values.</param>
        /// <example>
        ///     {{request : /api/...},
        ///     {nonce : 124434...},
        ///     rid : 74212}}
        /// </example>
        public void setRequestHeaders(Dictionary<dynamic, dynamic> requestBody)
        {
            this.sBody = JsonConvert.SerializeObject(this.body);
            this.bBody = System.Text.Encoding.UTF8.GetBytes(this.sBody);

            this.payload = System.Convert.ToBase64String(this.bBody);
            this.bPayload = System.Text.Encoding.UTF8.GetBytes(this.payload);

            this.hash = this.hasher.ComputeHash(this.bPayload);

            this.shash = Tools.byteToString(this.hash).ToLower();

            this.request.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");

            this.request.Headers.Add("X-BFX-APIKEY", this.key);
            this.request.Headers.Add("X-BFX-PAYLOAD", this.payload);
            this.request.Headers.Add("X-BFX-SIGNATURE", this.shash);
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

            dicoResponse.Add("key", this.key);
            dicoResponse.Add("secret", this.secret);
            dicoResponse.Add("BTC Adress", BTCAdress);
            dicoResponse.Add("ETH Adress", BTCAdress);
            dicoResponse.Add("XRP Adress", XRPAdress);
            dicoResponse.Add("LTC Adress", LTCAdress);

            return dicoResponse;
        }


        /// <summary>
        ///     Function that cancel an order placed on bitfinex markets,
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
            MainCryptos ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());;
            Dictionary<dynamic, dynamic> newBody = new Dictionary<dynamic, dynamic>();

            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, 0, 0);

            this.endpoint = "order/cancel";
            string completeUrl = this.apiV1 + this.endpoint;
            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);

            this.body.Clear();
            this.body.Add("request", "/v1/" + this.endpoint);
            this.body.Add("nonce", nonce.ToString());
            this.body.Add("rid", orderId);

            this.setRequestHeaders(this.body);

            this.response = this.client.SendAsync(this.request).Result;
            this.jStringResponse = this.response.Content.ReadAsStringAsync().Result;

            try
            {
                this.requestResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.jStringResponse);
                oneNewOrder.message = this.requestResponse["success"];
                oneNewOrder.success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.requestResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
                oneNewOrder.message = "ERROR";
                oneNewOrder.success = false;
            }

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that return order ID of the specified <paramref name="currency"/>
        ///     order, previsouly placed on bitfinex markets
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <returns>
        ///     the ID of the order, if it exists, else, returns ERROR
        /// </returns>
        override public string GetOrderId(string currency)
        {
            string message = "";
            Dictionary<dynamic, dynamic> newBody = new Dictionary<dynamic, dynamic>();

            this.endpoint = "orders";
            string completeUrl = this.apiV1 + this.endpoint;
            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);

            this.body.Clear();
            this.body.Add("request", "/v1/" + this.endpoint);
            this.body.Add("nonce", nonce.ToString());

            this.setRequestHeaders(this.body);

            this.response = this.client.SendAsync(this.request).Result;
            this.jStringResponse = this.response.Content.ReadAsStringAsync().Result;

            try
            {
                if (this.jStringResponse != "[]")
                {
                    this.requestResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.jStringResponse);
                    message = this.requestResponse["id"];
                }
                else
                {
                    this.requestResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "NO OPEN ORDERS" } };
                    message = "ERROR";
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.requestResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
                message = "ERROR";
            }

            return message;
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
            Exchanges oneExchange = Exchanges.BITFINEX;
            MainCryptos ccyBase;
            MainCryptos ccyPair;
            double amount;
            double orderPrice;
            List<Order> orderList = new List<Order>();
            Order oneNewOrder;
            Dictionary<dynamic, dynamic> newBody = new Dictionary<dynamic, dynamic>();

            this.endpoint = "orders";
            string completeUrl = this.apiV1 + this.endpoint;
            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);

            this.body.Clear();
            this.body.Add("request", "/v1/" + this.endpoint);
            this.body.Add("nonce", nonce.ToString());

            this.setRequestHeaders(this.body);

            this.response = this.client.SendAsync(this.request).Result;
            this.jStringResponse = this.response.Content.ReadAsStringAsync().Result;

            try
            {
                if (this.jStringResponse != "[]")
                {
                    this.requestResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.jStringResponse);

                    if (this.requestResponse.ContainsKey("message"))
                    {
                        oneNewOrder = new Order(error: true);
                        oneNewOrder.message = this.requestResponse["message"];
                    }
                }
                else
                {
                    oneNewOrder = new Order(error: true);
                    oneNewOrder.message = "NO OPEN ORDERS";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                oneNewOrder = new Order(error: true);

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
            Exchanges oneExchange = Exchanges.BITFINEX;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, 0);
            Dictionary<dynamic, dynamic> newBody = new Dictionary<dynamic, dynamic>();
            string completeUrl;
            //string message = "";
            //string sucess = "";
            string cryptoName = MyConstants.MainCryptosDico[currency.ToUpper()];

            this.endpoint = "withdraw";
            completeUrl = this.apiV1 + this.endpoint;
            this.request = new HttpRequestMessage(HttpMethod.Post, completeUrl);
            this.nonce = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            this.body.Clear();
            this.body.Add("request", "/v1/" + this.endpoint);
            this.body.Add("withdraw_type", cryptoName.ToLower());
            this.body.Add("walletselected", "exchange");
            this.body.Add("amount", amount.ToString());
            this.body.Add("address", adress);
            this.body.Add("nonce", nonce.ToString());

            this.setRequestHeaders(this.body);

            this.response = this.client.SendAsync(this.request).Result;
            this.jStringResponse = this.response.Content.ReadAsStringAsync().Result;

            try
            {
                if (this.jStringResponse != "[]" && this.response.ReasonPhrase != "Not Allowed")
                {
                    if (this.jStringResponse[0] == '[')
                    {
                        this.jStringResponse = this.jStringResponse.Substring(1, this.jStringResponse.Length - 2);
                    }

                    this.requestResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.jStringResponse);

                    if (this.requestResponse["status"].ToUpper() == "ERROR")
                    {
                        oneNewOrder.success = false;
                    }

                    oneNewOrder.message = this.requestResponse["message"];
                }
                else
                {
                    if (this.response.ReasonPhrase == "Not Allowed")
                    {
                        this.requestResponse = new Dictionary<dynamic, dynamic>() { { "ERROR",  this.response.ReasonPhrase} };
                        oneNewOrder.success = false;
                        oneNewOrder.message = this.response.ReasonPhrase;
                    }
                    else
                    {
                        this.requestResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "Error while sending crypto currency" } };
                        oneNewOrder.success = false;
                        oneNewOrder.message = "Error while sending crypto currency";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.requestResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
                oneNewOrder.success = false;
                oneNewOrder.message = "ERROR";
            }

            return oneNewOrder;
        }
    }
}
