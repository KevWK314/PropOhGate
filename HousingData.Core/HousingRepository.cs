using System;
using PropOhGate.Data;

namespace HousingData.Core
{
    public class HousingRepository : Repository
    {
        public HousingRepository()
        {
            CreateColumns();
        }

        public RepositoryColumn<string> PostCode { get; private set; }
        public RepositoryColumn<int> Count { get; private set; }
        public RepositoryColumn<double> TotalValue { get; private set; }
        public RepositoryColumn<double> AveragePrice { get; private set; }
        public RepositoryColumn<DateTime> FirstDate { get; private set; }
        public RepositoryColumn<DateTime> LastDate { get; private set; }

        private void CreateColumns()
        {
            PostCode = CreateColumn<string>("PostCode");
            Count = CreateColumn<int>("Count");
            TotalValue = CreateColumn<double>("TotalValue");
            AveragePrice = CreateColumn<double>("AveragePrice");
            FirstDate = CreateColumn<DateTime>("FirstDate");
            LastDate = CreateColumn<DateTime>("LastDate");
        }
    }
}
