using System.Collections.Generic;

namespace Crypto_Trader_V1.Models
{


    /// <summary>
    ///     Enum containing main cryptocurrencies in term of market cap
    /// </summary>
    public enum MainCryptos
    {
        BTC,
        ETH,
        XRP,
        BCH,
        ADA,
        LTC,
        NEO,
        XLM,
        EOS,
        IOTA,
        DASH,
        XMR,
        TRX,
        ETC,
        USDT,
        BTG,
        ZEC,
        ZCL,
        OMG,
        XVG,
        IGNIS,
    }


    /// <summary>
    ///     Enum containing main currencies
    /// </summary>
    public enum MainCurrency
    {
        USD,
        EUR,
        GBP,
        CAD,
    }


    /// <summary>
    ///     Enum containing order types
    /// </summary>
    public enum OrderType
    {
        BUY,
        SELL,
        SEND,
        CANCEL,
    }


    /// <summary>
    ///     Enum containing order styles (Limit, market, etc)
    /// </summary>
    public enum OrderStyle
    {
        LIMIT,
        MARKET,
        STOP_LOSS,
        WITHDRAWAL,
        CANCELLATION,
    }


    /// <summary>
    ///     Enum containing main cryptocurrency exchanges/markets
    /// </summary>
    public enum Exchanges
    {
        BITTREX,
        BINANCE,
        BITFINEX,
        CEXIO,
        COINBASE,
        //GDAX,
        //KRAKEN,
        //COINMAMA,
        //LOCALBITCOINS,
        //POLONIEX,
        //GEMINI,
        //BISQ,
        //BITSTAMP,
        //BIBOX,
        //GATEIO,
        //YOBIT,
        //BITMEX,
        //CHANGELLY,
        //UPCOIN,
        //KUCOIN,
        //CRYPTOPIA
    }


    /// <summary>
    ///     Class representing the Constants of the program
    /// </summary>
    public class MyConstants
    {
        public string ERROR = "ERROR";


        /// <summary>
        ///     Dictionary containing transco between ticker and real name
        ///     for main cryptocurencies
        /// </summary>
        public static Dictionary<string, string> MainCryptosDico = new Dictionary<string, string>{
            {"BTC", "BITCOIN"},
            {"ETH", "ETHEREUM"},
            {"XRP", "RIPPLE"},
            {"BCH", "BITCOIN CASH"},
            {"ADA", "CARDANO"},
            {"LTC", "LITECOIN"},
            {"NEO", "NEO"},
            {"XLM", "STELLAR LUMENS"},
            {"EOS", "EOS"},
            {"IOTA", "IOTA"},
            //{"NEM", "NEM"},
            {"DASH", "DASH"},
            {"XMR", "MONERO"},
            {"TRX", "TRON"},
            {"ETC", "ETHEREUM CLASSIC"},
            {"USDT", "TETHER"},
            {"BTG", "BITCOIN GOLD"},
            {"ZEC", "ZCASH"},
            {"ZCL", "ZCLASSIC"},
            {"OMG", "OMISEGO"},
            {"XVG", "VERGE"},
            {"IGNIS", "IGNIS"},
        };


        /// <summary>
        ///     Dictionary containing transco between ticker and real name
        ///     for main curencies
        /// </summary>
        public static Dictionary<string, string> MainCurrencyDico = new Dictionary<string, string>{
            {"USD", "DOLLARS"},
            {"EUR", "EURO"},
            {"GBP", "STERLINGS"},
            {"CAD", "CANADIAN DOLLARS"},
        };
    }
}
