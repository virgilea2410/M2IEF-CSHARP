using System;
using System.Collections.Generic;
using System.Threading;
using Gtk;
using Crypto_Trader_V1.Models;
using Crypto_Trader_V1.Models.My_CryptoTrader;


namespace Crypto_Trader_V1.ApplicationForm
{


    /// <summary>
    ///     Class representing the GUI Application
    /// </summary>
    /// <remarks>
    ///     The entire class is composed by 3 partial class :
    ///         - One partial class for Attributes definitions
    ///         - One partial class for Intialisation of those attributes
    ///         - One partial class for definition of the application methods
    /// </remarks>
    public partial class ApplicationForm : Gtk.Window
    {
        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Open Orders view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_openOrders"/> is clicked
        /// </summary>
        public void ViewOpenOrders(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.labelFrame.Label = "OPEN ORDERS";

            this.hBoxOpenOrdersEntry.Show();
            this.hBoxOpenOrdersButton.Show();
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function request a cancel order for <paramref name="currency"/> 
        ///     order <paramref name="orderId"/> to the right <paramref name="exchange"/>
        /// </summary>
        /// <param name="currency">
        ///     Is selected by the user from <see cref="comboBoxCancelOrderCryptos"/>.
        ///     It have to be in <see cref="MainCryptos"/>
        /// </param>
        /// <param name="exchange">
        ///     Is selected by the user from <see cref="comboBoxCancelOrderExchanges"/>.
        ///     It have to be in <see cref="Exchanges"/>
        /// </param>
        /// <param name="orderId">
        ///     Is selected by the user from <see cref="entryCancelOrderId"/>.
        /// </param>
        public void DoCancelOrder(string currency, string exchange, string orderId)
        {
            //string sucess;
            Func<string, string, Order> goodCancelOrder;
            Order oneOrder;

            Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
            goodCancelOrder = this.cryptoTrader.Exchange2CancelOrder[goodExchange];
            oneOrder = (Order)goodCancelOrder.DynamicInvoke(currency, orderId);

            System.Threading.Thread.Sleep(200);

            this.labelCol1.Text = "\n";
            this.labelCol2.Text = "\n";
            System.Threading.Thread.Sleep(200);
            this.labelCol3.Text = "Sucess Cancel Order :\n";
            System.Threading.Thread.Sleep(200);
            this.labelCol4.Text = "\n";
            this.labelCol5.Text = "\n";

            System.Threading.Thread.Sleep(200);

            this.labelCol1.Text += "\n";
            this.labelCol2.Text += "\n";
            System.Threading.Thread.Sleep(200);
            this.labelCol3.Text += oneOrder.success + "\n";
            System.Threading.Thread.Sleep(200);
            this.labelCol4.Text += "\n";
            this.labelCol5.Text += "\n";

            this.workIsFinished = true;
        }


        /// <summary>
        ///     Set Functions are used to automatically set use entries,
        ///     Such as maximum amount to send in consideration of a specific portfolio,
        ///     Or an order id
        ///     This function automatically set the good order ID, if it exists
        ///     on the <see cref="ViewCancelOrder"/> view, into the <see cref="entryCancelOrderId"/> widget
        /// </summary>
        public void SetCancelOrderId(object sender, EventArgs e)
        {
            string exchange;
            string currency = "";
            string orderId;
            Func<string, string> goodGetOrderId;

            this.labelCol3.Text = "";

            try
            {
                this.comboBoxCancelOrderExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxCancelOrderExchanges.Model.GetValue(this.tree, 0).ToString();
                this.comboBoxCancelOrderCryptos.GetActiveIter(out this.tree);
                currency = this.comboBoxCancelOrderCryptos.Model.GetValue(this.tree, 0).ToString();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                exchange = "ERROR";
            }

            if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(Exchanges)), exchange) &&
                System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), currency))
            {
                Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                goodGetOrderId = this.cryptoTrader.Exchange2GetOrderId[goodExchange];
                orderId = (string)goodGetOrderId.DynamicInvoke(currency);

                if (orderId != "" && orderId != "ERROR")
                {
                    this.entryCancelOrderId.Text = orderId;    
                }
                else
                {
                    this.entryCancelOrderId.Text = "ERROR";
                    this.labelCol3.Text = "You don't have any " + currency + " open orders on " + exchange;
                }
            }
            else
            {
                this.labelCol3.Text = "Please Select an Exchange and a Currency";
            }
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Cancel Order view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_cancelOrder"/> is clicked
        /// </summary>
        public void ViewCancelOrder(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.labelFrame.Label = "CANCEL ORDER";

            this.hBoxCancelOrderEntry.Show();
            this.hBoxCancelOrderButton.Show();
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function displays account informations on the GUI, for a specific exchange
        /// </summary>
        public void DoAccountInfos(object sender, EventArgs e)
        {
            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";
            this.labelCol5.Text = "";

            string exchange;
            Exchanges goodExchange;
            Func<Dictionary<string, string>> accountInfos2Call;
            Dictionary<string, string> dicoAccountInfos;

            try
            {
                this.comboBoxAccountInfosExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxAccountInfosExchanges.Model.GetValue(this.tree, 0).ToString();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                exchange = "ERROR";
            }

            if (System.Linq.Enumerable.Contains(
            Enum.GetNames(typeof(Exchanges)), exchange))
            {
                goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                accountInfos2Call = this.cryptoTrader.Exchange2GetAccountInfos[goodExchange];
                dicoAccountInfos = (Dictionary<string, string>)accountInfos2Call.DynamicInvoke();
            }
            else
            {
                dicoAccountInfos = new Dictionary<string, string>();
                this.labelCol3.Text = "Please entre an exchange";
            }

            System.Threading.Thread.Sleep(400);

            foreach(KeyValuePair<string, string> row in dicoAccountInfos)
            {
                this.labelCol1.Text += "\n\n";
                this.labelCol2.Text += "\n\n";
                this.labelCol3.Text += row.Key.ToUpper() + "\n" + row.Value + "\n";
                this.labelCol4.Text += "\n\n";
                this.labelCol5.Text += "\n\n";

                System.Threading.Thread.Sleep(400);
            }

        }


        /// <summary>
        ///     Set Functions are used to automatically set use entries,
        ///     Such as maximum amount to send in consideration of a specific portfolio,
        ///     Or an order id
        ///     This function automatically set the good adress to send  crypto to,
        ///     for a specific exchange, on the <see cref="ViewSendNReceive"/> view
        ///     into the <see cref="entrySendAdress"/> widget
        /// </summary>
        public void SetSendAdress(object sender, EventArgs e)
        {
            string adress;
            string exchange;
            string currency;

            try
            {
                this.comboBoxSendExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxSendExchanges.Model.GetValue(this.tree, 0).ToString();
                this.comboSendCryptos.GetActiveIter(out this.tree);
                currency = this.comboSendCryptos.Model.GetValue(this.tree, 0).ToString();                
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                exchange = "ERROR";
                currency = "ERROR";
            }

            if (exchange != "ERROR" &&
                System.Linq.Enumerable.Contains(Enum.GetNames(typeof(Exchanges)), exchange))
            {
                Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                Func<string, string> goodGetAdress = this.cryptoTrader.Exchange2GetAdress[goodExchange];
                adress = goodGetAdress.DynamicInvoke(currency).ToString();

                if (adress != "")
                {
                    this.labelCol3.Text = "";    
                }
                else
                {
                    this.labelCol3.Text = "Error while getting adress";    
                }
            }
            else
            {
                adress = "ERROR";
                this.labelCol3.Text = "Please set a Currency & an Exchange";
            }

            this.entrySendAdress.Text = adress;
        }


        /// <summary>
        ///     Set Functions are used to automatically set use entries,
        ///     Such as maximum amount to send in consideration of a specific portfolio,
        ///     Or an order id
        ///     This function set the <see cref="withStats"/> bool, in order
        ///     to know if price have to be displayed with or without
        ///     basic statistics, on the <see cref="ViewOnePrice"/> view
        /// </summary>
        public void SetPriceStats(object sender, EventArgs e)
        {
            string sWithStats = this.buttonPriceStats.Label;

            if (sWithStats == "With Stats")
            {
                this.withStats = true;
                this.buttonPriceStats.Label = "Without Stats";
                this.buttonPriceStats.ModifyBg(StateType.Normal, this.redColorStrong);
                this.buttonPriceStats.ModifyBg(StateType.Prelight, this.redColorLight);
            }
            else if (sWithStats == "Without Stats")
            {
                this.withStats = false;
                this.buttonPriceStats.Label = "With Stats";
                this.buttonPriceStats.ModifyBg(StateType.Normal, this.yellowColorStrong);
                this.buttonPriceStats.ModifyBg(StateType.Prelight, this.yellowColorLight);
            }
        }


        /// <summary>
        ///     Set Functions are used to automatically set use entries,
        ///     Such as maximum amount to send in consideration of a specific portfolio,
        ///     Or an order id
        ///     This function automatically set Max amount to sell, by checking the available
        ///     amount on a specific portfolio exchange.
        ///     it then displays it, if it exists, this amout, on the <see cref="ViewSellNBuy"/>
        ///     into the <see cref="entryOrderAmount"/> widget
        /// </summary>
        void SetOrderMax(object sender, EventArgs e)
        {
            string ccy = "";
            string exchange = "";
            Exchanges goodExchange;
            Func<Portfolio> getFolio2Invoke;
            Portfolio oneFolio;

            try
            {
                this.comboBoxOrderCryptos.GetActiveIter(out this.tree);
                ccy = this.comboBoxOrderCryptos.Model.GetValue(this.tree, 0).ToString();

                this.comboBoxOrderExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxOrderExchanges.Model.GetValue(this.tree, 0).ToString();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            double totalCoins = 0.00;
            bool sucess = false;

            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";

            if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), ccy) &&
                System.Linq.Enumerable.Contains(Enum.GetNames(typeof(Exchanges)), exchange))
            {
                goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                getFolio2Invoke = this.cryptoTrader.Exchange2GetFolio[goodExchange];

                oneFolio = (Portfolio)getFolio2Invoke.DynamicInvoke();

                foreach(Balance oneBalance in oneFolio.balanceList)
                {
                    if (oneBalance.ccyBase.ToString() == ccy.ToUpper())
                    {
                        totalCoins = oneBalance.amount;
                        sucess = true;
                    }
                }

                if (sucess)
                {
                    this.entryOrderAmount.Text = totalCoins.ToString();
                }
                else
                {
                    this.labelCol3.Text = "You don't have any " + ccy + " in your " + exchange + " portfolio";
                    this.entryOrderAmount.Text = 0.ToString();
                }
            }
            else
            {
                this.labelCol3.Text = "Please enter a Crypto and\nan exchange to set max amount";
            }
        }


        /// <summary>
        ///     Set Functions are used to automatically set use entries,
        ///     Such as maximum amount to send in consideration of a specific portfolio,
        ///     Or an order id
        ///     This function automatically set Max amount to sell, by checking the available
        ///     amount on a specific portfolio exchange.
        ///     it then displays it, if it exists, this amout, on the <see cref="ViewSendNReceive"/>
        ///     into the <see cref="entrySendAmount"/> widget
        /// </summary>
        void SetSendMax(object sender, EventArgs e)
        {
            string ccy = "";
            string exchange = "";
            Exchanges goodExchange;
            Func<Portfolio> getFolio2Invoke;
            Portfolio oneFolio;

            try
            {
                this.comboSendCryptos.GetActiveIter(out this.tree);
                ccy = this.comboSendCryptos.Model.GetValue(this.tree, 0).ToString();

                this.comboBoxSendFromExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxSendFromExchanges.Model.GetValue(this.tree, 0).ToString();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                ccy = "ERROR";
                exchange = "ERROR";
            }

            double totalCoins = 0.00;
            bool sucess = false;

            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";

            if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), ccy) &&
                System.Linq.Enumerable.Contains(Enum.GetNames(typeof(Exchanges)), exchange))
            {
                goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                getFolio2Invoke = this.cryptoTrader.Exchange2GetFolio[goodExchange];

                oneFolio = (Portfolio)getFolio2Invoke.DynamicInvoke();

                foreach (Balance oneBalance in oneFolio.balanceList)
                {
                    if (oneBalance.ccyBase.ToString() == ccy.ToUpper())
                    {
                        totalCoins = oneBalance.amount;
                        sucess = true;
                    }
                }

                if (sucess && totalCoins != 0)
                {
                    this.entrySendAmount.Text = totalCoins.ToString();
                }
                else
                {
                    this.labelCol3.Text = "You don't have any " + ccy + " in your " + exchange + " portfolio";
                    this.entrySendAmount.Text = 0.ToString();
                }
            }
            else
            {
                this.labelCol3.Text = "Please enter a Crypto to set max amount";
            }
        }


        /// <summary>
        ///     Set Functions are used to automatically set use entries,
        ///     Such as maximum amount to send in consideration of a specific portfolio,
        ///     Or an order id
        ///     This function automatically set best market price, for a specific exchange,
        ///     and then displays it on the <see cref="ViewSellNBuy"/>
        ///     into the <see cref="entryOrderPrice"/> widget
        /// </summary>
        public void SetOrder2BestPrice(object sender, EventArgs e)
        {
            string orderType;
            string ccyBase;
            string exchange;

            try
            {
                this.comboBoxOrderType.GetActiveIter(out this.tree);
                orderType = this.comboBoxOrderType.Model.GetValue(this.tree, 0).ToString();
                this.comboBoxOrderCryptos.GetActiveIter(out this.tree);
                ccyBase = this.comboBoxOrderCryptos.Model.GetValue(this.tree, 0).ToString();
                this.comboBoxOrderExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxOrderExchanges.Model.GetValue(this.tree, 0).ToString();                
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                orderType = "ERROR";
                ccyBase = "ERROR";
                exchange = "ERROR";
            }

            Price marketPrice;

            if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(OrderType)), orderType))
            {
                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), ccyBase))
                {
                    if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(Exchanges)), exchange))
                    {
                        Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                        Func<string, Price> func2invoke = this.cryptoTrader.Exchange2GetPrice[goodExchange];
                        marketPrice = (Price)func2invoke.DynamicInvoke(ccyBase);

                        this.entryOrderPrice.Text = marketPrice.price.ToString("F5");
                        this.labelCol3.Text = "";
                    }
                    else
                    {
                        this.labelCol3.Text = "Please Select a Order Type, a Currency\n& an Exchange";
                    }
                }
                else
                {
                    this.labelCol3.Text = "Please Select a Order Type, a Currency\n& an Exchange";
                }
            }
            else
            {
                this.labelCol3.Text = "Please Select a Order Type, a Currency\n& an Exchange";
            }
        }


        /// <summary>
        ///     Set Functions are used to automatically set use entries,
        ///     Such as maximum amount to send in consideration of a specific portfolio,
        ///     Or an order id
        ///     This function automatically set best exchange for a specific crypto,
        ///     and then displays it on the <see cref="ViewSellNBuy"/>
        ///     into the <see cref="comboBoxOrderExchanges"/> widget
        /// </summary>
        public void SetOrder2BestExchange(object sender, EventArgs e)
        {
            this.comboBoxOrderType.GetActiveIter(out this.tree);
            string orderType = this.comboBoxOrderType.Model.GetValue(this.tree, 0).ToString();
            this.comboBoxOrderCryptos.GetActiveIter(out this.tree);
            string ccyBase = this.comboBoxOrderCryptos.Model.GetValue(this.tree, 0).ToString();

            int index;

            if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(OrderType)), orderType))
            {
                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), ccyBase))
                {
                    Price bestPrice = this.cryptoTrader.GetBestPrice(ccyBase, orderType, new CancellationToken());
                    Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), bestPrice.exchange.ToUpper());

                    this.entryOrderPrice.Text = bestPrice.price.ToString("F5");
                    index = Array.IndexOf(Enum.GetNames(typeof(Exchanges)), goodExchange.ToString());
                    Gtk.TreeIter iter;
                    this.comboBoxOrderExchanges.Model.IterNthChild(out iter, index + 2);
                    this.comboBoxOrderExchanges.SetActiveIter(iter);
                }
                else
                {
                    this.labelCol3.Text = "Please Select a Order Type & a Currency";
                }
            }
            else
            {
                this.labelCol3.Text = "Please Select a Order Type & a Currency";
            }
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Dashboard view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="buttonDashboard"/> is clicked
        /// </summary>
        public void ViewDashBoard(object sender, EventArgs e, CancellationToken ct)
        {
            double oneReturn = 0.00;
            Dictionary<string, double> dicoStats = new Dictionary<string, double>();

            this.labelFrame.Label = "DASHBOARD";

            this.labelCol1.Text = "Crypto\n";
            this.labelCol2.Text = "10D Avg\n";
            this.labelCol3.Text = "10D Std\n";
            this.labelCol4.Text = "\n";
            txtView.Buffer.Text += "1D Return\n";

            foreach (string cryptos in Enum.GetNames(typeof(MainCryptos)))
            {
                dicoStats = this.cryptoTrader.ComputeStats(cryptos, Exchanges.BITTREX.ToString(), 10);

                Thread.Sleep(200);

                if (!System.Linq.Enumerable.Contains(dicoStats.Keys, "ERROR") && !(dicoStats.Count < 3))
                {
                    TextIter endColumn = this.txtView.Buffer.EndIter;
                    oneReturn = dicoStats["RETURN"];

                    this.labelCol1.Text += cryptos.ToString() + "\n";
                    this.labelCol2.Text += Math.Round(dicoStats["SMA"], 4) + "\n";
                    this.labelCol3.Text += Tools.Double2Percentage(dicoStats["STD"]) + "\n";
                    this.labelCol4.Text += "\n";

                    Thread.Sleep(200);

                    if (oneReturn > 0)
                    {
                        txtView.Buffer.InsertWithTags(ref endColumn, Tools.Double2Percentage(Math.Round(dicoStats["RETURN"], 4)) + "\n",
                                                      this.bullTag);
                    }
                    else if (oneReturn < 0)
                    {
                        txtView.Buffer.InsertWithTags(ref endColumn, Tools.Double2Percentage(Math.Round(dicoStats["RETURN"], 4)) + "\n",
                                                      this.bearTag);
                    }
                    else
                    {
                        txtView.Buffer.InsertWithTags(ref endColumn, Tools.Double2Percentage(Math.Round(dicoStats["RETURN"], 4)) + "\n",
                                                      this.flatTag);
                    }
                }
                else
                {
                    TextIter endColumn = this.txtView.Buffer.EndIter;

                    this.labelCol1.Text += cryptos.ToString() + "\n";
                    this.labelCol2.Text += "0\n";
                    this.labelCol3.Text += "0\n";
                    this.labelCol4.Text += "\n";
                    txtView.Buffer.InsertWithTags(ref endColumn, "0\n", this.flatTag);

                    Thread.Sleep(200);
                }

                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }

            this.workIsFinished = true;

            this.SetWidgets();
            this.labelCol5.Hide();
            this.txtView.Show();
            this.hBoxLoadWheel.Hide();
        }


        /// <summary>
        ///     Set Functions are used to automatically set use entries,
        ///     Such as maximum amount to send in consideration of a specific portfolio,
        ///     Or an order id
        ///     This function automatically show the good combo box, between 
        ///     <see cref="comboExportCryptosPair"/> and <see cref="comboExportCcyPair"/>
        ///     according to <see cref="comboExportTypePair"/> active value
        ///     It's called each time <see cref="comboExportTypePair"/> value is changed
        /// </summary>
        public void SetGoodPaire(object sender, EventArgs e)
        {
            this.comboExportTypePair.GetActiveIter(out this.tree);
            string typePaire = this.comboExportTypePair.Model.GetValue(this.tree, 0).ToString();

            if (typePaire.ToUpper() == "FIAT")
            {
                this.comboExportCryptosPair.Hide();
                this.comboExportCcyPair.Show();
            }
            else if (typePaire.ToUpper() == "CRYPTO")
            {
                this.comboExportCcyPair.Hide();
                this.comboExportCryptosPair.Show();
            }
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function export historical prices into a sqlite database
        /// </summary>
        public void DoExportPrices(object sender, EventArgs e)
        {
            string cryptoBase;
            int nbDays;
            string paireType;
            string ccyPair = "";
            string dbName = "";
            int response;
            bool sucess;
            string[] dir;
            int size;
            string projDir;

            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";
            this.labelCol5.Text = "";

            try
            {
                this.comboExportCryptosBase.GetActiveIter(out this.tree);
                cryptoBase = this.comboExportCryptosBase.Model.GetValue(this.tree, 0).ToString();

                this.comboExportNbDays.GetActiveIter(out this.tree);
                nbDays = Int32.Parse(this.comboExportNbDays.Model.GetValue(this.tree, 0).ToString());

                this.comboExportTypePair.GetActiveIter(out this.tree);
                paireType = this.comboExportTypePair.Model.GetValue(this.tree, 0).ToString();   
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                cryptoBase = "ERROR";
                nbDays = 0;
                paireType = "ERROR";
            }

            if (paireType.ToUpper() == "FIAT")
            {
                try
                {
                    this.comboExportCcyPair.GetActiveIter(out this.tree);
                    ccyPair = this.comboExportCcyPair.Model.GetValue(this.tree, 0).ToString();   
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    ccyPair = "ERROR";
                }
            }
            else if (paireType.ToUpper() == "CRYPTO")
            {
                try
                {
                    this.comboExportCryptosPair.GetActiveIter(out this.tree);
                    ccyPair = this.comboExportCryptosPair.Model.GetValue(this.tree, 0).ToString();
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    ccyPair = "ERROR";
                }
            }
            else
            {
                ccyPair = "ERROR";
            }

            if (ccyPair != "ERROR")
            {
                (dbName, response) = this.cryptoTrader.ExportHistoricalData(cryptoBase, ccyPair, nbDays);    
            }
            else
            {
                response = 0;
            }

            sucess = false;
            if (response > 0)
            {
                sucess = true;
            }

            dir = Tools.GetDirectory().Split('/');
            size = dir.Length;
            projDir = dir[size - 2];

            this.labelCol2.Text = "Prices export sucess :\n\n\n";
            this.labelCol3.Text = "\n\n\n";
            this.labelCol4.Text = sucess.ToString() + "\n\n\n";
            this.labelCol2.Text += "\n\n";
            this.labelCol3.Text += "Export Path :\n";
            this.labelCol3.Text += projDir + "/Static/Databases/ ...\n" + dbName;
            this.labelCol4.Text += "\n\n";
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Export Prices view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_exportPrices"/> is clicked
        /// </summary>
        public void ViewExportPrices(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.labelFrame.Label = "EXPORT PRICES";

            this.hBoxExportEntry.Show();
            this.hBoxExportButton.Show();
            this.comboExportCryptosPair.Hide();
            this.comboExportCcyPair.Hide();
        }

        public void DoOpenOrders(string exchange, CancellationToken ct)
        {
            bool pendingOrders = false;
            Dictionary<dynamic, dynamic> dicoResult = new Dictionary<dynamic, dynamic>();
            Dictionary<dynamic, dynamic> dicoErrors = new Dictionary<dynamic, dynamic>();
            List<Order> listResult = new List<Order>();
            Exchanges goodExchange;
            Func<string, List<Order>> getOpenOrders2Call;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                try
                {
                    goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                    getOpenOrders2Call = this.cryptoTrader.Exchange2GetOpenOrders[goodExchange];
                    listResult = (List<Order>)getOpenOrders2Call.DynamicInvoke(crypto);

                    if (listResult.Count != 0)
                    {
                        break;
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }

            this.labelCol2.Text = "\n\n\n";
            this.labelCol3.Text = "Open Orders :\n\n\n";
            this.labelCol4.Text = "\n\n\n";

            System.Threading.Thread.Sleep(400);

            if (!ct.IsCancellationRequested)
            {
                foreach (Order order in listResult)
                {
                    pendingOrders = true;

                    this.labelCol2.Text = "Order Base Crypto :\n";
                    this.labelCol3.Text += "\n";

                    System.Threading.Thread.Sleep(400);

                    this.labelCol4.Text = order.ccyBase.ToString() + "\n";
                    this.labelCol2.Text += "Order Pair :\n";

                    System.Threading.Thread.Sleep(400);

                    this.labelCol3.Text += "\n";
                    this.labelCol4.Text += order.ccyPair + "\n";

                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }

                    System.Threading.Thread.Sleep(400);

                    this.labelCol2.Text += "Order Type" + " :\n";
                    this.labelCol3.Text += "\n";
                    this.labelCol4.Text += order.side.ToString() + "\n";

                    System.Threading.Thread.Sleep(400);

                    this.labelCol2.Text += "Quantity" + " :\n";
                    this.labelCol3.Text += "\n";
                    this.labelCol4.Text += order.amount.ToString() + "\n";

                    System.Threading.Thread.Sleep(400);

                    this.labelCol2.Text += "Price" + " :\n";
                    this.labelCol3.Text += "\n";
                    this.labelCol4.Text += order.orderPrice.price.ToString() + "\n";

                    System.Threading.Thread.Sleep(400);

                    this.labelCol2.Text += "Order ID" + " :\n";
                    this.labelCol3.Text += "\n" + order.ID + "\n";
                    this.labelCol4.Text += "\n";

                    System.Threading.Thread.Sleep(400);

                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }

            if (!pendingOrders)
            {
                this.labelCol3.Text += "No open orders ...";
            }

            this.workIsFinished = true;
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function request a new <paramref name="orderType"/> order
        ///     for <paramref name="amount"/> <paramref name="currency"/> at a limit price
        ///     of <paramref name="price"/>, and on <paramref name="exchange"/> platform exchange
        /// </summary>
        /// <param name="orderType">
        ///     Is selected by the user from <see cref="comboBoxOrderType"/>.
        ///     It have to be in <see cref="OrderType"/>
        /// </param>
        /// <param name="currency">
        ///     Is selected by the user from <see cref="comboBoxOrderCryptos"/>.
        ///     It have to be in <see cref="MainCryptos"/>
        /// </param>
        /// <param name="amount">
        ///     Is selected by the user from <see cref="entryOrderAmount"/>.
        /// </param>
        /// <param name="price">
        ///     Is selected by the user from <see cref="entryOrderPrice"/>.
        /// </param>
        /// <param name="exchange">
        ///     Is selected by the user from <see cref="comboBoxOrderExchanges"/>.
        ///     It have to be in <see cref="Exchanges"/>
        /// </param>
        public void DoPassOrder(string orderType, string currency, double amount,
                              double price, string exchange, CancellationToken ct)
        {
            Func<string, double, double, Order> function2invoke;
            Order oneOrder = new Order();

            foreach (string oneExchange in Enum.GetNames(typeof(Exchanges)))
            {
                if (exchange == oneExchange)
                {
                    if (orderType == OrderType.BUY.ToString())
                    {
                        Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                        function2invoke = this.cryptoTrader.Exchange2Buy[goodExchange];

                        try
                        {
                            oneOrder = (Order)function2invoke.DynamicInvoke(currency, amount, price);
                        }
                        catch (Exception e)
                        {
                            var t = e.Data;
                            Console.WriteLine(e.Message);
                        }

                    }
                    else if (orderType == OrderType.SELL.ToString())
                    {
                        Exchanges goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                        function2invoke = this.cryptoTrader.Exchange2Sell[goodExchange];
                        try
                        {
                            oneOrder = (Order)function2invoke.DynamicInvoke(currency, amount, price);
                        }
                        catch (Exception e)
                        {
                            var t = e.Data;
                            Console.WriteLine(e.Message);
                        }
                    }
                }

                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }

            this.labelCol2.Text = "Fill Order Suceed :\n\n";
            this.labelCol3.Text = "\n" + oneOrder.success.ToString() + "\n\n";

            System.Threading.Thread.Sleep(200);

            if (oneOrder.success != true || oneOrder.message != "")
            {
                this.labelCol2.Text += "ERROR MESSAGE :\n\n";

                System.Threading.Thread.Sleep(200);

                if (oneOrder.message.Length < 50)
                {
                    this.labelCol3.Text += oneOrder.message + "\n";
                }
                else
                {
                    this.labelCol2.Text += "\n";
                    this.labelCol3.Text += oneOrder.message.Substring(0, 49) + "-\n";
                    this.labelCol3.Text += "-" + oneOrder.message.Substring(50, oneOrder.message.Length - 50) + "\n";
                }

            }

            this.workIsFinished = true;
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function request to send some specific amount of cryptocurrency to a specific adress
        /// </summary>
        public void DoSend(object sender, EventArgs e)
        {
            double amount;
            string currency = "";
            string exchangeFrom = "";
            string adress;
            Exchanges goodExchange;
            Func<string, string, double, Order> sendCrypto2Call;
            Order oneOrder = new Order();

            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";

            try
            {
                this.comboSendCryptos.GetActiveIter(out this.tree);
                currency = this.comboSendCryptos.Model.GetValue(this.tree, 0).ToString();

                this.comboBoxSendFromExchanges.GetActiveIter(out this.tree);
                exchangeFrom = this.comboBoxSendFromExchanges.Model.GetValue(this.tree, 0).ToString();

                adress = this.entrySendAdress.Text;
                amount = Double.Parse(this.entrySendAmount.Text, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                currency = "ERROR";
                adress = "ERROR";
                amount = 0;
                this.labelCol3.Text = "Please set a currency, an adress\nand an amount";
            }

            if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), currency) &&
                System.Linq.Enumerable.Contains(Enum.GetNames(typeof(Exchanges)), exchangeFrom))
            {
                goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchangeFrom);
                sendCrypto2Call = this.cryptoTrader.Exchange2SendCryptos[goodExchange];
                oneOrder = (Order)sendCrypto2Call.DynamicInvoke(adress, currency, amount);
            }
            else
            {
                oneOrder.success = false;
                oneOrder.message = "ERROR";
            }

            if (oneOrder.message != "ERROR")
            {
                this.labelCol2.Text = "Withdraw sucess :\n";
                this.labelCol3.Text = "\n";
                this.labelCol4.Text = oneOrder.success.ToString() + "\n";
                this.labelCol2.Text += "ERROR MESSAGE :\n";
                this.labelCol3.Text += "\n";

                if (oneOrder.message.Length <= 30)
                {
                    this.labelCol4.Text += oneOrder.message + "\n";
                }
                else
                {
                    try
                    {
                        if (oneOrder.message.Split('.').Length >= 2)
                        {
                            this.labelCol4.Text += oneOrder.message.Split('.')[0] + "\n";
                            this.labelCol4.Text += oneOrder.message.Split('.')[1] + "\n"; 
                        }
                        else if (oneOrder.message.Split('.').Length == 1)
                        {
                            this.labelCol3.Text += oneOrder.message.Split('.')[0] + "\n";
                        }
                        else
                        {
                            this.labelCol4.Text += "ERROR WHILE DISPLAYING MESSAGE";
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                        this.labelCol3.Text += "ERROR WHILE DISPLAYING MESSAGE";
                    }
                }
            }
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function animate a loading wheel, and is always called in the same time
        ///     That another Do function, through 2 separate threads.
        ///     It keep spinning the loading wheel, while the other Do function task 
        ///     send that <see cref="workIsfinished"/> is true, or while <see cref="cancelTokenSource"/>
        ///     send a cancellation token to <paramref name="ct"/>, when it's taking too much time
        /// </summary>
        public void DoLoadWheel(object sender, EventArgs e, CancellationToken ct)
        {
            Gtk.Button oneSender = (Gtk.Button)sender;
            string originalTxt = oneSender.Label;

            this.imgLoadWheel.Show();
            this.imgLoadWheel.Pixbuf = this.loaderPixBuf;
            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";
            this.labelCol5.Text = "";

            this.begin = DateTime.Now;
            this.clock = DateTime.Now;

            while (!this.workIsFinished && !this.cancelToken.IsCancellationRequested)
            {
                this.imgLoadWheel.Pixbuf = this.imgLoadWheel.Pixbuf.RotateSimple(Gdk.PixbufRotation.Counterclockwise);
                this.imgLoadWheel.Show();
                Thread.Sleep(150);

                if (this.clock > this.begin.AddSeconds(30))
                {
                    Console.WriteLine("Time Out ! Cancellation Token Requested");
                    this.cancelTokenSource.Cancel();
                    this.labelCol1.Text = "";
                    this.labelCol2.Text = "";
                    this.labelCol3.Text = "Time Out : Thread aborted";
                    this.labelCol4.Text = "";
                    this.labelCol5.Text = "";
                }

                if (this.cancelToken.IsCancellationRequested)
                {
                    break;
                }

                this.clock = DateTime.Now;
            }

            Console.WriteLine("Work is finished");
            this.imgLoadWheel.Clear();
            this.imgLoadWheel.Hide();
        }


        /// <summary>
        ///     Async Function is always linked to the same-named non-async function.
        ///     They are the function that are actually called when buttons are pressed.
        ///     They all call both the function that let the loading wheel get animated while the other function
        ///     is getting completed
        ///     <paramref name="sender"/> and <paramref name="e"/> are required due to the fact
        ///     that those functions are handled by event handler
        ///     This function is called when <see cref="buttonDashboard"/> is pressed
        /// </summary>
        public void ViewDashBoardAsync(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.loadSeparatorH.Show();
            this.hBoxLoadWheel.Show();
            this.loadSeparatorL.Show();

            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;

            this.workIsFinished = false;
            this.task1 = this.taskFactory.StartNew(() => this.DoLoadWheel(sender, e, this.cancelToken));
            this.task2 = this.taskFactory.StartNew(() => this.ViewDashBoard(sender, e, this.cancelToken));
        }


        /// <summary>
        ///     Async Function is always linked to the same-named non-async function.
        ///     They are the function that are actually called when buttons are pressed.
        ///     They all call both the function that let the loading wheel get animated while the other function
        ///     is getting completed
        ///     <paramref name="sender"/> and <paramref name="e"/> are required due to the fact
        ///     that those functions are handled by event handler
        ///     This function is called when <see cref="buttonPrice"/> is pressed
        /// </summary>
        public void DoGetPriceAsync(object sender, EventArgs e)
        {
            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;
            string exchange;
            string currency;

            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";
            this.labelCol5.Text = "";

            try
            {
                this.comboBoxPriceExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxPriceExchanges.Model.GetValue(this.tree, 0).ToString();
                this.comboBoxPriceCryptos.GetActiveIter(out this.tree);
                currency = this.comboBoxPriceCryptos.Model.GetValue(this.tree, 0).ToString();

                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), currency) &&
                    System.Linq.Enumerable.Contains(Enum.GetNames(typeof(Exchanges)), exchange))
                {
                    this.workIsFinished = false;
                    this.task1 = this.taskFactory.StartNew(() => this.DoLoadWheel(sender, e, this.cancelToken));
                    this.task2 = this.taskFactory.StartNew(() => this.DoGetPrice(currency, exchange, this.cancelToken));
                }
                else
                {
                    this.labelCol3.Text = "Please select an exchange and a currency";
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                this.labelCol3.Text = "Please select an exchange and a currency";
            }

            // Require so that the program doesn't crash if user multiple click on the button
            Thread.Sleep(200);
        }


        /// <summary>
        ///     Async Function is always linked to the same-named non-async function.
        ///     They are the function that are actually called when buttons are pressed.
        ///     They all call both the function that let the loading wheel get animated while the other function
        ///     is getting completed
        ///     <paramref name="sender"/> and <paramref name="e"/> are required due to the fact
        ///     that those functions are handled by event handler
        ///     This function is called when <see cref="buttonCancelOrder"/> is pressed
        /// </summary>
        public void DoCancelOrderAsync(object sender, EventArgs e)
        {
            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;
            string exchange;
            string currency;
            string orderId;

            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";
            this.labelCol5.Text = "";

            try
            {
                this.comboBoxCancelOrderExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxCancelOrderExchanges.Model.GetValue(this.tree, 0).ToString();
                this.comboBoxCancelOrderCryptos.GetActiveIter(out this.tree);
                currency = this.comboBoxCancelOrderCryptos.Model.GetValue(this.tree, 0).ToString();
                orderId = this.entryCancelOrderId.Text;

                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), currency) &&
                    System.Linq.Enumerable.Contains(Enum.GetNames(typeof(Exchanges)), exchange))
                {
                    this.workIsFinished = false;
                    this.task1 = this.taskFactory.StartNew(() => this.DoLoadWheel(sender, e, this.cancelToken));
                    this.task2 = this.taskFactory.StartNew(() => this.DoCancelOrder(currency, exchange, orderId));
                }
                else
                {
                    this.labelCol3.Text = "Please select an exchange, an order ID,\nand a currency";
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                this.labelCol3.Text = "Please select an exchange, an order ID,\nand a currency";
            }

            // Require so that the program doesn't crash if user multiple click on the button
            Thread.Sleep(200);
        }


        /// <summary>
        ///     Async Function is always linked to the same-named non-async function.
        ///     They are the function that are actually called when buttons are pressed.
        ///     They all call both the function that let the loading wheel get animated while the other function
        ///     is getting completed
        ///     <paramref name="sender"/> and <paramref name="e"/> are required due to the fact
        ///     that those functions are handled by event handler
        ///     This function is called when <see cref="buttonOpenOrders"/> is pressed
        /// </summary>
        public void DoOpenOrdersAsync(object sender, EventArgs e)
        {
            string exchange;

            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;

            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";
            this.labelCol5.Text = "";

            try
            {
                this.comboBoxOpenOrderExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxOpenOrderExchanges.Model.GetValue(this.tree, 0).ToString();


                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(Exchanges)), exchange))
                {
                    this.workIsFinished = false;
                    this.task1 = this.taskFactory.StartNew(() => this.DoLoadWheel(sender, e, this.cancelToken));
                    this.task2 = this.taskFactory.StartNew(() => this.DoOpenOrders(exchange, this.cancelToken));
                }
                else
                {
                    this.labelCol3.Text = "Please select an exchange";
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                this.labelCol3.Text = "Please select an exchange";
            }

            // Require so that the program doesn't crash if user multiple click on the button
            Thread.Sleep(200);
        }


        /// <summary>
        ///     Async Function is always linked to the same-named non-async function.
        ///     They are the function that are actually called when buttons are pressed.
        ///     They all call both the function that let the loading wheel get animated while the other function
        ///     is getting completed
        ///     <paramref name="sender"/> and <paramref name="e"/> are required due to the fact
        ///     that those functions are handled by event handler
        ///     This function is called when <see cref="buttonOrder"/> is pressed
        /// </summary>
        public void DoPassOrderAsync(object sender, EventArgs e)
        {
            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;
            string orderType, currency, exchange;
            double amount, price;

            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";
            this.labelCol5.Text = "";

            try
            {
                this.comboBoxOrderType.GetActiveIter(out this.tree);
                orderType = this.comboBoxOrderType.Model.GetValue(this.tree, 0).ToString();
                this.comboBoxOrderCryptos.GetActiveIter(out this.tree);
                currency = this.comboBoxOrderCryptos.Model.GetValue(this.tree, 0).ToString();
                this.comboBoxOrderExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxOrderExchanges.Model.GetValue(this.tree, 0).ToString();
                amount = Double.Parse(this.entryOrderAmount.Text, System.Globalization.CultureInfo.InvariantCulture);
                price = Double.Parse(this.entryOrderPrice.Text, System.Globalization.CultureInfo.InvariantCulture);

                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), currency) &&
                    System.Linq.Enumerable.Contains(Enum.GetNames(typeof(OrderType)), orderType) &&
                    System.Linq.Enumerable.Contains(Enum.GetNames(typeof(Exchanges)), exchange))
                {
                    this.workIsFinished = false;
                    this.task1 = this.taskFactory.StartNew(() => this.DoLoadWheel(sender, e, this.cancelToken));
                    this.task2 = this.taskFactory.StartNew(() => this.DoPassOrder(orderType, currency, amount, price, exchange, this.cancelToken));                    
                }
                else
                {
                    this.labelCol3.Text = "Please select an order type, a currency,\nan exchange, an amount and a price";
                }

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                this.labelCol3.Text = "Please select an order type, a currency,\nan exchange, an amount and a price";
            }

            // Require so that the program doesn't crash if user multiple click on the button
            Thread.Sleep(200);
        }


        /// <summary>
        ///     Async Function is always linked to the same-named non-async function.
        ///     They are the function that are actually called when buttons are pressed.
        ///     They all call both the function that let the loading wheel get animated while the other function
        ///     is getting completed
        ///     <paramref name="sender"/> and <paramref name="e"/> are required due to the fact
        ///     that those functions are handled by event handler
        ///     This function is called when <see cref="buttonBestPrice"/> is pressed
        /// </summary>
        public void DoGetBestPriceAsync(object sender, EventArgs e)
        {
            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;
            string currency;
            string orderType;

            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";
            this.labelCol5.Text = "";

            try
            {
                this.comboBoxBestPriceCryptos.GetActiveIter(out this.tree);
                currency = this.comboBoxBestPriceCryptos.Model.GetValue(this.tree, 0).ToString();
                this.comboBoxPriceOrderType.GetActiveIter(out this.tree);
                orderType = this.comboBoxPriceOrderType.Model.GetValue(this.tree, 0).ToString();

                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), currency) &&
                    System.Linq.Enumerable.Contains(Enum.GetNames(typeof(OrderType)), orderType))
                {
                    this.workIsFinished = false;
                    this.task1 = this.taskFactory.StartNew(() => this.DoLoadWheel(sender, e, this.cancelToken));
                    this.task2 = this.taskFactory.StartNew(() => DoPrintBestPrice(currency, orderType, this.cancelToken));                    
                }
                else
                {
                    this.labelCol3.Text = "Please select a currency and an order type"; 
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                this.labelCol3.Text = "Please select a currency and an order type";
            }

            // Require so that the program doesn't crash if user multiple click on the button
            Thread.Sleep(200);
        }


        /// <summary>
        ///     Async Function is always linked to the same-named non-async function.
        ///     They are the function that are actually called when buttons are pressed.
        ///     They all call both the function that let the loading wheel get animated while the other function
        ///     is getting completed
        ///     <paramref name="sender"/> and <paramref name="e"/> are required due to the fact
        ///     that those functions are handled by event handler
        ///     This function is called when <see cref="buttonAllPrice"/> is pressed
        /// </summary>
        public void DoGetAllPriceAsync(object sender, EventArgs e)
        {
            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;

            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";
            this.labelCol5.Text = "";
            this.txtView.Buffer.Text = "";

            try
            {
                this.comboBoxAllPriceCryptos.GetActiveIter(out this.tree);
                string currency = this.comboBoxAllPriceCryptos.Model.GetValue(this.tree, 0).ToString();

                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), currency))
                {
                    this.workIsFinished = false;
                    this.task1 = this.taskFactory.StartNew(() => this.DoLoadWheel(sender, e, this.cancelToken));
                    this.task2 = this.taskFactory.StartNew(() => DoPrintAllPrices(currency, this.cancelToken));   
                }
                else
                {
                    this.labelCol3.Text = "Please select an Exchange";
                }
            }
            catch
            {
                this.labelCol3.Text = "Please select an Exchange";
            }


            // Require so that the program doesn't crash if user multiple click on the button
            Thread.Sleep(200);
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function request crypto price informations, on <paramref name="currency"/>, 
        ///     and on <paramref name="exchange"/> platform echange.
        ///     wether <see cref="buttonPriceStats"/> has been pressed or not, it also displays
        ///     some basic statistics (Moving Average, etc)
        /// </summary>
        /// <param name="currency">
        ///     Is selected by the user from <see cref="comboBoxPriceCryptos"/>.
        ///     It have to be in <see cref="MainCryptos"/>
        /// </param>
        /// <param name="exchange">
        ///     Is selected by the user from <see cref="comboBoxPriceExchanges"/>.
        ///     It have to be in <see cref="Exchanges"/>
        /// </param>
        public void DoGetPrice(string currency, string exchange, CancellationToken ct)
        {
            Func<string, Price> getPrice2Invoke;
            Price price = new Price(error:true);
            Exchanges goodExchange;
            Dictionary<string, double> dicoStats = new Dictionary<string, double>();
            string today = DateTime.Now.ToString("dd/MM/yyyy");
            double BTCUSD = this.cryptoTrader.GetSpotPrice(MainCryptos.BTC.ToString());
            double ETHUSD = this.cryptoTrader.GetSpotPrice(MainCryptos.ETH.ToString());

            foreach (string oneExchange in Enum.GetNames(typeof(Exchanges)))
            {
                if (exchange == oneExchange)
                {
                    goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                    getPrice2Invoke = this.cryptoTrader.Exchange2GetPrice[goodExchange];
                    price = (Price)getPrice2Invoke.DynamicInvoke(currency);

                    if (this.withStats)
                    {
                        dicoStats = this.cryptoTrader.ComputeStats(currency, exchange, 10);
                    }
                }

                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }   

            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";

            Thread.Sleep(200);

            if (price.ccyPair == MainCryptos.BTC.ToString() && !ct.IsCancellationRequested &&
               price.error != true)
            {
                this.labelCol2.Text += "Base Currency :\n";
                this.labelCol2.Text += "Paire Currency:\n";

                Thread.Sleep(300);

                this.labelCol2.Text += "Exchange :\n";
                this.labelCol2.Text += "Market Price:\n";
                this.labelCol2.Text += "USD Price:\n";

                Thread.Sleep(300);

                this.labelCol4.Text += price.ccyBase + "\n";
                this.labelCol4.Text += price.ccyPair + "\n";

                Thread.Sleep(300);

                this.labelCol4.Text += price.exchange + "\n";
                this.labelCol4.Text += price.price.ToString("F5") + "\n";
                this.labelCol4.Text += (price.price * BTCUSD).ToString("F5") + "\n";

                Thread.Sleep(300);

                if (this.withStats && exchange != "ERROR" && exchange != "ERROR")
                {
                    Thread.Sleep(300);

                    this.labelCol2.Text += "Average Price 10D :\n";
                    this.labelCol2.Text += "Std Deviation 10D :\n";
                    this.labelCol2.Text += "Return 1D :\n";

                    Thread.Sleep(300);

                    this.labelCol4.Text += Math.Round(dicoStats["SMA"], 4) + "\n";

                    Thread.Sleep(200);

                    this.labelCol4.Text += Tools.Double2Percentage(Math.Round(dicoStats["STD"], 6)) + "\n";

                    Thread.Sleep(200);

                    this.labelCol4.Text += Tools.Double2Percentage(Math.Round(dicoStats["RETURN"], 6)) + "\n";

                    Thread.Sleep(300);
                }
            }
            else if (price.ccyPair == MainCryptos.ETH.ToString() && !ct.IsCancellationRequested &&
                    price.error != true)
            {
                this.labelCol2.Text += "Base Currency :\n";
                this.labelCol2.Text += "Paire Currency:\n";

                Thread.Sleep(300);

                this.labelCol2.Text += "Exchange :\n";
                this.labelCol2.Text += "Market Price:\n";
                this.labelCol2.Text += "USD Price:\n";

                Thread.Sleep(300);

                this.labelCol4.Text += price.ccyBase + "\n";
                this.labelCol4.Text += price.ccyPair + "\n";

                Thread.Sleep(300);

                this.labelCol4.Text += price.exchange + "\n";
                this.labelCol4.Text += price.price.ToString("F5") + "\n";
                this.labelCol4.Text += (price.price * ETHUSD).ToString("F5") + "\n";

                Thread.Sleep(300);

                if (this.withStats && exchange != "ERROR" && exchange != "ERROR")
                {
                    Thread.Sleep(300);

                    this.labelCol2.Text += "Average Price 10D :\n";
                    this.labelCol2.Text += "Std Deviation 10D :\n";
                    this.labelCol2.Text += "Return 1D :\n";

                    Thread.Sleep(300);

                    this.labelCol4.Text += Math.Round(dicoStats["SMA"], 4) + "\n";

                    Thread.Sleep(200);

                    this.labelCol4.Text += Tools.Double2Percentage(Math.Round(dicoStats["STD"], 6)) + "\n";

                    Thread.Sleep(200);

                    this.labelCol4.Text += Tools.Double2Percentage(Math.Round(dicoStats["RETURN"], 6)) + "\n";

                    Thread.Sleep(300);
                }
            } 
            else if (!ct.IsCancellationRequested && !price.error)
            {
                this.labelCol2.Text += "Base Currency :\n";
                this.labelCol2.Text += "Paire Currency:\n";
                this.labelCol2.Text += "Exchange :\n";
                this.labelCol2.Text += "Market Price:\n";
                this.labelCol2.Text += "USD Price:\n";

                Thread.Sleep(200);

                this.labelCol4.Text += price.ccyBase + "\n";
                this.labelCol4.Text += price.ccyPair + "\n";

                Thread.Sleep(200);

                this.labelCol4.Text += price.exchange + "\n";
                this.labelCol4.Text += price.price.ToString("F5") + "\n";
                this.labelCol4.Text += price.price.ToString("F5") + "\n";

                Thread.Sleep(200);

                if (this.withStats && currency != "ERROR" && exchange != "ERROR")
                {
                    Thread.Sleep(200);

                    this.labelCol2.Text += "Average Price 10D :\n";
                    this.labelCol2.Text += "Std Deviation 10D :\n";
                    this.labelCol2.Text += "Return 1D :\n";

                    Thread.Sleep(200);

                    this.labelCol4.Text += Math.Round(dicoStats["SMA"], 4) + "\n";
                    this.labelCol4.Text += Tools.Double2Percentage(Math.Round(dicoStats["STD"], 6)) + "\n";
                    this.labelCol4.Text += Tools.Double2Percentage(Math.Round(dicoStats["RETURN"], 6)) + "\n";

                    Thread.Sleep(200);
                }
            }
            else
            {
                this.labelCol3.Text = "ERROR : Please retry";
            }

            this.workIsFinished = true;
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Account Infos view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_account"/> is clicked
        /// </summary>
        public void ViewAccountInfos(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.labelFrame.Label = "ACCOUNT INFOS";

            this.hBoxAccountInfosEntry.Show();
            this.hBoxAccountInfosButton.Show();
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Platform Arbitrage view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_platform_arbitrage"/> is clicked
        /// </summary>
        public void ViewPlatformArbitrage(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.labelFrame.Label = "PLATFORM ARBITRAGE";

            this.hBoxPlatformArbitrageEntry.Show();
            this.hBoxPlatformArbitrageButton.Show();
        }


        /// <summary>
        ///     Async Function is always linked to the same-named non-async function.
        ///     They are the function that are actually called when buttons are pressed.
        ///     They all call both the function that let the loading wheel get animated while the other function
        ///     is getting completed
        ///     <paramref name="sender"/> and <paramref name="e"/> are required due to the fact
        ///     that those functions are handled by event handler
        ///     This function is called when <see cref="buttonPlatformArbitrage"/> is pressed
        /// </summary>
        public void DoCheckPlatformArbitrageAsync(object sender, EventArgs e)
        {
            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;
            string currency;

            this.labelCol1.Text = "";
            this.labelCol2.Text = "";
            this.labelCol3.Text = "";
            this.labelCol4.Text = "";
            this.labelCol5.Text = "";

            try
            {
                this.comboPlatformArbitrageCryptos.GetActiveIter(out this.tree);
                currency = this.comboPlatformArbitrageCryptos.Model.GetValue(this.tree, 0).ToString();

                if (System.Linq.Enumerable.Contains(Enum.GetNames(typeof(MainCryptos)), currency))
                {
                    this.workIsFinished = false;
                    this.task1 = this.taskFactory.StartNew(() => this.DoLoadWheel(sender, e, this.cancelToken));
                    this.task2 = this.taskFactory.StartNew(() => this.DoCheckPlatformArbitrage(currency, this.cancelToken));
                }
                else
                {
                    this.labelCol3.Text = "Please select a currency";
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                this.labelCol3.Text = "Please select a currency";
            }
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function request a check on arbitrage opportunities, 
        ///     for <paramref name="currency"/>.
        /// </summary>
        /// <param name="currency">
        ///     Is selected by the user from <see cref="comboPlatformArbitrageCryptos"/>.
        ///     It have to be in <see cref="MainCryptos"/>
        /// </param>
        public void DoCheckPlatformArbitrage(string currency, CancellationToken ct)
        {
            bool result = false;
            bool error = false;

            try
            {
                result = this.cryptoTrader.CheckPlatformArbitrage(currency);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                error = true;
            }

            if (result && !ct.IsCancellationRequested && !error)
            {
                this.labelCol3.Text = currency + " : Arbitrage Detected !";
            }
            else if (!ct.IsCancellationRequested && !error)
            {
                this.labelCol3.Text = currency + " : No Arbitrage Detected ...";
            }
            else if (error)
            {
                this.labelCol3.Text = "ERROR : PLEASE RETRY";
            }

            this.workIsFinished = true;
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Sell & Buy view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_sellnbuy"/> is clicked
        /// </summary>
        public void ViewSellNBuy(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.labelFrame.Label = "TRADE CRYPTOS";

            this.vBoxOrderEntry.Show();
            this.hBoxOrderButton.Show();
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Send Cryptos view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_sendnreceive"/> is clicked
        /// </summary>
        public void ViewSendNReceive(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.labelFrame.Label = "SEND CRYPTOS";

            this.vBoxSendEntry.Show();
            this.hBoxSendButton.Show();
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the One Price view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_getOnePrice"/> is clicked
        /// </summary>
        public void ViewOnePrice(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.labelFrame.Label = "ONE PRICE";

            this.withStats = false;
            this.buttonPriceStats.ModifyBg(StateType.Normal, yellowColorStrong);
            this.buttonPriceStats.ModifyBg(StateType.Prelight, yellowColorStrong);
            this.buttonPriceStats.Label = "With Stats";

            this.hBoxPriceEntry.Show();
            this.hBoxPriceButton.Show();
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Best Price view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_getBestPrice"/> is clicked
        /// </summary>
        public void ViewBestPrice(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.labelFrame.Label = "BEST PRICE";

            this.hBoxBestPriceEntry.Show();
            this.hBoxBestPriceButton.Show();
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the All Prices view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_getAllPrice"/> is clicked
        /// </summary>
        public void ViewAllPrice(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.labelFrame.Label = "ALL PRICE";

            this.hBoxAllPriceEntry.Show();
            this.hBoxAllPriceButton.Show();
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Home view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="buttonHome"/> is clicked
        /// </summary>
        public void ViewHome(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.buttonActualize.Hide();
            this.buttonDashboard.Show();

            this.imgLogo.Show();
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function animate a progress bar, when <see cref="DoFolio"/> 
        ///     is called through a task, in <see cref="DoFolioAsync"/>
        /// </summary>
        /// <param name="typeOfWork">
        ///     specifies wether the progress bar have to display "LOADING..." or "ACTUALIZING..."
        /// </param>
        public void DoProgressBarFolio(string typeOfWork)
        {
            this.begin = DateTime.Now;
            this.clock = DateTime.Now;

            this.oneProgressBar.Show();
            this.oneProgressBar.Text = "";

            while (!this.workIsFinished)
            {
                this.oneProgressBar.Pulse();

                if (typeOfWork.ToUpper() == "LOAD")
                {
                    Models.Tools.IterateLoad(ref this.oneProgressBar);
                    Thread.Sleep(100);
                }
                else if (typeOfWork.ToUpper() == "ACTUALIZE")
                {
                    Models.Tools.IterateActualize(ref this.oneProgressBar);
                    Thread.Sleep(100);
                }
                else
                {
                    Models.Tools.IterateLoad(ref this.oneProgressBar);
                    Thread.Sleep(100);
                }

                this.clock = DateTime.Now;
                if (this.workIsFinished)
                {
                    this.oneProgressBar.Hide();
                    this.oneProgressBar.Fraction = 0.00;
                    this.oneProgressBar.Text = "";
                }
                else if (this.clock.Minute == this.begin.Minute && this.clock.Second >= this.begin.Second + 20)
                {
                    Console.WriteLine("Work Folio wasn't finished");
                    this.cancelTokenSource.Cancel();
                    this.labelCol1.Text = "";
                    this.labelCol2.Text = "";
                    this.labelCol3.Text = "OUT OF TIME : Request Cancelled";
                    this.labelCol4.Text = "";
                    this.labelCol5.Text = "";
                }
                else if (this.clock.Minute > this.begin.Minute && this.clock.Second >= this.begin.Second - 20)
                {
                    Console.WriteLine("Work Folio wasn't finished");
                    this.cancelTokenSource.Cancel();
                    this.labelCol1.Text = "";
                    this.labelCol2.Text = "";
                    this.labelCol3.Text = "OUT OF TIME : Request Cancelled";
                    this.labelCol4.Text = "";
                    this.labelCol5.Text = "";
                }
            }
        }


        /// <summary>
        ///     View Functions are those who print the differents avaible views of the GUI application
        ///     This function print the Portfolio view on the GUI
        ///     <paramref name="sender"/> and <paramref name="e"/> are necessary due to the fact that this function
        ///     is handled by a event handler. 
        ///     Event is activated when <see cref="m_folio"/> is clicked
        /// </summary>
        public void ViewFolio(object sender, EventArgs e)
        {
            this.ClearWidgets();
            this.SetWidgets();
            this.buttonDashboard.Hide();
            this.buttonActualize.Show();
            this.labelFrame.Label = "My Portofolio";
            this.hBoxLoadWheel.Show();

            this.hBoxFolioEntry.Show();
            this.hBoxFolioButton.Show();
        }


        /// <summary>
        ///     Async Function is always linked to the same-named non-async function.
        ///     They are the function that are actually called when buttons are pressed.
        ///     They all call both the function that let the loading wheel get animated while the other function
        ///     is getting completed
        ///     <paramref name="sender"/> and <paramref name="e"/> are required due to the fact
        ///     that those functions are handled by event handler
        ///     This function is called when <see cref="buttonFolio"/> is pressed
        /// </summary>
        public void DoFolioAsync(object sender, EventArgs e)
        {
            Dictionary<string, Dictionary<string, double>> dicoFolio = new Dictionary<string, Dictionary<string, double>>();

            this.workIsFinished = false;
            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;

            this.task1 = this.taskFactory.StartNew(() => this.DoProgressBarFolio("LOAD"));
            this.task2 = this.taskFactory.StartNew(() => this.DoFolio());
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function request the portfolio of the user, and displays it on the GUI
        /// </summary>
        public void DoFolio()
        {
            string exchange;
            Exchanges goodExchange;
            Func<Portfolio> getFolio;
            Portfolio oneFolio = new Portfolio();
            Dictionary<string, string> oneBalance = new Dictionary<string, string>();
            Dictionary<dynamic, dynamic> dicoFolio;
            double sumUSD = 0.00;
            double sumBTC = 0.00;

            try
            {
                this.comboBoxFolioExchanges.GetActiveIter(out this.tree);
                exchange = this.comboBoxFolioExchanges.Model.GetValue(this.tree, 0).ToString();
                goodExchange = (Exchanges)Enum.Parse(typeof(Exchanges), exchange);
                getFolio = this.cryptoTrader.Exchange2GetFolio[goodExchange];

                oneFolio = (Portfolio)getFolio.DynamicInvoke();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                exchange = "ERROR";
                dicoFolio = new Dictionary<dynamic, dynamic>();
            }

            sumUSD = 0.00;
            sumBTC = 0.00;

            this.labelCol1.Text = "Crypto\n\n";
            this.labelCol2.Text = "N° Coins\n\n";
            this.labelCol3.Text = "BTC Val\n\n";
            this.labelCol4.Text = "USD Val\n\n";

            Thread.Sleep(400);

            foreach (Balance oneRow in oneFolio.balanceList)
            {
                try 
                {
                    if (!this.cancelToken.IsCancellationRequested && oneRow.amount != 0)
                    {
                        this.labelCol1.Text += oneRow.ccyBase.ToString() + "\n";
                        this.labelCol2.Text += Math.Round(oneRow.amount, 4) + "\n";

                        Thread.Sleep(400);

                        this.labelCol3.Text += Math.Round(oneRow.totalValueBTC, 4) + "\n";
                        this.labelCol4.Text += Math.Round(oneRow.totalValueUSD, 4) + "\n";

                        Thread.Sleep(400);

                        sumBTC += oneRow.totalValueBTC;
                        sumUSD += oneRow.totalValueUSD;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    sumBTC += 0;
                    sumUSD += 0;
                }
            }

            if (!this.cancelToken.IsCancellationRequested)
            {
                Thread.Sleep(400);

                this.labelCol1.Text += "\nTOTAL\n";
                this.labelCol2.Text += "\n\n";

                Thread.Sleep(400);

                this.labelCol3.Text += "\n" + Math.Round(sumBTC, 4).ToString() + "\n";
                this.labelCol4.Text += "\n" + Math.Round(sumUSD, 4).ToString() + "\n";   
            }

            this.workIsFinished = true;
        }


        /// <summary>
        ///     Async Function is always linked to the same-named non-async function.
        ///     They are the function that are actually called when buttons are pressed.
        ///     They all call both the function that let the loading wheel get animated while the other function
        ///     is getting completed
        ///     <paramref name="sender"/> and <paramref name="e"/> are required due to the fact
        ///     that those functions are handled by event handler
        ///     This function is called when <see cref="buttonActualize"/> is pressed
        /// </summary>
        public void DoActualizeAsync(object sender, EventArgs e)
        {
            Dictionary<string, Dictionary<string, double>> dicoFolio = new Dictionary<string, Dictionary<string, double>>();

            this.workIsFinished = false;
            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;

            this.task1 = this.taskFactory.StartNew(() => this.DoProgressBarFolio("ACTUALIZE"));
            this.task2 = this.taskFactory.StartNew(() => this.DoFolio());
        }


        /// <summary>
        ///     Event handler exit function (For Delete Events)
        /// </summary>
        public void DoWindowsDelete(object sender, Gtk.DeleteEventArgs e)
        {
            this.Destroy();

            Application.Quit();

            e.RetVal = true;
        }


        /// <summary>
        ///     Event handler exit function (For Destroy Events)
        /// </summary>
        public void DoWindowsDelete(object sender, Gtk.DestroyEventArgs e)
        {
            this.Destroy();

            Application.Quit();

            e.RetVal = true;
        }


        /// <summary>
        ///     Event handler exit function
        ///     (For any other Events than Delete or Destroy Events)
        /// </summary>
        public void DoExit(object sender, EventArgs e)
        {
            this.Destroy();
            Application.Quit();
        }


        /// <summary>
        ///     Non-event-handled exit function
        /// </summary>
        public void DoQuit()
        {
            this.Destroy();
            Application.Quit();
        }


        /// <summary>
        ///     Function that clear all particular-view widgets and keep only the 
        ///     Mains and generals widgets (Menu Bar, Main Buttons, etc)
        /// </summary>
        public void ClearWidgets()
        {
            if (this.labelCol1.Text != "")
            {
                this.labelCol1.Text = "";
            }
            if (this.labelCol2.Text != "")
            {
                this.labelCol2.Text = "";
            }
            if (this.labelCol3.Text != "")
            {
                this.labelCol3.Text = "";
            }
            if (this.labelCol4.Text != "")
            {
                this.labelCol4.Text = "";
            }

            if (this.labelCol5.Text != "")
            {
                this.labelCol5.Text = "";
            }

            this.hBoxAccountInfosEntry.Hide();
            this.hBoxAccountInfosButton.Hide();
            this.hBoxFolioEntry.Hide();
            this.hBoxFolioButton.Hide();
            this.buttonActualize.Hide();
            this.imgLogo.Hide();
            this.imgLoadWheel.Hide();
            this.hBoxLoadWheel.Hide();
            this.eventBoxLabelFrame.Hide();
            this.loadSeparatorH.Hide();
            this.loadSeparatorL.Hide();
            this.hBoxLabel.Hide();
            this.labelFrame.Hide();
            this.hBoxOpenOrdersButton.Hide();
            this.hBoxOpenOrdersEntry.Hide();
            this.hBoxPriceButton.Hide();
            this.hBoxPriceEntry.Hide();
            this.oneProgressBar.Hide();
            this.hBoxOrderButton.Hide();
            this.vBoxOrderEntry.Hide();
            this.hBoxCancelOrderEntry.Hide();
            this.hBoxCancelOrderButton.Hide();
            this.hBoxPlatformArbitrageButton.Hide();
            this.hBoxPlatformArbitrageEntry.Hide();
            this.hBoxBestPriceEntry.Hide();
            this.hBoxBestPriceButton.Hide();
            this.hBoxAllPriceEntry.Hide();
            this.hBoxAllPriceButton.Hide();
            this.vBoxSendEntry.Hide();
            this.hBoxSendButton.Hide();
            this.hBoxExportEntry.Hide();
            this.hBoxExportButton.Hide();
            this.txtView.Hide();
            this.txtView.Buffer.Clear();
            this.txtView.Buffer.Text = "";
        }


        /// <summary>
        ///     Function that set all general widgets,
        ///     but which may have been hided for some particular
        ///     purpose, in particular views.
        /// </summary>
        public void SetWidgets()
        {
            this.txtView.Hide();
            this.labelCol5.Show();
            this.buttonDashboard.Show();
            this.buttonActualize.Hide();
            this.loadSeparatorH.Hide();
            this.loadSeparatorL.Hide();
            this.eventBoxLabelFrame.ShowAll();
            this.hBoxLabel.ShowAll();
            this.labelFrame.ShowAll();
            this.hBoxLoadWheel.Show();
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function request all prices for a specific <paramref name="currency"/>
        ///     It then displays it on the <see cref="ViewAllPrice"/>
        /// </summary>
        /// <param name="currency">
        ///     Is selected by the user from <see cref="comboBoxAllPriceCryptos"/>.
        ///     It have to be in <see cref="MainCryptos"/>
        /// </param>
        public void DoPrintAllPrices(string currency, CancellationToken ct)
        {
            this.labelCol5.Hide();
            this.txtView.Show();

            double BTCSpot = this.cryptoTrader.GetSpotPrice(MainCryptos.BTC.ToString());
            double ETHSpot = this.cryptoTrader.GetSpotPrice(MainCryptos.ETH.ToString());

            Thread.Sleep(400);

            SortedDictionary<string, Price> dicoResult = this.cryptoTrader.GetAllPrices(currency, ct);

            Thread.Sleep(400);

            this.labelCol1.Text = "Exchange\n\n";
            this.labelCol2.Text = "Base\n\n";
            this.labelCol3.Text = "Paire\n\n";
            this.labelCol4.Text = "Mkt Price\n\n";
            this.txtView.Buffer.Text = "USD Price\n\n";

            Thread.Sleep(400);

            foreach (var result in dicoResult)
            {
                this.labelCol1.Text += result.Key + "\n";
                this.labelCol2.Text += result.Value.ccyBase + "\n";
                this.labelCol3.Text += result.Value.ccyPair + "\n";
                this.labelCol4.Text += Math.Round(result.Value.price, 6).ToString("F5") + "\n";

                Thread.Sleep(400);

                if (result.Value.ccyPair == MainCryptos.BTC.ToString())
                {
                    this.txtView.Buffer.Text += Math.Round(result.Value.price * BTCSpot, 4).ToString() + "\n";

                    Thread.Sleep(400);
                }
                else if (result.Value.ccyPair == MainCryptos.ETH.ToString())
                {
                    this.txtView.Buffer.Text += Math.Round(result.Value.price * ETHSpot, 4).ToString() + "\n";

                    Thread.Sleep(400);
                }
                else
                {
                    this.txtView.Buffer.Text += Math.Round(result.Value.price, 2).ToString() + "\n";

                    Thread.Sleep(400);
                }

                Thread.Sleep(400);

                if (ct.IsCancellationRequested)
                {
                    break;
                }
            }

            this.workIsFinished = true;
        }


        /// <summary>
        ///     Do Function are functions that interact with the GUI, often by
        ///     working on data, then displaying it on the GUI application
        ///     This function request Best price for a specific <paramref name="currency"/>
        ///     and <paramref name="orderType"/> (The higher price if BUY or the smaller if SELL)
        ///     It then displays it on the <see cref="ViewBestPrice"/>
        /// </summary>
        /// <param name="currency">
        ///     Is selected by the user from <see cref="comboBoxBestPriceCryptos"/>.
        ///     It have to be in <see cref="MainCryptos"/>
        /// </param>
        /// <param name="orderType">
        ///     Is selected by the user from <see cref="comboBoxPriceOrderType"/>.
        ///     It have to be in <see cref="OrderType"/>
        /// </param>
        public void DoPrintBestPrice(string currency, string orderType, CancellationToken ct)
        {
            double BTCUSD = 0;
            double ETHUSD = 0;
            Price cryptoPrice;
            BTCUSD = this.cryptoTrader.GetSpotPrice(MainCryptos.BTC.ToString());
            ETHUSD = this.cryptoTrader.GetSpotPrice(MainCryptos.ETH.ToString());
            cryptoPrice = this.cryptoTrader.GetBestPrice(currency, orderType, ct);
            this.workIsFinished = true;

            Thread.Sleep(400);

            if (cryptoPrice.error != true)
            {
                this.labelCol2.Text = "\n\n";
                this.labelCol3.Text = "Best " + orderType + " Price\n\n";
                this.labelCol4.Text = "\n\n";

                Thread.Sleep(400);

                this.labelCol2.Text += "Base Currency :\n";
                this.labelCol3.Text += "\n";
                this.labelCol4.Text += cryptoPrice.ccyBase + "\n";

                Thread.Sleep(400);

                this.labelCol2.Text += "Exchange :\n";
                this.labelCol3.Text += "\n";
                this.labelCol4.Text += cryptoPrice.exchange + "\n";

                Thread.Sleep(400);

                this.labelCol2.Text += "Paire Currency :\n";
                this.labelCol3.Text += "\n";
                this.labelCol4.Text += cryptoPrice.ccyPair + "\n";

                Thread.Sleep(400);

                this.labelCol2.Text += "Market Price :\n";
                this.labelCol3.Text += "\n";
                this.labelCol4.Text += cryptoPrice.price.ToString("F5") + "\n";

                Thread.Sleep(400);

                this.labelCol2.Text += "USD Price :\n";
                this.labelCol3.Text += "\n";

                if (cryptoPrice.ccyPair == MainCryptos.BTC.ToString())
                {
                    this.labelCol4.Text += (cryptoPrice.price * BTCUSD).ToString() + "\n";
                }
                else if (cryptoPrice.ccyPair == MainCryptos.ETH.ToString())
                {
                    this.labelCol4.Text += (cryptoPrice.price * ETHUSD).ToString() + "\n";
                }
                else
                {
                    this.labelCol4.Text += (cryptoPrice.price).ToString() + "\n";
                }

            }
            else
            {
                this.labelCol3.Text = "ERROR for " + cryptoPrice.exchange + "\nPRICE VALUE IS 0\n"; ;
            }

            this.workIsFinished = true;
        }
    }
}
