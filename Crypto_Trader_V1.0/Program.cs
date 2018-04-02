using System;
using Crypto_Trader_V1.ApplicationForm;

namespace Crypto_Trader
{


    /// <summary>
    ///     Class representing the entry point of the program
    /// </summary>
    public class MainClass
    {

        /// <summary>
        ///     Function representing the entry point of the program
        /// </summary>
        public static void Main(string[] args)
        {
            Program.Run();
        }
    } 


    /// <summary>
    ///     Class representing the GUI application
    /// </summary>
    public class Program
    {


        /// <summary>
        ///     Function that run the GUI application
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("Lancement de CRYPTO TRADER V 1.0 - 2018");

            //Initialisation de GTKSharp
            Gtk.Application.Init();

            ApplicationForm my_Main_Form = new ApplicationForm();
            //var my_Main_Form = new MainForm();

            //Lancement du 'loopback' principal
            Gtk.Application.Run();

            Console.WriteLine("Arret de CRYPTO TRADER V 1.0 - 2018");
            //////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////
            //////////////////////////////////////////////////////////////////

            //var BTCHistorical = My_CryptoCompare.GetHistoricalData("BTC", "EUR", 200);
            //My_CryptoCompare.ExportHistoricalData("BTC", "EUR", 600);
            //string path2graph = My_Graph.CreateGraph("BTC", "USD", 400);
            //System.Collections.Generic.Dictionary<string, double> dicoStats = My_CryptoCompare.getCryptoStats("BTC", "BINANCE", 10);

            //string sucess, message, adress, orderId;
            //System.Collections.Generic.Dictionary<dynamic, dynamic> portfolio =
            //    new System.Collections.Generic.Dictionary<dynamic, dynamic>();
            //System.Collections.Generic.Dictionary<dynamic, dynamic> openOrder =
            //    new System.Collections.Generic.Dictionary<dynamic, dynamic>();

            //Crypto_Trader_V1.ExchangesModels.My_Coinbase.My_Coinbase coinbase = new Crypto_Trader_V1.ExchangesModels.My_Coinbase.My_Coinbase();
            //////coinbase.GetAccountId();
            //////(sucess, message) = coinbase.buy("BTC", 1, 10000);
            //////(sucess, message) = coinbase.sell("BTC", 1, 10000);
            //////adress = coinbase.getAdress("BTC");
            //////portfolio = coinbase.GetFolio();
            //////openOrder = coinbase.GetOpenOrders("BTC");
            //////orderId = coinbase.GetOrderId("BTC");
            ////(sucess, message) = coinbase.GetPaymentMethod("EUR");
            //(sucess, message) = coinbase.SendCryptos("ADni294nN9n2IDe92ND02ndd342", "BTC", 1);

            //Crypto_Trader_V1.ExchangesModels.My_CexIO.My_CexIO cexio = new Crypto_Trader_V1.ExchangesModels.My_CexIO.My_CexIO();
            //////cexio.buy("BTC", 100, 10000);
            //////string t = cexio.getAdress("BTC");
            //////portfolio = cexio.GetFolio();
            //////openOrder = cexio.GetOpenOrders("BTC");
            //////orderId = cexio.GetOrderId("BTC");
            //(sucess, message) = cexio.SendCryptos("IJDLARNDA", "BTC", 1);

            //Crypto_Trader_V1.ExchangesModels.My_Binance.My_Binance binance = new Crypto_Trader_V1.ExchangesModels.My_Binance.My_Binance();
            //////adress = binance.getAdress("BTC");
            //////portfolio = binance.GetFolio();
            //////openOrder = binance.GetOpenOrders("BTC");
            //////orderId = binance.GetOrderId("BTC");
            //(sucess, message) = binance.SendCryptos("IJDLARNDA", "BTC", 1);
            //binance.GetPrice("TRX");

            //Crypto_Trader_V1.ExchangesModels.My_Bitfinex.My_Bitfinex bitfinex = new Crypto_Trader_V1.ExchangesModels.My_Bitfinex.My_Bitfinex();
            ////(sucess, message) = bitfinex.buy("BTC", 0.23, 10000);
            ////adress = bitfinex.getAdress("BTC");
            ////portfolio = bitfinex.GetFolio();
            ////openOrder = bitfinex.GetOpenOrders("BTC");
            ////orderId = bitfinex.GetOrderId("BTC");
            //(sucess, message) = bitfinex.SendCryptos("IJDLARNDA", "BTC", 1);
            //var price = bitfinex.GetPrice("BCH");

            //Crypto_Trader_V1.ExchangesModels.My_Bittrex.My_Bittrex bittrex = new Crypto_Trader_V1.ExchangesModels.My_Bittrex.My_Bittrex();
            //////adress = bittrex.getAdress("BTC");
            ////openOrder = bittrex.GetOpenOrders();
            ////orderId = bittrex.GetOrderId("BTC");
            //(sucess, message) = bittrex.SendCryptos("IJDLARNDA", "BTC", 1);
        }
    }
}