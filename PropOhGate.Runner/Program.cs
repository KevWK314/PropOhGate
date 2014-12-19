using System;

namespace PropOhGate.Runner
{
    using PropOhGate.Collection;
    using PropOhGate.Receive;
    using PropOhGate.Receive.Test;
    using PropOhGate.Send.Test;
    using PropOhGate.WebSockets.Receive;
    using PropOhGate.WebSockets.Send;

    class Program
    {
        static void Main(string[] args)
        {
            var repository = new PlanetRepository();
            var childRepository = new PlanetRepository();

            var collection = new RepositoryObservableCollection<Planet>(childRepository);
            /*
            var sendListener = new TestSendListener(repository);
            var receiver = new TestReceiver();
            var receiveListener = new ReceiveListener(childRepository, receiver);
            receiveListener.Start();
            sendListener.Listen(receiver);
            */

            childRepository.SubscribeToRowAdded(r => Console.WriteLine("Row Added with Id {0}", r.RowId));
            childRepository.SubscribeToRowRemoved(r => Console.WriteLine("Row Removed with Id {0}", r.RowId));
            childRepository.SubscribeToCellUpdated(u => Console.WriteLine("Cell" + "Updated {0}:{1}:{2}", u.RowId, u.ColumnId, u));

            var mercury = repository.AddRow("Mercury", 57900000, TimeSpan.FromDays(59), 0.38);
            var venus = repository.AddRow("Venus", 108160000, TimeSpan.FromDays(243), 0.9);
            var earth = repository.AddRow("Earth", 149600000, TimeSpan.FromHours(23) + TimeSpan.FromMinutes(56), 1);
            var mars = repository.AddRow("Mars", 141700000, TimeSpan.FromHours(24) + TimeSpan.FromMinutes(37), 0.38);
            var jupiter = repository.AddRow("Jupiter", 778369000, TimeSpan.FromHours(9) + TimeSpan.FromMinutes(55), 2.64);
            var saturn = repository.AddRow("Saturn", 1427034000, TimeSpan.FromHours(10) + TimeSpan.FromMinutes(39), 1.16);
            var uranus = repository.AddRow("Uranus", 2870658186, TimeSpan.FromHours(17) + TimeSpan.FromMinutes(14), 1.11);
            var neptune = repository.AddRow("Neptune", 4496976000, TimeSpan.FromHours(16) + TimeSpan.FromMinutes(7), 1.21);

            var sendListener = new WebSocketSendListener(repository);
            var receiver = new WebSocketReceiver("ws://localhost:8080/planet", childRepository.GetRepositoryHash());
            var receiveListener = new ReceiveListener(childRepository, receiver);
            receiveListener.Start();
            sendListener.Start("ws://0.0.0.0:8080/planet");

            Console.ReadKey();

            Console.WriteLine(childRepository.ToString());

            venus.Set(repository.Gravity,0.88);
            saturn.Set(repository.DistanceFromSun, 1427034111);

            repository.RemoveRow(uranus);
            var uranus2 = repository.AddRow("Uranus II", 2870658186, TimeSpan.FromHours(17) + TimeSpan.FromMinutes(14), 1.11);

            Console.WriteLine();
            Console.WriteLine(childRepository.ToString());


            Console.ReadKey();
        }
    }
}
