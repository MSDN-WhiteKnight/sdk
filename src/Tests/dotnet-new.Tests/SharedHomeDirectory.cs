// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.NET.TestFramework;
using Microsoft.NET.TestFramework.Assertions;
using Microsoft.NET.TestFramework.Commands;
using Microsoft.TemplateEngine.TestHelper;
using Xunit.Abstractions;

namespace Microsoft.DotNet.New.Tests
{
    /// <summary>
    /// This class represents shared /tmp/RANDOM-GUID/.templateengine/dotnetcli-preview/ folder
    /// shared between multiple unit tests in same class, this is so each test
    /// doesn't have to install everything from 0. To save some time executing tests.
    /// </summary>
    public class SharedHomeDirectory : IDisposable
    {
        private readonly HashSet<string> _installedPackages = new HashSet<string>();

        public SharedHomeDirectory(IMessageSink messageSink)
        {
            Log = new SharedTestOutputHelper(messageSink);
            Initialize();
        }

        public string HomeDirectory { get; } = TestUtils.CreateTemporaryFolder("Home");

        protected ITestOutputHelper Log { get; private set; }

        public void Dispose() => Directory.Delete(HomeDirectory, true);

        public void InstallPackage(string packageName, string? workingDirectory = null, string? nugetSource = null)
        {
            if (!_installedPackages.Add(packageName))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(workingDirectory))
            {
                workingDirectory = Directory.GetCurrentDirectory();
            }
            var args = new List<string> { "-i", packageName, };
            if (!string.IsNullOrWhiteSpace(nugetSource))
            {
                args.AddRange(new[] { "--nuget-source", nugetSource });
            }
            new DotnetNewCommand(Log, args.ToArray())
                .WithCustomHive(HomeDirectory)
                .WithWorkingDirectory(workingDirectory)
                .Execute()
                .Should()
                .ExitWith(0)
                .And
                .NotHaveStdErr();
        }

        private void Initialize()
        {
            new DotnetNewCommand(Log)
                .WithCustomHive(HomeDirectory)
                .Execute()
                .Should()
                .ExitWith(0)
                .And
                .NotHaveStdErr();

            new DotnetNewCommand(Log, "--install", TemplatePackagesPaths.MicrosoftDotNetCommonProjectTemplates31Path)
                .WithCustomHive(HomeDirectory)
                .Execute()
                .Should()
                .ExitWith(0)
                .And
                .NotHaveStdErr();

            new DotnetNewCommand(Log, "--install", TemplatePackagesPaths.MicrosoftDotNetCommonProjectTemplates50Path)
                .WithCustomHive(HomeDirectory)
                .Execute()
                .Should()
                .ExitWith(0)
                .And
                .NotHaveStdErr();
        }
    }
}
