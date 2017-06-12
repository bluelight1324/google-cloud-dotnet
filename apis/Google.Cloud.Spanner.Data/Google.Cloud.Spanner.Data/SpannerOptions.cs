﻿// Copyright 2017 Google Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Google.Api.Gax;
using Google.Cloud.Spanner.V1;
using Google.Cloud.Spanner.V1.Logging;

namespace Google.Cloud.Spanner.Data
{
    /// <summary>
    /// Settings for <see cref="SpannerConnection"/>.
    /// </summary>
    public sealed class SpannerOptions
    {
        /// <summary>
        /// The singleton instance of <see cref="SpannerOptions"/>.
        /// </summary>
        public static SpannerOptions Instance { get; } = new SpannerOptions();

        /// <summary>
        /// The maximum number of Spanner sessions that are kept in a pool.
        /// If the number of pooled sessions reaches this level, then additional
        /// Sessions released to the pool will be deleted immediately.
        /// </summary>
        public int MaximumPooledSessions
        {
            get => SessionPool.MaximumPooledSessions;
            set => SessionPool.MaximumPooledSessions = value;
        }

        /// <summary>
        /// The maximum number of sessions that can be in active use by the application.
        /// If this number is reached, then any additional allocations from
        /// the internal session pool will act according to
        /// <see cref="ResourcesExhaustedBehavior"/>. If <see>
        /// <cref>ResourcesExhaustedBehavior.Block</cref>
        /// </see>
        /// , the operation will block until other operations have completed.
        /// If <see>
        /// <cref>ResourcesExhaustedBehavior.Fail</cref>
        /// </see>
        /// , the operation will immediately throw a <see cref="SpannerException"/> with
        /// an error code <see>
        /// <cref>ErrorCode.ResourceExhausted</cref>
        /// </see>.
        /// This number will be similar to the maximum number of simultaneous operations.
        /// </summary>
        public int MaximumActiveSessions
        {
            get => SessionPool.MaximumActiveSessions;
            set => SessionPool.MaximumActiveSessions = value;
        }

        /// <summary>
        /// Spanner sessions expire on the server after 60 minutes. However,
        /// <see cref="SpannerConnection"/> reserves a session for indefinite use
        /// and preserves the session by sending keepalive messages according to this
        /// interval.
        /// It should be set below the session expiry (currently 60 minutes) on the server.
        /// </summary>
        public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromMinutes(55);

        /// <summary>
        /// Sets the log level for diagnostic logs sent to Trace (for desktop) or stderr (for .Net Core).
        /// </summary>
        public LogLevel LogLevel
        {
            get
            {
                var underlyingLevel = (int) Logger.LogLevel;
                return GaxPreconditions.CheckEnumValue((LogLevel) underlyingLevel, nameof(LogLevel));
            }
            set
            {
                var underlyingLevel = (int) value;
                Logger.LogLevel = GaxPreconditions.CheckEnumValue((V1.Logging.LogLevel) underlyingLevel,
                    nameof(LogLevel));
            }
        }

        /// <summary>
        /// If true, then log messages will be displayed indicating the duration and
        /// count of various internal metrics, which can help identify bottlenecks in an
        /// application.
        /// </summary>
        internal bool LogPerformanceTraces
        {
            get => Logger.LogPerformanceTraces;
            set => Logger.LogPerformanceTraces = value;
        }

        /// <summary>
        /// Controls the duration between periodic performance trace logs.
        /// </summary>
        internal int PerformanceTraceLogInterval
        {
            get => Logger.PerformanceTraceLogInterval;
            set => Logger.PerformanceTraceLogInterval = value;
        }

        /// <summary>
        /// Specifies the duration which Spanner sessions will remain in the
        /// internal session pool.
        /// After this time expires, the session will be removed from the pool
        /// and deleted.
        /// </summary>
        public TimeSpan PoolEvictionDelay
        {
            get => SessionPool.PoolEvictionDelay;
            set => SessionPool.PoolEvictionDelay = value;
        }

        internal bool ResetPerformanceTracesEachInterval
        {
            get => Logger.ResetPerformanceTracesEachInterval;
            set => Logger.ResetPerformanceTracesEachInterval = value;
        }

        /// <summary>
        /// Defines the behavior once <see cref="MaximumActiveSessions"/> has been reached.
        /// If <see>
        /// <cref>ResourcesExhaustedBehavior.Block</cref>
        /// </see>
        /// , the operation will block until other operations have completed.
        /// If <see>
        /// <cref>ResourcesExhaustedBehavior.Fail</cref>
        /// </see>
        /// , the operation will immediately throw a <see cref="SpannerException"/> with
        /// an error code <see>
        /// <cref>ErrorCode.ResourceExhausted</cref></see>.
        /// </summary>
        public ResourcesExhaustedBehavior ResourcesExhaustedBehavior
        {
            get => SessionPool.WaitOnResourcesExhausted
                ? ResourcesExhaustedBehavior.Block
                : ResourcesExhaustedBehavior.Fail;
            set => SessionPool.WaitOnResourcesExhausted = value == ResourcesExhaustedBehavior.Block;
        }

        /// <summary>
        /// Defines the global timeout duration.
        /// Operations sent to the server that take greater than this duration will fail
        /// with a <see cref="SpannerException"/> and error code <see cref="ErrorCode.DeadlineExceeded"/>.
        /// </summary>
        public TimeSpan Timeout
        {
            get => SessionPool.Timeout;
            set => SessionPool.Timeout = value;
        }

        private SpannerOptions() { }
    }
}
