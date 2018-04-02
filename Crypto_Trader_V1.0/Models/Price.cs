using System;


namespace Crypto_Trader_V1.Models
{


    /// <summary>
    ///     Class representing a Price
    ///         <see cref="exchange"/> from which is get the exchange
    ///         <see cref="ccyBase"/> base crypto currency of the price
    ///         <see cref="ccyBase"/> pair currency of the price
    ///         <see cref="price"/> market price
    ///         <see cref="error"/> true if there is an pricing error with this price
    /// </summary>
    public class Price
    {
        public string exchange;
        public string ccyBase;
        public string ccyPair;
        public string market;
        public double price;
        public bool error;


        /// <summary>
        ///     Constructor
        /// </summary>
        public Price()
        {
            this.exchange = Exchanges.BITTREX.ToString();
            this.ccyBase = MainCryptos.BTC.ToString();
            this.ccyPair = MainCryptos.USDT.ToString();
            this.market = ccyBase + "-" + ccyPair;
            this.price = 0.00;
            this.error = false;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Price(bool error, string exchange = "ERROR")
        {
            this.exchange = exchange;
            this.ccyBase = "ERROR";
            this.ccyPair = "ERROR";
            this.market = "ERROR";
            this.price = 0.00;
            this.error = error;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Price(string a_exchange)
        {
            this.exchange = a_exchange;
            this.ccyBase = MainCryptos.BTC.ToString();
            this.ccyPair = MainCryptos.USDT.ToString();
            this.market = ccyBase + "-" + ccyPair;
            this.price = 0.00;
            this.error = false;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Price(string a_exchange, string a_ccyBase)
        {
            this.exchange = a_exchange;
            this.ccyBase = a_ccyBase;
            this.ccyPair = MainCryptos.USDT.ToString();
            this.market = ccyBase + "-" + ccyPair;
            this.price = 0.00;
            this.error = false;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Price(string a_exchange, string a_ccyBase, string a_ccyPair)
        {
            this.exchange = a_exchange;
            this.ccyBase = a_ccyBase;
            this.ccyPair = a_ccyPair;
            this.market = ccyBase + "-" + ccyPair;
            this.price = 0.00;
            this.error = false;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Price(string a_exchange, string a_ccyBase, string a_ccyPair, double a_price)
        {
            this.exchange = a_exchange;
            this.ccyBase = a_ccyBase;
            this.ccyPair = a_ccyPair;
            this.market = ccyBase + "-" + ccyPair;
            this.price = a_price;
            this.error = false;
        }
    }
}
