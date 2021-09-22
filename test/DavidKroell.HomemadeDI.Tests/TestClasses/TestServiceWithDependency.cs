namespace DavidKroell.HomemadeDI.Tests.TestClasses
{
    internal class TestServiceWithDependency : ITestService
    {
        private readonly ITestDependencyX _testDependencyX;

        public TestServiceWithDependency(ITestDependencyX testDependencyX)
        {
            _testDependencyX = testDependencyX;
        }

        public void Dummy()
        {
        }
    }
}
