using System;
using System.Threading;
using System.Threading.Tasks;
using Gtk;
using Crypto_Trader_V1.ExchangesModels.My_Bittrex;
using Crypto_Trader_V1.ExchangesModels.My_Binance;
using Crypto_Trader_V1.ExchangesModels.My_Bitfinex;
using Crypto_Trader_V1.ExchangesModels.My_CexIO;
using Crypto_Trader_V1.ExchangesModels.My_Coinbase;
using Crypto_Trader_V1.Models;


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
        // Base attributs
        private Gdk.Rectangle allocation;
        private int desired_height;
        private int desired_width;

        // CryptoTrader
        private Models.My_CryptoTrader.My_CryptoTrader cryptoTrader;

        // Exchanges
        private My_Bittrex bittrex;
        private My_Binance binance;
        private My_Bitfinex bitfinex;
        private My_CexIO cexio;
        private My_Coinbase coinbase;

        // Logo Image
        private Gtk.Image imgLogo;
        private Gtk.Image imgLoadWheel;

        // Bool
        private bool workIsFinished;
        private bool withStats;

        // Image Buffer
        private Gdk.Pixbuf logoPixBuf;
        private Gdk.Pixbuf loaderPixBuf;

        // Background Color
        private Gdk.Color bkgColor;
        private Gdk.Color resultColor;
        private Gdk.Color fontColor;
        private Gdk.Color greenColorLight;
        private Gdk.Color greenColorStrong;
        private Gdk.Color redColorLight;
        private Gdk.Color redColorStrong;
        private Gdk.Color greyColorLight;
        private Gdk.Color greyColorStrong;
        private Gdk.Color yellowColorLight;
        private Gdk.Color yellowColorStrong;

        // Buttons
        private Gtk.Button buttonQuit;
        private Gtk.Button buttonHome;
        private Gtk.Button buttonActualize;
        private Gtk.Button buttonAccountInfos;
        private Gtk.Button buttonFolio;
        private Gtk.Button buttonOpenOrders;
        private Gtk.Button buttonDashboard;
        private Gtk.Button buttonPrice;
        private Gtk.Button buttonPriceStats;
        private Gtk.Button buttonOrder;
        private Gtk.Button buttonOrderMaxAmount;
        private Gtk.Button buttonOrderSetBP;
        private Gtk.Button buttonOrderSetBE;
        private Gtk.Button buttonCancelOrder;
        private Gtk.Button buttonCancelOrderGetId;
        private Gtk.Button buttonPlatformArbitrage;
        private Gtk.Button buttonBestPrice;
        private Gtk.Button buttonAllPrice;
        private Gtk.Button buttonSend;
        private Gtk.Button buttonSendMax;
        private Gtk.Button buttonSendAdress;
        private Gtk.Button buttonSendFromAdress;
        private Gtk.Button buttonExport;

        // Frame
        private Gtk.Frame labelFrame;

        // Event Box
        private Gtk.EventBox eventBoxLabelFrame;

        // Menu
            // Menu bar
        private Gtk.MenuBar menuBar;
            // Menu 
        private Gtk.Menu menuAccount;
        private Gtk.Menu menuTrade;
            // MenuItem
        private Gtk.MenuItem m_account;
        private Gtk.MenuItem m_trade;
        private Gtk.MenuItem m_infos;
        private Gtk.MenuItem m_folio;
        private Gtk.MenuItem m_sendnreceive;
        private Gtk.MenuItem m_openOrders;
        private Gtk.MenuItem m_exportPrices;
        private Gtk.MenuItem m_exit;
        private Gtk.MenuItem m_getOnePrice;
        private Gtk.MenuItem m_getBestPrice;
        private Gtk.MenuItem m_getAllPrice;
        private Gtk.MenuItem m_sellnbuy;
        private Gtk.MenuItem m_cancelOrder;
        private Gtk.MenuItem m_platform_arbitrage;

        // HBox, VBox
        private Gtk.HBox hBoxMainButton;
        private Gtk.VBox vBoxMain;
        private Gtk.HBox hBoxLoadWheel;
        private Gtk.HBox hBoxAccountInfosEntry;
        private Gtk.HBox hBoxAccountInfosButton;
        private Gtk.HBox hBoxFolioEntry;
        private Gtk.HBox hBoxFolioButton;
        private Gtk.HBox hBoxOpenOrdersEntry;
        private Gtk.HBox hBoxOpenOrdersButton;
        private Gtk.HBox hBoxPriceEntry;
        private Gtk.HBox hBoxPriceButton;
        private Gtk.VBox vBoxOrderEntry;
        private Gtk.HBox hBoxOrderMaxAmount;
        private Gtk.HBox hBoxOrderButton;
        private Gtk.HBox hBoxCancelOrderEntry;
        private Gtk.HBox hBoxCancelOrderButton;
        private Gtk.HBox hBoxPlatformArbitrageEntry;
        private Gtk.HBox hBoxPlatformArbitrageButton;
        private Gtk.HBox hBoxBestPriceEntry;
        private Gtk.HBox hBoxBestPriceButton;
        private Gtk.HBox hBoxAllPriceEntry;
        private Gtk.HBox hBoxAllPriceButton;
        private Gtk.VBox vBoxSendEntry;
        private Gtk.HBox hBoxSendAdress;
        private Gtk.HBox hBoxSendFromAdress;
        private Gtk.HBox hBoxSendButton;
        private Gtk.HBox hBoxLabel;
        private Gtk.VBox vBoxLabelCol1;
        private Gtk.VBox vBoxLabelCol2;
        private Gtk.VBox vBoxLabelCol3;
        private Gtk.VBox vBoxLabelCol4;
        private Gtk.VBox vBoxLabelCol5;
        private Gtk.HBox hBoxExportEntry;
        private Gtk.HBox hBoxExportButton;

        // Label
        private Gtk.Label labelCol1;
        private Gtk.Label labelCol2;
        private Gtk.Label labelCol3;
        private Gtk.Label labelCol4;
        private Gtk.Label labelCol5;

        // Text View
        private Gtk.TextView txtView;

        // Text Tags
        private TextTag bullTag;
        private TextTag bearTag;
        private TextTag flatTag;

        // Combo Box
        private Gtk.ComboBox comboBoxAccountInfosExchanges;
        private Gtk.ComboBox comboBoxFolioExchanges;
        private Gtk.ComboBox comboBoxOpenOrderExchanges;
        private Gtk.ComboBox comboBoxPriceExchanges;
        private Gtk.ComboBox comboBoxPriceCryptos;
        private Gtk.ComboBox comboBoxOrderCryptos;
        private Gtk.ComboBox comboBoxOrderExchanges;
        private Gtk.ComboBox comboBoxOrderType;
        private Gtk.ComboBox comboBoxCancelOrderExchanges;
        private Gtk.ComboBox comboBoxCancelOrderCryptos;
        private Gtk.ComboBox comboBoxPriceOrderType;
        private Gtk.ComboBox comboBoxBestPriceCryptos;
        private Gtk.ComboBox comboBoxAllPriceCryptos;
        private Gtk.ComboBox comboPlatformArbitrageCryptos;
        private Gtk.ComboBox comboSendCryptos;
        private Gtk.ComboBox comboBoxSendExchanges;
        private Gtk.ComboBox comboBoxSendFromExchanges;
        private Gtk.ComboBox comboExportCryptosBase;
        private Gtk.ComboBox comboExportCryptosPair;
        private Gtk.ComboBox comboExportCcyPair;
        private Gtk.ComboBox comboExportNbDays;
        private Gtk.ComboBox comboExportTypePair;

        // Iteration Tree (for getting values of Combo Boxes)
        private Gtk.TreeIter tree;

        // Entry
        private Gtk.Entry entryOrderPrice;
        private Gtk.Entry entryOrderAmount;
        private Gtk.Entry entrySendAdress;
        private Gtk.Entry entrySendAmount;
        private Gtk.Entry entryCancelOrderId;

        // Progress Bar
        private Gtk.ProgressBar oneProgressBar;

        // Font
        private Pango.FontDescription oneFont;

        // Separator
        private HSeparator loadSeparatorH;
        private HSeparator loadSeparatorL;

        // File Streams
        private byte[] streamLogo = System.IO.File.ReadAllBytes(Tools.GetDirectory() + "Static/Images/CryptoTrader-SlopeOpera-340x104.png");
        private byte[] streamWheel = System.IO.File.ReadAllBytes(Tools.GetDirectory() + "Static/Images/Spinner-105x105.png");

        // Cancelation Token to manage cancellation of Threads
        private CancellationTokenSource cancelTokenSource;
        private CancellationToken cancelToken;

        // DateTime objects to manage time
        private DateTime begin;
        private DateTime clock;

        // Thread/Tasks and task factory to manage them
        private Task task1;
        private Task task2;
        private TaskFactory taskFactory;
    }
}
