﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace NeinLinq
{
    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public abstract class RewriteQueryable : IOrderedQueryable
    {
        /// <summary>
        /// Actual query.
        /// </summary>
        public IQueryable Query { get; }

        /// <summary>
        /// Rewriter to rewrite the query.
        /// </summary>
        public RewriteQueryProvider Provider { get; }

        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        protected RewriteQueryable(IQueryable query, RewriteQueryProvider provider)
        {
            if (query is null)
                throw new ArgumentNullException(nameof(query));
            if (provider is null)
                throw new ArgumentNullException(nameof(provider));

            Query = query;
            Provider = provider;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            // rewrite on enumeration
            return Provider.RewriteQuery(Expression).GetEnumerator();
        }

        /// <inheritdoc />
        public Type ElementType
            => Query.ElementType;

        /// <inheritdoc />
        public Expression Expression
            => Query.Expression;

        /// <inheritdoc />
        IQueryProvider IQueryable.Provider
            => Provider; // replace query provider
    }

    /// <summary>
    /// Proxy for rewritten queries.
    /// </summary>
    public class RewriteQueryable<T> : RewriteQueryable, IOrderedQueryable<T>
#if !(NET40 || NET45)
        , IAsyncEnumerable<T>
#endif
    {
        /// <summary>
        /// Create a new query to rewrite.
        /// </summary>
        /// <param name="query">The actual query.</param>
        /// <param name="provider">The provider to rewrite the query.</param>
        public RewriteQueryable(IQueryable query, RewriteQueryProvider provider)
            : base(query, provider)
        {
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            // rewrite on enumeration
            return Provider.RewriteQuery<T>(Expression).GetEnumerator();
        }

#if !(NET40 || NET45)
        /// <inheritdoc />
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            // rewrite on enumeration
            var enumerable = Provider.RewriteQuery<T>(Expression);
            return enumerable is IAsyncEnumerable<T> asyncEnumerable
                ? asyncEnumerable.GetAsyncEnumerator(cancellationToken)
                : new RewriteQueryEnumerator<T>(enumerable.GetEnumerator());
        }
#endif
    }
}
