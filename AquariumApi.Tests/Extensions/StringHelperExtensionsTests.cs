using AquariumApi.Core;
using AquariumApi.DataAccess;
using AquariumApi.DataAccess.AutoMapper;
using AquariumApi.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AquariumApiTests
{
    public class StringHelperExtensionsTests
    {
        public StringHelperExtensionsTests()
        {
            
        }
        [Fact]
        public void TestAggregateParams()
        {
            var testString = "this/is/a/rest/{apiKey}/endpoint";
            var expectedString = $"this/is/a/rest/expected/endpoint";

            var test = testString.AggregateParams("expected");
            Assert.Equal(expectedString, test);
        }
    }
}
