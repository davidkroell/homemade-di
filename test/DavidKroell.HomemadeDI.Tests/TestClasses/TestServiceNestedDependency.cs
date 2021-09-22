namespace DavidKroell.HomemadeDI.Tests.TestClasses
{
    internal class TestServiceNestedDependency : ITestService
    {
        private readonly ITestDependencyY _testDependencyY;

        public TestServiceNestedDependency(ITestDependencyY testDependencyY)
        {
            _testDependencyY = testDependencyY;
        }

        public void Dummy()
        {
        }
    }
}
