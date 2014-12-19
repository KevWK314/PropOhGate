using System;
using HousingData.Core;
using PropOhGate.Data;

namespace HousingData.Server
{
    public class HousingRow
    {
        private readonly RepositoryRow _row;
        private readonly HousingRepository _repository;

        public HousingRow(string postCode, RepositoryRow row, HousingRepository repository)
        {
            _row = row;
            _repository = repository;
            PostCode = postCode;
        }

        public string PostCode 
        {
            get { return _row.Get(_repository.PostCode); }
            private set { _row.Set(_repository.PostCode, value); }
        }

        public int Count 
        {
            get { return _row.Get(_repository.Count); }
            private set { _row.Set(_repository.Count, value); }
        }

        public double TotalValue 
        {
            get { return _row.Get(_repository.TotalValue); }
            private set { _row.Set(_repository.TotalValue, value); }
        }

        public double AveragePrice 
        {
            get { return _row.Get(_repository.AveragePrice); }
            private set { _row.Set(_repository.AveragePrice, value); }
        }

        public DateTime FirstDate 
        {
            get { return _row.Get(_repository.FirstDate); }
            private set { _row.Set(_repository.FirstDate, value); }
        }

        public DateTime LastDate 
        {
            get { return _row.Get(_repository.LastDate); }
            private set { _row.Set(_repository.LastDate, value); }
        }

        public void AddProperty(DateTime date, double value)
        {
            Count += 1;
            TotalValue += value;
            AveragePrice = TotalValue / Count;
            if (date < FirstDate || FirstDate == DateTime.MinValue)
            {
                FirstDate = date;
            }
            if (date > LastDate || LastDate == DateTime.MinValue)
            {
                LastDate = date;
            }
        }
    }
}
