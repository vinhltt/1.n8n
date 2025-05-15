using System.Linq.Expressions;
using CoreFinance.Contracts.BaseEfModels;
using CoreFinance.Contracts.Constants;
using CoreFinance.Contracts.DTOs;
using CoreFinance.Contracts.Enums;
using CoreFinance.Contracts.Extensions;
using Microsoft.EntityFrameworkCore;
// ReSharper disable All
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.

namespace CoreFinance.Contracts.EntityFrameworkUtilities
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Filter<T>(this IQueryable<T> source, FilterRequest? filter) where T : class
        {
            if (filter == null || filter.Details.IsNullOrEmpty())
            {
                return source;
            }
            var fields = typeof(T).GetProperties();
            var filters = filter.Details!.Where(d => fields.Any(f => string.Equals(d.AttributeName, f.Name, StringComparison.OrdinalIgnoreCase))).Select(e => new FilterDescriptor
            {
                Field = e.AttributeName,
                Values = e.FilterType == FilterType.In ? e.Value!.Split("|") : [e.Value ?? ""],
                LogicalOperator = filter.LogicalOperator,
                Operator = e.FilterType
            }).ToList();
            return source.Where(ExpressionBuilder.Build<T>(filters));
        }

        /// <summary> 
        /// Determines whether this instance is ordered. 
        /// </summary>
        /// <typeparam name = "T" ></typeparam >
        /// <param name="source">The queryable.</param> 
        /// <returns> 
        /// <c>true</c> if the specified queryable is ordered; otherwise, <c>false</c>. 
        /// </returns> 
        /// <exception cref="ArgumentNullException">queryable</exception> O references 
        public static bool IsOrdered<T>(this IQueryable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return source.Expression.Type.IsAssignableFrom(typeof(IOrderedQueryable<T>));
        }
        #region Where
        /// <summary> 
        /// Filters data in the given source with filter descriptor. 
        /// </summary> WII <typeparam name="T"></typeparam> 
        /// <param name="source">The source.</param> WII <param name="filter">The filter.</param> 
        /// <param name="parameterName">Name of the parameter.</param> 
        /// <returns></returns> 2 references 
        public static IQueryable<T> Where<T>(this IQueryable<T> source, FilterDescriptor filter, string parameterName = "x")
        where T : class
        {
            var expression = ExpressionBuilder.Build<T>(filter, parameterName);
            if (expression == null)
                return source;
#if AS_DEBUG
            System.Diagnostics.Debug.WriteLine("Filter Expression: " + expression.Body.ToString());
#endif
            return source.Where(expression);
        }

        /// <summary> 
        /// Filters data in the given source with filter descriptors. 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="source">The source.</param> W <param name="filters">The filters.</param> 
        /// <param name="parameterName">Name of the parameter.</param> 
        /// <returns></returns> 4 references 
        public static IQueryable<T> Where<T>(this IQueryable<T> source, IEnumerable<FilterDescriptor> filters, string parameterName = "x")
        where T : class
        {
            var expression = ExpressionBuilder.Build<T>(filters, parameterName);
            if (expression == null)
                return source;
#if AS_DEBUG
            System.Diagnostics.Debug.WriteLine("Filter Expression: " + expression.Body.ToString();
#endif
            return source.Where(expression);
        }

        #endregion
        #region OrderBy

        /// <summary> 
        /// Orders data in the given source with sort descriptor. 
        /// </summary> VII <typeparam name="T"></typeparam>, 
        /// <param name="source">The source.</param> 
        /// <param name="sort">The sort.</param>
        /// <param name = "replaceOrder" >if set to<c>true</c> [replace the current order in source].</param> 
        /// <returns></returns> O references 
        public static IQueryable<T>? OrderBy<T>(this IQueryable<T>? source, SortDescriptor? sort, bool replaceOrder = true)
        where T : class
        {
            if (source == null || sort == null)
                return source;
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, sort.Field);
            return source.OrderBy(property, parameter, sort.Direction, replaceOrder);
        }

        /// <summary> 
        /// Orders data in the given source with sort descriptors. 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="source">The source.</param> MI <param name="sorts">The sorts.</param>
        /// <param name = "replaceOrder" >if set to<c>true</c> [replace the current order in source].</param>)
        /// <returns></returns> 5 references 
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IEnumerable<SortDescriptor>? sorts, bool replaceOrder = true)
        where T : class
        {
            if (source.IsNullOrEmpty() || sorts.IsNullOrEmpty())
            {
                return source;
            }
            var parameter = Expression.Parameter(typeof(T));
            foreach (var item in sorts)
            {
                var property = Expression.Property(parameter, item.Field);
                source = source.OrderBy(property, parameter, item.Direction, replaceOrder);
                replaceOrder = false;
            }
            return source;
        }

        /// <summary> 
        /// Orders data in the given source. 
        /// </summary>
        /// <typeparam name="T"></typeparam> MI <param name="source">The source.</param> 
        /// <param name="property">The property.</param>
        /// <param name="parameter">The parameter.</param> 
        /// <param name="direction">The direction.</param>
        /// <param name = "replaceOrder" >if set to<c>true</c> [replace the current order in source].</param> 
        /// <returns></returns> 2 references 
        private static IQueryable<T> OrderBy<T>(this IQueryable<T> source, MemberExpression property, ParameterExpression parameter, SortDirection direction, bool replaceOrder = true)
        where T : class
        {
            var propertyType = property.Type;
            var underlyingType = Nullable.GetUnderlyingType(propertyType);
            var isNullable = underlyingType != null;
            if (propertyType == typeof(string))
            {
                return source.OrderBy<T, string>(property, parameter, direction, replaceOrder);
            }
            if (propertyType == typeof(DateTime) || isNullable && underlyingType == typeof(DateTime))
            {
                if (isNullable)
                    return source.OrderBy<T, DateTime?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, DateTime>(property, parameter, direction, replaceOrder);
            }
            if (propertyType == typeof(int) || isNullable && underlyingType == typeof(int))
            {
                if (isNullable)
                    return source.OrderBy<T, int?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, int>(property, parameter, direction, replaceOrder);
            }
            if (propertyType == typeof(long) || isNullable && underlyingType == typeof(long))
            {
                if (isNullable)
                    return source.OrderBy<T, long?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, long>(property, parameter, direction, replaceOrder);
            }
            if (propertyType == typeof(bool) || isNullable && underlyingType == typeof(bool))
            {
                if (isNullable)
                    return source.OrderBy<T, bool?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, bool>(property, parameter, direction, replaceOrder);
            }
            if (propertyType == typeof(Guid) || isNullable && underlyingType == typeof(Guid))
            {
                if (isNullable)
                    return source.OrderBy<T, Guid?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, Guid>(property, parameter, direction, replaceOrder);
            }

            if (propertyType == typeof(TimeSpan) || isNullable && underlyingType == typeof(TimeSpan))
            {
                if (isNullable)
                    return source.OrderBy<T, TimeSpan?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, TimeSpan>(property, parameter, direction, replaceOrder);
            }
            if (propertyType == typeof(DateTimeOffset) || isNullable && underlyingType == typeof(DateTimeOffset))
            {
                if (isNullable)
                    return source.OrderBy<T, DateTimeOffset?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, DateTimeOffset>(property, parameter, direction, replaceOrder);
            }
            if (propertyType == typeof(byte) || isNullable && underlyingType == typeof(byte))
            {
                if (isNullable)
                    return source.OrderBy<T, byte?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, byte>(property, parameter, direction, replaceOrder);
            }
            if (propertyType == typeof(float) || isNullable && underlyingType == typeof(float))
            {
                if (isNullable)
                    return source.OrderBy<T, float?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, float>(property, parameter, direction, replaceOrder);
            }

            if (propertyType == typeof(double) || isNullable && underlyingType == typeof(double))
            {
                if (isNullable)
                    return source.OrderBy<T, double?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, double>(property, parameter, direction, replaceOrder);
            }
            if (propertyType == typeof(decimal) || isNullable && underlyingType == typeof(decimal))
            {
                if (isNullable)
                    return source.OrderBy<T, decimal?>(property, parameter, direction, replaceOrder);
                return source.OrderBy<T, decimal>(property, parameter, direction, replaceOrder);
            }
            return source.OrderBy<T, dynamic>(property, parameter, direction, replaceOrder);
        }

        /// <summary> 
        /// Orders data in the given source. 
        /// </summary> 777 <typeparam name="T"></typeparam> 
        /// <typeparam name="TProperty">The type of the property.</typeparam> 
        /// <param name="source">The source.</param> W <param name="property">The property.</param> 
        /// <param name="parameter">The parameter.</param> III <param name="direction">The direction.</param>
        /// <param name="replaceOrder">if set to <c>true</c> [replace the current order in source.</param> 
        /// <returns></returns> 24 references 
        private static IOrderedQueryable<T> OrderBy<T, TProperty>(this IQueryable<T> source, MemberExpression property, ParameterExpression parameter, SortDirection direction, bool replaceOrder = true)
        {
            var expression = Expression.Lambda<Func<T, TProperty>>(property, parameter);
            if (replaceOrder || !source.Expression.Type.IsAssignableFrom(typeof(IOrderedQueryable<T>)) || source is not IOrderedQueryable<T> orderedQueryable)
                return direction == SortDirection.Asc ? source.OrderBy(expression) : source.OrderByDescending(expression);
            return direction == SortDirection.Asc ? orderedQueryable.ThenBy(expression) : orderedQueryable.ThenByDescending(expression);
        }
        #endregion

        #region Paging
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static async Task<IBasePaging<TSource>> ToPagingAsync<TSource>(this IQueryable<TSource> queryable, Pagination pagination)
        {
            if (pagination == null)
            {
                throw new ArgumentNullException(nameof(pagination));
            }
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }
            var pageIndex = pagination.PageIndex;
            if (pageIndex < 1)
            {
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(pageIndex),
                    actualValue: pageIndex,
                    message: ConstantCommon.PAGE_NUMBER_BELOW_1
                );
            }
            var pageSize = pagination.PageSize;
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pageSize),
                    pageSize,
                    ConstantCommon.PAGE_SIZE_LESS_THAN_1
                );
            }
            var result = new BasePaging<TSource>();
            var skip = (pageIndex - 1) * pageSize;
            var totalRow = await queryable.CountAsync();
            if (totalRow > 0 && totalRow <= skip)
            {
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(pageSize),
                    actualValue: pageSize,
                    message: ConstantCommon.PAGE_INDEX_OUT_OF_RANGE
                );
            }
            result.Data = await queryable
                                   .Skip(skip)
                                   .Take(pageSize)
                                   .ToListAsync();
            var newPagination = new Pagination
            {
                TotalRow = totalRow,
                PageCount = totalRow > 0 ? (int)Math.Ceiling(totalRow / (double)pageSize) : 0,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            result.Pagination = newPagination;
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<IBasePaging<TSource>> ToPagingAsync<TSource>(this IQueryable<TSource> queryable, IFilterBodyRequest request)
            where TSource : class
        {

            if (request.Filter != null && !request.Filter.Details.IsNullOrEmpty())
                queryable = queryable.Filter(request.Filter);

            if (!request.Orders.IsNullOrEmpty())
                queryable = queryable.OrderBy(request.Orders);

            if (request.Pagination == null || request.Pagination.PageIndex < 1)
            {
                request.Pagination = new Pagination();
            }
            var result = await queryable.ToPagingAsync(request.Pagination);

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="request"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<IBasePaging<TResult>> ToPagingAsync<TResult, TSource>(this IQueryable<TSource> queryable, IFilterBodyRequest request, Expression<Func<TSource, TResult>> selector) where TResult : class
        {
            if (request == null || request.Pagination == null)
                throw new ArgumentNullException(nameof(request));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var viewModelQuery = queryable.Select(selector);

            var result = await viewModelQuery.ToPagingAsync(request);

            return result;
        }
        #endregion
    }
}
