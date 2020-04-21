﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Steeltoe.Common.Expression;
using Steeltoe.Messaging;
using Steeltoe.Stream.Config;
using System;

namespace Steeltoe.Stream.Binder
{
    public class PartitionHandler
    {
        internal readonly IPartitionKeyExtractorStrategy _partitionKeyExtractorStrategy;
        internal readonly IPartitionSelectorStrategy _partitionSelectorStrategy;

        private readonly IExpressionParser _expressionParser;
        private readonly IEvaluationContext _evaluationContext;
        private readonly IProducerOptions _producerOptions;

        public PartitionHandler(
                IExpressionParser expressionParser,
                IEvaluationContext evaluationContext,
                IProducerOptions options,
                IPartitionKeyExtractorStrategy partitionKeyExtractorStrategy,
                IPartitionSelectorStrategy partitionSelectorStrategy)
        {
            _expressionParser = expressionParser;
            _evaluationContext = evaluationContext;
            _producerOptions = options;
            _partitionKeyExtractorStrategy = partitionKeyExtractorStrategy;
            _partitionSelectorStrategy = partitionSelectorStrategy;
            PartitionCount = _producerOptions.PartitionCount;
        }

        public int PartitionCount { get; set; }

        public int DeterminePartition(IMessage message)
        {
            var key = ExtractKey(message);

            int partition;
            if (!string.IsNullOrEmpty(_producerOptions.PartitionSelectorExpression) && _expressionParser != null)
            {
                var expr = _expressionParser.ParseExpression(_producerOptions.PartitionSelectorExpression);
                partition = expr.GetValue<int>(_evaluationContext, key);
            }
            else
            {
                partition = _partitionSelectorStrategy.SelectPartition(key, PartitionCount);
            }

            //// protection in case a user selector returns a negative.
            return Math.Abs(partition % PartitionCount);
        }

        private object ExtractKey(IMessage message)
        {
            var key = InvokeKeyExtractor(message);
            if (key == null && !string.IsNullOrEmpty(_producerOptions.PartitionKeyExpression) && _expressionParser != null)
            {
                var expr = _expressionParser.ParseExpression(_producerOptions.PartitionKeyExpression);
                key = expr.GetValue(_evaluationContext, message);
            }

            if (key == null)
            {
                throw new ArgumentException("Partition key cannot be null");
            }

            return key;
        }

        private object InvokeKeyExtractor(IMessage message)
        {
            if (_partitionKeyExtractorStrategy != null)
            {
                return _partitionKeyExtractorStrategy.ExtractKey(message);
            }

            return null;
        }
    }
}
