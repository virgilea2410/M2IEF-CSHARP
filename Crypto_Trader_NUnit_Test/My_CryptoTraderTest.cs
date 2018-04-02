using System;
using System.Collections.Generic;
using Crypto_Trader_V1.Models.My_CryptoTrader;
using Crypto_Trader_V1.Models;
using NUnit.Framework;


namespace Crypto_Trader_NUnit_Test
{
    /// <summary>
    ///     Unit Test Class for My_CryptoTrader module from Models
    /// </summary>
    [TestFixture()]
    public class My_CryptoTraderTest
    {
        My_CryptoTrader cryptoTrader;
        int numTest = 1;
        double marginError = 0.05;

        [SetUp()]
        public void My_CryptoTraderTest_Init()
        {
            Console.WriteLine("------ My_CryptoTrader Unit Test N° " + numTest + "------");
            this.cryptoTrader = new My_CryptoTrader();
            numTest++;
        }

        [Test()]
        public void GetHistoricalData_Test()
        {
            Dictionary<string, Dictionary<string, string>> dicoResults;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                dicoResults = this.cryptoTrader.GetHistoricalData(crypto, MainCurrency.USD.ToString(), 100);
                Assert.True(dicoResults[DateTime.Now.ToString("dd/MM/yyyy")]["close"] != "ERROR");
            }
        }

        [Test()]
        public void ExportHistoricalData_Test()
        {
            string result;
            int nbRows;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                (result, nbRows) = this.cryptoTrader.ExportHistoricalData(crypto, MainCurrency.USD.ToString(), 100);
                Assert.Greater(nbRows, 0);
            }
        }

        [Test()]
        public void ComputeStats_Test()
        {
            string result;
            Dictionary<string, double> dicoStats;
            int nbRows;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                foreach (string exchange in Enum.GetNames(typeof(Exchanges)))
                {
                    dicoStats = this.cryptoTrader.ComputeStats(crypto, exchange, 100);
                }
            }
        }

        [Test()]
        public void GetSpotPrice_Test()
        {
            string result;
            double spotPrice;

            foreach (string crypto in Enum.GetNames(typeof(MainCryptos)))
            {
                spotPrice = this.cryptoTrader.GetSpotPrice(crypto);
            }
        }

        [Test()]
        public void GetBestPrice_Test()
        {
            string orderType = "BUY";
            string currency = "BTC";

            Price bestPrice = this.cryptoTrader.GetBestPrice(currency, orderType,
                                               new System.Threading.CancellationToken());
            SortedDictionary<string, Price> allPrices = this.cryptoTrader.GetAllPrices(currency,
                                                                         new System.Threading.CancellationToken());

            if (orderType == "BUY")
            {
                foreach (Price price in allPrices.Values)
                {
                    Assert.True(bestPrice.price <= price.price * (1 + this.marginError),
                                "Best buy price should be the min price of all prices");
                }
            }
            else if (orderType == "SELL")
            {
                foreach (Price price in allPrices.Values)
                {
                    Assert.True(bestPrice.price >= price.price * (1 - this.marginError),
                                "Best sell price should be the max of all prices");
                }
            }
        }

        [Test()]
        public void GetAllPrices_TestSorted()
        {
            string currency = "BTC";

            SortedDictionary<string, Price> allPrices = this.cryptoTrader.GetAllPrices(currency,
                                                                         new System.Threading.CancellationToken());

            double prevPrice = 100000000;
            foreach (Price price in allPrices.Values)
            {
                Assert.True(price.price <= prevPrice * (1 + this.marginError),
                            "Price are supposed to be sorted but previous price is inferior to current price");
                prevPrice = price.price;
            }
        }
    }
}
