namespace PropOhGate.Runner
{
    using System;

    using PropOhGate.Collection;
    using PropOhGate.Data;

    public class Planet : RepositoryItem
    {
        private PlanetRepository _repository;

        public string Name
        {
            get { return Get(_repository.Name); }
            set { Set(_repository.Name, value); }
        }

        public double DistanceFromSun
        {
            get { return Get(_repository.DistanceFromSun); }
            set { Set(_repository.DistanceFromSun, value); }
        }

        public TimeSpan RotationTime
        {
            get { return Get(_repository.RotationTime); }
            set { Set(_repository.RotationTime, value); }
        }

        public double Gravity
        {
            get { return Get(_repository.Gravity); }
            set { Set(_repository.Gravity, value); }
        }

        public override void SetRepository(Repository repository)
        {
            _repository = repository as PlanetRepository;
        }
    }
}
