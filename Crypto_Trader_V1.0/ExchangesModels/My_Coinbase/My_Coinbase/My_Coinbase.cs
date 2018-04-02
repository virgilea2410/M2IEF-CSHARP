using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Coinbase.ObjectModel;
using Crypto_Trader_V1.Models;

namespace Crypto_Trader_V1.ExchangesModels.My_Coinbase
{

    /// <summary>
    ///     Class representing the Coinbase API Wrapper
    /// </summary>
    /// <remarks>
    ///     It use itself a coinbase API wrapper available on : https://github.com/bchavez/Coinbase
    /// </remarks>
    public class My_Coinbase : My_ExchangeAPI
    {
        Coinbase.CoinbaseApi coinbase;
        string apiKey;
        string apiSecret;

        CoinbaseResponse response;

        List<Dictionary<dynamic, dynamic>> requestResponse = new List<Dictionary<dynamic, dynamic>>();
        Dictionary<dynamic, dynamic> dicoResponse = new Dictionary<dynamic, dynamic>();


        /// <summary>
        ///     Constructor
        /// </summary>
        public My_Coinbase()
        {
            this.apiKey = "Jgr8yrdDUpUn8llf";
            this.apiSecret = "EDl4b17qKlDKJC5xjIjUnUVwJhtxxvhV";
            this.coinbase = new Coinbase.CoinbaseApi(apiKey, apiSecret, false, null, true);
        }

        /// <summary>
        ///     Function returning the current market price for one <paramref name="currency"/>
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <returns>A <see cref="Price"/> object representing the current market price</returns>
        override public Price GetPrice(string currency)
        {
            Dictionary<string, string> optionsDico = new Dictionary<string, string>();
            Dictionary<string, string> priceDico = new Dictionary<string, string>();
            string options = "";
            string ccyBase = "";
            string ccyPair = "";
            double price = 0.00;
            Price cryptoPrice;
            string exchange;
            string sJsonPrice;
            object JsonPrice;

            try
            {
                string request = "/prices/" + currency + "-USD/buy";
                optionsDico.Add("currency_pair", currency + "-USD");
                options = JsonConvert.SerializeObject(optionsDico);

                this.response = this.coinbase.SendRequest(request, options, RestSharp.Method.GET);
                sJsonPrice = this.response.Data.ToString();
                JsonPrice = JsonConvert.DeserializeObject(sJsonPrice);   
            }
            catch (Exception exc)
            {
                this.response = new CoinbaseResponse();
                this.response.Data = null;
                Console.WriteLine(exc.Message);
            }

            if (this.response.Data != null)
            {
                sJsonPrice = this.response.Data.ToString();
                JsonPrice = JsonConvert.DeserializeObject(sJsonPrice);
                JsonConvert.PopulateObject(sJsonPrice, priceDico);

                exchange = Exchanges.COINBASE.ToString();
                ccyBase = priceDico["base"];
                ccyPair = priceDico["currency"];
                price = double.Parse(priceDico["amount"], System.Globalization.CultureInfo.InvariantCulture);
                cryptoPrice = new Price(exchange, ccyBase, ccyPair, price);
            }
            else
            {
                cryptoPrice = new Price(error: true, exchange:Exchanges.COINBASE.ToString());
            }

            return cryptoPrice;
        }


        /// <summary>
        ///     Function that send an order to buy <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to coinbase market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new Buy order that has been send to Coinbase market
        /// </returns>
        override public Order Buy(string currency, double amount, double price)
        {
            Dictionary<string, string> body = new Dictionary<string, string>();
            Models.OrderType oneOrderType = Models.OrderType.BUY;
            Models.OrderStyle oneOrderStyle = Models.OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.COINBASE;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = MainCryptos.USDT;
            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, price);
            string ccyId;

            if (System.Linq.Enumerable.Contains(this.GetAccountId().Keys, currency))
            {
                ccyId = this.GetAccountId()[currency];    
            }
            else
            {
                ccyId = "";
            }

            string api = "https://api.coinbase.com/";
            string queryString = "v2/accounts/" + ccyId + "/buys";

            body.Add("currency", currency);
            body.Add("amount", amount.ToString());

            this.response = this.coinbase.SendRequest(api + queryString, body, RestSharp.Method.POST);

            if (this.response.Errors != null)
            {
                oneNewOrder.success = false;
                oneNewOrder.message = this.response.Errors[0].Message;
            }
            else
            {
                oneNewOrder.success = false;
                oneNewOrder.message = this.response.Data.ToString();
            }

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that send an order to sell <paramref name="amount"/> <paramref name="currency"/>,
        ///     at a limit price of <paramref name="price"/>, to coinbase market
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        ///     <paramref name="amount"/> have to be positive
        ///     <paramref name="price"/> have to be positive
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <param name="amount">amount to buy.</param>
        /// <param name="price">limit price of the order.</param>
        /// <returns>
        ///     A <see cref="Order"/> object, representing the new Sell order that has been send to Coinbase market
        /// </returns>
        override public Order Sell(string currency, double amount, double price)
        {
            Dictionary<string, string> body = new Dictionary<string, string>();
            Models.OrderType oneOrderType = Models.OrderType.BUY;
            Models.OrderStyle oneOrderStyle = Models.OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.COINBASE;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = MainCryptos.USDT;
            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, price);
            string ccyId;

            if (System.Linq.Enumerable.Contains(this.GetAccountId().Keys, currency))
            {
                ccyId = this.GetAccountId()[currency];
            }
            else
            {
                ccyId = "";
            }

            string api = "https://api.coinbase.com/";
            string queryString = "v2/accounts/" + ccyId + "/sells";

            body.Add("currency", currency);
            body.Add("amount", amount.ToString());

            this.response = this.coinbase.SendRequest(api + queryString, body, RestSharp.Method.POST);

            if (this.response.Errors != null)
            {
                oneNewOrder.success = false;
                oneNewOrder.message = this.response.Errors[0].Message;
            }
            else
            {
                oneNewOrder.success = false;
                oneNewOrder.message = this.response.Data.ToString();
            }

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that get all currencies acount IDs.
        ///     On coinbase, each currency have a specific account ID.
        /// </summary>
        /// <returns>
        ///     A Dictionary containing the account ID of each available currency
        /// </returns>
        public Dictionary<string, string> GetAccountId()
        {
            string api = "https://api.coinbase.com/v2/accounts";

            Dictionary<string, string> dicoStrResponse = new Dictionary<string, string>();

            this.response = this.coinbase.SendRequest(api, "", RestSharp.Method.GET);

            try 
            {
                this.requestResponse = JsonConvert.DeserializeObject<List<Dictionary<dynamic, dynamic>>>(this.response.Data.ToString());
                foreach (Dictionary<dynamic, dynamic> row in this.requestResponse)
                {
                    dicoStrResponse.Add(row["currency"], row["id"]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                dicoStrResponse = new Dictionary<string, string>() { { "ERROR", "ERROR" } };
            }

            return dicoStrResponse;
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
            string ccyId;
            string api = "https://api.coinbase.com/v2/accounts/:account_id/addresses";

            try
            {
                ccyId = this.GetAccountId()[currency];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ccyId = "ERROR";
            }

            api = "https://api.coinbase.com/v2/accounts/" + ccyId + "/addresses";
            string adress;

            this.response = this.coinbase.SendRequest(api, "", RestSharp.Method.POST);

            try
            {
                this.dicoResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.response.Data.ToString());
                adress = this.dicoResponse["address"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                adress = "ERROR";
            }

            return adress;
        }


        /// <summary>
        ///     Function returning the portfolio of the user 
        /// </summary>
        /// <returns>
        ///     A <see cref="Portfolio"/> object representing the portfolio of the user on Coinbase
        /// </returns>
        override public Portfolio GetFolio()
        {
            Portfolio myFolio = new Portfolio();
            Balance oneBalance;
            MainCryptos ccyBase;
            MainCryptos ccyPair;
            string currency;
            string api = "https://api.coinbase.com/v2/accounts";
            Price BTCPrice = this.GetPrice(MainCryptos.BTC.ToString());
            double BTCSpot = BTCPrice.price;

            this.dicoResponse = new Dictionary<dynamic, dynamic>();
            Dictionary<dynamic, dynamic> dicoOneBalance = new Dictionary<dynamic, dynamic>();

            this.response = this.coinbase.SendRequest(api, "", RestSharp.Method.GET);

            try
            {
                this.requestResponse = JsonConvert.DeserializeObject<List<Dictionary<dynamic, dynamic>>>(this.response.Data.ToString());

                foreach (Dictionary<dynamic, dynamic> row in this.requestResponse)
                {
                    Dictionary<string, double> dico1Crypto = new Dictionary<string, double>();
                    currency = row["currency"].ToUpper();
                    dicoOneBalance = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(row["balance"].ToString());

                    if (Double.Parse(dicoOneBalance["amount"], System.Globalization.CultureInfo.InvariantCulture) != 0.00)
                    //if (Double.Parse(row["available"], System.Globalization.CultureInfo.InvariantCulture) != 0.00)
                    {
                        ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
                        ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency);
                        oneBalance = new Balance(ccyBase, ccyPair, Double.Parse(dicoOneBalance["amount"], System.Globalization.CultureInfo.InvariantCulture));
                        myFolio.AddBalance(oneBalance);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
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

            dicoResponse.Add("key", this.apiKey);
            dicoResponse.Add("secret", this.apiSecret);
            dicoResponse.Add("BTC Adress", BTCAdress);
            dicoResponse.Add("ETH Adress", BTCAdress);
            dicoResponse.Add("XRP Adress", XRPAdress);
            dicoResponse.Add("LTC Adress", LTCAdress);

            return dicoResponse;
        }


        /// <summary>
        ///     Function that cancel an order placed on coinbase markets,
        ///     identified with its <paramref name="orderId"/>
        /// </summary>
        /// <param name="orderId">the ID of the order to cancel.</param>
        /// <returns>
        ///     a <see cref="Order"/> object, representing the cancelled order
        /// </returns>
        override public Order CancelOrder(string currency, string orderId)
        {
            string ccyId = "";
            string api = "https://api.coinbase.com/v2/accounts/:account_id/addresses";
            Models.OrderType oneOrderType = Models.OrderType.CANCEL;
            Models.OrderStyle oneOrderStyle = Models.OrderStyle.CANCELLATION;
            Exchanges oneExchange = Exchanges.BINANCE;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper()); ;

            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, 0, 0);
            Dictionary<string, string> dicoCcyIds = this.GetAccountId();

            foreach (KeyValuePair<string, string> row in dicoCcyIds)
            {
                if (row.Key.ToUpper() == currency.ToUpper())
                {
                    ccyId = row.Value;
                }
            }

            api = "https://api.coinbase.com/v2/accounts/" + ccyId + "/addresses";

            this.response = this.coinbase.SendRequest(api, "", RestSharp.Method.POST);

            try
            {
                this.dicoResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.response.Data.ToString());
                oneNewOrder.success = true;
                oneNewOrder.message = this.dicoResponse["success"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                oneNewOrder.success = false;
                oneNewOrder.message = "ERROR";
            }

            return oneNewOrder;
        }


        /// <summary>
        ///     Function that return order ID of the specified <paramref name="currency"/>
        ///     order, previsouly placed on coinbase markets
        ///     <paramref name="currency"/> have to be in <see cref="MainCryptos"/>
        /// </summary>
        /// <param name="currency">the name of the cryptocurrency.</param>
        /// <returns>
        ///     the ID of the order, if it exists, else, returns ERROR
        /// </returns>
        override public string GetOrderId(string currency)
        {
            string ccyId = "";
            string orderId;
            List<Order> listOpenOrders;
            string api = "https://api.coinbase.com/v2/accounts/:account_id/transactions";

            listOpenOrders = this.GetOpenOrders(currency);

            foreach (Order oneOrder in listOpenOrders)
            {
                if (!(oneOrder.message == "ERROR"))
                {
                    if (oneOrder.ccyBase.ToString().ToUpper() == currency.ToUpper())
                    {
                        ccyId = oneOrder.ID;
                    }
                } 
            }

            if (ccyId == "")
            {
                ccyId = "ERROR";
            }

            api = "https://api.coinbase.com/v2/accounts/" + ccyId + "/addresses";

            this.response = this.coinbase.SendRequest(api, "", RestSharp.Method.GET);

            try
            {
                this.dicoResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.response.Data.ToString());
                orderId = this.dicoResponse["order id"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
            Models.OrderType oneOrderType;
            Models.OrderStyle oneOrderStyle = Models.OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.CEXIO;
            MainCryptos ccyBase;
            MainCryptos ccyPair;
            double amount;
            double orderPrice;
            List<Order> orderList = new List<Order>();
            Order oneNewOrder;

            string ccyId = "";
            string api = "https://api.coinbase.com/v2/accounts/:account_id/transactions";

            Dictionary<string, string> accountId = this.GetAccountId();

            foreach (KeyValuePair<string, string> row in accountId)
            {
                if (row.Key.ToUpper() == currency.ToString())
                {
                    ccyId = row.Value;
                }
            }

            api = "https://api.coinbase.com/v2/accounts/" + ccyId + "/transactions";

            this.response = this.coinbase.SendRequest(api, "", RestSharp.Method.GET);

            try
            {
                if (this.response.Errors == null && this.response.Data.ToString() != "[]")
                {
                    this.dicoResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.response.Data.ToString());    
                }
                else
                {
                    this.dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "NO OPEN ORDERS" } };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
            }

            return orderList;
        }


        public (string, string) GetPaymentMethod(string currency)
        {
            string api = "https://api.coinbase.com/v2/payment-methods";
            List<Dictionary<dynamic, dynamic>> listDico = new List<Dictionary<dynamic, dynamic>>();

            this.response = this.coinbase.SendRequest(api, "", RestSharp.Method.GET);

            try
            {
                if (this.response.Data.ToString() != "[]")
                {
                    listDico = JsonConvert.DeserializeObject<List<Dictionary<dynamic, dynamic>>>(this.response.Data.ToString());
                }
                else
                {
                    this.dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "NO OPEN ORDERS" } };
                    listDico.Add(this.dicoResponse);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
                listDico.Add(this.dicoResponse);
            }

            foreach (Dictionary<dynamic, dynamic> row in listDico)
            {
                if (row["currency"] == currency)
                {
                    return ("true", row["id"]);
                }
            }

            return ("false", "Error while getting payment methods");
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
            Models.OrderType oneOrderType = Models.OrderType.SEND;
            Models.OrderStyle oneOrderStyle = Models.OrderStyle.WITHDRAWAL;
            Exchanges oneExchange = Exchanges.COINBASE;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair, amount, 0);
            string ccyId = "";
            string api = "https://api.coinbase.com/v2/accounts/:account_id/transactions";
            Dictionary<string, string> body = new Dictionary<string, string>();

            Dictionary<string, string> accountId = this.GetAccountId();

            foreach (KeyValuePair<string, string> row in accountId)
            {
                if (row.Key.ToUpper() == currency.ToString())
                {
                    ccyId = row.Value;
                }
            }

            api = "https://api.coinbase.com/v2/accounts/" + ccyId + "/transactions";

            body.Add("type", "send");
            body.Add("to", adress);
            body.Add("amount", amount.ToString());
            body.Add("currency", currency);

            this.response = this.coinbase.SendRequest(api, body, RestSharp.Method.POST);

            try
            {
                if (this.response.Data.ToString() != "[]")
                {
                    this.dicoResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.response.Data.ToString());
                    oneNewOrder.success = true;
                    oneNewOrder.message = this.dicoResponse["message"];
                }
                else
                {
                    this.dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR WHILE SENDING CRYPTOS" } };
                    oneNewOrder.success = false;
                    oneNewOrder.message = "ERROR WHILE SENDING CRYPTOS";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                if (this.response != null)
                {
                    this.dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", this.response.Errors[0].Message } };
                    oneNewOrder.success = false;
                    oneNewOrder.message = this.response.Errors[0].Message;

                    return oneNewOrder;
                }
                else
                {
                    this.dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "This currency isn't handled on Coinbase" } };
                    oneNewOrder.success = false;
                    oneNewOrder.message = "This crypto isn't handled on Coinbase";

                    return oneNewOrder;
                }
            }

            return oneNewOrder;
        }


        public (string, string) Withdrawal(string adress, string currency, double amount)
        {
            string ccyId = "";
            string api = "https://api.coinbase.com/v2/accounts/:account_id/withdrawals";
            Dictionary<string, string> body = new Dictionary<string, string>();
            string paymentMethod, sucess;

            Dictionary<string, string> accountId = this.GetAccountId();

            foreach (KeyValuePair<string, string> row in accountId)
            {
                if (row.Key.ToUpper() == currency.ToString())
                {
                    ccyId = row.Value;
                }
            }

            (paymentMethod, sucess) = this.GetPaymentMethod("EUR");

            api = "https://api.coinbase.com/v2/accounts/" + ccyId + "/withdrawals";

            body.Add("currency", currency);
            body.Add("amount", amount.ToString());
            body.Add("payment_method", paymentMethod);

            this.response = this.coinbase.SendRequest(api, "", RestSharp.Method.POST);

            try
            {
                if (this.response.Data.ToString() != "[]")
                {
                    this.dicoResponse = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(this.response.Data.ToString());
                }
                else
                {
                    this.dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "NO OPEN ORDERS" } };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.dicoResponse = new Dictionary<dynamic, dynamic>() { { "ERROR", "ERROR" } };
            }

            return ("", "");
        }
    }
}
