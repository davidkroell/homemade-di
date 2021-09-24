using DavidKroell.HomemadeDI.Tests.TestClasses;
using NUnit.Framework;

namespace DavidKroell.HomemadeDI.Tests
{
    [TestFixture]
    public class ServiceLibraryTests
    {
        [Test]
        public void CreateMapping_SingleCtor_Works()
        {
            var sl = new ServiceLibrary();

            sl.Map<ITestService, TestService>();
        }

        [Test]
        public void CreateMapping_MultipleCtor_Throws()
        {
            var sl = new ServiceLibrary();

            Assert.Throws<ServiceMappingException>(() => sl.Map<ITestService, TestServiceWithMultiCtor>());
        }

        [Test]
        public void GetService_MissingMapping_Throws()
        {
            var sl = new ServiceLibrary();

            Assert.Throws<ServiceMappingNotFoundException>(() => sl.GetService<ITestService>());
        }

        [Test]
        public void GetService_NoDependencies_Works()
        {
            var sl = new ServiceLibrary();

            sl.Map<ITestService, TestService>();

            var service = sl.GetService<ITestService>();

            service.Dummy();
        }

        [Test]
        public void GetService_MultipleTimes_ReturnsNewInstances()
        {
            var sl = new ServiceLibrary();

            sl.Map<ITestService, TestService>();

            var service1 = sl.GetService<ITestService>();
            var service2 = sl.GetService<ITestService>();

            Assert.AreNotEqual(service1, service2);
        }

        [Test]
        public void GetService_SingleDependency_Works()
        {
            var sl = new ServiceLibrary();

            sl.Map<ITestService, TestServiceWithDependency>();
            sl.Map<ITestDependencyX, TestDependencyX>();

            var service = sl.GetService<ITestService>();

            service.Dummy();
        }

        [Test]
        public void GetService_MultipleDependencies_Works()
        {
            var sl = new ServiceLibrary();

            sl.Map<ITestService, TestServiceWithMultiDependency>();
            sl.Map<ITestDependencyX, TestDependencyX>();
            sl.Map<ITestDependencyY, TestDependencyY>();

            var service = sl.GetService<ITestService>();

            service.Dummy();
        }

        [Test]
        public void GetService_NestedDependency_Works()
        {
            var sl = new ServiceLibrary();

            sl.Map<ITestService, TestServiceNestedDependency>();
            sl.Map<ITestDependencyX, TestDependencyX>();
            sl.Map<ITestDependencyY, TestDependencyY>();

            var service = sl.GetService<ITestService>();

            service.Dummy();
        }
    }
}
