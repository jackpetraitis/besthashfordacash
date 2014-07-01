using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleFindItems
{
    class RefVideoCard
    {
        private int _id;
        private string _cardName;
        private int _cardCount;
        private double _priceSoFar;
        private string _averagePrice;
        private double _hashSoFar;
        private string _averageHash;

        public int GetId()
        {
            return _id; 
        }

        public string GetCardName()
        {
            return _cardName;
        }

        public int GetCardCount()
        {
            return _cardCount;
        }

        public double GetPriceSoFar()
        {
            return _priceSoFar;
        }

        public string GetAveragePrice()
        {
            return _averagePrice;
        }

        public double GetHashSoFar()
        {
            return _hashSoFar;
        }

        public string GetAverageHash()
        {
            return _averageHash;
        }

        public void SetId(int id)
        {
            _id = id;
        }

        public void SetCardName(string cardName)
        {
            _cardName = cardName;
        }

        public void SetCardCount(int cardCount)
        {
            _cardCount = cardCount;
        }

        public void SetPriceSoFar(double priceSoFar)
        {
            _priceSoFar = priceSoFar;
        }

        public void SetAveragePrice(string averagePrice)
        {
            _averagePrice = averagePrice;
        }

        public void SetHashSoFar(double hashSoFar)
        {
            _hashSoFar = hashSoFar;
        }

        public void SetAverageHash(string averageHash)
        {
            _averageHash = averageHash;
        }

    }
}
