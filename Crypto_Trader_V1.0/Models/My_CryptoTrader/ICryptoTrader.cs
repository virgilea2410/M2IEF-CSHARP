using System;
using System.Collections.Generic;
using System.Threading;


namespace Crypto_Trader_V1.Models.My_CryptoTrader
{


    /// <summary>
    ///     Interface representing a Robot Trader
    ///         Our Crypto Trader class will inherits and implements this interface
    ///         As an automated trader, it have, at least, to implement the following methods
    /// </summary>
    public interface ICryptoTrader
    {
        Dictionary<string, Dictionary<string, string>> GetHistoricalData(string ccyBase, string ccyPair, int nbDays);
        (string, int) ExportHistoricalData(string ccyBase, string ccyPair, int nbDays);
        Dictionary<string, double> ComputeStats(string ccy, string exchange, int nbDays);
        double GetSpotPrice(string currency);
        Price GetBestPrice(string currency, string orderType, CancellationToken ct);
        SortedDictionary<string, Price> GetAllPrices(string currency, CancellationToken ct);
    }
}
