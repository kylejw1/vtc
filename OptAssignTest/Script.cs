using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Emgu.CV;
using Emgu.CV.Structure;

namespace OptAssignTest
{
    /// <summary>
    /// Traffic script.
    /// </summary>
    class Script
    {
        /// <summary>
        /// Car collection.
        /// </summary>
        public ReadOnlyCollection<Car> Cars
        {
            get { return _cars.AsReadOnly(); }
        }
        private readonly List<Car> _cars = new List<Car>();

        /// <summary>
        /// Last frame for the scene.
        /// </summary>
        public uint MaxFrame
        {
            get { return _cars.Max(car => car.MaxFrame); }
        }

        /// <summary>
        /// Create new car in the collection.
        /// </summary>
        /// <returns></returns>
        public Car CreateCar()
        {
            var car = new Car();
            _cars.Add(car);
            return car;
        }

        /// <summary>
        /// Draw the scene.
        /// </summary>
        /// <param name="frame">Frame number.</param>
        /// <param name="scene">Scene to draw car to</param>
        public void Draw(uint frame, Image<Bgr, byte> scene)
        {
            _cars.ForEach(car => car.Draw(frame, scene));
        }
    }
}
