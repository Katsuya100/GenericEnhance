using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Katuusagi.GenericEnhance.Tests
{
    public class SpecializationPerformanceTest
    {
        [Test]
        [Performance]
        public void Direct()
        {
            Measure.Method(() =>
            {
                TestFunctions.Add(10.0, 20.0);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void VirtualStrategy()
        {
            Measure.Method(() =>
            {
                TestFunctions.WrappedAdd_VirtualStrategy<double, double, double>(10.0, 20.0);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .ProfilerMarkers(new SampleGroup("Allocate", SampleUnit.Byte, false))
            .Run();
        }

        [Test]
        [Performance]
        public void TypeComparison()
        {
            Measure.Method(() =>
            {
                TestFunctions.WrappedAdd_TypeComparison<double, double, double>(10.0, 20.0);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void TypeIdComparison()
        {
            Measure.Method(() =>
            {
                TestFunctions.WrappedAdd_TypeIdComparison<double, double, double>(10.0, 20.0);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void DelegateStrategy()
        {
            Measure.Method(() =>
            {
                TestFunctions.WrappedAdd_DelegateStrategy<double, double, double>(10.0, 20.0);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void OptimizedVirtualStrategy()
        {
            Measure.Method(() =>
            {
                TestFunctions.Add_VirtualStrategy<double, double, double>(10.0, 20.0);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void OptimizedTypeComparison()
        {
            Measure.Method(() =>
            {
                TestFunctions.Add_TypeComparison<double, double, double>(10.0, 20.0);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void OptimizedTypeIdComparison()
        {
            Measure.Method(() =>
            {
                TestFunctions.Add_TypeIdComparison<double, double, double>(10.0, 20.0);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }

        [Test]
        [Performance]
        public void OptimizedDelegateStrategy()
        {
            Measure.Method(() =>
            {
                TestFunctions.Add_DelegateStrategy<double, double, double>(10.0, 20.0);
            })
            .WarmupCount(1)
            .IterationsPerMeasurement(10000)
            .MeasurementCount(20)
            .Run();
        }
    }
}
