using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleFindItems
{
    class VideoCard
    {
        private int _id = 0;
        private string _cardName = "";
        private int _hashRate = 0;
        private double _cashPerHash = 0;
        private double _price;

        public int GetId(){ return _id; }
        public string GetCardName() { return _cardName; }
        public int GetHashRate() { return _hashRate; }
        public double GetCashPerHash() { return _cashPerHash; }
        public double GetPrice() { return _price; }

        public void SetId(int index)
        {
            _id = index;
        }

        public void SetCardName(string modelName)
        {
            _cardName = modelName;
        }

        public void SetHashRate(int inputRate)
        {
            _hashRate = inputRate;
        }

        public void SetCashPerHash(double calculatedRate)
        {
            _cashPerHash = calculatedRate;
        }

        public void SetPrice(double price)
        {
            _price = price;
        }
    }
}
