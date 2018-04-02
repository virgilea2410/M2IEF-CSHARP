using System;
using System.Collections.Generic;
using TaskAlias = System.Threading.Tasks;
using Crypto_Trader_V1.ExchangesModels.My_Bittrex;
using Crypto_Trader_V1.ExchangesModels.My_Binance;
using Crypto_Trader_V1.ExchangesModels.My_Bitfinex;
using Crypto_Trader_V1.ExchangesModels.My_CexIO;
using Crypto_Trader_V1.ExchangesModels.My_Coinbase;

namespace Crypto_Trader_V1.Models
{


    /// <summary>
    ///     Class representive a toolbox, containing various usefull functions
    /// </summary>
    public class Tools
    {


        /// <summary>
        ///     Function that animate the word "LOADING...", during
        ///     function calls
        /// </summary>
        public static void IterateLoad(ref Gtk.ProgressBar oneProgressBar)
        {
            if (oneProgressBar.Text != "LOADING...")
            {
                if (oneProgressBar.Text == "")
                {
                    oneProgressBar.Text += "L";
                }
                else if (oneProgressBar.Text == "L")
                {
                    oneProgressBar.Text += "O";
                }
                else if (oneProgressBar.Text == "LO")
                {
                    oneProgressBar.Text += "A";
                }
                else if (oneProgressBar.Text == "LOA")
                {
                    oneProgressBar.Text += "D";
                }
                else if (oneProgressBar.Text == "LOAD")
                {
                    oneProgressBar.Text += "I";
                }
                else if (oneProgressBar.Text == "LOADI")
                {
                    oneProgressBar.Text += "N";
                }
                else if (oneProgressBar.Text == "LOADIN")
                {
                    oneProgressBar.Text += "G";
                }
                else if (oneProgressBar.Text == "LOADING")
                {
                    oneProgressBar.Text += ".";
                }
                else if (oneProgressBar.Text == "LOADING.")
                {
                    oneProgressBar.Text += ".";
                }
                else if (oneProgressBar.Text == "LOADING..")
                {
                    oneProgressBar.Text += ".";
                }
            }
            else
            {
                oneProgressBar.Text = "";
            }
        }


        /// <summary>
        ///     Function that animate the word "ACTUALIZING...", during
        ///     function calls
        /// </summary>
        public static void IterateActualize(ref Gtk.ProgressBar oneProgressBar)
        {
            if (oneProgressBar.Text != "ACTUALIZING...")
            {
                if (oneProgressBar.Text == "")
                {
                    oneProgressBar.Text += "AC";
                }
                else if (oneProgressBar.Text == "AC")
                {
                    oneProgressBar.Text += "TU";
                }
                else if (oneProgressBar.Text == "ACTU")
                {
                    oneProgressBar.Text += "AL";
                }
                else if (oneProgressBar.Text == "ACTUAL")
                {
                    oneProgressBar.Text += "IZ";
                }
                else if (oneProgressBar.Text == "ACTUALIZ")
                {
                    oneProgressBar.Text += "IN";
                }
                else if (oneProgressBar.Text == "ACTUALIZIN")
                {
                    oneProgressBar.Text += "G.";
                }
                else if (oneProgressBar.Text == "ACTUALIZING.")
                {
                    oneProgressBar.Text += "..";
                }
            }
            else
            {
                oneProgressBar.Text = "";
            }
        }


        /// <summary>
        ///     Function that return the main directory of the user 
        /// </summary>
        public static string GetDirectory()
        {
            string debugPath = AppDomain.CurrentDomain.BaseDirectory;
            int index = debugPath.IndexOf("bin");
            string mainPath = debugPath.Substring(0, index);

            return mainPath;
        }


        /// <summary>
        ///     Function that convert the byte array <paramref name="buff"/> into string 
        /// </summary>
        public static string byteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2");
            return sbinary;
        }


        /// <summary>
        ///     Function that convert the double <paramref name="ratio"/>
        ///     into percentage string representation
        /// </summary>
        public static string Double2Percentage(double ratio)
        {
            string percentage = string.Format("{0:0.0%}", ratio);

            return percentage;
        }
    }
}
