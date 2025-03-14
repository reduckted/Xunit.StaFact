// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE file in the project root for full license information.

using System.Windows.Threading;
using Xunit.Sdk;

namespace Xunit;

/// <summary>
/// Identifies an xunit theory that starts on an STA thread
/// with a WPF <see cref="DispatcherSynchronizationContext" />.
/// Tests will be Skipped on non-Windows operating systems.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer(typeof(WpfTheoryDiscoverer))]
public class WpfTheoryAttribute : TheoryAttribute
{
}
