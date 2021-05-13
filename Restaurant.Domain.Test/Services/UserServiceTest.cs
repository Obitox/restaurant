using Moq;
using Restaurant.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Restaurant.Infrastracture.Test.Services
{
    public class UserServiceTest
    {
        [Fact]
        public void TestIsRegistered()
        {
            // Setup
            var mock = new Mock<UserService>
            {
                CallBase = true
            };

        }
    }
}
