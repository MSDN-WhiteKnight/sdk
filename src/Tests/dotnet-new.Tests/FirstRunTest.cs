// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentAssertions;
using Microsoft.NET.TestFramework;
using Microsoft.NET.TestFramework.Assertions;
using Microsoft.NET.TestFramework.Commands;
using Microsoft.TemplateEngine.TestHelper;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.DotNet.New.Tests
{
    public class FirstRunTest : SdkTest
    {
        private readonly ITestOutputHelper _log;

        public FirstRunTest(ITestOutputHelper log) : base(log)
        {
            _log = log;
        }

        [Fact]
        public void FirstRunSuccess()
        {
            var home = TestUtils.CreateTemporaryFolder("Home");
            new DotnetNewCommand(_log)
                .WithCustomHive(home)
                .Execute()
                .Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.NotHaveStdOutContaining("Error");

            new DotnetNewCommand(_log, "--list")
                .WithCustomHive(home)
                .Execute()
                .Should()
                .ExitWith(0)
                .And.NotHaveStdErr()
                .And.HaveStdOutContaining("classlib");
        }
    }
}
