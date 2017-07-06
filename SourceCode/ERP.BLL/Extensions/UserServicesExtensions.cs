using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL
{
    public static class UserServicesExtensions
    {
        private const string HIERACHY_KEY = "$ERP_Hierachies";

        /// <summary>
        /// 判断数据访问权限的扩展方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="currentUser">查询该方法的用户实体对象</param>
        /// <param name="module">所需要查询的权限对应的模块名</param>
        /// <param name="createUserPropertyName">数据库中创建数据的用户外键对象的属性名称，默认值为：SystemUser</param>
        /// <returns></returns>
        public static IQueryable<T> SetDataPermissionConditions<T>(this IQueryable<T> query, VMERPUser currentUser, DataPermissionModules module, string createUserPropertyName = "SystemUser") where T : class
        {
            if (currentUser != null && !currentUser.IsSuperAdmin && query != null)
            {
                DataPermissions dataPermissions = default(DataPermissions);
                switch (module)
                {
                    case DataPermissionModules.ForCustomer:
                        dataPermissions = currentUser.DataPermissionForCustomer;
                        break;

                    case DataPermissionModules.ForFactory:
                        dataPermissions = currentUser.DataPermissionForFactory;
                        break;

                    case DataPermissionModules.ForOrder:
                        dataPermissions = currentUser.DataPermissionForOrder;
                        break;

                    case DataPermissionModules.ForOutsourcing:
                        dataPermissions = currentUser.DataPermissionForOutsourcing;
                        break;

                    case DataPermissionModules.ForPacks:
                        dataPermissions = currentUser.DataPermissionForPacks;
                        break;

                    case DataPermissionModules.ForProduct:
                        dataPermissions = currentUser.DataPermissionForProduct;
                        break;

                    case DataPermissionModules.ForPurchase:
                        dataPermissions = currentUser.DataPermissionForPurchase;
                        break;

                    case DataPermissionModules.ForQuote:
                        dataPermissions = currentUser.DataPermissionForQuote;
                        break;

                    case DataPermissionModules.ForSample:
                        dataPermissions = currentUser.DataPermissionForSample;
                        break;

                    case DataPermissionModules.ForShipmentAgency:
                        dataPermissions = currentUser.DataPermissionForShipmentAgency;
                        break;

                    case DataPermissionModules.ForDelivery:
                        dataPermissions = currentUser.DataPermissionForDelivery;
                        break;

                    case DataPermissionModules.ForProducePlan:
                        dataPermissions = currentUser.DataPermissionForProducePlan;
                        break;

                    case DataPermissionModules.ForInspectionReceipt:
                        dataPermissions = currentUser.ForInspectionReceipt;
                        break;

                    case DataPermissionModules.ForInspectionCustoms:
                        dataPermissions = currentUser.ForInspectionCustoms;
                        break;

                    case DataPermissionModules.ForInspectionClearance:
                        dataPermissions = currentUser.ForInspectionClearance;
                        break;

                    case DataPermissionModules.ForInspectionExchange:
                        dataPermissions = currentUser.ForInspectionExchange;
                        break;

                    case DataPermissionModules.ForThirdParty:
                        dataPermissions = currentUser.ForThirdParty;
                        break;

                    case DataPermissionModules.ForShippingMark:
                        dataPermissions = currentUser.ForShippingMark;
                        break;

                    case DataPermissionModules.ForDocumentsIndexing:
                        dataPermissions = currentUser.ForDocumentsIndexing;
                        break;

                    case DataPermissionModules.ForFinance:
                        dataPermissions = currentUser.ForFinance;
                        break;

                    default: break;
                }

                List<VMHierarchy> hierachies = System.Web.HttpContext.Current.Cache[HIERACHY_KEY] as List<VMHierarchy>;
                if (hierachies == null)
                {
                    hierachies = new UserServices().GetAllHierarchiesCascade();
                    System.Web.HttpContext.Current.Cache[HIERACHY_KEY] = hierachies;
                }

                List<int> h = new List<int>();
                switch (dataPermissions)
                {
                    case DataPermissions.InnerAndSameLevelAndAllSubHierachies:
                        {
                            var currentHierachy4InnerAndSameLevelAndAllSubHierachies = hierachies.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID);
                            if (currentHierachy4InnerAndSameLevelAndAllSubHierachies.ParentHierarchy != null)
                            {
                                var parentHierachy = hierachies.FirstOrDefault(p => p.HierarchyID == currentHierachy4InnerAndSameLevelAndAllSubHierachies.ParentHierarchy.HierarchyID);
                                int recursiveMaxDepth1 = AdminConsts.RECURSIVE_MAX_DEPTH;
                                h.AddRange(GetAllHierachiesAndSubs(parentHierachy, ref recursiveMaxDepth1));
                                query = query.Where(FilterByHierachies<T>(h));
                            }
                            break;
                        }
                    case DataPermissions.InnerAndSameLevelHierachies:
                        {
                            var currentHierachy4InnerAndSameLevelHierachies = hierachies.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID);
                            if (currentHierachy4InnerAndSameLevelHierachies.ParentHierarchy != null)
                            {
                                h = hierachies.Where(p => p.HierarchyID == currentHierachy4InnerAndSameLevelHierachies.ParentHierarchy.HierarchyID).SelectMany(p => p.SubHierarchies).Select(p => p.HierarchyID).ToList();
                                query = query.Where(FilterByHierachies<T>(h));
                            }
                            break;
                        }
                    case DataPermissions.InnerAndSubHierachies:
                        {
                            var currentHierachy = hierachies.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID);
                            int recursiveMaxDepth = AdminConsts.RECURSIVE_MAX_DEPTH;
                            h.AddRange(GetAllHierachiesAndSubs(currentHierachy, ref recursiveMaxDepth));
                            h.Add(currentUser.HierachyID);
                            query = query.Where(FilterByHierachies<T>(h));
                            break;
                        }
                    case DataPermissions.InnerHierachy:
                        {
                            h.Add(currentUser.HierachyID);
                            query = query.Where(FilterByHierachies<T>(h));
                            break;
                        }
                    case DataPermissions.SelfOnly:
                        {
                            query = query.Where(FilterByUserID<T>(currentUser.UserID));
                            break;
                        }
                    case DataPermissions.InnerHierachyAndAllAgencies:
                        {
                            h.Add(currentUser.HierachyID);
                            // 包含所有办事处
                            foreach (var hierachy in hierachies)
                            {
                                if (hierachy.HierachyType == HierachyType.Agency)
                                {
                                    h.Add(hierachy.HierarchyID);
                                }
                            }
                            query = query.Where(FilterByHierachies<T>(h));
                            break;
                        }
                    case DataPermissions.InnerAndSubHierachiesAndAllAgencies:
                        {
                            var currentHierachy = hierachies.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID);
                            int recursiveMaxDepth = AdminConsts.RECURSIVE_MAX_DEPTH;
                            h.AddRange(GetAllHierachiesAndSubs(currentHierachy, ref recursiveMaxDepth));
                            h.Add(currentUser.HierachyID);
                            // 包含所有办事处
                            foreach (var hierachy in hierachies)
                            {
                                if (hierachy.HierachyType == HierachyType.Agency)
                                {
                                    h.Add(hierachy.HierarchyID);
                                }
                            }
                            query = query.Where(FilterByHierachies<T>(h));
                            break;
                        }
                    case DataPermissions.InnerAndSameLevelAndSubHierachiesAndAllAgencies:
                        {
                            var currentHierachy = hierachies.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID);
                            int recursiveMaxDepth = AdminConsts.RECURSIVE_MAX_DEPTH;
                            h.AddRange(GetAllHierachiesAndSubs(currentHierachy, ref recursiveMaxDepth));
                            h.Add(currentUser.HierachyID);

                            // 所有的同级部门
                            var currentHierachy4InnerAndSameLevelHierachies = hierachies.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID);
                            if (currentHierachy4InnerAndSameLevelHierachies.ParentHierarchy != null)
                            {
                                var sameLevelHierachies = hierachies.Where(p => p.HierarchyID == currentHierachy4InnerAndSameLevelHierachies.ParentHierarchy.HierarchyID).SelectMany(p => p.SubHierarchies).Select(p => p.HierarchyID).ToList();
                                if (sameLevelHierachies != null && sameLevelHierachies.Count > 0)
                                {
                                    h.AddRange(sameLevelHierachies);
                                }
                            }
                            // 包含所有办事处
                            foreach (var hierachy in hierachies)
                            {
                                if (hierachy.HierachyType == HierachyType.Agency)
                                {
                                    h.Add(hierachy.HierarchyID);
                                }
                            }
                            query = query.Where(FilterByHierachies<T>(h));
                            break;
                        }
                    case DataPermissions.InnerHierachyAndAllAgenciesAndAllDesigners:
                        {
                            var currentHierachy = hierachies.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID);
                            int recursiveMaxDepth = AdminConsts.RECURSIVE_MAX_DEPTH;
                            h.AddRange(GetAllHierachiesAndSubs(currentHierachy, ref recursiveMaxDepth));
                            h.Add(currentUser.HierachyID);
                            // 包含所有办事处和设计部
                            foreach (var hierachy in hierachies)
                            {
                                if (hierachy.HierachyType == HierachyType.Agency || hierachy.HierachyType == HierachyType.Designer)
                                {
                                    h.Add(hierachy.HierarchyID);
                                }
                            }
                            query = query.Where(FilterByHierachies<T>(h));
                            break;
                        }
                    case DataPermissions.IncludeAllBusinessAndDesginDeptAndAgencies:
                        {
                            h.Add(currentUser.HierachyID);
                            var allBusinessAndDegisnDeptsAndAgencies = hierachies.Where(p => p.HierachyType == HierachyType.Business || p.HierachyType == HierachyType.Designer || p.HierachyType == HierachyType.Agency).Select(p => p.HierarchyID).ToList();
                            if (allBusinessAndDegisnDeptsAndAgencies != null)
                            {
                                h.AddRange(allBusinessAndDegisnDeptsAndAgencies);
                            }

                            query = query.Where(FilterByHierachies<T>(h));
                            break;
                        }
                    case DataPermissions.IncludeAllBusinessDept:
                        {
                            h.Add(currentUser.HierachyID);
                            var allBusinessApat = hierachies.Where(p => p.HierachyType == HierachyType.Business).Select(p => p.HierarchyID).ToList();
                            if (allBusinessApat != null)
                            {
                                h.AddRange(allBusinessApat);
                            }

                            query = query.Where(FilterByHierachies<T>(h));
                            break;
                        }
                    case DataPermissions.InnerAndSameLevelAndSubHierachiesAndAllBusinessDept:
                        {
                            var currentHierachy = hierachies.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID);
                            int recursiveMaxDepth = AdminConsts.RECURSIVE_MAX_DEPTH;
                            h.AddRange(GetAllHierachiesAndSubs(currentHierachy, ref recursiveMaxDepth));
                            h.Add(currentUser.HierachyID);

                            // 所有的同级部门
                            var currentHierachy4InnerAndSameLevelHierachies = hierachies.FirstOrDefault(p => p.HierarchyID == currentUser.HierachyID);
                            if (currentHierachy4InnerAndSameLevelHierachies.ParentHierarchy != null)
                            {
                                var sameLevelHierachies = hierachies.Where(p => p.HierarchyID == currentHierachy4InnerAndSameLevelHierachies.ParentHierarchy.HierarchyID).SelectMany(p => p.SubHierarchies).Select(p => p.HierarchyID).ToList();
                                if (sameLevelHierachies != null && sameLevelHierachies.Count > 0)
                                {
                                    h.AddRange(sameLevelHierachies);
                                }
                            }
                            // 包含所有业务部
                            foreach (var hierachy in hierachies)
                            {
                                if (hierachy.HierachyType == HierachyType.Business)
                                {
                                    h.Add(hierachy.HierarchyID);
                                }
                            }
                            query = query.Where(FilterByHierachies<T>(h));
                            break;
                        }
                    default: break;
                }
            }
            return query;
        }

        private static List<int> GetAllHierachiesAndSubs(VMHierarchy parentHierarchy, ref int recursiveDepth)
        {
            if (recursiveDepth < 0)
            {
                throw new Exception("递归次数超过了最大限制：" + AdminConsts.RECURSIVE_MAX_DEPTH);
            }
            List<int> result = new List<int>();
            if (parentHierarchy != null)
            {
                foreach (var h in parentHierarchy.SubHierarchies)
                {
                    result.Add(h.HierarchyID);
                    result.AddRange(GetAllHierachiesAndSubs(h, ref recursiveDepth));
                }
            }
            recursiveDepth--;
            return result;
        }

        private static MethodInfo _methodInfo = typeof(List<int>).GetMethod("Contains", new Type[] { typeof(int) });

        private static Expression<Func<T, bool>> FilterByHierachies<T>(List<int> hierachies)
        {
            var list = Expression.Constant(hierachies);

            var param = Expression.Parameter(typeof(T), "j");
            var value = Expression.Property(Expression.Property(param, "SystemUser"), "HierarchyID");
            var body = Expression.Call(list, _methodInfo, value);

            // j => codes.Contains(j.Code)
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        private static Expression<Func<T, bool>> FilterByUserID<T>(int userID)
        {
            var userFromParam = Expression.Constant(userID);
            var param = Expression.Parameter(typeof(T), "j");
            var value = Expression.Property(Expression.Property(param, "SystemUser"), "UserID");
            var userFromProperty = Expression.Constant(value);
            var body = Expression.Equal(value, userFromParam);

            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        /// <summary>
        /// 遍历集合查找属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="childrenSelector"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static T FirstOrDefaultFromMany<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector, Predicate<T> condition)
        {
            // return default if no items
            if (source == null || !source.Any()) return default(T);

            // return result if found and stop traversing hierarchy
            var attempt = source.FirstOrDefault(t => condition(t));
            if (!Equals(attempt, default(T))) return attempt;

            // recursively call this function on lower levels of the
            // hierarchy until a match is found or the hierarchy is exhausted
            return source.SelectMany(childrenSelector)
                .FirstOrDefaultFromMany(childrenSelector, condition);
        }
    }
}