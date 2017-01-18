using Xunit;
using WebService.Controllers;

namespace WebService.UnitTests
{
    public class HealthCheckControllerTests
    {
        [Fact]
        public void DoesNotBlowUpOnLogger()
        {
            new HealthCheckController().Get();
        }
    }
}