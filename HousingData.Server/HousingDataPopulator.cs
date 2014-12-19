using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using HousingData.Core;

namespace HousingData.Server
{
    public class HousingDataPopulator
    {
        private const string DataFileName = @"Data\pp-2014-sorted.csv";

        private readonly HousingRepository _repository;
        private readonly Dictionary<string, HousingRow> _housingRows = new Dictionary<string, HousingRow>();

        public HousingDataPopulator(HousingRepository repository)
        {
            _repository = repository;
        }

        public Task StartAsync()
        {
            if (!File.Exists(DataFileName))
            {
                throw new FileNotFoundException("No data found");
            }

            return Task.Run(() =>
                {
                    using (var fs = File.OpenRead(DataFileName))
                    {
                        using (var reader = new StreamReader(fs))
                        {
                            var line = reader.ReadLine();
                            while (!string.IsNullOrEmpty(line))
                            {
                                var tokens = line.Split(new[] { ',' });
                                if (tokens.Length == 15)
                                {
                                    var postcode = tokens[3].Split(new[] { ' ' })[0];
                                    var date = DateTime.ParseExact(tokens[2], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                                    var value = double.Parse(tokens[1]);

                                    var row = GetHousingRow(postcode);
                                    row.AddProperty(date, value);
                                }

                                line = reader.ReadLine();
                            }
                        }
                    }

                    _housingRows.Clear();
                    _repository.Clear();
                    StartAsync();
                });
        }

        private HousingRow GetHousingRow(string postCode)
        {
            HousingRow row;
            if (!_housingRows.TryGetValue(postCode, out row))
            {
                var repositoryRow = _repository.AddRow();
                row = new HousingRow(postCode, repositoryRow, _repository);
                _housingRows[postCode] = row;
            }
            return row;
        }
    }
}
