﻿using Insql.Mappers;
using Insql.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Insql
{
    internal class InsqlFactory : IInsqlFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IDbSessionFactory sessionFactory;
        private readonly IInsqlModel insqlModel;

        public InsqlFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            this.insqlModel = serviceProvider.GetRequiredService<IInsqlModel>();
            this.sessionFactory = serviceProvider.GetRequiredService<IDbSessionFactory>();
        }

        public IInsql Create(Type scopeType)
        {
            var insqlResolver = (IInsqlResolver)this.serviceProvider.GetRequiredService(typeof(IInsqlResolver).MakeGenericType(scopeType));

            return new InsqlImpl(scopeType, this.insqlModel, insqlResolver, this.sessionFactory);
        }
    }
}
