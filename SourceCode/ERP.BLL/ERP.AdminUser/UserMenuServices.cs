using ERP.BLL.Consts;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.CustomAttribute;
using ERP.Models.CustomEnums;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.AdminUser
{
    public class UserMenuServices
    {
        static List<DTOAdminUserMenus> _system_all_menus;
        public static List<DTOAdminUserMenus> SYSTEM_ALL_MENUS
        {
            get
            {
                return _system_all_menus;
            }
        }

        static UserMenuServices()
        {
            if (_system_all_menus == null)
            {
                _system_all_menus = new List<DTOAdminUserMenus>();
            }

            DescriptionAttribute da = null;
            PageMenuAttribute menuInfo = null;
            FieldInfo fi;
            Type enumType = typeof(ERPPage);
            string description = string.Empty;
            string iconClass = string.Empty;
            string url = string.Empty;
            string winType = string.Empty;
            string winSize = string.Empty;
            int? parentID = null;

            var types = Enum.GetValues(enumType);
            foreach (var type in types)
            {
                description = string.Empty;
                iconClass = string.Empty;

                fi = enumType.GetField((type.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if (da != null)
                {
                    description = da.Description;
                }
                menuInfo = (PageMenuAttribute)Attribute.GetCustomAttribute(fi, typeof(PageMenuAttribute));
                if (menuInfo != null)
                {
                    iconClass = menuInfo.IconClass;
                    if (menuInfo.ParentID != 0)
                    {
                        parentID = menuInfo.ParentID;
                    }
                    url = menuInfo.URL;
                    winType = menuInfo.WinType;
                    winSize = menuInfo.WinSize;
                }

                _system_all_menus.Add(new DTOAdminUserMenus() { MenuID = (int)type, ParentMenuID = parentID, Name = da.Description, PageURL = url, WinType = winType, WinSize = winSize, Icons = iconClass, PageElementEnumType = menuInfo.PageElementEnumType });
            }
        }

        /// 获取所有的菜单，如果用户拥有菜单权限，则设置CanView
        public static List<DTOAdminUserMenus> GetRoleAllowedMenuWithDetails(int roleID)
        {
            List<DTOAdminUserMenus> result = new List<DTOAdminUserMenus>();
            try
            {
                var dbResult = _system_all_menus.Where(p => p.ParentMenuID == null).ToList();
                UserServices userAuthServices = new UserServices();
                List<int> userAllowedPages = userAuthServices.GetRoleAllowedPages(roleID);

                if (userAllowedPages == null)
                {
                    userAllowedPages = new List<int>();
                }

                if (dbResult != null && dbResult.Count > 0)
                {
                    foreach (var r in dbResult)
                    {
                        int recursiveMaxDepth = AdminConsts.RECURSIVE_MAX_DEPTH;

                        var menu = GetMenus(r, userAllowedPages, ref recursiveMaxDepth, true);
                        // 子节点才让CanView为True，否则easyui-tree的根节点将被全部选中
                        menu.CanView = !_system_all_menus.Exists(p => p.ParentMenuID == r.MenuID) && userAllowedPages.Contains(r.MenuID);
                        result.Add(menu);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return result;
        }

        public static List<DTOAdminUserMenus> GetUserAllowedMenu(VMERPUser user)
        {
            List<DTOAdminUserMenus> result = new List<DTOAdminUserMenus>();
            try
            {
                var dbResult = _system_all_menus.Where(p => p.ParentMenuID == null).ToList();
                UserServices userAuthServices = new UserServices();
                List<int> userAllowedPages = userAuthServices.GetUserAllowedPages(user);

                if (dbResult != null && dbResult.Count > 0 && userAllowedPages != null && userAllowedPages.Count > 0)
                {
                    foreach (var r in dbResult)
                    {
                        int recursiveMaxDepth = AdminConsts.RECURSIVE_MAX_DEPTH;

                        if (userAllowedPages.Contains(r.MenuID))
                        {
                            result.Add(GetMenus(r, userAllowedPages, ref recursiveMaxDepth));
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

        private static DTOAdminUserMenus GetMenus(DTOAdminUserMenus currentMenu, List<int> userAllowedPages, ref int recursiveDepth, bool alwaysInclude = false)
        {
            if (recursiveDepth < 0)
            {
                throw new Exception("递归次数超过了最大限制：" + AdminConsts.RECURSIVE_MAX_DEPTH);
            }
            DTOAdminUserMenus menu = new DTOAdminUserMenus();
            menu.MenuID = currentMenu.MenuID;
            menu.ParentMenuID = currentMenu.ParentMenuID;
            menu.Name = currentMenu.Name;
            menu.PageURL = currentMenu.PageURL;
            menu.WinSize = currentMenu.WinSize;
            menu.WinType = currentMenu.WinType;
            menu.Icons = currentMenu.Icons;
            menu.SubMenus = new List<DTOAdminUserMenus>();

            var subMenus = _system_all_menus.Where(p => p.ParentMenuID == currentMenu.MenuID).ToList();
            if (subMenus != null && subMenus.Count > 0)
            {
                foreach (var subMenu in subMenus)
                {
                    if (alwaysInclude)
                    {
                        var tmpMenu = GetMenus(subMenu, userAllowedPages, ref recursiveDepth, alwaysInclude);
                        // 子节点才让CanView为True，否则easyui-tree的根节点将被全部选中
                        tmpMenu.CanView = !_system_all_menus.Exists(p => p.ParentMenuID == subMenu.MenuID) && userAllowedPages.Contains(subMenu.MenuID);
                        menu.SubMenus.Add(tmpMenu);
                    }
                    else
                    {
                        if (userAllowedPages.Contains(subMenu.MenuID))
                        {
                            menu.SubMenus.Add(GetMenus(subMenu, userAllowedPages, ref recursiveDepth, alwaysInclude));
                        }
                    }
                }
            }

            recursiveDepth--;

            return menu;
        }
    }
}