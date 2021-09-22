namespace DavidKroell.HomemadeDI.Tests.TestClasses
{
    internal class TestServiceWithMultiCtor : ITestService
    {
        public TestServiceWithMultiCtor(ITestDependencyX testDependencyX)
        {
        }

        public TestServiceWithMultiCtor(string dummy)
        {
        }

        public void Dummy()
        {
        }
    }
}
