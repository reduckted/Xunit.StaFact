﻿// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE.txt file in the project root for full license information.

#if !NET45 && !NETCOREAPP1_0

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class StaTheoryTests
{
    public static object[][] MemberDataSource => new object[][]
    {
        new object[] { 1, 2 },
        new object[] { 3, 4 },
    };

    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task StaTheory_OnSTAThread(int arg)
    {
        Assert.Null(SynchronizationContext.Current);
#if !NETCOREAPP1_0
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
#endif
        await Task.Yield();

        // Without a single-threaded SynchronizationContext, we won't come back to the STA thread.
        Assert.Null(SynchronizationContext.Current);
#if !NETCOREAPP1_0
        Assert.Equal(ApartmentState.MTA, Thread.CurrentThread.GetApartmentState());
#endif

        Assert.True(arg == 0 || arg == 1);
    }

    [Trait("Category", "FailureExpected")]
    [StaTheory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task StaTheoryFails(int arg)
    {
        Assert.Null(SynchronizationContext.Current);
#if !NETCOREAPP1_0
        Assert.Equal(ApartmentState.STA, Thread.CurrentThread.GetApartmentState());
#endif
        await Task.Yield();

        // Without a single-threaded SynchronizationContext, we won't come back to the STA thread.
        Assert.Null(SynchronizationContext.Current);
#if !NETCOREAPP1_0
        Assert.Equal(ApartmentState.MTA, Thread.CurrentThread.GetApartmentState());
#endif

        Assert.False(arg == 0 || arg == 1);
    }

    [StaTheory]
    [MemberData(nameof(MemberDataSource))]
    public void MemberBasedTheory(int a, int b)
    {
        Assert.Equal(b, a + 1);
    }

    [StaTheory, Trait("Category", "FailureExpected")]
    [InlineData(1)]
    public async Task OperationCanceledException_Thrown(int a)
    {
        Assert.Equal(1, a);
        await Task.Yield();
        throw new OperationCanceledException();
    }
}

#endif