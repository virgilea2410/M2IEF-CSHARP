using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using BittrexSharp;
using Crypto_Trader_V1.Models;


namespace Crypto_Trader_V1.ExchangesModels.My_Bittrex
{

    /// <summary>
    ///     Class representing the Bittrex API Wrapper
    /// </summary>
    /// <remarks>
    ///     It use itself a bittrex API wrapper available on : https://github.com/Domysee/BittrexSharp
    /// </remarks>
    public class My_Bittrex : My_ExchangeAPI
    {
        public string api_key;
        public string api_secret;
        public Bittrex my_bittrex;
        public int portfolio_size;
        Dictionary<string, double> my_balances;


        /// <summary>
        ///     Constructor
        /// </summary>
        public My_Bittrex()
        {
            // Connection to bittrex
            this.api_key = "705f33f99cab480d977177e06ddce266";
            this.api_secret = "392f505579ff436d993672b9f7a21682";
            this.my_bittrex = new Bittrex(this.api_key, this.api_secret);
            this.my_balances = new Dictionary<string, double>();
            this.portfolio_size = this.my_balances.Count;
        }


        /// <summary>
        ///     Function returning the balance of one currency in the portfolio
        ///     <paramref name="currency"/> have to be in <see cref="MainCurrency"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        public double GetBalance(string currency)
        {
            BittrexSharp.Domain.CurrencyBalance one_balance = my_bittrex.GetBalance(currency).Result.Result;

            return (double)one_balance.Available;
        }


        /// <summary>
        ///     Function returning the portfolio of the user 
        /// </summary>
        /// <returns>
        ///     A <see cref="Portfolio"/> object representing the portfolio of the user on Bittrex
        /// </returns>
        override public Portfolio GetFolio()
        {
            Portfolio myFolio = new Portfolio();
            Balance oneBalance;
            MainCryptos ccyBase;
            MainCryptos ccyPair;
            string currency;

            IEnumerable<BittrexSharp.Domain.CurrencyBalance> one_balance = this.my_bittrex.GetBalances().Result.Result;

            this.my_balances.Clear();

            foreach (var row in one_balance)
            {
                if ((double)row.Available > 0)
                {
                    this.my_balances.Add(row.Currency, (double)row.Available);
                }
            }

            foreach (KeyValuePair<string, double> row in this.my_balances)
            {
                currency = row.Key;

                if (row.Value != 0.00)
                {
                    ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
                    ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
                    oneBalance = new Balance(ccyBase, ccyPair, row.Value);
                    myFolio.AddBalance(oneBalance);
                }
            }

            return myFolio;
        }


        /// <summary>
        ///     Function returning all open orders (BUY or SELL) placed on any crypto currencies
        /// </summary>
        /// <returns>
        ///     a List of <see cref="Order"/>, representing all open orders
        /// </returns>
        override public List<Order> GetOpenOrders(string currency)
        {
            OrderType oneOrderType;
            OrderStyle oneOrderStyle = OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.BITTREX;
            MainCryptos ccyBase;
            MainCryptos ccyPair;
            double amount;
            double orderPrice;
            string ID;
            List<Order> orderList = new List<Order>();
            Order oneNewOrder;

            IEnumerable<BittrexSharp.Domain.OpenOrder> openOrders;
            Dictionary<dynamic, dynamic> dicoAllOrders = new Dictionary<dynamic, dynamic>();
            Dictionary<string, Dictionary<double, double>> dicoResult = new Dictionary<string, Dictionary<double, double>>();
            Dictionary<double, double> dicoOrder;

            string market;

            if (currency.ToUpper() != MainCryptos.BTC.ToString())
            {
                market = MainCryptos.BTC.ToString() + "-" + currency;
            }
            else
            {
                market = MainCryptos.USDT.ToString() + "-" + currency;
            }

            openOrders = this.my_bittrex.GetOpenOrders(market).Result.Result;

            foreach (BittrexSharp.Domain.OpenOrder oneOrder in openOrders)
            {
                dicoOrder = new Dictionary<double, double>{{(double)oneOrder.Quantity, (double)oneOrder.Limit}};
                dicoResult = new Dictionary<string, Dictionary<double, double>> { { oneOrder.OrderType, dicoOrder } };
                dicoAllOrders.Add(oneOrder.Exchange, dicoResult);

                oneOrderType = (OrderType)Enum.Parse(typeof(OrderType), oneOrder.OrderType.Split('_')[1]);
                ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), oneOrder.Exchange.Split('-')[1]);
                ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), oneOrder.Exchange.Split('-')[0]);
                amount = (double)oneOrder.Quantity;
                orderPrice = (double)oneOrder.Limit;
                ID = oneOrder.OrderUuid;

                oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle,  true, ccyBase, ccyPair, amount, orderPrice, ID);

                orderList.Add(oneNewOrder);
            }

            return orderList;
        }


        /// <summary>
        ///     Function returning the current market price for one <paramref name="currency"/>
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <returns>A <see cref="Price"/> object representing the current market price</returns>
        override public Price GetPrice(string currency)
        {
            BittrexSharp.Domain.MarketSummary onePrice;
            Price cryptoPrice = new Price();
            string exchange;
            string ccyBase;
            string ccyPair;
            double mid;
            string[] mktName;

            // Handle the fact that Bitcoin Cash have different ticker on Binance
            if (currency == MainCryptos.BCH.ToString())
            {
                currency = "BCC";
            }

            if (currency != MainCryptos.BTC.ToString())
            {
                try
                {
                    onePrice = my_bittrex.GetMarketSummary(MainCryptos.BTC.ToString() + "-" + currency).Result.Result;
                }
                catch
                {
                    cryptoPrice = new Price(error: true, exchange: Exchanges.BITTREX.ToString());
                    onePrice = new BittrexSharp.Domain.MarketSummary();
                    onePrice.Ask = (decimal)0.00;
                    onePrice.Bid = (decimal)0.00;
                }

            }
            else
            {
                try
                {
                    onePrice = my_bittrex.GetMarketSummary(MainCryptos.USDT.ToString() + "-" + MainCryptos.BTC.ToString()).Result.Result;
                }
                catch
                {
                    cryptoPrice = new Price(error: true, exchange: Exchanges.BITTREX.ToString());
                    onePrice = new BittrexSharp.Domain.MarketSummary();
                    onePrice.Ask = (decimal)0.00;
                    onePrice.Bid = (decimal)0.00;
                }
            }

            if (cryptoPrice.error != true)
            {
                exchange = Exchanges.BITTREX.ToString();
                mktName = onePrice.MarketName.Split('-');
                ccyBase = mktName[1];
                ccyPair = mktName[0];
                mid = (double)(onePrice.Ask + onePrice.Bid) / 2.00;

                cryptoPrice = new Price(exchange, ccyBase, ccyPair, mid);                
            }

            return cryptoPrice;
        }



        /// <summary>
        ///     Function that send an order to buy <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to bittrex market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new Buy order that has been send to Bittrex market
        /// </returns>
        override public Order Buy(string currency, double amount, double price)
        {
            OrderType oneOrderType = OrderType.BUY;
            OrderStyle oneOrderStyle = OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.BITTREX;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair;
            Order oneNewOrder;
            BittrexSharp.ResponseWrapper<BittrexSharp.Domain.AcceptedOrder> oneBuy;
            string market;

            if (currency != MainCryptos.BTC.ToString())
            {
                ccyPair = MainCryptos.BTC;
                oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, price);
                market = MainCryptos.BTC.ToString() + "-" + currency;

                if (currency == MainCryptos.BCH.ToString())
                {
                    currency = "BCC";
                }

                try
                {
                    oneBuy = this.my_bittrex.BuyLimit(market, (decimal)amount, (decimal)price).Result;
                    oneNewOrder.success = oneBuy.Success;
                    oneNewOrder.message = oneBuy.Message;
                }
                catch
                {
                    Console.WriteLine("Buy Order wasn't completed or isn't permitted");
                    oneNewOrder.success = false;
                    oneNewOrder.message = "Buy Order wasn't completed or isn't permitted";
                }

            }
            else
            {
                ccyPair = MainCryptos.USDT;
                oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, price);
                market =MainCryptos.USDT.ToString() + "-" + currency;

                try
                {
                    oneBuy = this.my_bittrex.BuyLimit(market, (decimal)amount, (decimal)price).Result;
                    oneNewOrder.success = oneBuy.Success;

                    if (oneBuy.Message == "")
                    {
                        oneNewOrder.message = "Buy Order Succes at price " + price;
                    }
                    else
                    {
                        oneNewOrder.message = oneBuy.Message;
                    }
                }
                catch
                {
                    Console.WriteLine("Buy Order wasn't completed");
                    oneNewOrder.success = false;
                    oneNewOrder.message = "Buy Order wasn't completed or isn't permitted";
                }
            }

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that send an order to sell <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to bittrex market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new Sell order that has been send to Bittrex market
        /// </returns>
        override public Order Sell(string currency, double amount, double price)
        {
            OrderType oneOrderType = OrderType.SELL;
            OrderStyle oneOrderStyle = OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.BITTREX;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair;
            Order oneNewOrder;
            BittrexSharp.ResponseWrapper<BittrexSharp.Domain.AcceptedOrder> oneSell;
            string market;

            if (currency != MainCryptos.BTC.ToString())
            {
                ccyPair = MainCryptos.BTC;
                oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, price);
                market = MainCryptos.BTC.ToString() + "-" + currency;

                if (currency == MainCryptos.BCH.ToString())
                {
                    currency = "BCC";
                }

                try
                {
                    oneSell = this.my_bittrex.SellLimit(market, (decimal)amount, (decimal)price).Result;
                    oneNewOrder.success = oneSell.Success;

                    if (oneSell.Message == "")
                    {
                        oneNewOrder.message = "Sell Order Succes at price " + price;
                    }
                    else
                    {
                        oneNewOrder.message = oneSell.Message;   
                    }
                }
                catch
                {
                    Console.WriteLine("Sell Order wasn't completed or isn't permitted");
                    oneNewOrder.success = false;
                    oneNewOrder.message = "Sell Order wasn't completed or isn't permitted";
                }

            }
            else
            {
                ccyPair = MainCryptos.USDT;
                oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, price);
                market = MainCryptos.USDT.ToString() + "-" + currency;

                try
                {
                    oneSell = this.my_bittrex.SellLimit(market, (decimal)amount, (decimal)price).Result;
                    oneNewOrder.success = oneSell.Success;
                    oneNewOrder.message = oneSell.Message;
                }
                catch
                {
                    Console.WriteLine("Sell Order wasn't completed");
                    oneNewOrder.success = false;
                    oneNewOrder.message = "Sell Order wasn't completed or isn't permitted";
                }
            }

            return oneNewOrder;
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
            string sparametersCcy, completeUri, shash, jStringResponse;
            byte[] uriBytes, secretBytes, hash;
            string BaseUrl = "https://bittrex.com/api/v1.1/";
            string uri = BaseUrl + "account/withdraw";
            long nonce = DateTime.Now.Ticks;

            OrderType oneOrderType = OrderType.SEND;
            OrderStyle oneOrderStyle = OrderStyle.WITHDRAWAL;
            Exchanges oneExchange = Exchanges.BITTREX;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, 0);

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            HttpResponseMessage response = new HttpResponseMessage();

            System.Security.Cryptography.HMACSHA512 encryptedUri = new System.Security.Cryptography.HMACSHA512();
            Dictionary<string, string> dicoResponse = new Dictionary<string, string>();
            Dictionary<string, string> parametersCcy = new Dictionary<string, string>
            {
                { "apikey", this.api_key.ToString() },
                { "currency", currency },
                { "quantity", amount.ToString(System.Globalization.CultureInfo.InvariantCulture) },
                { "address", adress },
                { "nonce", nonce.ToString() }
            };

            sparametersCcy = parametersCcy.Select(param => System.Net.WebUtility.UrlEncode(param.Key) + "=" + System.Net.WebUtility.UrlEncode(param.Value)).Aggregate((l, r) => l + "&" + r);
            completeUri = uri + "?" + sparametersCcy;
            uriBytes = System.Text.Encoding.UTF8.GetBytes(completeUri);
            secretBytes = System.Text.Encoding.UTF8.GetBytes(this.api_secret);
            encryptedUri = new System.Security.Cryptography.HMACSHA512(secretBytes);
            hash = encryptedUri.ComputeHash(uriBytes);
            shash = Tools.byteToString(hash);

            request = new HttpRequestMessage(HttpMethod.Get, completeUri);
            request.Headers.Add("apisign", shash);

            response = client.SendAsync(request).Result;
            jStringResponse = response.Content.ReadAsStringAsync().Result;
            Newtonsoft.Json.JsonConvert.PopulateObject(jStringResponse, dicoResponse);

            oneNewOrder.success = bool.Parse(dicoResponse["success"]);
            oneNewOrder.message = dicoResponse["message"];

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
            BittrexSharp.Domain.DepositAddress response = new BittrexSharp.Domain.DepositAddress();
            string adress;

            try
            {
                response = this.my_bittrex.GetDepositAddress(currency).Result.Result;
                adress = response.Address;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                adress = "ERROR";
            }

            return adress;
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
        ///     Function that cancel an order placed on bittrex markets,
        ///     identified with its <paramref name="orderId"/>
        /// </summary>
        /// <param name="orderId">the ID of the order to cancel.</param>
        /// <returns>
        ///     a <see cref="Order"/> object, representing the cancelled order
        /// </returns>
        override public Order CancelOrder(string currency, string orderId)
        {
            BittrexSharp.ResponseWrapper<object> responseCancelOrder;
            object canceledOrder;
            OrderType oneOrderType = OrderType.CANCEL;
            OrderStyle oneOrderStyle = OrderStyle.CANCELLATION;
            Exchanges oneExchange = Exchanges.BINANCE;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper()); ;

            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, 0, 0);

            try
            {
                responseCancelOrder = this.my_bittrex.CancelOrder(orderId).Result;
                canceledOrder = this.my_bittrex.CancelOrder(orderId).Result;
                oneNewOrder.message = responseCancelOrder.Message;
                oneNewOrder.success = responseCancelOrder.Success;

                return oneNewOrder;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                oneNewOrder.message = "ERROR";
                oneNewOrder.success = false;
                return oneNewOrder;   
            }
        }


        /// <summary>
        ///     Function that return order ID of the specified <paramref name="currency"/>
        ///     order, previsouly placed on bittrex markets
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <returns>
        ///     the ID of the order, if it exists, else, returns ERROR
        /// </returns>
        override public string GetOrderId(string currency)
        {
            List<Order> listOpenOrders;
            string orderId = "";
            string market;

            if (currency.ToUpper() != MainCryptos.BTC.ToString())
            {
                market = MainCryptos.BTC.ToString() + "-" + currency;
            }
            else
            {
                market = MainCryptos.USDT.ToString() + "-" + currency;
            }

            try
            {
                listOpenOrders = this.GetOpenOrders(currency);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return "ERROR";
            }

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
    }
}
