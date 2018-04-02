using System;
using System.Collections.Generic;
using Crypto_Trader_V1.Models;

namespace Crypto_Trader_V1.ExchangesModels
{


    /// <summary>
    ///     Abstract Class representing A Crypto Currency Exchange API Wrapper
    ///         All Crypto currency exchange (Bittrex, Binance, etc...) API Wrapper will
    ///         inherits this abstract class
    ///         and will have to implement following methods
    /// </summary>
    public abstract class My_ExchangeAPI
    {
        public abstract Price GetPrice(string currency);
        public abstract Order Buy(string currency, double amount, double price);
        public abstract Order Sell(string currency, double amount, double price);
        public abstract string GetAdress(string currency);
        public abstract Portfolio GetFolio();
        public abstract Dictionary<string, string> GetAccountInfos();
        public abstract Order CancelOrder(string currency, string orderId);
        public abstract string GetOrderId(string currency);
        public abstract List<Order> GetOpenOrders(string currency);
        public abstract Order SendCryptos(string adress, string currency, double amount);

        public virtual Order NewOrder(string currency, double amount, double price, string side)
        {
            OrderType oneOrderType = (OrderType)Enum.Parse(typeof(OrderType), side.ToUpper());
            OrderStyle oneOrderStyle = OrderStyle.LIMIT;
            Exchanges oneExchange = Exchanges.BINANCE;
            MainCryptos ccyBase = (MainCryptos)Enum.Parse(typeof(MainCryptos), currency.ToUpper());
            MainCryptos ccyPair = MainCryptos.USDT;
            Order oneNewOrder = new Order(oneExchange, oneOrderType, oneOrderStyle, false, ccyBase, ccyPair,amount , price);

            return oneNewOrder;
        }
    }
}
