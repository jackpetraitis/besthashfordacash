using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace ConsoleFindItems
{
    class FinishedVideoCard
    {
        private int _id;
        private string _cardName;
        private string _averageHash;
        private string _averagePrice;
        private string _hashForCash;

        public int getId()
        {
            return _id;
        }

        public string getCardName()
        {
            return _cardName;
        }

        public string getAverageHash()
        {
            return _averageHash;
        }

        public string getAveragePrice()
        {
            return _averagePrice;
        }

        public string getHashForCash()
        {
            return _hashForCash;
        }

        public void setId(int id)
        {
            _id = id;
        }

        public void setCardName(string cardName)
        {
            _cardName = cardName;
        }

        public void setAverageHash(string averageHash)
        {
            _averageHash = averageHash;
        }

        public void setAveragePrice(string averagePrice)
        {
            _averagePrice = averagePrice;
        }

        public void setHashForCash(string hashForCash)
        {
            _hashForCash = hashForCash;
        }
    }
}
