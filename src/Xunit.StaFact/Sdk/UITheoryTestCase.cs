// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace Xunit.Sdk;

public class UITheoryTestCase : XunitTheoryTestCase
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public UITheoryTestCase()
    {
    }

    internal UITheoryTestCase(UITestCase.SyncContextType synchronizationContextType, TestMethodDisplay defaultMethodDisplay, TestMethodDisplayOptions defaultMethodDisplayOptions, ITestMethod testMethod, UISettingsAttribute settings)
        : base(defaultMethodDisplay, defaultMethodDisplayOptions, testMethod)
    {
        this.Settings = settings;
        settings.ApplyTraits(this);
        this.SynchronizationContextType = synchronizationContextType;
    }

    internal UISettingsAttribute Settings { get; private set; } = UISettingsAttribute.Default;

    internal UITestCase.SyncContextType SynchronizationContextType { get; private set; }

    public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
    {
        using ThreadRental threadRental = await ThreadRental.CreateAsync(UITestCase.GetAdapter(this.SynchronizationContextType), this.TestMethod);
        await threadRental.SynchronizationContext;

        // TODO: retry here if any test cases failed
        return await new UITheoryTestCaseRunner(this, this.DisplayName, this.SkipReason, constructorArguments, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource, threadRental).RunAsync();
    }

    public override void Deserialize(IXunitSerializationInfo data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        base.Deserialize(data);

        this.Settings = new UISettingsAttribute() { MaxAttempts = data.GetValue<int>(nameof(UISettingsAttribute.MaxAttempts)) };
        this.SynchronizationContextType = (UITestCase.SyncContextType)Enum.Parse(typeof(UITestCase.SyncContextType), data.GetValue<string>("SyncContextType"));
    }

    public override void Serialize(IXunitSerializationInfo data)
    {
        if (data is null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        base.Serialize(data);

        data.AddValue(nameof(UISettingsAttribute.MaxAttempts), this.Settings.MaxAttempts);
        data.AddValue("SyncContextType", this.SynchronizationContextType.ToString());
    }
}
