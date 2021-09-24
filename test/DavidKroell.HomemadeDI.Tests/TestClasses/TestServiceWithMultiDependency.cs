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

    internal class TestServiceWithMultiDependency : ITestService
    {
        private readonly ITestDependencyX _testDependencyX;
        private readonly ITestDependencyY _testDependencyY;

        public TestServiceWithMultiDependency(ITestDependencyX testDependencyX, ITestDependencyY testDependencyY)
        {
            _testDependencyX = testDependencyX;
            _testDependencyY = testDependencyY;
        }

        public void Dummy()
        {
        }
    }
}
