using System;
using HousingData.Core;
using PropOhGate.Collection;
using PropOhGate.Data;

namespace HousingData.Client.Model
{
    public class HousingItem : RepositoryItem
    {
        private HousingRepository _repository;

        public override void SetRepository(Repository repository)
        {
            _repository = repository as HousingRepository;
        }

        public string PostCode
        {
            get { return Get(_repository.PostCode); }
        }

        public int Count
        {
            get { return Get(_repository.Count); }
        }

        public double TotalValue
        {
            get { return Get(_repository.TotalValue); }
        }

        public double AveragePrice
        {
            get { return Get(_repository.AveragePrice); }
        }

        public DateTime FirstDate
        {
            get { return Get(_repository.FirstDate); }
        }

        public DateTime LastDate
        {
            get { return Get(_repository.LastDate); }
        }
    }
}
