using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Crypto_Trader_V1.ExchangesModels.My_Bittrex;
using Crypto_Trader_V1.ExchangesModels.My_Binance;
using Crypto_Trader_V1.ExchangesModels.My_Bitfinex;
using Crypto_Trader_V1.ExchangesModels.My_CexIO;
using Crypto_Trader_V1.ExchangesModels.My_Coinbase;


namespace Crypto_Trader_V1.Models.My_CryptoTrader
{


    /// <summary>
    ///     Class representing A Crypto Trader :
    ///         It can select best price, all prices, on platform it handles
    ///         It can compute some statistics on crypto currencies
    ///         It can get and export some historical datas on crypto currencies
    ///         It also bind each handled exchanges with their most used functions
    ///         (GetPrice, Buy/Sell, etc)
    ///         It also check arbitrage opportunities
    /// </summary>
    /// <remarks>
    ///     It use a bit Crypto Compare REST API : https://www.cryptocompare.com/api/.
    ///     Crypto Compare isn't an exchange. It's just a tool for doing analysis 
    ///     and getting market data on crypto currencies
    /// </remarks>
    public class My_CryptoTrader : ICryptoTrader
    {
        /// <summary>
        ///     Dictionary binding Exchanges and their GetPrice function
        /// </summary>
        public Dictionary<Exchanges, Func<string, Price>> Exchange2GetPrice;

        /// <summary>
        ///     Dictionary binding Exchanges and their Buy function
        /// </summary>
        public Dictionary<Exchanges, Func<string, double, double, Order>> Exchange2Buy;

        /// <summary>
        ///     Dictionary binding Exchanges and their Sell function
        /// </summary>
        public Dictionary<Exchanges, Func<string, double, double, Order>> Exchange2Sell;

        /// <summary>
        ///     Dictionary binding Exchanges and their GetAdress function
        /// </summary>
        public Dictionary<Exchanges, Func<string, string>> Exchange2GetAdress;

        /// <summary>
        ///     Dictionary binding Exchanges and their GetFolio function
        /// </summary>
        public Dictionary<Exchanges, Func<Portfolio>> Exchange2GetFolio;

        /// <summary>
        ///     Dictionary binding Exchanges and their GetAccountInfos function
        /// </summary>
        public Dictionary<Exchanges, Func<Dictionary<string, string>>> Exchange2GetAccountInfos;

        /// <summary>
        ///     Dictionary binding Exchanges and their CancelOrder function
        /// </summary>
        public Dictionary<Exchanges, Func<string, string, Order>> Exchange2CancelOrder;

        /// <summary>
        ///     Dictionary binding Exchanges and their GetOrderId function
        /// </summary>
        public Dictionary<Exchanges, Func<string, string>> Exchange2GetOrderId;

        /// <summary>
        ///     Dictionary binding Exchanges and their GetOpenOrders function
        /// </summary>
        public Dictionary<Exchanges, Func<string, List<Order>>> Exchange2GetOpenOrders;

        /// <summary>
        ///     Dictionary binding Exchanges and their SendCryptos function
        /// </summary>
        public Dictionary<Exchanges, Func<string, string, double, Order>> Exchange2SendCryptos;

        public My_CryptoTrader()
        {
            this.Exchange2GetPrice = new Dictionary<Exchanges, Func<string, Price>>{
            {Exchanges.BITTREX, (new My_Bittrex()).GetPrice},
            {Exchanges.BINANCE, (new My_Binance()).GetPrice},
            {Exchanges.BITFINEX, (new My_Bitfinex()).GetPrice},
            {Exchanges.COINBASE, (new My_Coinbase()).GetPrice},
            {Exchanges.CEXIO, (new My_CexIO()).GetPrice},
            };

            this.Exchange2Buy = new Dictionary<Exchanges, Func<string, double, double, Order>>{
            {Exchanges.BITTREX, (new My_Bittrex()).Buy},
            {Exchanges.BINANCE, (new My_Binance()).Buy},
            {Exchanges.BITFINEX, (new My_Bitfinex()).Buy},
            {Exchanges.COINBASE, (new My_Coinbase()).Buy},
            {Exchanges.CEXIO, (new My_CexIO()).Buy},
            };
                
            this.Exchange2Sell = new Dictionary<Exchanges, Func<string, double, double, Order>>{
            {Exchanges.BITTREX, (new My_Bittrex()).Sell},
            {Exchanges.BINANCE, (new My_Binance()).Sell},
            {Exchanges.BITFINEX, (new My_Bitfinex()).Sell},
            {Exchanges.COINBASE, (new My_Coinbase()).Sell},
            {Exchanges.CEXIO, (new My_CexIO()).Sell},
            };

            this.Exchange2GetAdress = new Dictionary<Exchanges, Func<string, string>>{
            {Exchanges.BITTREX, (new My_Bittrex()).GetAdress},
            {Exchanges.BINANCE, (new My_Binance()).GetAdress},
            {Exchanges.BITFINEX, (new My_Bitfinex()).GetAdress},
            {Exchanges.COINBASE, (new My_Coinbase()).GetAdress},
            {Exchanges.CEXIO, (new My_CexIO()).GetAdress},
            };

            this.Exchange2GetFolio = new Dictionary<Exchanges, Func<Portfolio>>{
            {Exchanges.BITTREX, (new My_Bittrex()).GetFolio},
            {Exchanges.BINANCE, (new My_Binance()).GetFolio},
            {Exchanges.BITFINEX, (new My_Bitfinex()).GetFolio},
            {Exchanges.COINBASE, (new My_Coinbase()).GetFolio},
            {Exchanges.CEXIO, (new My_CexIO()).GetFolio},
            };

            this.Exchange2GetAccountInfos = new Dictionary<Exchanges, Func<Dictionary<string, string>>>{
            {Exchanges.BITTREX, (new My_Bittrex()).GetAccountInfos},
            {Exchanges.BINANCE, (new My_Binance()).GetAccountInfos},
            {Exchanges.BITFINEX, (new My_Bitfinex()).GetAccountInfos},
            {Exchanges.COINBASE, (new My_Coinbase()).GetAccountInfos},
            {Exchanges.CEXIO, (new My_CexIO()).GetAccountInfos},
            };

            this.Exchange2CancelOrder = new Dictionary<Exchanges, Func<string, string, Order>>{
            {Exchanges.BITTREX, (new My_Bittrex()).CancelOrder},
            {Exchanges.BINANCE, (new My_Binance()).CancelOrder},
            {Exchanges.BITFINEX, (new My_Bitfinex()).CancelOrder},
            {Exchanges.COINBASE, (new My_Coinbase()).CancelOrder},
            {Exchanges.CEXIO, (new My_CexIO()).CancelOrder},
            };

            this.Exchange2GetOrderId = new Dictionary<Exchanges, Func<string, string>>{
            {Exchanges.BITTREX, (new My_Bittrex()).GetOrderId},
            {Exchanges.BINANCE, (new My_Binance()).GetOrderId},
            {Exchanges.BITFINEX, (new My_Bitfinex()).GetOrderId},
            {Exchanges.COINBASE, (new My_Coinbase()).GetOrderId},
            {Exchanges.CEXIO, (new My_CexIO()).GetOrderId},
            };

            this.Exchange2GetOpenOrders = new Dictionary<Exchanges, Func<string, List<Order>>>{
            {Exchanges.BITTREX, (new My_Bittrex()).GetOpenOrders},
            {Exchanges.BINANCE, (new My_Binance()).GetOpenOrders},
            {Exchanges.BITFINEX, (new My_Bitfinex()).GetOpenOrders},
            {Exchanges.COINBASE, (new My_Coinbase()).GetOpenOrders},
            {Exchanges.CEXIO, (new My_CexIO()).GetOpenOrders},
            };

            this.Exchange2SendCryptos = new Dictionary<Exchanges, Func<string, string, double, Order>>{
            {Exchanges.BITTREX, (new My_Bittrex()).SendCryptos},
            {Exchanges.BINANCE, (new My_Binance()).SendCryptos},
            {Exchanges.BITFINEX, (new My_Bitfinex()).SendCryptos},
            {Exchanges.COINBASE, (new My_Coinbase()).SendCryptos},
            {Exchanges.CEXIO, (new My_CexIO()).SendCryptos},
            };
        }


        /// <summary>
        ///     Function that return historical price (high, low, close) for the pair
        ///     <paramref name="ccyBase"/> VS <paramref name="ccyPair"/>.
        ///     The time series is long by <paramref name="nbDays"/> days
        /// </summary>
        /// <param name="ccyBase">the name of the base cryptocurrency.</param>
        /// <param name="ccyPair">the name of the paire cryptocurrency.</param>
        /// <param name="nbDays">the length of the history.</param>
        /// <returns>
        ///     A tuple containing, in this order, the binary sucess (true or false),
        ///     and eventually a message explaining why the order failed
        /// </returns>
        /// <example>
        ///     in BTC-EUR price :
        ///     BTC is the base currency
        ///     EUR is the paire currency
        /// </example>
        public Dictionary<string, Dictionary<string, string>> GetHistoricalData(string ccyBase, string ccyPair, int nbDays)
        {
            string apiUrl = "https://min-api.cryptocompare.com/data/histoday";
            HttpClient httpClient = new System.Net.Http.HttpClient();
            HttpRequestMessage request = new System.Net.Http.HttpRequestMessage();
            HttpResponseMessage response = new System.Net.Http.HttpResponseMessage();
            Dictionary<string, string> dicoResults = new Dictionary<string, string>();
            Dictionary<string, Dictionary<string, string>> dicoAllResults = new Dictionary<string, Dictionary<string, string>>();
            string jsonResponse = "";
            string limit = nbDays.ToString();
            string aggregate = 1.ToString();
            string codeExchange = "CCCAGG";
            string completeURL = "";
            string[] splitStr;
            string[] jsons;
            string goodString;
            DateTimeOffset goodUKDate;
            string sGoodDate;

            completeURL += apiUrl + "?";
            completeURL += "fsym=" + ccyBase + "&";
            completeURL += "tsym=" + ccyPair + "&";
            completeURL += "limit=" + limit + "&";
            completeURL += "aggregate=" + aggregate + "&";
            completeURL += "e=" + codeExchange;

            try
            {
                request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, completeURL);

                response = httpClient.SendAsync(request).Result;
                jsonResponse = response.Content.ReadAsStringAsync().Result;
                splitStr = jsonResponse.Split('[');
                jsons = splitStr[1].Split('}');

                for (int i = 0; i <= Int32.Parse(limit); i++)
                {
                    dicoResults = new Dictionary<string, string>();
                    goodString = jsons[i] + "}";

                    if (i >= 1)
                    {
                        goodString = goodString.Substring(1);
                    }

                    JsonConvert.PopulateObject(goodString, dicoResults);
                    goodUKDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(dicoResults["time"]));
                    sGoodDate = goodUKDate.ToString("dd/MM/yyyy");
                    dicoAllResults.Add(sGoodDate, dicoResults);
                }   
            }
            catch (Exception exc)
            {
                dicoResults = new Dictionary<string, string>(){{"close", "ERROR"}};
                dicoAllResults.Add(DateTime.Now.ToString("dd/MM/yyyy"), dicoResults);
                Console.WriteLine(exc.Message);
            }

            return dicoAllResults;
        }


        /// <summary>
        ///     Function that export historical price (high, low, close) for the pair
        ///     <paramref name="ccyBase"/> VS <paramref name="ccyPair"/>.
        ///     It export it in a SQLite database, located in the the Project Folder, 
        ///     in the ../static/databases folder
        /// </summary>
        /// <param name="ccyBase">the name of the base cryptocurrency.</param>
        /// <param name="ccyPair">the name of the paire cryptocurrency.</param>
        /// <param name="nbDays">the length of the history.</param>
        /// <returns>
        ///     A tuple containing, in this order, the binary sucess (true or false),
        ///     and eventually a message explaining why the order failed
        /// </returns>
        /// <example>
        ///     in BTC-EUR price :
        ///     BTC is the base currency
        ///     EUR is the paire currency
        /// </example>
        public (string, int) ExportHistoricalData(string ccyBase, string ccyPair, int nbDays)
        {
            System.Data.SQLite.SQLiteConnection mainConnexion = new System.Data.SQLite.SQLiteConnection();
            System.Data.SQLite.SQLiteCommand mainRequest;
            string goodDate = DateTime.Now.ToString("dd-MM-yyy__HHmmss");
            string dbName = "DataBase_" + ccyBase + "_" + ccyPair + "_" + nbDays + "_Days_" + goodDate + ".sqlite";
            string dbPath = Tools.GetDirectory() + "Static/Databases/";
            dbPath += dbName;
            string tableName = ccyBase + "-" + ccyPair + "-" + nbDays.ToString() + "Days";
            string request = "";
            int response;
            Dictionary<string, Dictionary<string, string>> BTCPrice;
            int i;

            // Création de la DB Sqlite3
            System.Data.SQLite.SQLiteConnection.CreateFile(dbPath);

            // Connection à la BD nouvellement crée
            mainConnexion = new System.Data.SQLite.SQLiteConnection("Data Source=" + dbPath + ";Version=3");
            mainConnexion.Open();

            // Création d'un nouvelle table
            request = "CREATE TABLE [" + tableName + "] (";
            request += "id INTEGER PRIMARY KEY AUTOINCREMENT,";
            request += "date VARCHAR(100),";
            request += "open DOUBLE,";
            request += "high DOUBLE,";
            request += "low DOUBLE,";
            request += "close DOUBLE,";
            request += "volume INT";
            request += ");";

            mainRequest = new System.Data.SQLite.SQLiteCommand(request, mainConnexion);
            response = mainRequest.ExecuteNonQuery();

            // Historique des prix 
            BTCPrice = GetHistoricalData(ccyBase, ccyPair, nbDays);

            // Export des prix historiques vers la base de données
            i = 1;
            foreach (var price in BTCPrice)
            {
                request = "INSERT INTO [" + tableName + "] VALUES (";
                request += i.ToString() + ",";
                request += "'" + price.Key.ToString() + "',";
                request += price.Value["open"].ToString() + ",";
                request += price.Value["high"].ToString() + ",";
                request += price.Value["low"].ToString() + ",";
                request += price.Value["close"].ToString() + ",";
                request += (Double.Parse(price.Value["volumefrom"], System.Globalization.CultureInfo.InvariantCulture) + Double.Parse(price.Value["volumeto"], System.Globalization.CultureInfo.InvariantCulture)).ToString();
                request += ");";
                mainRequest = new System.Data.SQLite.SQLiteCommand(request, mainConnexion);
                response = mainRequest.ExecuteNonQuery();
                i++;
            }

            mainConnexion.Close();

            return (dbName, response);
        }


        /// <summary>
        ///     Function that return various statistics for <paramref name="ccy"/>, on the specified
        ///     <paramref name="exchange"/>, and computed on a sample
        ///     long of <paramref name="nbDays"/> days.
        /// </summary>
        /// <param name="ccy">the cryptocurrency on which to compute the statistics.</param>
        /// <param name="exchange">the exchange on which we based information (price mostly).</param>
        /// <param name="nbDays">number of days taken in account in the computations.</param>
        /// <returns>
        ///     A tuple containing, in this order, the binary sucess (true or false),
        ///     and eventually a message explaining why the order failed
        /// </returns>
        /// <example>
        ///     the following args : "BTC", "BITTREX", "30"
        ///     are going to return statistics for BTC, computed with 30 days history 
        ///     (30 days rolling mean, standard deviation, etc).
        ///     The price taken into account will be Bittrex ones.
        /// </example>
        public Dictionary<string, double> ComputeStats(string ccy, string exchange, int nbDays)
        {
            Dictionary<string, Dictionary<string, string>> histPrice;
            Price currPrice;
            double[] close = new double[nbDays + 1];
            double sma;
            double ema;
            double mom;
            double ret;
            double std;
            Func<string, Price> func2invoke;
            try
            {
                histPrice = GetHistoricalData(ccy, "USD", nbDays);
                if (System.Linq.Enumerable.Contains(histPrice.Keys, DateTime.Today.ToString("dd/MM/yyyy")))
                {
                    if (histPrice[DateTime.Today.ToString("dd/MM/yyyy")]["close"] == "ERROR")
                    {
                        Console.WriteLine("Error historical price for " + ccy);
                        return new Dictionary<string, double> { { "ERROR", 0 } };
                    }
                }
                else
                {
                    if (histPrice[DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy")]["close"] == "ERROR")
                    {
                        Console.WriteLine("Error historical price for " + ccy);
                        return new Dictionary<string, double> { { "ERROR", 0 } };
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new Dictionary<string, double> { { "ERROR", 0 } };
            }

            int i;
            Dictionary<string, double> dicoStats = new Dictionary<string, double>();

            i = 0;
            foreach (KeyValuePair<string, Dictionary<string, string>> onePrice in histPrice)
            {
                close[i] = Double.Parse(onePrice.Value["close"], System.Globalization.CultureInfo.InvariantCulture);
                i++;
            }

            if (System.Linq.Enumerable.Contains((IEnumerable<string>)Enum.GetNames(typeof(Exchanges)), exchange))
            {
                Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                func2invoke = this.Exchange2GetPrice[goodExchange];
                currPrice = (Price)func2invoke.DynamicInvoke(ccy);
                sma = (MgFx.Indicators.SMA.Calculate(close, nbDays))[nbDays];
                ema = (MgFx.Indicators.EMA.Calculate(close, nbDays))[nbDays];
                mom = (MgFx.Indicators.Momentum.Calculate(close, nbDays))[nbDays];
                ret = (close[close.Length - 1] - close[close.Length - 2]) / close[close.Length - 2];
                std = MathNet.Numerics.Statistics.Statistics.StandardDeviation(close)/sma;

                dicoStats.Add("SMA", sma);
                dicoStats.Add("EMA", ema);
                dicoStats.Add("MOMENTUM", mom);
                dicoStats.Add("RETURN", ret);
                dicoStats.Add("STD", std);
            }
            else
            {
                // ERROR
            }

            return dicoStats;
        }


        /// <summary>
        ///     returns the spot price, for <paramref name="currency"/>, 
        ///     and is computed as the mean price above all crypto exchanges (Crypto Compare API)
        /// </summary>
        /// <param name="currency">
        ///     It have to be in <see cref="MainCryptos"/>
        /// </param>
        public double GetSpotPrice(string currency)
        {
            string today = DateTime.Now.ToString("dd/MM/yyyy");
            Dictionary<string, Dictionary<string, string>> BTCHistoric = this.GetHistoricalData(currency, "USD", 1);
            string sBTCSpot = "";
            double BTCSpot;

            if (System.Linq.Enumerable.Contains(BTCHistoric.Keys, today))
            {
                sBTCSpot = BTCHistoric[today]["close"];
            }
            else
            {
                today = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");
                if (System.Linq.Enumerable.Contains(BTCHistoric.Keys, today))
                {
                    sBTCSpot = BTCHistoric[today]["close"];
                }
                else
                {
                    today = DateTime.Now.AddDays(-2).ToString("dd/MM/yyyy");
                    if (System.Linq.Enumerable.Contains(BTCHistoric.Keys, today))
                    {
                        sBTCSpot = BTCHistoric[today]["close"];
                    }
                    else
                    {
                        sBTCSpot = "0";
                        Console.WriteLine("Error getting BTC Spot");
                    }
                }
            }

            BTCSpot = Double.Parse(sBTCSpot, System.Globalization.CultureInfo.InvariantCulture);

            return BTCSpot;
        }


        /// <summary>
        ///     returns the best price, for a <paramref name="orderType"/> on <paramref name="currency"/>
        ///     (Smaller price for BUY and higher price for SELL), and according to all handled exchanges
        ///     (ie <see cref="Exchanges"/>)
        /// </summary>
        /// <param name="currency">
        ///     It have to be in <see cref="MainCryptos"/>
        /// </param>
        /// <param name="orderType">
        ///     It have to be in <see cref="OrderType"/>
        /// </param>
        public Price GetBestPrice(string currency, string orderType, CancellationToken ct)
        {
            double resultPrice = 0.00;
            string resultXchange = "";
            Dictionary<Exchanges, Task> allTasks = new Dictionary<Exchanges, Task>();
            Dictionary<string, Price> allPrices = new Dictionary<string, Price>();
            SortedDictionary<string, Price> dicoResult = new SortedDictionary<string, Price>();
            Price cryptoPrice = new Price();
            Dictionary<Exchanges, Func<string, Price>> getprice2invoke = new Dictionary<Exchanges, Func<string, Price>>();

            foreach (string exchange in Enum.GetNames(typeof(Exchanges)))
            {
                Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                if (this.Exchange2GetPrice.ContainsKey(goodExchange))
                {
                    getprice2invoke.Add(goodExchange, this.Exchange2GetPrice[goodExchange]);
                    allTasks.Add(goodExchange, new Task(() => allPrices.Add(exchange, (Price)getprice2invoke[goodExchange].DynamicInvoke(currency))));
                }

                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }

            foreach (Task task in allTasks.Values)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                task.Start();
            }

            foreach (Task task in allTasks.Values)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                task.Wait();
            }

            foreach (var price in allPrices)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                dicoResult.Add(price.Key, price.Value);
            }

            if (orderType == OrderType.BUY.ToString())
            {
                resultPrice = 1000000000.00;
                foreach (Price onePrice in allPrices.Values)
                {
                    if (onePrice.error != true)
                    {
                        resultPrice = Math.Min(resultPrice, onePrice.price);
                    }

                    if (resultPrice == onePrice.price)
                    {
                        resultXchange = onePrice.exchange;
                        cryptoPrice = onePrice;
                    }

                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                }

                if (cryptoPrice.error == true)
                {
                    resultXchange = "REQUEST ERROR";
                }
            }
            else if (orderType == OrderType.SELL.ToString())
            {
                resultPrice = 0.00;
                foreach (Price onePrice in allPrices.Values)
                {
                    if (onePrice.error != true)
                    {
                        resultPrice = Math.Max(resultPrice, onePrice.price);
                    }

                    if (resultPrice == onePrice.price)
                    {
                        resultXchange = onePrice.exchange;
                        cryptoPrice = onePrice;
                    }

                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                }

                if (cryptoPrice.error == true)
                {
                    resultXchange = "REQUEST ERROR";
                }
            }
            else
            {
                resultPrice = 0;
                resultXchange = "ERROR : Request Failed\nCHOOSE BETWEEN 'SELL' or 'BUY'";
                cryptoPrice = new Price(error: true);
            }

            return cryptoPrice;
        }


        /// <summary>
        ///     returns a dictionary containing all prices with their market & USD values
        /// </summary>
        /// <param name="currency">
        ///     It have to be in <see cref="MainCryptos"/>
        /// </param>
        public SortedDictionary<string, Price> GetAllPrices(string currency, CancellationToken ct)
        {
            Dictionary<Exchanges, Task> allTasks = new Dictionary<Exchanges, Task>();
            Dictionary<string, Price> allPrices = new Dictionary<string, Price>();
            SortedDictionary<string, Price> dicoResult = new SortedDictionary<string, Price>();
            Price cryptoPrice = new Price();
            Dictionary<Exchanges, Func<string, Price>> getprice2invoke = new Dictionary<Exchanges, Func<string, Price>>();

            foreach (string exchange in Enum.GetNames(typeof(Exchanges)))
            {
                Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                if (this.Exchange2GetPrice.ContainsKey(goodExchange))
                {
                    getprice2invoke.Add(goodExchange, this.Exchange2GetPrice[goodExchange]);
                    allTasks.Add(goodExchange, new Task(() => allPrices.Add(exchange, (Price)getprice2invoke[goodExchange].DynamicInvoke(currency))));
                }

                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }

            foreach (Task task in allTasks.Values)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                task.Start();
            }

            foreach (Task task in allTasks.Values)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                task.Wait();
            }

            foreach (var price in allPrices)
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                dicoResult.Add(price.Key, price.Value);
            }

            return dicoResult;
        }


        /// <summary>
        ///     Function that check arbitrage, one the same <paramref name="ccy1"/>
        ///     , between all handled exchanges.
        ///     It checks if there's a free opportunity to make profit, by simply sending 
        ///     the crypto currency to an exchange (paying some fees), and selling it on the
        ///     other exchange.
        ///     Thus, it checks if the price + the fees are inferior to the price on the other exchange
        /// </summary>
        public bool CheckPlatformArbitrage(string ccy1)
        {
            My_Bittrex bittrex = new My_Bittrex();
            My_Bitfinex bitfinex = new My_Bitfinex();
            My_Binance binance = new My_Binance();
            My_CexIO cexio = new My_CexIO();
            My_Coinbase coinbase = new My_Coinbase();
            Dictionary<Exchanges, Price> handledExchanges = new Dictionary<Exchanges, Price>();
            Dictionary<Exchanges, double> handledExchangesUSD = new Dictionary<Exchanges, double>();
            Dictionary<Exchanges, Price> unhandledExchanges = new Dictionary<Exchanges, Price>();
            Dictionary<Exchanges, Price> allPrices = new Dictionary<Exchanges, Price>();
            Dictionary<string, double> allUSDPrices = new Dictionary<string, double>();
            Dictionary<string, Task> allTasks = new Dictionary<string, Task>();
            double fees = 0.01;
            double bpToAct = 0.02;
            Dictionary<Exchanges, Func<string, Price>> function2invoke = new Dictionary<Exchanges, Func<string, Price>>();

            foreach (var exchange in Enum.GetNames(typeof(Exchanges)))
            {
                Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                if (this.Exchange2GetPrice.ContainsKey(goodExchange))
                {
                    function2invoke.Add(goodExchange, this.Exchange2GetPrice[goodExchange]);
                    allTasks.Add(exchange, new Task(() => allPrices.Add(goodExchange, (Price)function2invoke[goodExchange].DynamicInvoke(ccy1))));
                }
            }

            foreach (Task task in allTasks.Values)
            {
                task.Start();
                System.Threading.Thread.Sleep(400);
            }

            foreach (Task task in allTasks.Values)
            {
                task.Wait();
                System.Threading.Thread.Sleep(400);
            }

            System.Threading.Thread.Sleep(500);

            foreach (Price price in allPrices.Values)
            {
                if (price.ccyPair != MainCurrency.USD.ToString() && price.ccyPair != MainCryptos.USDT.ToString())
                {
                    if (price.ccyPair == MainCryptos.BTC.ToString())
                    {
                        Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), price.exchange);
                        var getprice2invoke = this.Exchange2GetPrice[goodExchange];
                        Price BTCprice = (Price)getprice2invoke.DynamicInvoke("BTC");
                        allUSDPrices.Add(price.exchange, price.price * BTCprice.price);
                    }
                    if (price.ccyPair == "ERROR")
                    {
                        allUSDPrices.Add(price.exchange, price.price);
                    }
                }
                else
                {
                    allUSDPrices.Add(price.exchange, price.price);
                }
            }

            foreach (var price in allPrices.Values)
            {
                if (price.error != true)
                {
                    handledExchanges.Add((Exchanges)Enum.Parse(typeof(Exchanges), price.exchange), price);
                    handledExchangesUSD.Add((Exchanges)Enum.Parse(typeof(Exchanges), price.exchange), allUSDPrices[price.exchange]);
                }
                else
                {
                    unhandledExchanges.Add((Exchanges)Enum.Parse(typeof(Exchanges), price.exchange), price);
                }
            }

            double minPrice = 10000000.00;
            double maxPrice = 0.00;
            foreach (var price in handledExchanges.Values)
            {
                minPrice = Math.Min(minPrice, price.price);
                maxPrice = Math.Max(maxPrice, price.price);
            }

            minPrice = 10000000.00;
            maxPrice = 0.00;
            foreach (var price in handledExchangesUSD.Values)
            {
                minPrice = Math.Min(minPrice, price);
                maxPrice = Math.Max(maxPrice, price);
            }

            if (maxPrice >= (1 + fees + bpToAct) * minPrice || minPrice <= (1 - fees - bpToAct) * maxPrice)
            {
                return true;
            }
            else if (maxPrice == minPrice)
            {
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
