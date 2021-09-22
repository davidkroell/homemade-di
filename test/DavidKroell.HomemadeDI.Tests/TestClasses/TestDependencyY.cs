namespace DavidKroell.HomemadeDI.Tests.TestClasses
{
    internal class TestDependencyY : ITestDependencyY
    {
        private readonly ITestDependencyX _testDependencyX;

        public TestDependencyY(ITestDependencyX testDependencyX)
        {
            _testDependencyX = testDependencyX;
        }
    }
}
