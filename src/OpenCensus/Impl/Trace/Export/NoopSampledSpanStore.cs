﻿// <copyright file="NoopSampledSpanStore.cs" company="OpenCensus Authors">
// Copyright 2018, OpenCensus Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of theLicense at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace OpenCensus.Trace.Export
{
    using System;
    using System.Collections.Generic;

    internal sealed class NoopSampledSpanStore : SampledSpanStoreBase
    {
        private static readonly ISampledPerSpanNameSummary EmptyPerSpanNameSummary = SampledPerSpanNameSummary.Create(
            new Dictionary<ISampledLatencyBucketBoundaries, int>(), new Dictionary<CanonicalCode, int>());

        private static readonly ISampledSpanStoreSummary EmptySummary = SampledSpanStoreSummary.Create(new Dictionary<string, ISampledPerSpanNameSummary>());

        private static readonly IList<ISpanData> EmptySpanData = new List<ISpanData>();

        private readonly HashSet<string> registeredSpanNames = new HashSet<string>();

        public override ISampledSpanStoreSummary Summary
        {
            get
            {
                IDictionary<string, ISampledPerSpanNameSummary> result = new Dictionary<string, ISampledPerSpanNameSummary>();
                lock (this.registeredSpanNames)
                {
                    foreach (string registeredSpanName in this.registeredSpanNames)
                    {
                        result[registeredSpanName] = EmptyPerSpanNameSummary;
                    }
                }

                return SampledSpanStoreSummary.Create(result);
            }
        }

        public override ISet<string> RegisteredSpanNamesForCollection
        {
            get
            {
                return new HashSet<string>(this.registeredSpanNames);
            }
        }

        public override void ConsiderForSampling(ISpan span)
        {
        }

        public override IList<ISpanData> GetErrorSampledSpans(ISampledSpanStoreErrorFilter filter)
        {
            return EmptySpanData;
        }

        public override IList<ISpanData> GetLatencySampledSpans(ISampledSpanStoreLatencyFilter filter)
        {
            return EmptySpanData;
        }

        public override void RegisterSpanNamesForCollection(IList<string> spanNames)
        {
            if (spanNames == null)
            {
                throw new ArgumentNullException(nameof(spanNames));
            }

            lock (this.registeredSpanNames)
            {
                foreach (var name in spanNames)
                {
                    this.registeredSpanNames.Add(name);
                }
            }
        }

        public override void UnregisterSpanNamesForCollection(IList<string> spanNames)
        {
            if (spanNames == null)
            {
                throw new ArgumentNullException(nameof(spanNames));
            }

            lock (this.registeredSpanNames)
            {
                foreach (var name in spanNames)
                {
                    this.registeredSpanNames.Remove(name);
                }
            }
        }
    }
}
