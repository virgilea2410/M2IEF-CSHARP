using System;


namespace Crypto_Trader_V1.Models
{


    /// <summary>
    ///     Class representing an Order on a specific exchange
    ///         <see cref="exchange"/> exchange on which the order is placed, have to be in <see cref="Exchanges"/>
    ///         <see cref="side"/> side (BUY/SELL...) of the order, have to be in <see cref="OrderType"/>
    ///         <see cref="style"/> style (LIMIT/STOP_LOSS...) of the order, have to be in <see cref="OrderStyle"/>
    ///         <see cref="amount"/> total amount of the order
    ///         <see cref="ccyPair"/> pair currency of the order
    ///         <see cref="ccyBase"/> base crypto currency of the order
    ///         <see cref="orderPrice"/> <see cref="Price"/> object representing the order price
    ///         <see cref="success"/> true if the purpose of the order (BUY/SELL, CANCEL) has been successfully done
    ///         <see cref="ID"/> order ID
    ///         <see cref="valueBTC"/> total market of value of the order, in BTC units
    ///         <see cref="valueUSD"/> total market of value of the order, in USD units
    ///         <see cref="message"/> can reports various problems and errors, or even success message
    /// </summary>
    public class Order
    {
        public My_CryptoTrader.My_CryptoTrader cryptoTrader;
        public Exchanges exchange;
        public OrderType side;
        public OrderStyle style;
        public bool success;
        public MainCryptos ccyPair;
        public MainCryptos ccyBase;
        public double amount;
        public Price orderPrice;
        public string message;
        public string ID;
        public double valueBTC;
        public double valueUSD;


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order()
        {
            cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.exchange = Exchanges.BITTREX;
            this.side = OrderType.BUY;
            this.style = OrderStyle.LIMIT;
            this.success = false;
            this.ccyPair = MainCryptos.USDT;
            this.ccyBase = MainCryptos.BTC;
            this.amount = 0;
            this.orderPrice = new Price(error: true);
            this.message = "";
            this.ID = "";
            this.valueBTC = 0;
            this.valueUSD = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order(bool error)
        {
            if (error == true)
            {
                cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
                this.exchange = Exchanges.BITTREX;
                this.side = OrderType.BUY;
                this.style = OrderStyle.LIMIT;
                this.success = false;
                this.ccyPair = MainCryptos.USDT;
                this.ccyBase = MainCryptos.BTC;
                this.amount = 0;
                this.orderPrice = new Price(error: true);
                this.message = "ERROR";
                this.ID = "ERROR";
                this.valueBTC = 0;
                this.valueUSD = 0;
            }
            else
            {
                cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
                this.exchange = Exchanges.BITTREX;
                this.side = OrderType.BUY;
                this.style = OrderStyle.LIMIT;
                this.success = false;
                this.ccyPair = MainCryptos.USDT;
                this.ccyBase = MainCryptos.BTC;
                this.amount = 0;
                this.orderPrice = new Price(error: true);
                this.message = "";
                this.ID = "";
                this.valueBTC = 0;
                this.valueUSD = 0;
            }
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order(Exchanges exchange)
        {
            cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.exchange = exchange;
            this.side = OrderType.BUY;
            this.style = OrderStyle.LIMIT;
            this.success = false;
            this.ccyPair = MainCryptos.USDT;
            this.ccyBase = MainCryptos.BTC;
            this.amount = 0;
            this.orderPrice = new Price(error: true);
            this.message = "";
            this.ID = "";
            this.valueBTC = 0;
            this.valueUSD = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order(Exchanges exchange, OrderType side)
        {
            cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.exchange = exchange;
            this.side = side;
            this.style = OrderStyle.LIMIT;
            this.success = false;
            this.ccyPair = MainCryptos.USDT;
            this.ccyBase = MainCryptos.BTC;
            this.amount = 0;
            this.orderPrice = new Price(error: true);
            this.message = "";
            this.ID = "";
            this.valueBTC = 0;
            this.valueUSD = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order(Exchanges exchange, OrderType side, OrderStyle style)
        {
            cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.exchange = exchange;
            this.side = side;
            this.style = style;
            this.success = false;
            this.ccyPair = MainCryptos.USDT;
            this.ccyBase = MainCryptos.BTC;
            this.amount = 0;
            this.orderPrice = new Price(error: true);
            this.message = "";
            this.ID = "";
            this.valueBTC = 0;
            this.valueUSD = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order(Exchanges exchange, OrderType side, OrderStyle style, bool success)
        {
            cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.exchange = exchange;
            this.side = side;
            this.style = style;
            this.success = success;
            this.ccyPair = MainCryptos.USDT;
            this.ccyBase = MainCryptos.BTC;
            this.amount = 0;
            this.orderPrice = new Price(error: true);
            this.message = "";
            this.ID = "";
            this.valueBTC = 0;
            this.valueUSD = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order(Exchanges exchange, OrderType side, OrderStyle style, bool success, MainCryptos ccyBase)
        {
            cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.exchange = exchange;
            this.side = side;
            this.style = style;
            this.success = success;
            this.ccyPair = MainCryptos.USDT;
            this.ccyBase = ccyBase;
            this.amount = 0;
            this.orderPrice = new Price(error: true);
            this.message = "";
            this.ID = "";
            this.valueBTC = 0;
            this.valueUSD = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order(Exchanges exchange, OrderType side, OrderStyle style, bool success, MainCryptos ccyBase, MainCryptos ccyPair)
        {
            cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.exchange = exchange;
            this.side = side;
            this.style = style;
            this.success = success;
            this.ccyPair = ccyPair;
            this.ccyBase = ccyBase;
            this.amount = 0;
            this.orderPrice = new Price(error: true);
            this.message = "";
            this.ID = "";
            this.valueBTC = 0;
            this.valueUSD = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order(Exchanges exchange, OrderType side, OrderStyle style, bool success, MainCryptos ccyBase, MainCryptos ccyPair, double amount)
        {
            cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.exchange = exchange;
            this.side = side;
            this.style = style;
            this.success = success;
            this.ccyPair = ccyPair;
            this.ccyBase = ccyBase;
            this.amount = amount;
            this.orderPrice = new Price(this.exchange.ToString(), this.ccyBase.ToString(),
                                        this.ccyPair.ToString(), this.cryptoTrader.GetSpotPrice(this.ccyBase.ToString()));
            this.message = "";
            this.ID = "";
            this.valueBTC = 0;
            this.valueUSD = 0;
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order(Exchanges exchange, OrderType side, OrderStyle style, bool success, MainCryptos ccyBase, MainCryptos ccyPair, double amount, double price)
        {
            cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.exchange = exchange;
            this.side = side;
            this.style = style;
            this.success = success;
            this.ccyPair = ccyPair;
            this.ccyBase = ccyBase;
            this.amount = amount;
            this.orderPrice = new Price(this.exchange.ToString(), this.ccyBase.ToString(),
                                        this.ccyPair.ToString(), price);
            this.message = "";
            this.ID = "";

            if (this.ccyPair == MainCryptos.BTC)
            {
                this.valueBTC = this.amount * this.cryptoTrader.GetSpotPrice(this.ccyBase.ToString());
                this.valueUSD = this.amount * this.cryptoTrader.GetSpotPrice(this.ccyBase.ToString()) * this.cryptoTrader.GetSpotPrice(MainCryptos.BTC.ToString());
            }
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Order(Exchanges exchange, OrderType side, OrderStyle style, bool success, MainCryptos ccyBase, MainCryptos ccyPair, double amount, double price, string ID)
        {
            cryptoTrader = new My_CryptoTrader.My_CryptoTrader();
            this.exchange = exchange;
            this.side = side;
            this.style = style;
            this.success = success;
            this.ccyPair = ccyPair;
            this.ccyBase = ccyBase;
            this.amount = amount;
            this.orderPrice = new Price(this.exchange.ToString(), this.ccyBase.ToString(),
                                        this.ccyPair.ToString(), price);
            this.message = "";
            this.ID = ID;

            if (this.ccyPair == MainCryptos.BTC)
            {
                this.valueBTC = this.amount * this.cryptoTrader.GetSpotPrice(this.ccyBase.ToString());
                this.valueUSD = this.amount * this.cryptoTrader.GetSpotPrice(this.ccyBase.ToString()) * this.cryptoTrader.GetSpotPrice(MainCryptos.BTC.ToString());
            }
        }
    }
}
