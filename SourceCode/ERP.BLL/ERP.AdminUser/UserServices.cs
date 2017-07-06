using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.CustomEnums;
using ERP.Tools;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.AdminUser
{
    public class UserServices
    {
        public List<VMERPUser> GetAllAdminUsers(VMERPUser currentUser, string userName, string displayName, string email, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows)
        {
            List<VMERPUser> adminUsers = new List<VMERPUser>();
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.SystemUsers.Include("Hierarchy").Where(p => true);
                    if (!string.IsNullOrEmpty(userName))
                    {
                        query = query.Where(p => p.UserName.Contains(userName));
                    }

                    if (!string.IsNullOrEmpty(displayName))
                    {
                        query = query.Where(p => p.DisplayName.Contains(displayName));
                    }

                    if (!string.IsNullOrEmpty(email))
                    {
                        query = query.Where(p => p.Email.Contains(email));
                    }

                    if (sortColumnsNames != null && sortColumnsNames.Count > 0
                        && sortColumnOrders != null && sortColumnOrders.Count > 0
                        && sortColumnsNames.Count == sortColumnOrders.Count)
                    {
                        query = query.OrderBy(sortColumnsNames, sortColumnOrders);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.CreateDate);
                    }

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                    totalRows = query.Count();
                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        adminUsers = new List<VMERPUser>();
                        foreach (var entity in dataFromDB)
                        {
                            VMERPUser user = new VMERPUser();
                            user.DisplayName = entity.DisplayName;
                            user.UserID = entity.UserID;
                            user.UserName = entity.UserName;
                            user.Status = (AdminUserStatus)entity.Status;
                            user.Email = entity.Email;
                            user.HierachyName = entity.Hierarchy.Name;
                            if (entity.UserRoles != null)
                            {
                                user.RoleNames = new List<string>();
                                foreach (var r in entity.UserRoles)
                                {
                                    user.RoleNames.Add(r.ERPRole.Name);
                                }
                            }

                            adminUsers.Add(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return adminUsers;
        }

        public VMERPUser GetUser(string userName)
        {
            VMERPUser user = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    SystemUser dbUser = context.SystemUsers.FirstOrDefault(p => p.UserName == userName);

                    if (dbUser != null)
                    {
                        user = new VMERPUser();
                        user.UserID = dbUser.UserID;
                        user.UserName = dbUser.UserName;
                        user.Password = dbUser.Password;
                        user.DisplayName = dbUser.DisplayName;
                        user.Status = (AdminUserStatus)dbUser.Status;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return user;
        }

        public VMERPUser GetValidUser(string userName, string password)
        {
            VMERPUser user = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    SystemUser dbUser = context.SystemUsers.FirstOrDefault(p => p.UserName.ToLower() == userName.ToLower() && p.Password == password && p.Status != (short)AdminUserStatus.Locked);

                    if (dbUser != null)
                    {
                        user = new VMERPUser();
                        user.UserID = dbUser.UserID;
                        user.UserName = dbUser.UserName;
                        user.Password = dbUser.Password;
                        user.DisplayName = dbUser.DisplayName;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return user;
        }

        public bool CreateUser(VMERPUser erpUser)
        {
            bool isSuccess = false;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    context.SystemUsers.Add(new SystemUser()
                    {
                        UserName = erpUser.UserName,
                        Password = erpUser.Password,
                        DisplayName = erpUser.DisplayName,
                        EmailSign = erpUser.EmailSign,
                    });

                    int rows = context.SaveChanges();
                    isSuccess = (rows == 1);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return isSuccess;
        }

        public VMERPUser GetCurrentUserInfos(string userName)
        {
            VMERPUser user = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    SystemUser dbUser = context.SystemUsers.Include("Hierarchy").Where(p => p.UserName.ToLower() == userName.ToLower()).FirstOrDefault();
                    if (dbUser != null)
                    {
                        user = GetUserFromDB(dbUser);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return user;
        }

        public VMERPUser GetUserByID(int userID)
        {
            VMERPUser user = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    SystemUser dbUser = context.SystemUsers.Include("Hierarchy").Where(p => p.UserID == userID).FirstOrDefault();
                    if (dbUser != null)
                    {
                        user = GetUserFromDB(dbUser);
                        user.EmailSign = dbUser.EmailSign;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return user;
        }

        public DBOperationStatus UpdateUser(string updateUser, VMERPUser user)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    SystemUser dbUser = context.SystemUsers.Where(p => p.UserID == user.UserID).FirstOrDefault();
                    if (dbUser != null)
                    {
                        dbUser.LastModifyDate = DateTime.Now;
                        dbUser.LastModifyBy = user.UserID.ToString();

                        if (!string.IsNullOrEmpty(user.EmailPassword))
                        {
                            dbUser.EmailPassword = DTRequest.Encrypt(user.EmailPassword);//TODO:暂时的
                        }
                        dbUser.DisplayName = user.DisplayName;
                        dbUser.EmailSign = user.EmailSign;
                        dbUser.HierarchyID = user.HierachyID;
                        dbUser.DataPermissionForCustomer = (short)user.DataPermissionForCustomer;
                        dbUser.DataPermissionForProduct = (short)user.DataPermissionForProduct;
                        dbUser.DataPermissionForQuote = (short)user.DataPermissionForQuote;
                        dbUser.DataPermissionForSample = (short)user.DataPermissionForSample;
                        dbUser.DataPermissionForOrder = (short)user.DataPermissionForOrder;
                        dbUser.DataPermissionForPurchase = (short)user.DataPermissionForPurchase;
                        dbUser.DataPermissionForPacks = (short)user.DataPermissionForPacks;
                        dbUser.DataPermissionForOutsourcing = (short)user.DataPermissionForOutsourcing;
                        dbUser.DataPermissionForFactory = (short)user.DataPermissionForFactory;
                        dbUser.DataPermissionForCustomer = (short)user.DataPermissionForCustomer;
                        dbUser.DataPermissionForShipmentAgency = (short)user.DataPermissionForShipmentAgency;
                        dbUser.DataPermissionForDelivery = (short)user.DataPermissionForDelivery;
                        dbUser.DataPermissionForProducePlan = (short)user.DataPermissionForProducePlan;
                        dbUser.ForInspectionReceipt = (short)user.ForInspectionReceipt;
                        dbUser.ForInspectionCustoms = (short)user.ForInspectionCustoms;
                        dbUser.ForInspectionClearance = (short)user.ForInspectionClearance;
                        dbUser.ForInspectionExchange = (short)user.ForInspectionExchange;
                        dbUser.ForThirdParty = (short)user.ForThirdParty;
                        dbUser.ForShippingMark = (short)user.ForShippingMark;
                        dbUser.ForDocumentsIndexing = (short)user.ForDocumentsIndexing;
                        dbUser.ForFinance = (short)user.ForFinance;

                        dbUser.UserRoles.Clear();
                        dbUser.UserCustomerRelationships.Clear();
                        int affectRows = context.SaveChanges();

                        foreach (var roleid in user.Roles.Distinct())
                        {
                            if (roleid != 0)
                            {
                                dbUser.UserRoles.Add(new UserRole() { UserID = user.UserID, RoleID = roleid, CreateBy = updateUser, CreateDate = DateTime.Now });
                            }
                        }

                        if (user.Customers != null && user.Customers.Count > 0)
                        {
                            foreach (var item in user.Customers.Distinct())
                            {
                                if (item != 0)
                                {
                                    dbUser.UserCustomerRelationships.Add(new UserCustomerRelationship()
                                    {
                                        CustomerID = item,
                                        UserID = user.UserID,
                                        WorkflowType = 20,//TODO 暂定都是20
                                    });
                                }
                            }
                        }
                        affectRows += context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            result = DBOperationStatus.Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return result;
        }

        private VMERPUser GetUserFromDB(SystemUser dbUser)
        {
            VMERPUser user = null;
            if (dbUser != null)
            {
                user = new VMERPUser();
                user.UserID = dbUser.UserID;
                user.DisplayName = dbUser.DisplayName;
                //user.EmailSign = dbUser.EmailSign;//如果内容有换行，再次就登录不了了。
                user.UserName = dbUser.UserName;
                user.Portrait = dbUser.Portrait;
                user.IsSuperAdmin = dbUser.IsSuperAdmin;
                user.Email = dbUser.Email;
                user.Status = (AdminUserStatus)dbUser.Status;
                user.HierachyType = (HierachyType)dbUser.Hierarchy.HierachyType;
                user.HierachyID = dbUser.HierarchyID;
                user.HierachyName = dbUser.Hierarchy.Name;
                user.DataPermissionForCustomer = (DataPermissions)dbUser.DataPermissionForCustomer;
                user.DataPermissionForFactory = (DataPermissions)dbUser.DataPermissionForFactory;
                user.DataPermissionForOrder = (DataPermissions)dbUser.DataPermissionForOrder;
                user.DataPermissionForOutsourcing = (DataPermissions)dbUser.DataPermissionForOutsourcing;
                user.DataPermissionForPacks = (DataPermissions)dbUser.DataPermissionForPacks;
                user.DataPermissionForProduct = (DataPermissions)dbUser.DataPermissionForProduct;
                user.DataPermissionForPurchase = (DataPermissions)dbUser.DataPermissionForPurchase;
                user.DataPermissionForQuote = (DataPermissions)dbUser.DataPermissionForQuote;
                user.DataPermissionForSample = (DataPermissions)dbUser.DataPermissionForSample;
                user.DataPermissionForShipmentAgency = (DataPermissions)dbUser.DataPermissionForShipmentAgency;
                user.DataPermissionForDelivery = (DataPermissions)dbUser.DataPermissionForDelivery;
                user.DataPermissionForProducePlan = (DataPermissions)dbUser.DataPermissionForProducePlan;
                user.ForInspectionReceipt = (DataPermissions)dbUser.ForInspectionReceipt;
                user.ForInspectionCustoms = (DataPermissions)dbUser.ForInspectionCustoms;
                user.ForInspectionClearance = (DataPermissions)dbUser.ForInspectionClearance;
                user.ForInspectionExchange = (DataPermissions)dbUser.ForInspectionExchange;
                user.ForThirdParty = (DataPermissions)dbUser.ForThirdParty;
                user.ForShippingMark = (DataPermissions)dbUser.ForShippingMark;
                user.ForDocumentsIndexing = (DataPermissions)dbUser.ForDocumentsIndexing;
                user.ForFinance = (DataPermissions)dbUser.ForFinance;

                user.EmailPassword = dbUser.EmailPassword;

                if (dbUser.UserRoles != null && dbUser.UserRoles.Count > 0)
                {
                    user.Roles = new List<int>();
                    user.RoleNames = new List<string>();
                    foreach (var ur in dbUser.UserRoles)
                    {
                        user.Roles.Add(ur.RoleID);
                        user.RoleNames.Add(ur.ERPRole.Name);
                    }
                }

                //user.ForCustomer = dbUser.DataPermissionForCustomer;
                //user.ForFactory = dbUser.DataPermissionForFactory;
                //user.ForOrder = dbUser.DataPermissionForOrder;
                //user.ForOutsourcing = dbUser.DataPermissionForOutsourcing;
                //user.ForPacks = dbUser.DataPermissionForPacks;
                //user.ForProduct = dbUser.DataPermissionForProduct;
                //user.ForPurchase = dbUser.DataPermissionForPurchase;
                //user.ForQuote = dbUser.DataPermissionForQuote;
                //user.ForSample = dbUser.DataPermissionForSample;
                //user.ForShipmentAgency = dbUser.DataPermissionForShipmentAgency;
                //user.ForDelivery = dbUser.DataPermissionForDelivery;//出运
            }
            return user;
        }

        public List<VMERPRoles> GetAllRoles(string roleName, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows)
        {
            List<VMERPRoles> roles = new List<VMERPRoles>();
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.ERPRoles.Where(p => !p.IsDeleted);

                    if (!string.IsNullOrEmpty(roleName))
                    {
                        query = query.Where(p => p.Name.Contains(roleName));
                    }

                    if (sortColumnsNames != null && sortColumnsNames.Count > 0
                        && sortColumnOrders != null && sortColumnOrders.Count > 0
                        && sortColumnsNames.Count == sortColumnOrders.Count)
                    {
                        query = query.OrderBy(sortColumnsNames, sortColumnOrders);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.CreateDate);
                    }

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                    totalRows = query.Count();
                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        roles = new List<VMERPRoles>();
                        foreach (var entity in dataFromDB)
                        {
                            VMERPRoles role = new VMERPRoles();
                            role.RoleID = entity.UserRoleID;
                            role.Name = entity.Name;
                            role.Description = entity.Description;
                            role.CreateDate = entity.CreateDate;
                            role.LastModifyDate = entity.LastModifyDate;

                            roles.Add(role);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return roles;
        }

        /// <summary>
        /// 返回所有的角色，如果用户属于某角色，该角色的CanView属性为true
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<VMERPRoles> GetAllRolesByUser(int userID)
        {
            List<VMERPRoles> roles = new List<VMERPRoles>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dataFromDB = context.ERPRoles.Where(p => !p.IsDeleted).ToList();
                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        var userRoles = context.UserRoles.Where(p => p.UserID == userID).ToList();
                        roles = new List<VMERPRoles>();
                        foreach (var entity in dataFromDB)
                        {
                            VMERPRoles role = new VMERPRoles();
                            role.RoleID = entity.UserRoleID;
                            role.Name = entity.Name;
                            role.Description = entity.Description;
                            role.CreateDate = entity.CreateDate;
                            role.LastModifyDate = entity.LastModifyDate;
                            role.CanView = userRoles.Exists(p => p.RoleID == entity.UserRoleID);

                            roles.Add(role);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return roles;
        }

        /// <summary>
        /// 返回所有的客户，如果客户属于该用户，该客户的CanView属性为true
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<VMCustomer> GetAllCustomer(int userID)
        {
            List<VMCustomer> list = new List<VMCustomer>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dataFromDB = context.Orders_Customers.Where(p => !p.IsDelete).OrderBy(d => d.CustomerCode).ToList();
                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        var query_UserCustomerRelationships = context.UserCustomerRelationships.Where(p => p.UserID == userID).ToList();
                        list = new List<VMCustomer>();
                        foreach (var entity in dataFromDB)
                        {
                            VMCustomer vm = new VMCustomer();
                            vm.CustomerID = entity.OCID;
                            vm.CustomerName = entity.CustomerName;
                            vm.CustomerCode = entity.CustomerCode;
                            vm.CanView = query_UserCustomerRelationships.Exists(p => p.CustomerID == entity.OCID);

                            list.Add(vm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return list;
        }
        public DBOperationStatus CreateRole(VMERPUser currentUser, VMERPRoles roleInfo)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    if (context.ERPRoles.Where(d => d.Name == roleInfo.Name).Count() > 0)//如果存在相同名称的角色
                    {
                        result = DBOperationStatus.ExistingRecord;
                        return result;
                    }

                    ERPRole role = new ERPRole();
                    role.UserRoleID = roleInfo.RoleID;
                    role.Name = roleInfo.Name;
                    role.Description = roleInfo.Description;
                    role.CreateBy = currentUser.UserName;
                    role.CreateDate = DateTime.Now;

                    context.ERPRoles.Add(role);
                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        result = DBOperationStatus.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        public DBOperationStatus UpdateRole(VMERPUser currentUser, VMERPRoles roleInfo)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    if (context.ERPRoles.Where(d => d.Name == roleInfo.Name).Count() > 0)//如果存在相同名称的角色
                    {
                        result = DBOperationStatus.ExistingRecord;
                        return result;
                    }

                    ERPRole role = context.ERPRoles.FirstOrDefault(p => p.UserRoleID == roleInfo.RoleID);
                    if (role != null)
                    {
                        role.Name = roleInfo.Name;
                        role.Description = roleInfo.Description;
                        role.LastModifyBy = currentUser.UserName;
                        role.LastModifyDate = DateTime.Now;

                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            result = DBOperationStatus.Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        public List<VMHierarchy> GetAllHierarchies(List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows)
        {
            List<VMHierarchy> hierarchies = new List<VMHierarchy>();
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Hierarchies.Where(p => !p.IsDeleted);
                    if (sortColumnsNames != null && sortColumnsNames.Count > 0
                        && sortColumnOrders != null && sortColumnOrders.Count > 0
                        && sortColumnsNames.Count == sortColumnOrders.Count)
                    {
                        query = query.OrderBy(sortColumnsNames, sortColumnOrders);
                    }
                    else
                    {
                        query = query.OrderByDescending(p => p.CreateDate);
                    }

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                    totalRows = query.Count();
                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        hierarchies = new List<VMHierarchy>();
                        foreach (var entity in dataFromDB)
                        {
                            VMHierarchy hierarchy = new VMHierarchy();
                            hierarchy.HierarchyID = entity.HierarchyID;
                            hierarchy.Name = entity.Name;
                            hierarchy.CreateDate = entity.CreateDate;

                            hierarchies.Add(hierarchy);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return hierarchies;
        }

        public List<VMHierarchy> GetAllHierarchies(HierachyType? type)
        {
            List<VMHierarchy> hierarchies = new List<VMHierarchy>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Hierarchies.Where(p => !p.IsDeleted);
                    if (type.HasValue)
                    {
                        query = query.Where(p => p.HierachyType == (short)type);
                    }
                    var dataFromDB = query.ToList();
                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        hierarchies = new List<VMHierarchy>();
                        foreach (var entity in dataFromDB)
                        {
                            VMHierarchy hierarchy = new VMHierarchy();
                            hierarchy.HierarchyID = entity.HierarchyID;
                            hierarchy.Name = entity.Name;
                            hierarchy.CreateDate = entity.CreateDate;
                            hierarchies.Add(hierarchy);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return hierarchies;
        }

        /// <summary>
        /// 获取所有部门关系，并包含主、从关系对象
        /// </summary>
        /// <returns></returns>
        public List<VMHierarchy> GetAllHierarchiesCascade()
        {
            List<VMHierarchy> hierarchies = new List<VMHierarchy>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dataFromDB = context.Hierarchies.Include("Hierarchy1").Include("Hierarchy2").Where(p => p.IsDeleted == false).ToList();
                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        hierarchies = new List<VMHierarchy>();
                        foreach (var entity in dataFromDB)
                        {
                            int recursiveMaxDepth = AdminConsts.RECURSIVE_MAX_DEPTH;

                            hierarchies.Add(GetHierarchies(entity, ref recursiveMaxDepth));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return hierarchies;
        }

        private static VMHierarchy GetHierarchies(Hierarchy currentHierarchy, ref int recursiveDepth)
        {
            if (recursiveDepth < 0)
            {
                throw new Exception("递归次数超过了最大限制：" + AdminConsts.RECURSIVE_MAX_DEPTH);
            }
            VMHierarchy hierarchy = new VMHierarchy();
            hierarchy.Name = currentHierarchy.Name;
            hierarchy.HierarchyID = currentHierarchy.HierarchyID;
            hierarchy.HierachyType = (HierachyType)currentHierarchy.HierachyType;
            if (currentHierarchy.Hierarchy2 != null)
            {
                hierarchy.ParentHierarchy = new VMHierarchy() { Name = currentHierarchy.Hierarchy2.Name, HierarchyID = currentHierarchy.Hierarchy2.HierarchyID };
            }
            hierarchy.SubHierarchies = new List<VMHierarchy>();
            foreach (var subHierarchy in currentHierarchy.Hierarchy1)
            {
                hierarchy.SubHierarchies.Add(GetHierarchies(subHierarchy, ref recursiveDepth));
            }

            recursiveDepth--;

            return hierarchy;
        }

        public VMERPRoles GetRoleByID(int roleID)
        {
            VMERPRoles role = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var entity = context.ERPRoles.Where(p => p.UserRoleID == roleID).FirstOrDefault();
                    if (entity != null)
                    {
                        role = new VMERPRoles();
                        role.RoleID = entity.UserRoleID;
                        role.Name = entity.Name;
                        role.Description = entity.Description;
                        role.CreateDate = entity.CreateDate;
                        role.LastModifyDate = entity.LastModifyDate;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return role;
        }

        public DBOperationStatus SetUserStatus(int userID, AdminUserStatus adminUserStatus)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var user = context.SystemUsers.Where(p => p.UserID == userID).FirstOrDefault();
                    if (user != null)
                    {
                        user.Status = (short)adminUserStatus;
                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            result = DBOperationStatus.Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        public AdminUserStatus GetUserStatus(string userName)
        {
            AdminUserStatus status = default(AdminUserStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.SystemUsers.Where(p => p.UserName.ToLower() == userName.ToLower()).Select(p => p.Status);
                    if (query!=null && query.Count()>0)
                    {
                        status = (AdminUserStatus)query.FirstOrDefault();
                    }
                    else
                    {
                        status = AdminUserStatus.NotFind;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return status;
        }

        public List<int> GetUserAllowedPages(VMERPUser user)
        {
            List<int> allowedMenus = new List<int>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    if (user.IsSuperAdmin)
                    {
                        allowedMenus = UserMenuServices.SYSTEM_ALL_MENUS.Select(p => p.MenuID).ToList();
                    }
                    else
                    {
                        var menusFromAllRoles = (from ur in context.UserRoles
                                                 where ur.UserID == user.UserID && ur.ERPRole != null && ur.ERPRole.IsDeleted == false
                                                 select new
                                                 {
                                                     AllowedMenus = (from perm in ur.ERPRole.RolePermissions
                                                                     where perm.IsDelete == false
                                                                     select perm.MenuID).ToList()
                                                 }).ToList();

                        foreach (var menu in menusFromAllRoles)
                        {
                            allowedMenus.AddRange(menu.AllowedMenus);
                        }

                        allowedMenus = allowedMenus.Distinct<int>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return allowedMenus;
        }

        public List<int> GetRoleAllowedPages(int roleID)
        {
            List<int> allowedMenus = new List<int>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var menusOfRole = (from ur in context.RolePermissions
                                       where ur.RoleID == roleID && ur.ERPRole.IsDeleted == false && ur.IsDelete == false
                                       select ur.MenuID).ToList();
                    if (menusOfRole != null)
                    {
                        foreach (var menu in menusOfRole)
                        {
                            allowedMenus.Add(menu);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return allowedMenus;
        }

        public bool HasPagePermission(int userID, int menuID)
        {
            bool hasPermission = false;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var allPermissions = (from ur in context.UserRoles
                                          where ur.UserID == userID && ur.ERPRole.IsDeleted == false
                                          select ur.ERPRole.RolePermissions.Any(p => p.MenuID == menuID && p.IsDelete == false)).ToList();
                    if (allPermissions != null)
                    {
                        foreach (var per in allPermissions)
                        {
                            hasPermission |= per;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return hasPermission;
        }

        public int GetPageElementsPrivileges(VMERPUser currentUser, int menuID)
        {
            int pagePrivileges = 0;
            try
            {
                // admin和数据字典不做页面元素控制
                if (currentUser.IsSuperAdmin || menuID == (int)ERPPage.SystemManagement_Dictionaries)
                {
                    // int类型31位均设置为1
                    pagePrivileges = 2147483647;
                }
                else
                {
                    using (ERPEntitiesNew context = new ERPEntitiesNew())
                    {
                        var pageElementsPrivilegesFromAllRoles = (from ur in context.UserRoles
                                                                  where ur.UserID == currentUser.UserID && ur.ERPRole != null && ur.ERPRole.IsDeleted == false
                                                                  select new
                                                                  {
                                                                      PageElementsPrivilegesFromAllRoles = (from perm in ur.ERPRole.RolePermissions
                                                                                                            where perm.MenuID == menuID
                                                                                                            select perm.MenuElementPermission).ToList()
                                                                  }).ToList();

                        if (pageElementsPrivilegesFromAllRoles != null && pageElementsPrivilegesFromAllRoles.Count > 0)
                        {
                            foreach (var priv in pageElementsPrivilegesFromAllRoles)
                            {
                                foreach (var t in priv.PageElementsPrivilegesFromAllRoles)
                                {
                                    pagePrivileges |= t;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return pagePrivileges;
        }

        public int GetPageElementsPrivileges(VMERPUser currentUser, List<int> menuIDs)
        {
            int pagePrivileges = 0;
            try
            {
                if (currentUser.IsSuperAdmin)
                {
                    // int类型31位均设置为1
                    pagePrivileges = 2147483647;
                }
                else
                {
                    using (ERPEntitiesNew context = new ERPEntitiesNew())
                    {
                        var pageElementsPrivilegesFromAllRoles = (from ur in context.UserRoles
                                                                  where ur.UserID == currentUser.UserID && ur.ERPRole != null && ur.ERPRole.IsDeleted == false
                                                                  select new
                                                                  {
                                                                      PageElementsPrivilegesFromAllRoles = (from perm in ur.ERPRole.RolePermissions
                                                                                                            where menuIDs.Contains(perm.MenuID)
                                                                                                            select perm.MenuElementPermission).ToList()
                                                                  }).ToList();

                        if (pageElementsPrivilegesFromAllRoles != null && pageElementsPrivilegesFromAllRoles.Count > 0)
                        {
                            foreach (var priv in pageElementsPrivilegesFromAllRoles)
                            {
                                foreach (var t in priv.PageElementsPrivilegesFromAllRoles)
                                {
                                    pagePrivileges |= t;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return pagePrivileges;
        }

        public int GetPageElementsPrivilegesByRole(int roleID, int menuID)
        {
            int pagePrivileges = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var pageElementsPrivileges = (from rolePer in context.RolePermissions
                                                  where rolePer.RoleID == roleID && rolePer.ERPRole.IsDeleted == false && rolePer.MenuID == menuID
                                                  select rolePer).FirstOrDefault();

                    if (pageElementsPrivileges != null)
                    {
                        pagePrivileges = pageElementsPrivileges.MenuElementPermission;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return pagePrivileges;
        }

        public DBOperationStatus SavePageElementPrivileges(VMERPUser currentUser, int roleID, int menuID, int elementPrivilege)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    // 修改页面元素控制权限
                    var permissionElements = context.RolePermissions.FirstOrDefault(p => p.RoleID == roleID && p.MenuID == menuID);
                    if (permissionElements != null)
                    {
                        permissionElements.MenuElementPermission = elementPrivilege;
                    }
                    else
                    {
                        permissionElements = new RolePermission();
                        permissionElements.RoleID = roleID;
                        permissionElements.MenuID = menuID;
                        permissionElements.MenuElementPermission = elementPrivilege;
                        permissionElements.CreateBy = currentUser.UserName;
                        permissionElements.CreateDate = DateTime.Now;
                        context.RolePermissions.Add(permissionElements);
                    }

                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        result = DBOperationStatus.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return result;
        }

        public DBOperationStatus SavePageViewPrivileges(VMERPUser currentUser, int roleID, List<int> menus)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                if (menus != null)
                {
                    menus.Remove(0);
                }

                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var currentPermissions = context.RolePermissions.Where(p => p.RoleID == roleID).ToList();

                    var menusRemoved = currentPermissions.Where(p => true);
                    if (menus != null)
                    {
                        foreach (var menu in menus)
                        {
                            var exist = currentPermissions.FirstOrDefault(p => p.MenuID == menu);
                            if (exist != null)
                            {
                                exist.IsDelete = false;
                            }
                            else
                            {
                                RolePermission rolePerm = new RolePermission();
                                rolePerm.RoleID = roleID;
                                rolePerm.MenuID = menu;
                                rolePerm.IsDelete = false;
                                rolePerm.CreateBy = currentUser.UserName;
                                rolePerm.CreateDate = DateTime.Now;
                                context.RolePermissions.Add(rolePerm);
                            }
                        }

                        menusRemoved = currentPermissions.Where(p => !menus.Contains(p.MenuID));
                    }

                    if (menusRemoved != null)
                    {
                        foreach (var m in menusRemoved)
                        {
                            m.IsDelete = true;
                        }
                    }

                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        result = DBOperationStatus.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return result;
        }

        public DBOperationStatus ChangePwd(string username, string newPassword)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var user = context.SystemUsers.Where(p => p.UserName.ToLower() == username.ToLower()).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = newPassword;
                        user.Status = (short)AdminUserStatus.Normal;
                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            result = DBOperationStatus.Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        public DBOperationStatus SaveOrUpdateUserCustomPageSettings(int userID, DatagridCustomColumnVisibilityModules module, DTOUserCustomPageSettings settings)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var settingsDBEntity = context.UserCustomPageSettings.Where(p => p.UserID == userID && p.PageID == (int)module).FirstOrDefault();
                    if (settingsDBEntity != null)
                    {
                        settingsDBEntity.DatagridColumnVisibility = settings.DatagridColumnVisibility ?? string.Empty;
                        settingsDBEntity.LastUpdateDate = DateTime.Now;
                    }
                    else
                    {
                        UserCustomPageSetting settingsNew = new UserCustomPageSetting();
                        settingsNew.UserID = userID;
                        settingsNew.PageID = (int)module;
                        settingsNew.DatagridColumnVisibility = settings.DatagridColumnVisibility ?? string.Empty;
                        settingsNew.LastUpdateDate = DateTime.Now;
                        context.UserCustomPageSettings.Add(settingsNew);
                    }

                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        result = DBOperationStatus.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        public DTOUserCustomPageSettings GetUserCustomPageSettings(int userID, DatagridCustomColumnVisibilityModules module)
        {
            DTOUserCustomPageSettings settings = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var settingsDBEntity = context.UserCustomPageSettings.Where(p => p.UserID == userID && p.PageID == (int)module).FirstOrDefault();
                    if (settingsDBEntity != null)
                    {
                        settings = new DTOUserCustomPageSettings();
                        settings.DatagridColumnVisibility = settingsDBEntity.DatagridColumnVisibility ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return settings;
        }

        public bool CompareOldPwd(int userID, string oldPassword)
        {
            bool result = false;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    return context.SystemUsers.Any(p => p.UserID == userID && p.Password == oldPassword);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        public List<VMERPUser> GetUserList()
        {
            List<VMERPUser> vm_list = new List<VMERPUser>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.SystemUsers;
                    foreach (var item in query)
                    {
                        vm_list.Add(new VMERPUser()
                        {
                            UserID = item.UserID,
                            DisplayName = item.DisplayName,
                            UserName = item.UserName,
                            Email = item.Email,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return vm_list;
        }
    }
}