using AutoFixture;
using LoccarTests.Common;

namespace LoccarTests.Common
{
    public abstract class BaseUnitTest
    {
        protected readonly IFixture _fixture;

        protected BaseUnitTest()
        {
            _fixture = new Fixture();
            _fixture.Customize(new TestFixtureCustomizations());
        }
    }
}