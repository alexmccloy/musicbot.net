using System;
using System.Linq;
using System.Linq.Expressions;

namespace Amccloy.MusicBot.Net.Utils;

public static class EfCoreExtensions
{
    public static IQueryable<TSource> DbDistinctBy<TSource, TKey>  (this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
    {
        return source.GroupBy(keySelector).Select(x => x.FirstOrDefault());
    }
}