using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomAttribute;
using ERP.Models.CustomEnums;
using ERP.Models.CustomEnums.PageElementsPrivileges;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    [ERP.WebNew.Attributes.AuthorizedUser(PageID = (int)ERPPage.SystemManagement_SystemUsers)]
    public class AuthController : Controller
    {
        private const string QUERY_USERNAME = "UserName";
        private const string QUERY_DISPLAYNAME = "DisplayName";
        private const string QUERY_EMAIL = "Email";
        private const string QUERY_ROLENAME = "Name";

        UserServices _userServices = new UserServices();
        public ActionResult Admin()
        {
            VMERPUser user = new VMERPUser();
            user.UserName = HttpContext.Request[QUERY_USERNAME];
            user.DisplayName = HttpContext.Request[QUERY_DISPLAYNAME];
            user.Email = HttpContext.Request[QUERY_EMAIL];
            return View(user);
        }

        public ActionResult GetAllAdminUsers()
        {
            int currentPage = 0, pageSize = 0, totalRows = 0;
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], out currentPage);
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], out pageSize);

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES]))
            {
                sortColumnsNames = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS]))
            {
                sortColumnOrders = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            string userName = HttpContext.Request[QUERY_USERNAME];
            string displayName = HttpContext.Request[QUERY_DISPLAYNAME];
            string email = HttpContext.Request[QUERY_EMAIL];

            List<VMERPUser> products = _userServices.GetAllAdminUsers(CurrentUserServices.Me, userName, displayName, email, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = products });
        }

        public ActionResult Role()
        {
            VMERPRoles role = new VMERPRoles();
            role.Name = HttpContext.Request[QUERY_ROLENAME];
            return View(role);
        }

        [HttpPost]
        public ActionResult SetUserStatus(VMERPUser user)
        {
            DBOperationStatus result = _userServices.SetUserStatus(user.UserID, (AdminUserStatus)user.Status);
            return new CustomJsonResult((short)result);
        }

        public ActionResult GetAllRoles()
        {
            int currentPage = 0, pageSize = 0, totalRows = 0;
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], out currentPage);
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], out pageSize);

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES]))
            {
                sortColumnsNames = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS]))
            {
                sortColumnOrders = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            string roleName = HttpContext.Request[QUERY_ROLENAME];

            List<VMERPRoles> roles = _userServices.GetAllRoles(roleName, sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = roles });
        }

        public ActionResult GetAllUserRoles(int ID)
        {
            List<VMERPRoles> roles = _userServices.GetAllRolesByUser(ID);
            var entityForTree = from r in roles
                                select new DTOAuthItem()
                                {
                                    ID = r.RoleID,
                                    Text = r.Name,
                                    Checked = r.CanView
                                };
            return new CustomJsonResult(new[] { new { id = 0, text = "所有角色", children = entityForTree } });
        }

        public ActionResult GetAllCustomer(int ID)
        {
            List<VMCustomer> list = _userServices.GetAllCustomer(ID);
            var entityForTree = from r in list
                                select new DTOAuthItem()
                                {
                                    ID = r.CustomerID,
                                    Text = r.CustomerCode,
                                    Checked = r.CanView
                                };
            return new CustomJsonResult(new[] { new { id = 0, text = "所有客户", children = entityForTree } });
        }

        public ActionResult EditRole(int ID)
        {
            VMERPRoles role = _userServices.GetRoleByID(ID);
            if (role == null)
            {
                return RedirectToAction("PageNotFind", "Account");
            }
            return View(role);
        }

        /// <summary>
        /// ID为RoleID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult GetAllMenus(int ID)
        {
            List<DTOAdminUserMenus> menus = UserMenuServices.GetRoleAllowedMenuWithDetails(ID);
            var tmpForAll = (from m in menus
                             select m.DescendantsAndSelf()).ToList();

            List<DTOAdminUserMenus> entityForTree = new List<DTOAdminUserMenus>();
            foreach (var t in tmpForAll)
            {
                entityForTree.AddRange(t);
            }
            return new CustomJsonResult(new[] { new { id = 0, text = "所有页面", children = entityForTree } });
        }

        public ActionResult RolePagePriv(int roleID, int menuID)
        {
            int privilege = _userServices.GetPageElementsPrivilegesByRole(roleID, menuID);
            Dictionary<int, string> types = new Dictionary<int, string>();

            DTOAdminUserMenus menu = UserMenuServices.SYSTEM_ALL_MENUS.FirstOrDefault(p => p.MenuID == menuID);
            if (menu != null)
            {
                Type elementType = menu.PageElementEnumType;
                if (elementType != null)
                {
                    FieldInfo fi;
                    PageElementImpactOnPagesAttribute elementAttr = null;
                    DescriptionAttribute da = null;
                    ERPPage[] impactPages;
                    string elementName = string.Empty;
                    foreach (var et in Enum.GetValues(elementType))
                    {
                        fi = elementType.GetField((et.ToString()));
                        elementAttr = (PageElementImpactOnPagesAttribute)Attribute.GetCustomAttribute(fi, typeof(PageElementImpactOnPagesAttribute));
                        if (elementAttr != null)
                        {
                            impactPages = elementAttr.Pages;
                            if (impactPages.Contains((ERPPage)menu.MenuID))
                            {
                                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                                if (da != null)
                                {
                                    elementName = da.Description;
                                }
                                types.Add((int)et, elementName);
                            }
                        }
                    }
                }
                var entityForTree = from r in types
                                    select new DTOAuthItem()
                                    {
                                        ID = r.Key,
                                        Text = r.Value,
                                        Checked = (r.Key & privilege) > 0,
                                    };
                return new CustomJsonResult(new[] { new { id = 0, text = "所有权限", children = entityForTree } });
            }
            return new CustomJsonResult();
        }

        [HttpPost]
        public ActionResult SaveRolePageElementPriv(VMPrivEdit editEntity)
        {
            int privilege = 0;
            if (editEntity != null && editEntity.Items != null)
            {
                foreach (var item in editEntity.Items)
                {
                    if (item.Checked)
                    {
                        privilege += item.ID;
                    }
                }
            }

            DBOperationStatus result = _userServices.SavePageElementPrivileges(CurrentUserServices.Me, editEntity.RoleID, editEntity.MenuID, privilege);
            return new CustomJsonResult((short)result);
        }

        [HttpPost]
        public ActionResult SaveRolePageViewPriv(int roleID, List<int> pages)
        {
            DBOperationStatus result = _userServices.SavePageViewPrivileges(CurrentUserServices.Me, roleID, pages);
            return new CustomJsonResult((short)result);
        }

        [HttpPost]
        public ActionResult EditRole(int id, VMERPRoles roleInfo)
        {
            try
            {
                DBOperationStatus result = default(DBOperationStatus);
                if (id == 0)
                {
                    result = _userServices.CreateRole(CurrentUserServices.Me, roleInfo);
                }
                else
                {
                    result = _userServices.UpdateRole(CurrentUserServices.Me, roleInfo);
                }
                return new CustomJsonResult((short)result);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Hierarchy()
        {
            return View();
        }

        public ActionResult GetAllHierarchies()
        {
            int currentPage = 0, pageSize = 0, totalRows = 0;
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], out currentPage);
            int.TryParse(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], out pageSize);

            List<string> sortColumnsNames = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES]))
            {
                sortColumnsNames = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_NAMES].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            List<string> sortColumnOrders = new List<string>();
            if (!string.IsNullOrEmpty(HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS]))
            {
                sortColumnOrders = HttpContext.Request[EasyuiPagenationConsts.SORT_COLUMN_ORDERS].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            List<VMHierarchy> hierachies = _userServices.GetAllHierarchies(sortColumnsNames, sortColumnOrders, currentPage, pageSize, out totalRows);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = hierachies });
        }

        public static SelectList GetSelectionHierarchies(List<VMHierarchy> hierarchies)
        {
            List<SelectListItem> list = new List<SelectListItem>() { };
            foreach (var h in hierarchies)
            {
                list.Add(new SelectListItem() { Text = h.Name, Value = h.HierarchyID.ToString() });
            }
            SelectList generateList = new SelectList(list, "Value", "Text");
            return generateList;
        }

        public ActionResult EditUser(int ID)
        {
            List<VMHierarchy> hierarchies = _userServices.GetAllHierarchies(null);
            ViewData["Hierachies"] = GetSelectionHierarchies(hierarchies);

            VMERPUser user = _userServices.GetUserByID(ID);
            //这个是得到现有的模块名称
            Dictionary<string, string> dataPermissionModules = EnumHelper.GetCustomKeyEnums(typeof(DataPermissionModules));
            //将所有模块名称集合保存起来，传到页面上
            ViewData["DataName"] = dataPermissionModules;


            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(int ID, VMERPUser user)
        {
            try
            {
                DBOperationStatus result = default(DBOperationStatus);
                if (ID != 0)
                {
                    result = _userServices.UpdateUser(CurrentUserServices.Me.UserName, user);
                }
                return new CustomJsonResult((short)result);
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult SelectRPO()
        {
            //现在是需要得到所有的DataPermissions
            //将值传到前台页面
            List<DTODataPermissions> list = new List<DTODataPermissions>() { };
            Dictionary<short, KeyValuePair<string, string>> enumValues = EnumHelper.GetEnumKeyValuesWithDescription<short>(typeof(DataPermissions));
            foreach (var kvp in enumValues)
            {
                DTODataPermissions dto = new DTODataPermissions();
                dto.id = kvp.Key;
                dto.name = kvp.Value.Key;
                dto.text = kvp.Value.Value;
                list.Add(dto);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}