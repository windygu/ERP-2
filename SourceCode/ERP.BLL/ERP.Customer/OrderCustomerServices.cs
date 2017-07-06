using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Models.Customer;
using ERP.Models.Rep;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Customer
{
    public class OrderCustomerServices
    {
        /// <summary>
        /// 获取客户的主要信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<DTOOrderCustomers> GetAllCustomersKeyInfo(int userID)
        {
            List<DTOOrderCustomers> customerKeyinfo = new List<DTOOrderCustomers>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    customerKeyinfo = (from ur in context.Orders_Customers
                                       where !ur.IsDelete
                                       orderby ur.CustomerCode
                                       select new DTOOrderCustomers
                                       {
                                           OCID = ur.OCID,
                                           CustomerCode = ur.CustomerCode,
                                           CustomerName = ur.CustomerName
                                       }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return customerKeyinfo;
        }

        /// <summary>
        /// 根据指定客户自编号查询收货地址列表信息
        /// </summary>
        /// <param name="ocid">客户自编号</param>
        /// <returns></returns>
        public List<VMAcceptedInfo> GetCustomerAcceptedList(int ocid)
        {
            List<VMAcceptedInfo> vmAIlist = new List<VMAcceptedInfo>();

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dbCA = context.Orders_AcceptInformation.Where(p => p.OCID == ocid && !p.IsDelete);

                    if (dbCA != null)
                    {
                        VMAcceptedInfo vmAI = new VMAcceptedInfo();
                        foreach (var item1 in dbCA)
                        {
                            vmAI = new VMAcceptedInfo();
                            vmAI.AIID = item1.AIID;
                            vmAI.OCID = item1.OCID;
                            vmAI.AreaName = context.Reg_Area.Where(p => p.ARID == item1.Province).FirstOrDefault().AreaName;
                            vmAI.CountryName = context.Reg_Country.Where(p => p.COID == item1.Country).FirstOrDefault().CountryName;
                            vmAI.AcceptedDetail = item1.StreetAddress + "," + item1.City + "," + vmAI.AreaName + "," + vmAI.CountryName;

                            vmAI.CompanyName = item1.CompanyName;
                            vmAI.StreetAddress = item1.StreetAddress;
                            vmAI.City = item1.City;
                            vmAI.PostalCode = item1.PostalCode;

                            vmAI.FirstName = item1.FirstName;
                            vmAI.LastName = item1.LastName;

                            vmAIlist.Add(vmAI);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteError(e);
            }

            return vmAIlist;
        }

        public List<DTOOrderCustomers> GetCustomers(VMERPUser currentUser, int? countryID, int? provinceID, string customerCode, string customerName,
            List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows)
        {
            List<DTOOrderCustomers> customerInfo = new List<DTOOrderCustomers>();
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders_Customers.Where(p => p.IsDelete == false);

                    if (countryID.HasValue)
                    {
                        query = query.Where(p => p.Country == countryID.Value);
                    }
                    if (provinceID.HasValue)
                    {
                        query = query.Where(p => p.Province == provinceID.Value);
                    }
                    if (!string.IsNullOrEmpty(customerCode))
                    {
                        query = query.Where(p => p.CustomerCode.Contains(customerCode));
                    }
                    if (!string.IsNullOrEmpty(customerName))
                    {
                        query = query.Where(p => p.CustomerName.Contains(customerName));
                    }

                    //if (currentUser.GroupID != 0)
                    //{
                    //    query = query.Where(p => p.User.UserHierarchy.Hierarchy.GroupID == currentUser.GroupID && p.User.UserHierarchy.Hierarchy.GroupLevel >= currentUser.GroupLevel);
                    //}

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

                    totalRows = query.Count();
                    query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);
                    customerInfo = (from ur in query
                                    let defaultContact = ur.Orders_Contacts.Where(d => !d.IsDelete).FirstOrDefault(p => p.IsDefault)
                                    //let defaultAcceptAddress = ur.Orders_AcceptInformation.FirstOrDefault(p => p.IsDefault)
                                    select new DTOOrderCustomers
                                    {
                                        OCID = ur.OCID,
                                        CustomerCode = ur.CustomerCode,
                                        CustomerName = ur.CustomerName,
                                        ContactName = defaultContact == null ? string.Empty : defaultContact.FullName,
                                        Duty = defaultContact == null ? string.Empty : defaultContact.Duty,
                                        SeasonIDList = defaultContact.SeasonIDList,
                                        MobilePhone = defaultContact == null ? string.Empty : defaultContact.MobilePhone,
                                        Telephone = defaultContact == null ? string.Empty : defaultContact.TelPhone,
                                        Fax = defaultContact == null ? string.Empty : defaultContact.Fax,
                                        Email = defaultContact == null ? string.Empty : defaultContact.Email,
                                        QuoteTemplateFileName = ur.QuoteTemplateFileName,
                                        SelectCustomer = ur.SelectCustomer,
                                        CustomerAddress = ur.StreetAddress + "," + ur.City + "," + (ur.Province.HasValue ? ur.Reg_Area.AreaName : string.Empty) + "," + (ur.Country.HasValue ? ur.Reg_Country.CountryName : string.Empty),
                                        CreateDate = ur.CreateDate
                                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return customerInfo;
        }

        public DBOperationStatus Delete(VMERPUser currentUser, List<int> IDs, string ipAddress)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders_Customers.Where(p => IDs.Contains(p.OCID));
                    var dataFromDB = query.ToList();
                    if (dataFromDB != null)
                    {
                        foreach (var p in dataFromDB)
                        {
                            p.IsDelete = true;
                            p.LastModifyBy = currentUser.UserID;
                            p.LastModifyDate = DateTime.Now;
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

        public DTOOrderCustomers GetCustomerByCode(string customerCode)
        {
            return GetCustomer(null, customerCode);
        }

        public DTOOrderCustomers GetCustomerByID(VMERPUser currentUser, int customerID)
        {
            return GetCustomer(customerID, null);
        }

        /// <summary>
        /// 如果传入了ID，则用ID查询，如果没有则用name查询
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="customerCode"></param>
        /// <returns></returns>
        private DTOOrderCustomers GetCustomer(int? customerID, string customerCode)
        {
            DTOOrderCustomers customer = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Orders_Customers.Where(p => true);

                    if (customerID.HasValue)
                    {
                        query = query.Where(p => p.OCID == customerID && p.IsDelete == false);
                    }
                    else
                    {
                        query = query.Where(p => p.CustomerCode == customerCode && p.IsDelete == false);
                    }

                    var dbEntity = query.FirstOrDefault();
                    if (dbEntity != null)
                    {
                        customer = new DTOOrderCustomers()
                        {
                            OCID = dbEntity.OCID,
                            CustomerCode = dbEntity.CustomerCode,
                            CustomerName = dbEntity.CustomerName,
                            StreetAddress = dbEntity.StreetAddress,
                            City = dbEntity.City,
                            Province = dbEntity.Province,
                            Country = dbEntity.Country,
                            PostalCode = dbEntity.PostalCode,
                            MiscImportLoadAmount = dbEntity.MiscImportLoadAmount,
                            Commission = dbEntity.Commission,
                            Agent = dbEntity.Agent,
                            Allowance = dbEntity.Allowance,
                            CtnsPallet = dbEntity.CtnsPallet,
                            MU = dbEntity.MU,
                            FOBNET = dbEntity.FOBNET,
                            FinalFOB = dbEntity.FinalFOB,
                            PcsPallet = dbEntity.PcsPallet,
                            Palletpc = dbEntity.Palletpc,
                            QuoteTemplateFileName = dbEntity.QuoteTemplateFileName,
                            SelectCustomer = dbEntity.SelectCustomer,
                            Code = dbEntity.Code,
                            ELCFill = dbEntity.ELCFill,
                            PaymentType = dbEntity.PaymentType,
                            AcceptInformations = dbEntity.Orders_AcceptInformation.Where(d => !d.IsDelete).Select(p => new VMAcceptInformation
                            {
                                IsDefault = p.IsDefault ? "Yes" : "No",
                                AddressType = p.AddressType,
                                AIID = p.AIID,
                                City = p.City,
                                CompanyName = p.CompanyName,
                                Country = p.Country,
                                CreateDate = p.CreateDate,
                                FirstName = p.FirstName,
                                LastName = p.LastName,
                                MobilePhone = p.MobilePhone,
                                OCID = p.OCID,
                                PostalCode = p.PostalCode,
                                Province = p.Province,
                                Region = p.Region,
                                StreetAddress = p.StreetAddress,
                                TelPhone = p.TelPhone,
                                Comment = p.Comment,
                            }).ToList(),
                            Contacts = dbEntity.Orders_Contacts.Where(d => !d.IsDelete).Select(p => new VMContact
                            {
                                IsDefault = p.IsDefault ? "Yes" : "No",
                                CreateDate = p.CreateDate,
                                Duty = p.Duty,
                                SeasonIDList = p.SeasonIDList,
                                Email = p.Email,
                                Fax = p.Fax,
                                FirstName = p.FirstName,
                                LastName = p.LastName,
                                FullName = p.FullName,
                                MobilePhone = p.MobilePhone,
                                OCID = p.OCID,
                                OLID = p.OLID,
                                TelPhone = p.TelPhone,
                            }).ToList(),
                            VMFreight = dbEntity.Orders_FreightRate.Select(p => new VMFreightRate
                            {
                                OCID = p.OCID,
                                PortID = p.PortID,
                                FreightRate = p.FreightRate,
                            }).ToList(),
                            RepDataList = dbEntity.Customer_Commission.Select(p => new VMCommission
                            {
                                OCID = p.OCID,
                                RepID = p.RepID,
                                Commission = p.Commission,
                            }).ToList()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return customer;
        }

        //添加
        public VMAjaxProcessResult Create(int userID, string ipAddress, DTOOrderCustomers customerInfo)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            if (customerInfo == null)
            {
                result.IsSuccess = false;
                return result;
            }

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    #region 判断输入的基本信息里面的地址 要与选择客户的地址一样

                    var query_temp = context.Orders_Customers.Where(d => d.SelectCustomer == customerInfo.SelectCustomer && !d.IsDelete);
                    if (query_temp != null && query_temp.Count() > 0)
                    {
                        var query_temp_first = query_temp.OrderBy(d => d.CreateDate).First();
                        if (query_temp_first.CustomerCode != customerInfo.CustomerCode && customerInfo.SelectCustomer != "新客户")
                        {
                            if (query_temp_first.Country != customerInfo.Country)
                            {
                                result.IsSuccess = false;
                                result.Msg = "Country要与 选择的客户" + query_temp_first.CustomerCode + "的Country一样！";
                                return result;
                            }

                            if (query_temp_first.Province != customerInfo.Province)
                            {
                                result.IsSuccess = false;
                                result.Msg = "State/Province/Region要与 选择的客户" + query_temp_first.CustomerCode + "的State/Province/Region一样！";
                                return result;
                            }

                            if (query_temp_first.City != customerInfo.City)
                            {
                                result.IsSuccess = false;
                                result.Msg = "City or APO/AFO要与 选择的客户" + query_temp_first.CustomerCode + "的City or APO/AFO一样！";
                                return result;
                            }

                            if (query_temp_first.StreetAddress != customerInfo.StreetAddress)
                            {
                                result.IsSuccess = false;
                                result.Msg = "Street Address要与 选择的客户" + query_temp_first.CustomerCode + "的Street Address一样！";
                                return result;
                            }
                            if (query_temp_first.PostalCode != customerInfo.PostalCode)
                            {
                                result.IsSuccess = false;
                                result.Msg = "Zip/Postal Code要与 选择的客户" + query_temp_first.CustomerCode + "的Zip/Postal Code一样！";
                                return result;
                            }
                            if (query_temp_first.PaymentType != customerInfo.PaymentType)
                            {
                                result.IsSuccess = false;
                                result.Msg = "收款方式要与 选择的客户" + query_temp_first.CustomerCode + "的收款方式一样！";
                                return result;
                            }
                        }
                    }

                    #endregion 判断输入的基本信息里面的地址 要与选择客户的地址一样

                    Orders_Customers customerToAdd = new Orders_Customers();
                    customerToAdd = GetCustomerForDB(context, customerToAdd, customerInfo);
                    customerToAdd.CreateBy = userID;
                    customerToAdd.CreateDate = DateTime.Now;

                    context.Orders_Customers.Add(customerToAdd);
                    int affectRows = context.SaveChanges();
                    if (affectRows == 0)
                    {
                        result.IsSuccess = false;
                    }
                    else
                    {
                        result.IsSuccess = true;

                        // 更新地址信息
                        if (customerToAdd.Reg_Area != null && customerToAdd.Reg_Country != null)
                        {
                            customerToAdd.FullAddress = customerToAdd.StreetAddress + "," + customerToAdd.City + "," + customerToAdd.Reg_Area.AreaName + "," + customerToAdd.Reg_Country.CountryName;
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;

                LogHelper.WriteError(ex);
            }

            return result;
        }

        //添加
        private static Orders_Customers GetCustomerForDB(ERPEntitiesNew context, Orders_Customers customerToAdd, DTOOrderCustomers customerInfo)
        {
            customerToAdd.CustomerCode = customerInfo.CustomerCode;
            customerToAdd.CustomerName = customerInfo.CustomerName;
            customerToAdd.StreetAddress = customerInfo.StreetAddress;
            customerToAdd.City = customerInfo.City;
            customerToAdd.Province = customerInfo.Province;
            customerToAdd.Country = customerInfo.Country;
            customerToAdd.MiscImportLoadAmount = customerInfo.MiscImportLoadAmount ?? 0;
            customerToAdd.Commission = customerInfo.Commission ?? 0;
            customerToAdd.Agent = customerInfo.Agent ?? 0;
            customerToAdd.Allowance = customerInfo.Allowance ?? 0;
            customerToAdd.MU = customerInfo.MU ?? 0;
            customerToAdd.FOBNET = customerInfo.FOBNET ?? 0;
            customerToAdd.FinalFOB = customerInfo.FinalFOB ?? 0;
            customerToAdd.CtnsPallet = customerInfo.CtnsPallet ?? 0;
            customerToAdd.PcsPallet = customerInfo.PcsPallet ?? 0;
            customerToAdd.Palletpc = customerInfo.Palletpc ?? 0;
            customerToAdd.PostalCode = customerInfo.PostalCode;
            customerToAdd.QuoteTemplateFileName = customerInfo.QuoteTemplateFileName;
            customerToAdd.SelectCustomer = customerInfo.SelectCustomer;
            customerToAdd.Code = customerInfo.Code;
            customerToAdd.ELCFill = customerInfo.ELCFill ?? 0;
            customerToAdd.PaymentType = customerInfo.PaymentType;

            if (customerInfo.Contacts != null && customerInfo.Contacts.Count > 0)
            {
                customerToAdd.Orders_Contacts = new List<Orders_Contacts>();
                foreach (var con in customerInfo.Contacts)
                {
                    var contacts = context.Orders_Contacts.Find(con.OLID);
                    if (con.OLID == -1)
                    {
                        contacts = new Orders_Contacts();
                    }
                    contacts.IsDefault = con.IsDefault == "Yes" ? true : false;
                    contacts.FirstName = con.FirstName;
                    contacts.LastName = con.LastName;
                    contacts.Duty = con.Duty;
                    contacts.SeasonIDList = con.SeasonIDList;
                    contacts.TelPhone = con.TelPhone;
                    contacts.MobilePhone = con.MobilePhone;
                    contacts.Fax = con.Fax;
                    contacts.Email = con.Email;
                    contacts.CreateDate = DateTime.Now;

                    customerToAdd.Orders_Contacts.Add(contacts);
                }
            }

            if (customerInfo.AcceptInformations != null && customerInfo.AcceptInformations.Count > 0)
            {
                customerToAdd.Orders_AcceptInformation = new List<Orders_AcceptInformation>();
                foreach (var item in customerInfo.AcceptInformations)
                {
                    var acceptInfo = context.Orders_AcceptInformation.Find(item.AIID);
                    if (item.AIID == -1)
                    {
                        acceptInfo = new Orders_AcceptInformation();
                    }

                    acceptInfo.IsDefault = item.IsDefault == "Yes" ? true : false;
                    acceptInfo.FirstName = item.FirstName;
                    acceptInfo.LastName = item.LastName;
                    acceptInfo.CompanyName = item.CompanyName;
                    acceptInfo.TelPhone = item.TelPhone;
                    acceptInfo.MobilePhone = item.MobilePhone;
                    acceptInfo.AddressType = item.AddressType;
                    acceptInfo.StreetAddress = item.StreetAddress;
                    acceptInfo.City = item.City;
                    acceptInfo.Country = item.Country;
                    acceptInfo.Province = item.Province;
                    acceptInfo.PostalCode = item.PostalCode;
                    acceptInfo.CreateDate = DateTime.Now;
                    acceptInfo.Comment = item.Comment;

                    if (item.AIID == -1)
                    {
                        customerToAdd.Orders_AcceptInformation.Add(acceptInfo);
                    }
                }
            }

            if (customerInfo.VMFreight != null && customerInfo.VMFreight.Count > 0)
            {
                customerToAdd.Orders_FreightRate = new List<Orders_FreightRate>();
                foreach (var con in customerInfo.VMFreight)
                {
                    Orders_FreightRate freight = new Orders_FreightRate()
                    {
                        PortID = con.PortID,
                        FreightRate = (decimal)con.FreightRate,
                        IsDelete = false,
                        Type = con.PortID == (int)CustomerFreightRateTypeEnum.AllPort ? (int)CustomerFreightRateTypeEnum.AllPort : (int)CustomerFreightRateTypeEnum.SinglePort,
                    };
                    customerToAdd.Orders_FreightRate.Add(freight);
                }
            }

            if (customerInfo.RepDataList != null && customerInfo.RepDataList.Count > 0)
            {
                customerToAdd.Customer_Commission = new List<DAL.Customer_Commission>();
                foreach (var item in customerInfo.RepDataList)
                {
                    DAL.Customer_Commission repList = new DAL.Customer_Commission()
                    {
                        OCID = item.OCID,
                        Commission = item.Commission,
                        RepID = item.RepID,
                        IsDelete = false,
                    };
                    customerToAdd.Customer_Commission.Add(repList);
                }
            }
            return customerToAdd;
        }

        public VMAjaxProcessResult Update(VMERPUser currentUser, string ipAddress, DTOOrderCustomers customerInfo)
        {
            VMAjaxProcessResult result = new VMAjaxProcessResult();
            if (customerInfo == null)
            {
                result.IsSuccess = false;
                return result;
            }

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    #region 判断输入的基本信息里面的地址 要与选择客户的地址一样

                    var query_temp = context.Orders_Customers.Where(d => d.SelectCustomer == customerInfo.SelectCustomer && !d.IsDelete);
                    if (query_temp != null && query_temp.Count() > 0)
                    {
                        var query_temp_first = query_temp.OrderBy(d => d.CreateDate).First();
                        if (query_temp_first.CustomerCode != customerInfo.CustomerCode && customerInfo.SelectCustomer != "新客户")
                        {
                            if (query_temp_first.Country != customerInfo.Country)
                            {
                                result.IsSuccess = false;
                                result.Msg = "Country要与 选择的客户" + query_temp_first.CustomerCode + "的Country一样！";
                                return result;
                            }

                            if (query_temp_first.Province != customerInfo.Province)
                            {
                                result.IsSuccess = false;
                                result.Msg = "State/Province/Region要与 选择的客户" + query_temp_first.CustomerCode + "的State/Province/Region一样！";
                                return result;
                            }

                            if (query_temp_first.City != customerInfo.City)
                            {
                                result.IsSuccess = false;
                                result.Msg = "City or APO/AFO要与 选择的客户" + query_temp_first.CustomerCode + "的City or APO/AFO一样！";
                                return result;
                            }

                            if (query_temp_first.StreetAddress != customerInfo.StreetAddress)
                            {
                                result.IsSuccess = false;
                                result.Msg = "Street Address要与 选择的客户" + query_temp_first.CustomerCode + "的Street Address一样！";
                                return result;
                            }
                            if (query_temp_first.PostalCode != customerInfo.PostalCode)
                            {
                                result.IsSuccess = false;
                                result.Msg = "Zip/Postal Code要与 选择的客户" + query_temp_first.CustomerCode + "的Zip/Postal Code一样！";
                                return result;
                            }
                            if (query_temp_first.PaymentType != customerInfo.PaymentType)
                            {
                                result.IsSuccess = false;
                                result.Msg = "收款方式要与 选择的客户" + query_temp_first.CustomerCode + "的收款方式一样！";
                                return result;
                            }
                        }
                    }

                    #endregion 判断输入的基本信息里面的地址 要与选择客户的地址一样

                    var query = context.Orders_Customers.Where(p => p.OCID == customerInfo.OCID);
                    //if (currentUser.GroupID != 0)
                    //{
                    //    query = query.Where(p => p.User.UserHierarchy.Hierarchy.GroupID == currentUser.GroupID && p.User.UserHierarchy.Hierarchy.GroupLevel >= currentUser.GroupLevel);
                    //}

                    var list_AIID = context.Orders_AcceptInformation.Where(p => p.OCID == customerInfo.OCID && !p.IsDelete).Select(d => d.AIID).ToList();
                    var list_OLID = context.Orders_Contacts.Where(p => p.OCID == customerInfo.OCID && !p.IsDelete).Select(d => d.OLID).ToList();

                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        int OCID = dataFromDB.OCID;

                        var query_Orders_FreightRate = context.Orders_FreightRate.Where(p => p.OCID == OCID);
                        context.Orders_FreightRate.RemoveRange(query_Orders_FreightRate);

                        var query_Customer_Commission = context.Customer_Commission.Where(p => p.OCID == OCID);
                        context.Customer_Commission.RemoveRange(query_Customer_Commission);

                        dataFromDB = GetCustomerForDB(context, dataFromDB, customerInfo);
                        dataFromDB.LastModifyBy = currentUser.UserID;
                        dataFromDB.LastModifyDate = DateTime.Now;

                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result.IsSuccess = false;
                        }
                        else
                        {
                            result.IsSuccess = true;

                            // 更新地址信息
                            if (dataFromDB.Reg_Area != null && dataFromDB.Reg_Country != null)
                            {
                                dataFromDB.FullAddress = dataFromDB.StreetAddress + "," + dataFromDB.City + "," + dataFromDB.Reg_Area.AreaName + "," + dataFromDB.Reg_Country.CountryName;
                            }


                            var list_AcceptInformations_Delete = list_AIID.Except(customerInfo.AcceptInformations.Select(d => d.AIID)).ToList();
                            var qeruy_Orders_AcceptInformation = context.Orders_AcceptInformation.Where(d => list_AcceptInformations_Delete.Contains(d.AIID) && !d.IsDelete);
                            foreach (var item in qeruy_Orders_AcceptInformation)
                            {
                                item.IsDelete = true;
                            }

                            var list_Contacts_Delete = list_OLID.Except(customerInfo.Contacts.Select(d => d.OLID)).ToList();
                            var qeruy_Orders_Contacts = context.Orders_Contacts.Where(d => list_Contacts_Delete.Contains(d.OLID) && !d.IsDelete);
                            foreach (var item in qeruy_Orders_Contacts)
                            {
                                item.IsDelete = true;
                            }
                            context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;

                LogHelper.WriteError(ex);
            }

            return result;
        }
    }
}