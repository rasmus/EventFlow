﻿// The MIT License (MIT)
// 
// Copyright (c) 2015-2018 Rasmus Mikkelsen
// Copyright (c) 2015-2018 eBay Software Foundation
// https://github.com/eventflow/EventFlow
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Threading.Tasks;
using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using EventFlow.Commands;
using EventFlow.Configuration;
using EventFlow.Extensions;
using EventFlow.TestHelpers;
using EventFlow.TestHelpers.Aggregates;
using EventFlow.TestHelpers.Aggregates.Commands;
using EventFlow.TestHelpers.Aggregates.Queries;
using FluentAssertions;
using NUnit.Framework;

namespace EventFlow.Tests.IntegrationTests
{
    [Category(Categories.Integration)]
    public class ResolverTests
    {
        public class Service { }

        public class ServiceDependentAggregate : AggregateRoot<ServiceDependentAggregate, ThingyId>
        {
            public Service Service { get; }

            public ServiceDependentAggregate(ThingyId id, Service service) : base(id)
            {
                Service = service;
            }
        }

        [Test]
        public async Task ResolverAggregatesFactoryCanResolve()
        {
            using (var resolver = EventFlowOptions.New
                .RegisterServices(sr => sr.RegisterType(typeof(Service)))
                .CreateResolver())
            {
                // Arrange
                var aggregateFactory = resolver.Resolve<IAggregateFactory>();

                // Act
                var serviceDependentAggregate = await aggregateFactory.CreateNewAggregateAsync<ServiceDependentAggregate, ThingyId>(ThingyId.New).ConfigureAwait(false);

                // Assert
                serviceDependentAggregate.Service.Should()
                    .NotBeNull()
                    .And
                    .BeOfType<Service>();
            }
        }

        [Test]
        public void RegistrationDoesntCauseStackOverflow()
        {
            using (var resolver = EventFlowOptions.New
                .AddDefaults(EventFlowTestHelpers.Assembly)
                .RegisterServices(s =>
                {
                    s.Register<IScopedContext, ScopedContext>(Lifetime.Scoped);
                })
                .CreateResolver())
            {
                resolver.Resolve<ICommandHandler<ThingyAggregate, ThingyId, IExecutionResult, ThingyAddMessageCommand>>();
            }
        }
    }
}