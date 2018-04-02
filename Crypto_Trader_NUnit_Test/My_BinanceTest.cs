using System;
using System.Collections.Generic;
using Crypto_Trader_V1.ExchangesModels.My_Binance;
using Crypto_Trader_V1.Models;
using NUnit.Framework;

namespace Crypto_Trader_NUnit_Test
{
    /// <summary>
    ///     Unit Test Class for My_Binance module from ExchangesModels
    /// </summary>
    [TestFixture()]
    public class My_BinanceTest
    {
        My_Binance binance;
        int numTest = 1;
        double marginError = 0.05;

        [SetUp()]
        public void My_BinanceTest_Init()
        {
            Console.WriteLine("------ My_Binance Unit Test N° " + numTest + "------");
            this.binance = new My_Binance();
            numTest++;
        }

        [Test()]
        public void GetPrice_Test()
        {
            Price onePrice;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                onePrice = this.binance.GetPrice(crypto);
                Assert.False(onePrice.error && onePrice.price > 0, "Price.error for BTC price should not be true");
            }
        }

        [Test()]
        public void GetPrice_TestFake()
        {
            Price onePrice;
            onePrice = this.binance.GetPrice("fake price");

            Assert.True(onePrice.error && onePrice.price == 0, "Price.error for fake price should be true");
        }

        [Test()]
        public void GetFolio_Test()
        {
            Dictionary<dynamic, dynamic> dicoFolio = new Dictionary<dynamic, dynamic>();
            List<Dictionary<dynamic, dynamic>> dicoFolioValues = new List<Dictionary<dynamic, dynamic>>();
            double totalFolio = 0.00;

            dicoFolio = this.binance.GetFolio();

            foreach (KeyValuePair<dynamic, dynamic> row in dicoFolio)
            {
                var dicoVal = row.Value;

                foreach (var val in dicoVal)
                {
                    if ((string)val.Key == "total USD value")
                    {
                        totalFolio += val.Value;
                    }
                }
            }

            Assert.AreNotEqual(totalFolio, 0, "Total Portfolio value should not be equal zero");
        }

        [Test()]
        public void GetAdress_Test()
        {
            string adress;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                if (crypto == MainCryptos.BCH.ToString())
                {
                    adress = this.binance.GetAdress("BCC");
                }
                else
                {
                    adress = this.binance.GetAdress(crypto);
                }

                if (crypto != MainCryptos.EOS.ToString() && crypto != MainCryptos.IOTA.ToString() &&
                    crypto != MainCryptos.ADA.ToString() && crypto != MainCryptos.USDT.ToString() &&
                    crypto != MainCryptos.TRX.ToString())
                {
                    Assert.True(adress != "", "Deposit adress for " + crypto + " should not be equal zero");
                    Assert.True(adress != "ERROR", "Deposit adress for " + crypto + " should not be equal ERROR");
                }
            }
        }

        [Test()]
        public void GetAdress_TestFake()
        {
            string adress;

            adress = this.binance.GetAdress("fake crypto");
            Assert.True(adress == "ERROR", "Deposit adress for fake crypto should be equal ERROR");
        }

        [Test()]
        public void Buy_test()
        {
            string sucess, message;
            double amount;
            double spotPrice;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                spotPrice = this.binance.GetPrice(crypto).price;
                if (crypto != MainCryptos.BTC.ToString())
                {
                    amount = 1 / spotPrice;
                }
                else
                {
                    amount = 1;
                }

                if (crypto == MainCryptos.BCH.ToString())
                {
                    (sucess, message) = this.binance.Buy("BCC", amount, spotPrice);
                }
                else
                {
                    (sucess, message) = this.binance.Buy(crypto, amount, spotPrice);
                }

                if (crypto != MainCryptos.EOS.ToString() && crypto != MainCryptos.IOTA.ToString() &&
                    crypto != MainCryptos.ADA.ToString() && crypto != MainCryptos.USDT.ToString() &&
                    crypto != MainCryptos.TRX.ToString())
                {
                    Assert.True(sucess.ToLower() == "false", "sucess message for buying " + crypto + " should be false");
                    Assert.True(message.ToLower() == "insufficient_funds",
                                "message for buying " + crypto + " should indicate insufficient funds");
                }
            }
        }

        [Test()]
        public void Sell_test()
        {
            string sucess, message;
            double amount;
            double spotPrice;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                spotPrice = this.binance.GetPrice(crypto).price;
                if (crypto != MainCryptos.BTC.ToString())
                {
                    amount = 1 / spotPrice;
                }
                else
                {
                    amount = 1;
                }

                if (crypto == MainCryptos.BCH.ToString())
                {
                    (sucess, message) = this.binance.Sell("BCC", amount, spotPrice);
                }
                else
                {
                    (sucess, message) = this.binance.Sell(crypto, amount, spotPrice);
                }

                if (crypto != MainCryptos.EOS.ToString() && crypto != MainCryptos.IOTA.ToString() &&
                    crypto != MainCryptos.ADA.ToString() && crypto != MainCryptos.USDT.ToString() &&
                    crypto != MainCryptos.TRX.ToString())
                {
                    Assert.True(sucess.ToLower() == "false", "sucess message for selling " + crypto + " should be false");
                    Assert.True(message.ToLower() == "insufficient_funds",
                                "message for selling " + crypto + " should indicate insufficient funds");
                }
            }
        }

        [Test()]
        public void GetAccountInfos_Test()
        {
            Dictionary<string, string> accountInfos;

            accountInfos = this.binance.GetAccountInfos();

            Assert.True(accountInfos.Count > 0, "Account Informations Dictionary size should greater than 0");
            Assert.True(accountInfos.ContainsKey("key"), "Account Informations Dictionary should contain the key 'key'");
            Assert.True(accountInfos.ContainsKey("secret"), "Account Informations Dictionary should contain the key 'secret'");
            Assert.True(accountInfos.ContainsKey("BTC Adress"), "Account Informations Dictionary should contain the key 'BTC Adress'");
            Assert.True(accountInfos.ContainsKey("ETH Adress"), "Account Informations Dictionary should contain the key 'ETH Adress'");
            Assert.True(accountInfos.ContainsKey("XRP Adress"), "Account Informations Dictionary should contain the key 'XRP Adress'");
            Assert.True(accountInfos.ContainsKey("LTC Adress"), "Account Informations Dictionary should contain the key 'LTC Adress'");
        }

        [Test()]
        public void CancelOrder_Test()
        {
            string sucess;
            string orderId = "1234567890";

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                sucess = this.binance.CancelOrder(crypto, orderId);
                Assert.True(sucess.ToLower() == "false", "sucess message should be false");
            }
        }

        [Test()]
        public void GetOrderId_Test()
        {
            string orderId;
            string ccy = "BTC";

            orderId = this.binance.GetOrderId(ccy);

            Assert.True(orderId == "ERROR", "orderId should be equal 'ERROR'");
        }

        [Test()]
        public void GetOpenOrders_Test()
        {
            Dictionary<dynamic, dynamic> openOrders;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                if (crypto == MainCryptos.BCH.ToString())
                {
                    openOrders = this.binance.GetOpenOrders("BCC");
                }
                else if (crypto != MainCryptos.EOS.ToString() && crypto != MainCryptos.IOTA.ToString() &&
                    crypto != MainCryptos.ADA.ToString() && crypto != MainCryptos.USDT.ToString() &&
                    crypto != MainCryptos.TRX.ToString())
                {
                    openOrders = this.binance.GetOpenOrders(crypto);
                    Assert.True(openOrders.Count == 0 || !openOrders.ContainsKey("ERROR"),
                            "Open Order Dictionary should be length zero or not contains 'ERROR' key");
                }
            }
        }

        [Test()]
        public void SendCryptos_Test()
        {
            string sucess, message;
            string adress = "1234567890";
            double amount;
            double spotPrice;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                spotPrice = this.binance.GetPrice(crypto).price;
                if (crypto != MainCryptos.BTC.ToString())
                {
                    amount = 1 / spotPrice;
                }
                else
                {
                    amount = 1;
                }

                if (crypto == MainCryptos.BCH.ToString())
                {
                    (sucess, message) = this.binance.SendCryptos(adress, "BCC", amount);
                }
                else
                {
                    (sucess, message) = this.binance.SendCryptos(adress, crypto, amount);
                }

                if (crypto != MainCryptos.EOS.ToString() && crypto != MainCryptos.IOTA.ToString() &&
                    crypto != MainCryptos.ADA.ToString() && crypto != MainCryptos.USDT.ToString() &&
                    crypto != MainCryptos.TRX.ToString())
                {
                    Assert.True(sucess.ToLower() == "false", "sucess message for sending " + crypto + " should be false");
                    Assert.True(message.ToLower() == "insufficient_funds",
                                "message for sending " + crypto + " should indicate insufficient funds");
                }
            }
        }
    }

}
