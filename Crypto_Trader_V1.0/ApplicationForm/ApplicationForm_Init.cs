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
    public partial class ApplicationForm : Gtk.Window //On définit une nouvelle fenêtre par héritage
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        public ApplicationForm() : base("CRYPTO TRADER V 1.0 - 2018")
        {
            this.InitializeComponment();
        }


        /// <summary>
        ///     Function intializing all the widgets of the GUI Window 
        /// </summary>
        public void InitializeComponment()
        {
            int windowHeight = 400;
            int windowWidth = 400;

            /// <summary>
            ///     Top-Level Window caracteristics
            /// </summary>
            this.Name = "MainForm";
            this.SetSizeRequest(windowHeight, windowWidth);
            this.BorderWidth = 10;
            this.allocation = this.Allocation;
            this.desired_width = this.allocation.Width;
            this.desired_height = this.allocation.Height;

            /// <summary>
            ///     Color object used to represent a color
            ///     we use it as the background GUI application color
            /// </summary>
            this.bkgColor = new Gdk.Color(26, 26, 26);
            this.ModifyBg(StateType.Normal, this.bkgColor);

            /// <summary>
            ///     Cancellation Token Generator, used to cancel tasks 
            ///     When they took too much time, for example
            /// </summary>
            this.cancelTokenSource = new CancellationTokenSource();
            this.cancelToken = this.cancelTokenSource.Token;

            /// <summary>
            ///     2 variable representing begin and elapsed time
            /// </summary>
            this.begin = DateTime.Now;
            this.clock = DateTime.Now;

            /// <summary>
            ///     Task factory used to properly start different tasks
            /// </summary>
            this.taskFactory = new TaskFactory();

            /// <summary>
            ///     Font object used on all application writings 
            /// </summary>
            this.oneFont = Pango.FontDescription.FromString("Courier 12");
            oneFont.Weight = Pango.Weight.Bold;

            /// <summary>
            ///     Stream objects linking to images files (png),
            ///     and used to create Pixel Buffer 
            /// </summary>
            this.streamLogo = System.IO.File.ReadAllBytes(Tools.GetDirectory() + "Static/Images/CryptoTrader-SlopeOpera-340x104.png");
            this.streamWheel = System.IO.File.ReadAllBytes(Tools.GetDirectory() + "Static/Images/Spinner-105x105.png");

            /// <summary>
            ///     Pixel Buffer Created with the previously streamed image files
            ///     <c>ScaleSimple</c> is used to scale the image size properly
            /// </summary>
            this.logoPixBuf = new Gdk.Pixbuf(streamLogo);
            this.logoPixBuf.ScaleSimple(120, 550, Gdk.InterpType.Bilinear);
            this.loaderPixBuf = new Gdk.Pixbuf(streamWheel);
            this.loaderPixBuf = this.loaderPixBuf.AddAlpha(true, 200, 200, 200);

            /// <summary>
            ///     Images Widget objects, created from pixel buffer,
            ///     and used to display images on the GUI application
            /// </summary>
            this.imgLogo = new Gtk.Image(Stock.MediaPlay, IconSize.Button);
            this.imgLogo.Pixbuf = this.logoPixBuf;
            this.imgLogo.SetSizeRequest(windowHeight / 4, windowWidth / 4);
            this.imgLoadWheel = new Image(Stock.MediaPlay, IconSize.Button);
            this.imgLoadWheel.Pixbuf = this.loaderPixBuf;
            this.imgLoadWheel.SetSizeRequest(70, 70);

            /// <summary>
            ///     2 bool used to know when a task is finished
            ///     and pass this information through different methods for the first one
            ///     The seconde one is used to know wether a price have to be computed
            ///     with or without its statistics (fot the GetPrice function)
            /// </summary>
            this.workIsFinished = false;
            this.withStats = false;

            /// <summary>
            ///     Various Color objects :
            ///         - the color of the font
            ///         - the color for positive returns (green), negative (red) and flat (blue)
            ///         - the strong/light differentiation is to visually handle hoover events
            /// </summary>
            this.resultColor = new Gdk.Color(89, 89, 89);
            this.fontColor = new Gdk.Color(255, 255, 255);
            this.greenColorStrong = new Gdk.Color(46, 184, 46);
            this.greenColorLight = new Gdk.Color(133, 224, 133);
            this.redColorStrong = new Gdk.Color(230, 0, 0);
            this.redColorLight = new Gdk.Color(255, 102, 102);
            this.greyColorStrong = new Gdk.Color(77, 77, 77);
            this.greyColorLight = new Gdk.Color(140, 140, 140);
            this.yellowColorStrong = new Gdk.Color(255, 255, 51);
            this.yellowColorLight = new Gdk.Color(255, 255, 153);

            /// <summary>
            ///     Surronding our text-displaying widgets with Event Box 
            ///     Is necessary to bbe able to change their fonts, colors, etc
            /// </summary>
            this.eventBoxLabelFrame = new Gtk.EventBox();
            this.eventBoxLabelFrame.Add(this.labelFrame);
            this.eventBoxLabelFrame.Add(this.hBoxLabel);

            /// <summary>
            ///     The Crypto Trader Object :
            ///     It's the one that compute statistics,
            ///     Get best prices bby comparing exchanges prices,
            ///     Checking Arbitrage, etc...
            /// </summary>
            this.cryptoTrader = new Models.My_CryptoTrader.My_CryptoTrader();

            /// <summary>
            ///     The differents Exchanges API Wrapper Objects
            /// </summary>
            this.bittrex = new My_Bittrex();
            this.binance = new My_Binance();
            this.bitfinex = new My_Bitfinex();
            this.cexio = new My_CexIO();
            this.coinbase = new My_Coinbase();

            /// <summary>
            ///     Various Buttons Widgets
            ///     For each, we scale them, change their label fonts
            ///     and modify their backgrounds color
            /// </summary>
            this.buttonQuit = new Gtk.Button("Quitter");
            this.buttonQuit.SetSizeRequest(windowWidth / 5, windowHeight / 12);
            this.buttonQuit.ModifyFont(oneFont);
            this.buttonQuit.Child.ModifyFont(oneFont);
            this.buttonQuit.ModifyBg(StateType.Normal, greyColorStrong);
            this.buttonQuit.ModifyBg(StateType.Prelight, greyColorLight);

            this.buttonHome = new Gtk.Button("Home");
            this.buttonHome.SetSizeRequest(windowWidth / 5, windowHeight / 12);
            this.buttonHome.ModifyFont(oneFont);
            this.buttonHome.Child.ModifyFont(oneFont);
            this.buttonHome.ModifyBg(StateType.Normal, greyColorStrong);
            this.buttonHome.ModifyBg(StateType.Prelight, greyColorLight);

            this.buttonActualize = new Gtk.Button("Actualiser");
            this.buttonActualize.SetSizeRequest(windowWidth / 5, windowHeight / 12);
            this.buttonActualize.ModifyFont(oneFont);
            this.buttonActualize.Child.ModifyFont(oneFont);
            this.buttonActualize.ModifyBg(StateType.Normal, greyColorStrong);
            this.buttonActualize.ModifyBg(StateType.Prelight, greyColorLight);

            this.buttonDashboard = new Gtk.Button("DashBoard");
            this.buttonDashboard.SetSizeRequest(windowWidth / 5, windowHeight / 12);
            this.buttonDashboard.ModifyFont(oneFont);
            this.buttonDashboard.Child.ModifyFont(oneFont);
            this.buttonDashboard.ModifyBg(StateType.Normal, greyColorStrong);
            this.buttonDashboard.ModifyBg(StateType.Prelight, greyColorLight);

            this.buttonAccountInfos = new Gtk.Button("Get Account Infos");
            this.buttonAccountInfos.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonAccountInfos.Child.ModifyFont(oneFont);
            this.buttonAccountInfos.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonAccountInfos.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonFolio = new Gtk.Button("Get Portfolio");
            this.buttonFolio.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonFolio.Child.ModifyFont(oneFont);
            this.buttonFolio.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonFolio.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonOpenOrders = new Gtk.Button("Get Open Orders");
            this.buttonOpenOrders.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonOpenOrders.Child.ModifyFont(oneFont);
            this.buttonOpenOrders.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonOpenOrders.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonPrice = new Gtk.Button("Price");
            this.buttonPrice.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonPrice.Child.ModifyFont(oneFont);
            this.buttonPrice.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonPrice.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonPriceStats = new Gtk.Button("With Stats");
            this.buttonPriceStats.SetSizeRequest(windowWidth / 4, windowHeight / 12);
            this.buttonPriceStats.Child.ModifyFont(oneFont);
            this.buttonPriceStats.ModifyBg(StateType.Normal, this.yellowColorStrong);
            this.buttonPriceStats.ModifyBg(StateType.Prelight, this.yellowColorLight);

            this.buttonOrder = new Gtk.Button("Validate Order");
            this.buttonOrder.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonOrder.Child.ModifyFont(oneFont);
            this.buttonOrder.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonOrder.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonOrderMaxAmount = new Gtk.Button("Max Amount");
            this.buttonOrderMaxAmount.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonOrderMaxAmount.Child.ModifyFont(oneFont);
            this.buttonOrderMaxAmount.ModifyBg(StateType.Normal, this.yellowColorStrong);
            this.buttonOrderMaxAmount.ModifyBg(StateType.Prelight, this.yellowColorLight);

            this.buttonOrderSetBP = new Gtk.Button("Set Best Price");
            this.buttonOrderSetBP.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonOrderSetBP.Child.ModifyFont(oneFont);
            this.buttonOrderSetBP.ModifyBg(StateType.Normal, this.yellowColorStrong);
            this.buttonOrderSetBP.ModifyBg(StateType.Prelight, this.yellowColorLight);

            this.buttonOrderSetBE = new Gtk.Button("Set Best Exchange");
            this.buttonOrderSetBE.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonOrderSetBE.Child.ModifyFont(oneFont);
            this.buttonOrderSetBE.ModifyBg(StateType.Normal, this.yellowColorStrong);
            this.buttonOrderSetBE.ModifyBg(StateType.Prelight, this.yellowColorLight);

            this.buttonCancelOrder = new Gtk.Button("Cancel Order");
            this.buttonCancelOrder.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonCancelOrder.Child.ModifyFont(oneFont);
            this.buttonCancelOrder.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonCancelOrder.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonCancelOrderGetId = new Gtk.Button("Get Order ID");
            this.buttonCancelOrderGetId.SetSizeRequest(windowWidth / 4, windowHeight / 12);
            this.buttonCancelOrderGetId.Child.ModifyFont(oneFont);
            this.buttonCancelOrderGetId.ModifyBg(StateType.Normal, this.yellowColorStrong);
            this.buttonCancelOrderGetId.ModifyBg(StateType.Prelight, this.yellowColorLight);

            this.buttonPlatformArbitrage = new Gtk.Button("Check Platform Arbitrage");
            this.buttonPlatformArbitrage.SetSizeRequest(windowWidth / 2, windowHeight / 12);
            this.buttonPlatformArbitrage.Child.ModifyFont(oneFont);
            this.buttonPlatformArbitrage.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonPlatformArbitrage.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonBestPrice = new Gtk.Button("Get Best Price");
            this.buttonBestPrice.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonBestPrice.Child.ModifyFont(oneFont);
            this.buttonBestPrice.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonBestPrice.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonAllPrice = new Gtk.Button("Get All Prices");
            this.buttonAllPrice.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonAllPrice.Child.ModifyFont(oneFont);
            this.buttonAllPrice.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonAllPrice.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonSend = new Gtk.Button("Send Cryptos");
            this.buttonSend.SetSizeRequest(windowWidth / 4, windowHeight / 12);
            this.buttonSend.Child.ModifyFont(oneFont);
            this.buttonSend.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonSend.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonSendMax = new Gtk.Button("Max Amount");
            this.buttonSendMax.SetSizeRequest(windowWidth / 4, windowHeight / 12);
            this.buttonSendMax.Child.ModifyFont(oneFont);
            this.buttonSendMax.ModifyBg(StateType.Normal, this.yellowColorStrong);
            this.buttonSendMax.ModifyBg(StateType.Prelight, this.yellowColorLight);

            this.buttonExport = new Gtk.Button("Export Prices");
            this.buttonExport.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonExport.Child.ModifyFont(oneFont);
            this.buttonExport.ModifyBg(StateType.Normal, this.greenColorStrong);
            this.buttonExport.ModifyBg(StateType.Prelight, this.greenColorLight);

            this.buttonSendAdress = new Gtk.Button("Get Receiver Address");
            this.buttonSendAdress.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonSendAdress.Child.ModifyFont(oneFont);
            this.buttonSendAdress.ModifyBg(StateType.Normal, this.yellowColorStrong);
            this.buttonSendAdress.ModifyBg(StateType.Prelight, this.yellowColorLight);

            this.buttonSendFromAdress = new Gtk.Button("Max Amount");
            this.buttonSendFromAdress.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.buttonSendFromAdress.Child.ModifyFont(oneFont);
            this.buttonSendFromAdress.ModifyBg(StateType.Normal, this.yellowColorStrong);
            this.buttonSendFromAdress.ModifyBg(StateType.Prelight, this.yellowColorLight);

            /// <summary>
            ///     Text View : it's kind of a custom Label, used to personnalize the
            ///     displayed text.
            ///     We use it for Dashboard, in order to display coloured returns
            ///     when they are positive, negative, or flat
            /// </summary>
            this.txtView = new TextView();
            this.txtView.ModifyFont(this.oneFont);
            this.txtView.ModifyFg(StateType.Normal, this.fontColor);
            this.txtView.ModifyBg(StateType.Normal, this.bkgColor);
            this.txtView.ModifyText(StateType.Normal, this.fontColor);
            this.txtView.ModifyBase(StateType.Normal, this.bkgColor);

            /// <summary>
            ///     Text tags : they are used with a text view, when we insert a new piece
            ///     of text in the text view, we can also add a tag, which will define the style 
            ///     of the displayed text
            ///     We create 3 text tags object, one for each market direction :
            ///     positive (bull), negative (bear) and no direction (flat)
            /// </summary>
            this.bullTag = new TextTag("bull");
            this.bullTag.Weight = Pango.Weight.Bold;
            this.bullTag.Foreground = "green";
            this.txtView.Buffer.TagTable.Add(this.bullTag);

            this.bearTag = new TextTag("bear");
            this.bearTag.Weight = Pango.Weight.Bold;
            this.bearTag.Foreground = "red";
            this.txtView.Buffer.TagTable.Add(this.bearTag);

            this.flatTag = new TextTag("flat");
            this.flatTag.Weight = Pango.Weight.Bold;
            this.flatTag.Foreground = "blue";
            this.txtView.Buffer.TagTable.Add(this.flatTag);


            /// <summary>
            ///     Various label widget objects : they are used to simply
            ///     display some variable text on the GUI application
            ///     We created 5 label for 5 vertical columns, in order
            ///     to manage properly text disposal and position
            /// </summary>
            this.labelCol1 = new Gtk.Label();
            this.labelCol1.Text = "";
            this.labelCol1.ModifyFont(this.oneFont);

            this.labelCol2 = new Gtk.Label();
            this.labelCol2.Text = "";
            this.labelCol2.ModifyFont(this.oneFont);

            this.labelCol3 = new Gtk.Label();
            this.labelCol3.Text = "";
            this.labelCol3.ModifyFont(this.oneFont);

            this.labelCol4 = new Gtk.Label();
            this.labelCol4.Text = "";
            this.labelCol4.ModifyFont(this.oneFont);

            this.labelCol5 = new Gtk.Label();
            this.labelCol5.Text = "";
            this.labelCol5.ModifyFont(this.oneFont);

            this.labelCol1.ModifyFg(StateType.Normal, this.fontColor);
            this.labelCol2.ModifyFg(StateType.Normal, this.fontColor);
            this.labelCol3.ModifyFg(StateType.Normal, this.fontColor);
            this.labelCol4.ModifyFg(StateType.Normal, this.fontColor);
            this.labelCol5.ModifyFg(StateType.Normal, this.fontColor);

            /// <summary>
            ///     5 VBoxs (vertical boxes) used to wrap label
            ///     into proper columns
            /// </summary>
            this.vBoxLabelCol1 = new Gtk.VBox(true, 2);
            this.vBoxLabelCol1.PackStart(this.labelCol1, false, false, 2);
            this.vBoxLabelCol2 = new Gtk.VBox(true, 2);
            this.vBoxLabelCol2.PackStart(this.labelCol2, false, false, 2);
            this.vBoxLabelCol3 = new Gtk.VBox(true, 2);
            this.vBoxLabelCol3.PackStart(this.labelCol3, false, false, 2);
            this.vBoxLabelCol4 = new Gtk.VBox(true, 2);
            this.vBoxLabelCol4.PackStart(this.labelCol4, false, false, 2);
            this.vBoxLabelCol5 = new Gtk.VBox(true, 2);
            this.vBoxLabelCol5.PackStart(this.labelCol5, false, false, 2);
            this.vBoxLabelCol5 = new Gtk.VBox(true, 2);
            this.vBoxLabelCol5.PackStart(this.txtView, false, false, 2);

            /// <summary>
            ///     1 Global Label Hbox (horizontal boxes)
            ///     to Wrap the 5 Vertical Boxes into the GUI application
            /// </summary>
            this.hBoxLabel = new Gtk.HBox(true, 2);
            this.hBoxLabel.PackStart(this.vBoxLabelCol1, false, false, 2);
            this.hBoxLabel.PackStart(this.vBoxLabelCol2, false, false, 2);
            this.hBoxLabel.PackStart(this.vBoxLabelCol3, false, false, 2);
            this.hBoxLabel.PackStart(this.vBoxLabelCol4, false, false, 2);
            this.hBoxLabel.PackStart(this.vBoxLabelCol5, false, false, 2);

            /// <summary>
            ///     Used to wrap the H Box Label nicely :
            ///     ther is a border surronding the label box,
            ///     with shadow
            ///     and also a small title
            /// </summary>
            this.labelFrame = new Gtk.Frame();
            this.labelFrame.SetSizeRequest(windowWidth / 2, windowWidth / 2);
            this.labelFrame.Label = "OUTPUTS";
            this.labelFrame.LabelWidget.ModifyFont(oneFont);
            this.labelFrame.Add(this.hBoxLabel);
            this.labelFrame.Shadow = (ShadowType)4;
            this.labelFrame.Shadow = ShadowType.Out;

            /// <summary>
            ///     Iterator used to get active text in Combo Box Widgets
            /// </summary>
            this.tree = new TreeIter();

            /// <summary>
            ///     Combo Bbox : predefined Multiple-choice widgets
            ///     whith a nice scrolling list
            ///     They are used when the user have to choose between predefined choices :
            ///         - Exchanges platform
            ///         - Main Cryptocurrencies
            ///         - Order Type
            ///         - Main regular currencies (Fiat)
            /// </summary>
            this.comboBoxAccountInfosExchanges = new Gtk.ComboBox(new string[Enum.GetNames(typeof(Exchanges)).Length]);
            this.comboBoxAccountInfosExchanges.SetSizeRequest(windowWidth / 4, windowHeight / 10);
            this.comboBoxAccountInfosExchanges.Child.ModifyFont(oneFont);
            this.comboBoxAccountInfosExchanges.AppendText("Séléctionner la plateforme d'échange");
            this.comboBoxAccountInfosExchanges.AppendText("\t-\t");
            foreach (Exchanges exchange in Enum.GetValues(typeof(Exchanges)))
            {
                this.comboBoxAccountInfosExchanges.AppendText(exchange.ToString());
            }

            this.comboBoxOpenOrderExchanges = new Gtk.ComboBox(new string[Enum.GetNames(typeof(Exchanges)).Length]);
            this.comboBoxOpenOrderExchanges.SetSizeRequest(windowWidth / 4, windowHeight / 10);
            this.comboBoxOpenOrderExchanges.Child.ModifyFont(oneFont);
            this.comboBoxOpenOrderExchanges.AppendText("Séléctionner la plateforme d'échange");
            this.comboBoxOpenOrderExchanges.AppendText("\t-\t");
            foreach (Exchanges exchange in Enum.GetValues(typeof(Exchanges)))
            {
                this.comboBoxOpenOrderExchanges.AppendText(exchange.ToString());
            }

            this.comboBoxFolioExchanges = new Gtk.ComboBox(new string[Enum.GetNames(typeof(Exchanges)).Length]);
            this.comboBoxFolioExchanges.SetSizeRequest(windowWidth / 4, windowHeight / 10);
            this.comboBoxFolioExchanges.Child.ModifyFont(oneFont);
            this.comboBoxFolioExchanges.AppendText("Séléctionner la plateforme d'échange");
            this.comboBoxFolioExchanges.AppendText("\t-\t");
            foreach (Exchanges exchange in Enum.GetValues(typeof(Exchanges)))
            {
                this.comboBoxFolioExchanges.AppendText(exchange.ToString());
            }

            this.comboBoxPriceExchanges = new Gtk.ComboBox(new string[Enum.GetNames(typeof(Exchanges)).Length]);
            this.comboBoxPriceExchanges.SetSizeRequest(windowWidth / 4, windowHeight / 10);
            this.comboBoxPriceExchanges.Child.ModifyFont(oneFont);
            this.comboBoxPriceExchanges.AppendText("Séléctionner la plateforme d'échange");
            this.comboBoxPriceExchanges.AppendText("\t-\t");
            foreach (Exchanges exchange in Enum.GetValues(typeof(Exchanges)))
            {
                this.comboBoxPriceExchanges.AppendText(exchange.ToString());
            }

            this.comboBoxSendExchanges = new Gtk.ComboBox(new string[Enum.GetNames(typeof(Exchanges)).Length]);
            this.comboBoxSendExchanges.SetSizeRequest(windowWidth / 4, windowHeight / 10);
            this.comboBoxSendExchanges.Child.ModifyFont(oneFont);
            this.comboBoxSendExchanges.AppendText("Séléctionner la plateforme du portefeuille receiver");
            this.comboBoxSendExchanges.AppendText("\t-\t");
            foreach (Exchanges exchange in Enum.GetValues(typeof(Exchanges)))
            {
                this.comboBoxSendExchanges.AppendText(exchange.ToString());
            }

            this.comboBoxSendFromExchanges = new Gtk.ComboBox(new string[Enum.GetNames(typeof(Exchanges)).Length]);
            this.comboBoxSendFromExchanges.SetSizeRequest(windowWidth / 4, windowHeight / 10);
            this.comboBoxSendFromExchanges.Child.ModifyFont(oneFont);
            this.comboBoxSendFromExchanges.AppendText("Séléctionner la plateforme du portefeuille sender");
            this.comboBoxSendFromExchanges.AppendText("\t-\t");
            foreach (Exchanges exchange in Enum.GetValues(typeof(Exchanges)))
            {
                this.comboBoxSendFromExchanges.AppendText(exchange.ToString());
            }

            this.comboBoxOrderExchanges = new Gtk.ComboBox(new string[Enum.GetNames(typeof(Exchanges)).Length]);
            this.comboBoxOrderExchanges.SetSizeRequest(windowWidth / 4, windowHeight / 15);
            this.comboBoxOrderExchanges.Child.ModifyFont(oneFont);
            this.comboBoxOrderExchanges.AppendText("Séléctionner la plateforme d'échange");
            this.comboBoxOrderExchanges.AppendText("\t-\t");
            foreach (Exchanges exchange in Enum.GetValues(typeof(Exchanges)))
            {
                this.comboBoxOrderExchanges.AppendText(exchange.ToString());
            }

            this.comboBoxCancelOrderExchanges = new Gtk.ComboBox(new string[Enum.GetNames(typeof(Exchanges)).Length]);
            this.comboBoxCancelOrderExchanges.SetSizeRequest(windowWidth / 4, windowHeight / 15);
            this.comboBoxCancelOrderExchanges.Child.ModifyFont(oneFont);
            this.comboBoxCancelOrderExchanges.AppendText("Séléctionner la plateforme d'échange");
            this.comboBoxCancelOrderExchanges.AppendText("\t-\t");
            foreach (Exchanges exchange in Enum.GetValues(typeof(Exchanges)))
            {
                this.comboBoxCancelOrderExchanges.AppendText(exchange.ToString());
            }

            this.comboBoxCancelOrderCryptos = new Gtk.ComboBox(new string[Enum.GetNames(typeof(Exchanges)).Length]);
            this.comboBoxCancelOrderCryptos.SetSizeRequest(windowWidth / 4, windowHeight / 15);
            this.comboBoxCancelOrderCryptos.Child.ModifyFont(oneFont);
            this.comboBoxCancelOrderCryptos.AppendText("Séléctionner la Crypto");
            this.comboBoxCancelOrderCryptos.AppendText("\t-\t");
            foreach (MainCryptos crypto in Enum.GetValues(typeof(MainCryptos)))
            {
                this.comboBoxCancelOrderCryptos.AppendText(crypto.ToString());
            }

            this.comboBoxPriceCryptos = new Gtk.ComboBox(new string[Enum.GetNames(typeof(MainCryptos)).Length]);
            this.comboBoxPriceCryptos.SetSizeRequest(windowWidth / 5, windowHeight / 10);
            this.comboBoxPriceCryptos.Child.ModifyFont(oneFont);
            this.comboBoxPriceCryptos.AppendText("Séléctionner la cryptos");
            this.comboBoxPriceCryptos.AppendText("\t-\t");
            foreach (MainCryptos crypto in Enum.GetValues(typeof(MainCryptos)))
            {
                this.comboBoxPriceCryptos.AppendText(crypto.ToString());
            }

            this.comboBoxBestPriceCryptos = new Gtk.ComboBox(new string[Enum.GetNames(typeof(MainCryptos)).Length]);
            this.comboBoxBestPriceCryptos.SetSizeRequest(windowWidth / 5, windowHeight / 10);
            this.comboBoxBestPriceCryptos.Child.ModifyFont(oneFont);
            this.comboBoxBestPriceCryptos.AppendText("Séléctionner la cryptos");
            this.comboBoxBestPriceCryptos.AppendText("\t-\t");
            foreach (MainCryptos crypto in Enum.GetValues(typeof(MainCryptos)))
            {
                this.comboBoxBestPriceCryptos.AppendText(crypto.ToString());
            }

            this.comboBoxAllPriceCryptos = new Gtk.ComboBox(new string[Enum.GetNames(typeof(MainCryptos)).Length]);
            this.comboBoxAllPriceCryptos.SetSizeRequest(windowWidth / 5, windowHeight / 10);
            this.comboBoxAllPriceCryptos.Child.ModifyFont(oneFont);
            this.comboBoxAllPriceCryptos.AppendText("Séléctionner la cryptos");
            this.comboBoxAllPriceCryptos.AppendText("\t-\t");
            foreach (MainCryptos crypto in Enum.GetValues(typeof(MainCryptos)))
            {
                this.comboBoxAllPriceCryptos.AppendText(crypto.ToString());
            }

            this.comboBoxOrderCryptos = new Gtk.ComboBox(new string[Enum.GetNames(typeof(MainCryptos)).Length]);
            this.comboBoxOrderCryptos.SetSizeRequest(windowWidth / 5, windowHeight / 15);
            this.comboBoxOrderCryptos.Child.ModifyFont(oneFont);
            this.comboBoxOrderCryptos.AppendText("Séléctionner la cryptos");
            this.comboBoxOrderCryptos.AppendText("\t-\t");
            foreach (MainCryptos crypto in Enum.GetValues(typeof(MainCryptos)))
            {
                this.comboBoxOrderCryptos.AppendText(crypto.ToString());
            }

            this.comboSendCryptos = new Gtk.ComboBox(new string[Enum.GetNames(typeof(MainCryptos)).Length]);
            this.comboSendCryptos.SetSizeRequest(windowWidth / 5, windowHeight / 12);
            this.comboSendCryptos.Child.ModifyFont(oneFont);
            this.comboSendCryptos.AppendText("Séléctionner la cryptos");
            this.comboSendCryptos.AppendText("\t-\t");
            foreach (MainCryptos crypto in Enum.GetValues(typeof(MainCryptos)))
            {
                this.comboSendCryptos.AppendText(crypto.ToString());
            }

            this.comboPlatformArbitrageCryptos = new Gtk.ComboBox(new string[Enum.GetNames(typeof(MainCryptos)).Length]);
            this.comboPlatformArbitrageCryptos.SetSizeRequest(windowWidth / 5, windowHeight / 10);
            this.comboPlatformArbitrageCryptos.Child.ModifyFont(oneFont);
            this.comboPlatformArbitrageCryptos.AppendText("Séléctionner la cryptos");
            this.comboPlatformArbitrageCryptos.AppendText("\t-\t");
            foreach (MainCryptos crypto in Enum.GetValues(typeof(MainCryptos)))
            {
                this.comboPlatformArbitrageCryptos.AppendText(crypto.ToString());
            }

            this.comboBoxPriceOrderType = new Gtk.ComboBox(new string[Enum.GetNames(typeof(OrderType)).Length]);
            this.comboBoxPriceOrderType.SetSizeRequest(windowWidth / 5, windowHeight / 10);
            this.comboBoxPriceOrderType.Child.ModifyFont(oneFont);
            this.comboBoxPriceOrderType.AppendText("Séléctionner le type d'ordre");
            this.comboBoxPriceOrderType.AppendText("\t-\t");
            this.comboBoxPriceOrderType.AppendText(OrderType.BUY.ToString());
            this.comboBoxPriceOrderType.AppendText(OrderType.SELL.ToString());

            this.comboBoxOrderType = new Gtk.ComboBox(new string[Enum.GetNames(typeof(OrderType)).Length]);
            this.comboBoxOrderType.SetSizeRequest(windowWidth / 5, windowHeight / 15);
            this.comboBoxOrderType.Child.ModifyFont(oneFont);
            this.comboBoxOrderType.AppendText("Séléctionner le type d'ordre");
            this.comboBoxOrderType.AppendText("\t-\t");
            this.comboBoxOrderType.AppendText(OrderType.BUY.ToString());
            this.comboBoxOrderType.AppendText(OrderType.SELL.ToString());

            this.comboExportCryptosBase = new Gtk.ComboBox(new string[Enum.GetNames(typeof(MainCryptos)).Length]);
            this.comboExportCryptosBase.SetSizeRequest(windowWidth / 5, windowHeight / 10);
            this.comboExportCryptosBase.Child.ModifyFont(oneFont);
            this.comboExportCryptosBase.AppendText("Séléctionner la base");
            this.comboExportCryptosBase.AppendText("\t-\t");
            foreach (MainCryptos crypto in Enum.GetValues(typeof(MainCryptos)))
            {
                this.comboExportCryptosBase.AppendText(crypto.ToString());
            }

            this.comboExportCryptosPair = new Gtk.ComboBox(new string[Enum.GetNames(typeof(MainCryptos)).Length]);
            this.comboExportCryptosPair.SetSizeRequest(windowWidth / 5, windowHeight / 10);
            this.comboExportCryptosPair.Child.ModifyFont(oneFont);
            this.comboExportCryptosPair.AppendText("Séléctionner la paire");
            this.comboExportCryptosPair.AppendText("\t-\t");
            foreach (MainCryptos crypto in Enum.GetValues(typeof(MainCryptos)))
            {
                this.comboExportCryptosPair.AppendText(crypto.ToString());
            }

            this.comboExportCcyPair = new Gtk.ComboBox(new string[Enum.GetNames(typeof(MainCurrency)).Length]);
            this.comboExportCcyPair.SetSizeRequest(windowWidth / 5, windowHeight / 10);
            this.comboExportCcyPair.Child.ModifyFont(oneFont);
            this.comboExportCcyPair.AppendText("Séléctionner la paire");
            this.comboExportCcyPair.AppendText("\t-\t");
            foreach (MainCurrency ccy in Enum.GetValues(typeof(MainCurrency)))
            {
                this.comboExportCcyPair.AppendText(ccy.ToString());
            }

            this.comboExportNbDays = new Gtk.ComboBox(new string[400]);
            this.comboExportNbDays.SetSizeRequest(windowWidth / 5, windowHeight / 10);
            this.comboExportNbDays.Child.ModifyFont(oneFont);
            this.comboExportNbDays.AppendText("Séléctionner le nombre de jours voulus");
            this.comboExportNbDays.AppendText("\t-\t");
            for (int i = 1; i <= 400; i++)
            {
                this.comboExportNbDays.AppendText(i.ToString());
            }

            this.comboExportTypePair = new Gtk.ComboBox(new string[400]);
            this.comboExportTypePair.SetSizeRequest(windowWidth / 5, windowHeight / 10);
            this.comboExportTypePair.Child.ModifyFont(oneFont);
            this.comboExportTypePair.AppendText("Séléctionner le type de paire voulue");
            this.comboExportTypePair.AppendText("\t-\t");
            this.comboExportTypePair.AppendText("Fiat");
            this.comboExportTypePair.AppendText("Crypto");

            /// <summary>
            ///     Entry : widget used to get user entry 
            /// </summary>
            this.entryOrderPrice = new Gtk.Entry();
            this.entryOrderPrice.Text = "Select a Price rate";
            this.entryOrderPrice.SetSizeRequest(windowWidth / 5, windowHeight / 15);
            this.entryOrderPrice.ModifyFont(oneFont);

            this.entryOrderAmount = new Gtk.Entry();
            this.entryOrderAmount.Text = "Select an Amount";
            this.entryOrderAmount.SetSizeRequest(windowWidth / 3, windowHeight / 15);
            this.entryOrderAmount.ModifyFont(oneFont);

            this.entrySendAdress = new Gtk.Entry();
            this.entrySendAdress.Text = "Receiver Adress";
            this.entrySendAdress.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.entrySendAdress.ModifyFont(oneFont);

            this.entrySendAmount = new Gtk.Entry();
            this.entrySendAmount.Text = "Entrer the Amount to send";
            this.entrySendAmount.SetSizeRequest(windowWidth / 3, windowHeight / 12);
            this.entrySendAmount.ModifyFont(oneFont);

            this.entryCancelOrderId = new Gtk.Entry();
            this.entryCancelOrderId.Text = "Enter Id";
            this.entryCancelOrderId.SetSizeRequest(windowWidth / 4, windowHeight / 12);
            this.entryCancelOrderId.ModifyFont(oneFont);

            /// <summary>
            ///     Submenu items 
            /// </summary>
            this.m_infos = new Gtk.MenuItem("Account Infos");
            this.m_infos.Child.ModifyFont(oneFont);
            this.m_folio = new Gtk.MenuItem("Portfolios");
            this.m_folio.Child.ModifyFont(oneFont);
            this.m_openOrders = new Gtk.MenuItem("Open Orders");
            this.m_openOrders.Child.ModifyFont(oneFont);
            this.m_exportPrices = new Gtk.MenuItem("Export Historical Prices");
            this.m_exportPrices.Child.ModifyFont(oneFont);
            this.m_exit = new Gtk.MenuItem("Quit");
            this.m_exit.Child.ModifyFont(oneFont);
            this.m_getOnePrice = new Gtk.MenuItem("One Price");
            this.m_getOnePrice.Child.ModifyFont(oneFont);
            this.m_getBestPrice = new Gtk.MenuItem("Best Price");
            this.m_getBestPrice.Child.ModifyFont(oneFont);
            this.m_getAllPrice = new Gtk.MenuItem("All Prices");
            this.m_getAllPrice.Child.ModifyFont(oneFont);
            this.m_sellnbuy = new Gtk.MenuItem("Sell / Buy");
            this.m_sellnbuy.Child.ModifyFont(oneFont);
            this.m_cancelOrder = new Gtk.MenuItem("Cancel Order");
            this.m_cancelOrder.Child.ModifyFont(oneFont);
            this.m_platform_arbitrage = new Gtk.MenuItem("Platform Arbitrage");
            this.m_platform_arbitrage.Child.ModifyFont(oneFont);
            this.m_sendnreceive = new Gtk.MenuItem("Send/Receive Cryptos");
            this.m_sendnreceive.Child.ModifyFont(oneFont);

            /// <summary>
            ///     gathering all submenu items in 2 Menu Bar
            ///         - Menu Account (containing portfolio submenu, open orders, etc...)
            ///         - Menu Trade (containg sell/buby submenu, get best price, etc...)
            /// </summary>
            this.menuAccount = new Gtk.Menu();
            this.menuAccount.Append(this.m_infos);
            this.menuAccount.Append(this.m_folio);
            this.menuAccount.Append(this.m_sendnreceive);
            this.menuAccount.Append(this.m_openOrders);
            this.menuAccount.Append(this.m_exportPrices);
            this.menuAccount.Append(this.m_exit);

            this.menuTrade = new Gtk.Menu();
            this.menuTrade.Append(this.m_getOnePrice);
            this.menuTrade.Append(this.m_getBestPrice);
            this.menuTrade.Append(this.m_getAllPrice);
            this.menuTrade.Append(this.m_sellnbuy);
            this.menuTrade.Append(this.m_cancelOrder);
            this.menuTrade.Append(this.m_platform_arbitrage);

            /// <summary>
            ///     2 Menu items, that will be displayed in the main menu bar
            ///     The 2 menu items scrolling list are the previously defined 2 menu bar
            /// </summary>
            this.m_account = new Gtk.MenuItem("Mon Compte");
            this.m_account.Child.ModifyFont(oneFont);
            this.m_account.Submenu = this.menuAccount;
            this.m_account.Child.ModifyFg(StateType.Normal, greyColorStrong);
            this.m_account.Child.ModifyFg(StateType.Prelight, greyColorLight);

            this.m_trade = new Gtk.MenuItem("Trade");
            this.m_trade.Child.ModifyFont(oneFont);
            this.m_trade.Submenu = this.menuTrade;
            this.m_trade.Child.ModifyFg(StateType.Normal, greyColorStrong);
            this.m_trade.Child.ModifyFg(StateType.Prelight, greyColorLight);

            /// <summary>
            ///     Main Menu Bar Widgets, 
            ///     Containing 2 main Menu Items (Account and Trade)
            /// </summary>
            this.menuBar = new Gtk.MenuBar();
            this.menuBar.Append(this.m_account);
            this.menuBar.Append(this.m_trade);
            this.menuBar.SetSizeRequest(windowWidth, windowHeight / 10);

            /// <summary>
            ///     Progress bar use to animate the GUI while loading portfolio
            /// </summary>
            this.oneProgressBar = new Gtk.ProgressBar();
            this.oneProgressBar.SetSizeRequest(windowWidth - 20, windowHeight / 10);

            /// <summary>
            ///     Separator objects used to separate widget
            ///     They are kind of "empty" widgets
            /// </summary>
            this.loadSeparatorH = new HSeparator();
            this.loadSeparatorH.SetSizeRequest(windowWidth, windowHeight / 4);
            this.loadSeparatorH.ModifyBg(StateType.Normal, bkgColor);

            this.loadSeparatorL = new HSeparator();
            this.loadSeparatorL.SetSizeRequest(windowWidth, windowHeight / 4);
            this.loadSeparatorL.ModifyBg(StateType.Normal, bkgColor);

            /// <summary>
            ///     HBox & VBox : Main containers
            ///     Each view (mostly) have 2 Hbox :
            ///         - Hbox Entry : containing all widgets that represents user inputs
            ///             (combo box and enty widgets mostly)
            ///         - HBox Buttons : containing all buttons widgets, wether they are validation buttons
            ///             (green coloured), or optional buttons (yellow coloured)
            ///     Hbox & VBox are used to properly displayed widgets in the GUI window
            ///     at correct position
            /// </summary>
            this.hBoxMainButton = new Gtk.HBox(true, 2);
            this.hBoxMainButton.PackStart(this.buttonHome, false, false, 6);
            this.hBoxMainButton.PackStart(this.buttonActualize, true, true, 6);
            this.hBoxMainButton.PackStart(this.buttonDashboard, true, true, 6);
            this.hBoxMainButton.PackStart(this.buttonQuit, false, false, 6);

            this.hBoxFolioEntry = new Gtk.HBox(true, 2);
            this.hBoxFolioEntry.PackStart(this.comboBoxFolioExchanges, false, false, 2);

            this.hBoxFolioButton = new Gtk.HBox(true, 2);
            this.hBoxFolioButton.PackStart(this.buttonFolio, false, false, 2);

            this.hBoxAccountInfosEntry = new Gtk.HBox(true, 2);
            this.hBoxAccountInfosEntry.PackStart(this.comboBoxAccountInfosExchanges, false, false, 2);

            this.hBoxAccountInfosButton = new Gtk.HBox(true, 2);
            this.hBoxAccountInfosButton.PackStart(this.buttonAccountInfos, false, false, 2);

            this.hBoxLoadWheel = new Gtk.HBox(true, 20);
            this.hBoxLoadWheel.PackStart(this.imgLoadWheel, false, false, 10);
            this.hBoxLoadWheel.PackStart(this.oneProgressBar, false, false, 40);
            this.hBoxLoadWheel.SetSizeRequest(windowWidth - 20, windowHeight / 8);

            this.hBoxOpenOrdersEntry = new Gtk.HBox(true, 2);
            this.hBoxOpenOrdersEntry.PackStart(this.comboBoxOpenOrderExchanges, false, false, 2);

            this.hBoxOpenOrdersButton = new Gtk.HBox(true, 2);
            this.hBoxOpenOrdersButton.PackStart(this.buttonOpenOrders, false, false, 2);

            this.hBoxPriceEntry = new Gtk.HBox(true, 2);
            this.hBoxPriceEntry.PackStart(this.comboBoxPriceCryptos, false, false, 2);
            this.hBoxPriceEntry.PackStart(this.comboBoxPriceExchanges, false, false, 2);

            this.hBoxPriceButton = new Gtk.HBox(true, 2);
            this.hBoxPriceButton.PackStart(this.buttonPrice, false, false, 2);
            this.hBoxPriceButton.PackStart(this.buttonPriceStats, false, false, 2);
            this.hBoxPriceButton.SetSizeRequest(windowWidth / 5, windowHeight / 12);

            this.hBoxOrderMaxAmount = new Gtk.HBox(true, 2);
            this.hBoxOrderMaxAmount.PackStart(this.entryOrderAmount, false, false, 2);
            this.hBoxOrderMaxAmount.PackStart(this.buttonOrderMaxAmount, false, false, 2);

            this.vBoxOrderEntry = new Gtk.VBox(true, 5);
            this.vBoxOrderEntry.PackStart(this.comboBoxOrderType, false, false, 2);
            this.vBoxOrderEntry.PackStart(this.comboBoxOrderCryptos, false, false, 2);
            this.vBoxOrderEntry.PackStart(this.entryOrderPrice, false, false, 2);
            this.vBoxOrderEntry.PackStart(this.hBoxOrderMaxAmount, false, false, 2);
            this.vBoxOrderEntry.PackStart(this.comboBoxOrderExchanges, false, false, 2);
            this.vBoxOrderEntry.SetSizeRequest(windowWidth / 3, windowHeight / 3);

            this.hBoxOrderButton = new Gtk.HBox(true, 5);
            this.hBoxOrderButton.PackStart(this.buttonOrderSetBP, false, false, 5);
            this.hBoxOrderButton.PackStart(this.buttonOrder, false, false, 5);
            this.hBoxOrderButton.PackStart(this.buttonOrderSetBE, false, false, 5);
            this.hBoxOrderButton.SetSizeRequest(windowWidth / 5, windowHeight / 12);

            this.hBoxCancelOrderEntry = new Gtk.HBox(true, 5);
            this.hBoxCancelOrderEntry.PackStart(this.entryCancelOrderId, false, false, 5);
            this.hBoxCancelOrderEntry.PackStart(this.comboBoxCancelOrderExchanges, false, false, 5);
            this.hBoxCancelOrderEntry.PackStart(this.comboBoxCancelOrderCryptos, false, false, 5);
            this.hBoxCancelOrderEntry.PackStart(this.buttonCancelOrderGetId, false, false, 5);

            this.hBoxCancelOrderButton = new Gtk.HBox(true, 5);
            this.hBoxCancelOrderButton.PackStart(this.buttonCancelOrder, false, false, 5);

            this.hBoxPlatformArbitrageEntry = new HBox(true, 2);
            this.hBoxPlatformArbitrageEntry.PackStart(this.comboPlatformArbitrageCryptos, false, false, 2);
            this.hBoxPlatformArbitrageEntry.SetSizeRequest(windowWidth / 5, windowHeight / 10);

            this.hBoxPlatformArbitrageButton = new HBox(true, 2);
            this.hBoxPlatformArbitrageButton.PackStart(this.buttonPlatformArbitrage, false, false, 2);
            this.hBoxPlatformArbitrageButton.SetSizeRequest(windowWidth / 5, windowHeight / 12);

            this.hBoxBestPriceEntry = new HBox(true, 2);
            this.hBoxBestPriceEntry.PackStart(this.comboBoxPriceOrderType, false, false, 2);
            this.hBoxBestPriceEntry.PackStart(this.comboBoxBestPriceCryptos, false, false, 2);

            this.hBoxBestPriceButton = new HBox(true, 2);
            this.hBoxBestPriceButton.PackStart(this.buttonBestPrice, false, false, 2);

            this.hBoxAllPriceEntry = new HBox(true, 2);
            this.hBoxAllPriceEntry.PackStart(this.comboBoxAllPriceCryptos, false, false, 2);

            this.hBoxAllPriceButton = new HBox(true, 2);
            this.hBoxAllPriceButton.PackStart(this.buttonAllPrice, false, false, 2);

            this.hBoxSendAdress = new HBox();
            this.hBoxSendAdress.PackStart(this.entrySendAdress, false, false, 2);
            this.hBoxSendAdress.PackStart(this.comboBoxSendExchanges, false, false, 2);
            this.hBoxSendAdress.PackStart(this.buttonSendAdress, false, false, 2);

            this.hBoxSendFromAdress = new HBox();
            this.hBoxSendFromAdress.PackStart(this.entrySendAmount, false, false, 2);
            this.hBoxSendFromAdress.PackStart(this.comboBoxSendFromExchanges, false, false, 2);
            this.hBoxSendFromAdress.PackStart(this.buttonSendFromAdress, false, false, 2);

            this.vBoxSendEntry = new Gtk.VBox(true, 5);
            this.vBoxSendEntry.PackStart(this.comboSendCryptos, false, false, 2);
            this.vBoxSendEntry.PackStart(this.hBoxSendAdress, false, false, 2);
            this.vBoxSendEntry.PackStart(this.hBoxSendFromAdress, false, false, 2);

            this.hBoxSendButton = new HBox(true, 5);
            this.hBoxSendButton.PackStart(this.buttonSend, false, false, 2);

            this.hBoxExportEntry = new Gtk.HBox(true, 5);
            this.hBoxExportEntry.PackStart(this.comboExportCryptosBase, false, false, 2);
            this.hBoxExportEntry.PackStart(this.comboExportTypePair, false, false, 2);
            this.hBoxExportEntry.PackStart(this.comboExportCryptosPair, false, false, 2);
            this.hBoxExportEntry.PackStart(this.comboExportCcyPair, false, false, 2);
            this.hBoxExportEntry.PackStart(this.comboExportNbDays, false, false, 2);

            this.hBoxExportButton = new HBox(true, 5);
            this.hBoxExportButton.PackStart(this.buttonExport, false, false, 2);

            //vBoxMain
                //Header Widgets
            this.vBoxMain = new Gtk.VBox(false, 0);
            this.vBoxMain.PackStart(this.menuBar, false, false, 2);
            this.vBoxMain.PackStart(this.loadSeparatorH, true, true, 0);
            this.vBoxMain.PackStart(this.labelFrame, true, true, 2);
            this.vBoxMain.PackStart(this.hBoxLoadWheel, false, false, 2);
            this.vBoxMain.PackStart(this.imgLogo, true, true, 2);
            this.vBoxMain.PackStart(this.loadSeparatorL, true, true, 0);
                // Body Widgets
            this.vBoxMain.PackStart(this.hBoxAccountInfosEntry, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxAccountInfosButton, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxFolioEntry, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxFolioButton, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxOpenOrdersEntry, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxOpenOrdersButton, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxBestPriceEntry, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxBestPriceButton, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxAllPriceEntry, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxAllPriceButton, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxPlatformArbitrageEntry, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxPlatformArbitrageButton, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxPriceEntry, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxPriceButton, false, false, 2);
            this.vBoxMain.PackStart(this.vBoxOrderEntry, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxOrderButton, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxCancelOrderEntry, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxCancelOrderButton, false, false, 2);
            this.vBoxMain.PackStart(this.vBoxSendEntry, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxSendButton, false, false, 2);
            this.vBoxMain.PackStart(this.hBoxExportEntry, false, false, 5);
            this.vBoxMain.PackStart(this.hBoxExportButton, false, false, 5);
                // Footer Widgets
            this.vBoxMain.PackStart(this.hBoxMainButton, false, false, 2);

            /// <summary>
            ///     Event Handling for various Widgets :
            ///         - Delete event for Top Lovel Window 
            ///         - Menu items have to display their associated views when they are clicked
            ///         - Buttons have to run their associated function when they are pressed
            ///         - some Combo box need to take actions when selected value changed
            /// </summary>
                // Top-Level Window Events
            this.DeleteEvent += new Gtk.DeleteEventHandler(this.DoWindowsDelete);
            this.DestroyEvent += new Gtk.DestroyEventHandler(this.DoWindowsDelete);
                // Menu Items Events
            this.m_exit.Activated += new System.EventHandler(this.DoExit);
            this.m_infos.Activated += new System.EventHandler(this.ViewAccountInfos);
            this.m_folio.Activated += new System.EventHandler(this.ViewFolio);
            this.m_exportPrices.Activated += new System.EventHandler(this.ViewExportPrices);
            this.m_openOrders.Activated += new System.EventHandler(this.ViewOpenOrders);
            this.m_sendnreceive.Activated += new System.EventHandler(this.ViewSendNReceive);
            this.m_getOnePrice.Activated += new System.EventHandler(this.ViewOnePrice);
            this.m_getBestPrice.Activated += new System.EventHandler(this.ViewBestPrice);
            this.m_getAllPrice.Activated += new System.EventHandler(this.ViewAllPrice);
            this.m_sellnbuy.Activated += new System.EventHandler(this.ViewSellNBuy);
            this.m_cancelOrder.Activated += new System.EventHandler(this.ViewCancelOrder);
            this.m_platform_arbitrage.Activated += new System.EventHandler(this.ViewPlatformArbitrage);
                // Buttons Events
            this.buttonQuit.Clicked += new System.EventHandler(this.DoExit);
            this.buttonActualize.Clicked += new System.EventHandler(this.DoActualizeAsync);
            this.buttonDashboard.Clicked += new System.EventHandler(this.ViewDashBoardAsync);
            this.buttonHome.Clicked += new System.EventHandler(this.ViewHome);
            this.buttonAccountInfos.Clicked += new System.EventHandler(this.DoAccountInfos);
            this.buttonFolio.Clicked += new System.EventHandler(this.DoFolioAsync);
            this.buttonOpenOrders.Clicked += new System.EventHandler(this.DoOpenOrdersAsync);
            this.buttonPrice.Clicked += new System.EventHandler(this.DoGetPriceAsync);
            this.buttonPriceStats.Clicked += new System.EventHandler(this.SetPriceStats);
            this.buttonPlatformArbitrage.Clicked += new System.EventHandler(this.DoCheckPlatformArbitrageAsync);
            this.buttonBestPrice.Clicked += new System.EventHandler(this.DoGetBestPriceAsync);
            this.buttonAllPrice.Clicked += new System.EventHandler(this.DoGetAllPriceAsync);
            this.buttonOrder.Clicked += new System.EventHandler(this.DoPassOrderAsync);
            this.buttonOrderMaxAmount.Clicked += new System.EventHandler(this.SetOrderMax);
            this.buttonCancelOrder.Clicked += new System.EventHandler(this.DoCancelOrderAsync);
            this.buttonCancelOrderGetId.Clicked += new System.EventHandler(this.SetCancelOrderId);
            this.buttonOrderSetBP.Clicked += new System.EventHandler(this.SetOrder2BestPrice);
            this.buttonOrderSetBE.Clicked += new System.EventHandler(this.SetOrder2BestExchange);
            this.buttonSend.Clicked += new System.EventHandler(this.DoSend);
            this.buttonSendFromAdress.Clicked += new System.EventHandler(this.SetSendMax);
            this.buttonSendAdress.Clicked += new System.EventHandler(this.SetSendAdress);
            this.buttonExport.Clicked += new System.EventHandler(this.DoExportPrices);
                // ComboBox Events
            this.comboExportTypePair.Changed += new System.EventHandler(this.SetGoodPaire);

            /// <summary>
            ///     Adding the main VBox into the GUI Application Top Level Window 
            ///     Then call the clear widgets function to clear all widgets besides
            ///     of general ones
            ///     then, we show the main welcome window (only displaying Logo image)
            /// </summary>
            this.Add(this.vBoxMain);
            this.ShowAll();
            this.ClearWidgets();
            this.buttonDashboard.Show();

            this.imgLogo.Show();
            this.imgLoadWheel.Hide();
        }
    }
}