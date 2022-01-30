using System;
using System.Collections.Generic;
using System.Linq;

namespace AquariumApi.Models
{
    public class PaginationSliver
    {
        public int Start { get; set; }
        public int Count { get; set; }
        public bool Descending { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<int> Ids { get; set; }
        public Func<object,bool> CompareFunc;
    }
    public static class LINQPaginationSliverExtension
    {
        public static IEnumerable<T> ApplyPaginationSliver<T>(this IEnumerable<T> source,PaginationSliver pagination)
        {
            if (pagination == null)
                return source;
            if (pagination.Descending)
                source = source.OrderByDescending(p => 
                    ((Indexable)(object)p).StartTime
                );
            else
                source = source.OrderBy(p =>
                    ((Indexable)(object)p).StartTime
                );
            if (pagination.StartDate.HasValue)
            {
                source = source.Where(p =>
                {
                    Indexable obj = (Indexable)(object)p;
                    return obj.StartTime >= pagination.StartDate.Value;
                });
            }
            if (pagination.EndDate.HasValue)
            {
                source = source.Where(p =>
                {
                    Indexable obj = (Indexable)(object)p;
                    return obj.StartTime <= pagination.EndDate.Value;
                });
            }
            if (pagination.Ids != null && pagination.Ids.Any())
                source = source.Where(p => !pagination.Ids.Contains(((Indexable)(object)p).Id.Value));

            if (pagination.CompareFunc != null)
                source = source.Where(p => pagination.CompareFunc(p));
            source = source.Skip(pagination.Start).Take(pagination.Count);
            return source;
        }
    }
}
