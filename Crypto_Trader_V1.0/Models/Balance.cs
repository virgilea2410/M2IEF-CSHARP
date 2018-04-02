using System;
using Crypto_Trader_V1.Models.My_CryptoTrader;


namespace Crypto_Trader_V1.Models
{


    /// <summary>
    ///     Class representing a crypto currency balance
    ///         <see cref="ccyBase"/> base crypto currency
    ///         <see cref="ccyPair"/> pair currency
    ///         <see cref="amount"/> total amount of <see cref="ccyBase"/> crypto currency
    ///         <see cref="totalValueBTC"/> Total value of the balance, in BTC units
    ///         <see cref="totalValueUSD"/> Total value of the balance, in USD units
    /// </summary>
    public class Balance
    {
        public My_CryptoTrader.My_CryptoTrader cryptoTrader;
        public MainCryptos ccyBase;
        public MainCryptos ccyPair;
        public double amount;
        public double BTCUSDPrice;
        public double ccyUSDPrice;
        public double totalValueBTC;
        public double totalValueUSD;


        /// <summary>
        ///     Constructor
        /// </summary>
        public Balance()
        {
            this.cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.ccyBase = MainCryptos.BTC;
            this.ccyPair = MainCryptos.BTC;
            this.amount = 0;
            this.totalValueBTC = 0;
            this.totalValueUSD = 0;
            this.ccyUSDPrice = 0;
            this.BTCUSDPrice = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Balance(MainCryptos ccyBase)
        {
            this.cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.ccyBase = ccyBase;
            this.ccyPair = MainCryptos.BTC;
            this.amount = 0;
            this.totalValueBTC = 0;
            this.totalValueUSD = 0;
            this.ccyUSDPrice = 0;
            this.BTCUSDPrice = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Balance(MainCryptos ccyBase, MainCryptos ccyPair)
        {
            this.cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.ccyBase = ccyBase;
            this.ccyPair = ccyPair;
            this.amount = 0;
            this.totalValueBTC = 0;
            this.totalValueUSD = 0;
            this.ccyUSDPrice = 0;
            this.BTCUSDPrice = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Balance(MainCryptos ccyBase, MainCryptos ccyPair, double amount)
        {
            this.cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.ccyBase = ccyBase;
            this.ccyPair = ccyPair;
            this.amount = amount;
            this.ccyUSDPrice = this.cryptoTrader.GetSpotPrice(this.ccyBase.ToString());
            this.BTCUSDPrice = this.cryptoTrader.GetSpotPrice(MainCryptos.BTC.ToString());

            this.totalValueUSD = amount * this.ccyUSDPrice;
            this.totalValueBTC = (amount * this.ccyUSDPrice) / (this.BTCUSDPrice);
        }
    }
}
