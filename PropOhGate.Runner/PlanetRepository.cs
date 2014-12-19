namespace PropOhGate.Runner
{
    using System;

    using PropOhGate.Data;

    public class PlanetRepository : Repository
    {
        public PlanetRepository()
        {
            CreateColumns();
        }

        public RepositoryColumn<string> Name { get; private set; }

        public RepositoryColumn<double> DistanceFromSun { get; private set; }

        public RepositoryColumn<TimeSpan> RotationTime { get; private set; }

        public RepositoryColumn<double> Gravity { get; private set; }

        public RepositoryRow AddRow(string name, double distanceFromSun, TimeSpan rotationTime, double gravity)
        {
            var row = AddRow();

            row.Set(Name, name);
            row.Set(DistanceFromSun, distanceFromSun);
            row.Set(RotationTime, rotationTime);
            row.Set(Gravity, gravity);
            return row;
        }

        protected void CreateColumns()
        {
            this.Name = CreateColumn<string>("Name");
            this.DistanceFromSun = CreateColumn<double>("DistanceFromSun");
            this.RotationTime = CreateColumn<TimeSpan>("RotationTime");
            this.Gravity = CreateColumn<double>("Gravity");
        }
    }
}
