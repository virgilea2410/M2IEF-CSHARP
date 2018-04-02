using System;
using System.Collections.Generic;


namespace Crypto_Trader_V1.Models
{


    /// <summary>
    ///     Class representing a crypto curreny portfolio, on a specific Exchange
    ///         <see cref="balanceList"/> represents the list of all available <see cref="Balance"/> in this portfolio
    ///         <see cref="folioExchange"/> exchange of the portfolio
    /// </summary>
    public class Portfolio
    {
        public Exchanges folioExchange;
        public List<Balance> balanceList;


        /// <summary>
        ///     Constructor
        /// </summary>
        public Portfolio()
        {
            this.balanceList = new List<Balance>();
        }


        /// <summary>
        ///     Constructor
        /// </summary>
        public Portfolio(IEnumerable<dynamic> oneInputFolio)
        {
            try
            {
                foreach (var row in oneInputFolio)
                {

                }                
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                balanceList = new List<Balance>();
            }

        }


        /// <summary>
        ///     Method to add a <paramref name="ccyBase"/> <paramref name="ccyPair"/> <see cref="Balance"/> object,
        ///     for a total amount of <paramref name="amount"/> to the <see cref="balanceList"/> of the portfolio
        /// </summary>
        public void AddBalance(MainCryptos ccyBase, MainCryptos ccyPair, double amount)
        {
            this.balanceList.Add(new Balance(ccyBase, ccyPair, amount));
        }


        /// <summary>
        ///     Method to add a <see cref="Balance"/> object to the <see cref="balanceList"/> of the portfolio
        /// </summary>
        public void AddBalance(Balance oneBalance)
        {
            this.balanceList.Add(oneBalance);
        }
    }
}
