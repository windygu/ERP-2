using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Rep;
using ERP.Tools;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERP.BLL.ERP.Rep
{
    public class RepServices
    {
        #region UserMethod

        /// <summary>
        /// 获取列表数据
        /// </summary>
        public List<VMRep> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
          VMRepSearch vm_search)
        {
            List<VMRep> listModel = null;
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Customer_Rep.Where(p => !p.IsDelete);

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm_search.CompanyName))
                    {
                        query = query.Where(d => d.CompanyName.Contains(vm_search.CompanyName));
                    }
                    if (!string.IsNullOrEmpty(vm_search.ContactPerson))
                    {
                        query = query.Where(d => d.ContactPerson.Contains(vm_search.ContactPerson));
                    }
                    if (!string.IsNullOrEmpty(vm_search.Title))
                    {
                        query = query.Where(d => d.Title.Contains(vm_search.Title));
                    }
                    if (!string.IsNullOrEmpty(vm_search.CellPhone))
                    {
                        query = query.Where(d => d.CellPhone.Contains(vm_search.CellPhone));
                    }
                    if (!string.IsNullOrEmpty(vm_search.EmailAddress))
                    {
                        query = query.Where(d => d.EmailAddress.Contains(vm_search.EmailAddress));
                    }

                    #endregion 筛选条件

                    #region 排序

                    if (sortColumnsNames != null && sortColumnsNames.Count > 0
                        && sortColumnOrders != null && sortColumnOrders.Count > 0
                        && sortColumnsNames.Count == sortColumnOrders.Count)
                    {
                        query = query.OrderBy(sortColumnsNames, sortColumnOrders);
                    }
                    else
                    {
                        query = query.OrderByDescending(d => d.DT_MODIFYDATE);
                    }

                    #endregion 排序

                    totalRows = query.Count();//获取总条数

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        listModel = new List<VMRep>();
                        foreach (var item in dataFromDB)
                        {
                            listModel.Add(new VMRep()
                            {
                                RepID = item.RepID,
                                CompanyName = item.CompanyName,
                                ContactPerson = item.ContactPerson,
                                Title = item.Title,
                                CellPhone = item.CellPhone,
                                TelNumber = item.TelNumber,
                                CompanyAddress = item.CompanyAddress,
                                EmailAddress = item.EmailAddress,
                                DT_MODIFYDATEFormatter = Utils.DateTimeToStr2(item.DT_MODIFYDATE),
                            });
                        }
                    }

                    #endregion 给Model赋值
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return listModel;
        }

        /// <summary>
        /// 批量删除、删除
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="IDs"></param>
        /// <returns></returns>
        public DBOperationStatus Delete(VMERPUser currentUser, List<int> IDs)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Customer_Rep.Where(p => IDs.Contains(p.RepID));
                    if (query != null)
                    {
                        foreach (var item in query)
                        {
                            item.IsDelete = true;
                            item.DT_MODIFYDATE = DateTime.Now;
                            item.ST_MODIFYUSER = currentUser.UserID;
                            item.IPAddress = CommonCode.GetIP();
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
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return result;
        }

        /// <summary>
        /// 新建Rep
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Add(VMERPUser currentUser, VMRep vm)
        {
            var result = new VMAjaxProcessResult();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = new DAL.Customer_Rep
                    {
                        CompanyName = vm.CompanyName,
                        ContactPerson = vm.ContactPerson,
                        Title = vm.Title,
                        CellPhone = vm.CellPhone,
                        TelNumber = vm.TelNumber,
                        CompanyAddress = vm.CompanyAddress,
                        EmailAddress = vm.EmailAddress,

                        IsDelete = false,
                        DT_CREATEDATE = DateTime.Now,
                        ST_CREATEUSER = currentUser.UserID,
                        DT_MODIFYDATE = DateTime.Now,
                        ST_MODIFYUSER = currentUser.UserID,
                        IPAddress = CommonCode.GetIP(),
                    };
                    context.Customer_Rep.Add(query);

                    int affectRows = context.SaveChanges();
                    if (affectRows > 0)
                    {
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "出错了！";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Msg = "出错了！";
                LogHelper.WriteError(ex);
            }
            return result;
        }

        /// <summary>
        /// 保存Rep
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public VMAjaxProcessResult Save(VMERPUser currentUser, VMRep vm)
        {
            var result = new VMAjaxProcessResult();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Customer_Rep.Find(vm.RepID);
                    query.CompanyName = vm.CompanyName;
                    query.ContactPerson = vm.ContactPerson;
                    query.Title = vm.Title;
                    query.CellPhone = vm.CellPhone;
                    query.TelNumber = vm.TelNumber;
                    query.CompanyAddress = vm.CompanyAddress;
                    query.EmailAddress = vm.EmailAddress;

                    query.ST_MODIFYUSER = currentUser.UserID;
                    query.DT_MODIFYDATE = DateTime.Now;
                    query.IPAddress = CommonCode.GetIP();

                    int affectRows = context.SaveChanges();

                    if (affectRows > 0)
                    {
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Msg = "出错了！";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
                result.IsSuccess = false;
                result.Msg = "出错了！";
                LogHelper.WriteError(ex);
            }
            return result;
        }

        /// <summary>
        /// 获取Rep信息
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public VMRep GetDetailByID(VMERPUser currentUser, int id)
        {
            VMRep vm = null;
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Customer_Rep.Find(id);
                vm = new VMRep
                {
                    RepID = query.RepID,
                    CompanyName = query.CompanyName,
                    ContactPerson = query.ContactPerson,
                    Title = query.Title,
                    CellPhone = query.CellPhone,
                    TelNumber = query.TelNumber,
                    CompanyAddress = query.CompanyAddress,
                    EmailAddress = query.EmailAddress,
                    list_Commission = context.Customer_Commission.Where(d => d.RepID == query.RepID).Select(p => new VMCommission
                    {
                        OCID = p.OCID,
                        CustomerCode = p.Orders_Customers.CustomerCode,
                        RepID = p.RepID,
                        Commission = p.Commission,
                    }).ToList(),
                };
            }
            return vm;
        }

        #endregion UserMethod

        /// <summary>
        /// 获取Rep信息——客户信息用到了
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<VMRep> GetRepList()
        {
            List<VMRep> vm_list = new List<VMRep>();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Customer_Rep.Where(d => !d.IsDelete);
                foreach (var item in query)
                {
                    VMRep vm = new VMRep()
                    {
                        RepID = item.RepID,
                        CompanyName = item.CompanyName,
                        ContactPerson = item.ContactPerson,
                        Title = item.Title,
                        CellPhone = item.CellPhone,
                        TelNumber = item.TelNumber,
                        CompanyAddress = item.CompanyAddress,
                        EmailAddress = item.EmailAddress,
                    };
                    vm_list.Add(vm);
                }
            }
            return vm_list;
        }
    }
}