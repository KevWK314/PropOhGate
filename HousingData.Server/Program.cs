using System;
using HousingData.Core;

namespace HousingData.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var repository = new HousingRepository();
            var populator = new HousingDataPopulator(repository);
            var service = new HousingService(repository);

            service.Start();
            populator.StartAsync();

            Console.WriteLine("The housing server has started");
            Console.ReadKey();

            service.Stop();
        }
    }
}
