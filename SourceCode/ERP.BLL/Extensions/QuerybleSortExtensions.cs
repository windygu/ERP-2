using ERP.BLL.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL
{
    public static class QuerybleSortExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, List<string> sortColumnsNames, List<string> sortColumnOrders)
        {
            IOrderedQueryable<T> returnExpression = null;

            if (sortColumnsNames != null && sortColumnsNames.Count > 0
                && sortColumnOrders != null && sortColumnOrders.Count > 0
                && sortColumnOrders.Count == sortColumnsNames.Count)
            {
                if (sortColumnOrders[0] == EasyuiPagenationConsts.SORT_ASCENDING)
                {
                    returnExpression = source.OrderBy(sortColumnsNames[0]);
                }
                else
                {
                    returnExpression = source.OrderByDescending(sortColumnsNames[0]);
                }

                if (sortColumnsNames.Count > 1)
                {
                    for (int i = 1; i < sortColumnsNames.Count; i++)
                    {
                        if (sortColumnOrders[i] == EasyuiPagenationConsts.SORT_ASCENDING)
                        {
                            returnExpression = returnExpression.ThenBy(sortColumnsNames[i]);
                        }
                        else
                        {
                            returnExpression = returnExpression.ThenByDescending(sortColumnsNames[i]);
                        }
                    }
                }
            }
            return returnExpression;
        }

        static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, List<string> sortColumnsNames, List<string> sortColumnOrders)
        {
            IOrderedQueryable<T> returnExpression = null;
            for (int i = 0; i < sortColumnsNames.Count; i++)
            {
                if (sortColumnOrders[i] == EasyuiPagenationConsts.SORT_ASCENDING)
                {
                    returnExpression = source.ThenBy(sortColumnsNames[i]);
                }
                else if (sortColumnOrders[i] == EasyuiPagenationConsts.SORT_DESCENDING)
                {
                    returnExpression = source.ThenByDescending(sortColumnsNames[i]);
                }
            }
            return returnExpression;
        }

        /// <summary>
        /// 根据属性名称顺序排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property">属性名称</param>
        /// <returns></returns>
        static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderBy");
        }

        /// <summary>
        /// 根据属性名称降序排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property">属性名称</param>
        /// <returns></returns>
        static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "OrderByDescending");
        }

        /// <summary>
        /// 根据属性名称降序排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property">属性名称</param>
        /// <returns></returns>
        static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }

        /// <summary>
        /// 根据属性名称顺序排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property">属性名称</param>
        /// <returns></returns>
        static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }


        static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
    }
}
