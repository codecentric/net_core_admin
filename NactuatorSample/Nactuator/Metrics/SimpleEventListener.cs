﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NetCoreAdmin.Metrics
{
    public class SimpleEventListener : EventListener, ISimpleEventListener
    {
        private static List<string> ignoredTagPayloads = new List<string>() { "Name", "DisplayName", "Mean", "Increment", "CounterType" };
        private readonly EventLevel level;
        private readonly ILogger<SimpleEventListener> logger;

        private ConcurrentStack<string> missedMessages = new ConcurrentStack<string>();

        public SimpleEventListener(ILogger<SimpleEventListener> logger)
            : base()
        {
            // todo make intervalSec configurable
            level = EventLevel.Verbose; // todo make configurable too
            this.logger = logger;

            Metrics = new ConcurrentDictionary<string, MetricsData>();

            while (missedMessages.TryPop(out var msg))
            {
                logger.LogDebug("EventSource '{source}' ignored", msg);
            }
        }

        public int EventCount { get; private set; } = 0;

        public ConcurrentDictionary<string, MetricsData> Metrics { get; }

        protected override void OnEventSourceCreated([NotNull] EventSource eventSource)
        {
            if (eventSource is null)
            {
                throw new ArgumentNullException(nameof(eventSource));
            }

            // todo get them from config
            var whiteList = new List<string>() { "System.Runtime", "Microsoft.AspNetCore.Hosting", "Microsoft-AspNetCore-Server-Kestrel", "Microsoft-Extensions-DependencyInjection" };

            if (!whiteList.Contains(eventSource.Name))
            {
                if (logger == null)
                {
                    // this will happen because events are send BEFORE the Constructor runs.
                    missedMessages.Push(eventSource.Name);
                }
                else
                {
                    logger.LogDebug("EventSource '{source}' ignored", eventSource.Name);
                }

                return;
            }

            var refreshInterval = new Dictionary<string, string?>
            {
                { "EventCounterIntervalSec", "1" },
            };

            EnableEvents(eventSource, level, (EventKeywords)(-1), refreshInterval);
        }

        protected override void OnEventWritten([NotNull] EventWrittenEventArgs eventData)
        {
            if (eventData is null)
            {
                throw new ArgumentNullException(nameof(eventData));
            }

            string eventName = eventData.EventName ?? "unknown event";
            ReadOnlyCollection<object?> payload = eventData.Payload ?? new List<object?>().AsReadOnly();
            if (eventName.Equals("EventCounters", StringComparison.Ordinal))
            {
                for (int i = 0; i < payload.Count; i++)
                {
                    if (payload[i] is IDictionary<string, object> eventPayload)
                    {
                        var name = string.Intern(eventPayload["Name"].ToString()!);
                        var displayName = string.Intern(eventPayload["DisplayName"].ToString()!);

                        MetricsData metricsData = new MetricsData()
                        {
                            Name = displayName,
                            Description = eventData.Message ?? string.Empty,
                            BaseUnit = null!,
                            Measurements = GetMeasurement(eventPayload),
                            AvailableTags = GetTags(eventPayload),
                        };
                        Metrics[name] = metricsData;
                    }
                    else
                    {
                        logger.LogDebug("{eventName} has am unrecognized payload of type {unknownType}", eventName, payload.GetType());
                    }
                }
            }

            // there are lots of things which are not EventCounters ,e .g. opened connections from/to. Would be interesting to see if we can derive stats from this
        }

        private IEnumerable<Measurement> GetMeasurement(IDictionary<string, object> eventPayload)
        {
            var counterType = eventPayload["CounterType"].ToString();
            double value;
            string statistic;
            switch (counterType)
            {
                case "Sum":
                    value = double.Parse(eventPayload["Increment"].ToString()!, CultureInfo.InvariantCulture);
                    statistic = "VALUE";
                    break;
                case "Mean":
                    value = double.Parse(eventPayload["Mean"].ToString()!, CultureInfo.InvariantCulture);
                    statistic = "VALUE";
                    break;
                default:
                    value = 0;
                    logger.LogDebug("{counterName} is neither an Increment nor a Mean", eventPayload["Name"]);
                    statistic = "UNKNOWN"; // we cannot accurately map to actuators types
                    break;
            }

            return new List<Measurement>()
            {
                new Measurement()
                {
                    Statistic = statistic,
                    Value = value,
                },
            };
        }

        private IEnumerable<AvailableTag> GetTags(IDictionary<string, object> eventPayload)
        {
            return eventPayload.Where(x => !ignoredTagPayloads.Contains(x.Key)).Select(kvp => new AvailableTag()
            {
                Tag = kvp.Key,
                Values = new List<string>() { kvp.Value.ToString()! },
            }).ToList();
        }
    }
}
