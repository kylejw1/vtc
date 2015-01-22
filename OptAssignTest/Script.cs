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
    public class Script
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
        /// Create new car in the collection.
        /// </summary>
        /// <returns></returns>
        public Car CreateCar(uint carRadius)
        {
            var car = new Car(carRadius);
            _cars.Add(car);
            return car;
        }

        /// <summary>
        /// Draw the scene.
        /// </summary>
        /// <param name="frame">Frame number.</param>
        /// <param name="scene">Scene to draw cars to</param>
        public void Draw(uint frame, Image<Bgr, byte> scene)
        {
            _cars.ForEach(car => car.Draw(frame, scene));
        }

        /// <summary>
        /// Check if script is completed.
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool IsDone(uint frame)
        {
            // check all cars have finished their paths
            return _cars.All(car => car.IsDone(frame));
        }
    }
}
