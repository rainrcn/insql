﻿namespace Insql.Resolvers
{
    public interface IInsqlResolver<TContext> : IInsqlResolver
        where TContext : class
    {
    }
}
