using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.CustomEnums;
using ERP.Models.DataDictionary;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.DataDictionary
{
    public class DataDictionaryServices
    {
        public static Dictionary<int, string> GetTabkindContractStatus()
        {
            Dictionary<int, string> di = EnumHelper.GetCustomEnums<int>(typeof(DictionaryTableKind));
            return di;
        }

        /// <summary>
        /// 枚举的值
        /// </summary>
        /// <param name="i">编号</param>
        /// <returns></returns>
        public static string GetOutContract(int id)
        {
            Dictionary<int, string> dictionary = GetTabkindContractStatus();
            string name = "";
            foreach (var item in dictionary)
            {
                if (item.Key == id)
                {
                    name = item.Value;
                }
            }
            return name;
        }

        public List<DTODataDictionary> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
            VMDataDictionary vm)
        {
            List<DTODataDictionary> listModel = new List<DTODataDictionary>();
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    //IsDelete为0是未删除，1是已删除
                    var query = context.Com_DataDictionary.Where(n => n.IsDelete == 0);

                    #region
                    if (!string.IsNullOrEmpty(vm.AttrName))
                    {
                        int a = int.Parse(vm.AttrName);
                        if (vm.AttrName != "")
                        {
                            query = query.Where(n => n.TableKind == a);
                        }
                    }
                    if (!string.IsNullOrEmpty(vm.Name))
                    {
                        query = query.Where(n => n.Name.Contains(vm.Name));
                    }
                    #endregion 查找全部信息

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

                    query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);

                    #region 给Model赋值

                    listModel = (from entity in query
                                 select new DTODataDictionary
                                 {
                                     ID = entity.ID,
                                     Name = entity.Name,
                                     Code = entity.Code,
                                     AttrName = entity.AttrName,
                                     TableKind = entity.TableKind,
                                     Alias = entity.Alias,
                                 }).ToList();
                    #endregion 给Model赋值
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return listModel;
        }


        public DTODataDictionary GetDetailByID(int id)
        {
            DTODataDictionary vm = new DTODataDictionary();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Com_DataDictionary.Find(id);
                if (query != null)
                {
                    vm.ID = id;
                    vm.AttrName = query.AttrName;
                    vm.TableKind = query.TableKind;
                    vm.Name = query.Name;
                    vm.Code = query.Code;
                    vm.Alias = query.Alias;
                    vm.DataFlag = query.DataFlag;

                    vm.Country = query.Country;
                    vm.Province = query.Province;
                    vm.City = query.City;
                    vm.StreetAddress = query.StreetAddress;
                    vm.CompanyName = query.CompanyName;
                    vm.ZipCode = query.ZipCode;

                    vm.DepartmentNumber = query.DepartmentNumber;
                    vm.SeasonZhName = query.SeasonZhName;
                    vm.SelectCustomer = query.SelectCustomer;
                }


            }
            return vm;
        }

        public DBOperationStatus Save(DTODataDictionary vm, VMERPUser currentUser)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    if (vm.ID == -1)
                    {
                        var queryNum = context.Com_DataDictionary.Where(n => true);
                        //如果数据库有信息
                        if (queryNum.Count() > 0)
                        {
                            if (vm.TableKind != (int)DictionaryTableKind.Season)
                            {
                                var qu = context.Com_DataDictionary.Where(n => n.Name == vm.Name && n.TableKind == vm.TableKind && n.IsDelete == 0);
                                if (qu.Count() > 0)
                                {
                                    result = DBOperationStatus.Failed;
                                    return result;
                                }
                            }
                        }

                        //查找最大的code
                        var code = context.Com_DataDictionary.Max(n => n.Code);

                        DAL.Com_DataDictionary query = new Com_DataDictionary();
                        query.Name = vm.Name;
                        query.Alias = vm.Alias;
                        query.DataFlag = vm.DataFlag;

                        query.TableKind = vm.TableKind;
                        query.AttrName = GetOutContract(vm.TableKind);

                        query.Code = code + 1;//数据库有信息时，code从数据库已有的最大值加一

                        query.Country = vm.Country;
                        query.Province = vm.Province;
                        query.City = vm.City;
                        query.StreetAddress = vm.StreetAddress;
                        query.CompanyName = vm.CompanyName;
                        query.ZipCode = vm.ZipCode;

                        query.DepartmentNumber = vm.DepartmentNumber;
                        query.SeasonZhName = vm.SeasonZhName;
                        query.SelectCustomer = vm.SelectCustomer;

                        query.IPAdress = CommonCode.GetIP();
                        query.DT_CREATEDATE = DateTime.Now;
                        query.ST_CREATEUSER = currentUser.UserID;
                        query.DT_MODIFYDATE = DateTime.Now;
                        query.IPAdress = CommonCode.GetIP();
                        query.IsDelete = 0;

                        context.Com_DataDictionary.Add(query);
                        int a = context.SaveChanges();
                        if (a > 0)
                        {
                            result = DBOperationStatus.Success;
                        }
                        else
                        {
                            result = DBOperationStatus.Failed;
                        }

                    }
                    else
                    {
                        #region 编辑

                        var query = context.Com_DataDictionary.Find(vm.ID);
                        if (query != null)
                        {
                            int TableKind = query.TableKind;

                            var query2 = context.Com_DataDictionary.Where(n => n.Name == vm.Name && n.TableKind == TableKind && n.IsDelete == 0);
                            if (query2.Count() > 0)
                            {
                                var query3 = query2.Where(n => n.ID != vm.ID);
                                if (query3.Count() > 0 && TableKind != (int)DictionaryTableKind.Season)
                                {
                                    result = DBOperationStatus.Failed;
                                    return result;
                                }
                            }

                            if (TableKind == (int)DictionaryTableKind.Packing)
                            {
                                query.DataFlag = vm.DataFlag;
                            }

                            query.Name = vm.Name;
                            query.Alias = vm.Alias;

                            query.Country = vm.Country;
                            query.Province = vm.Province;
                            query.City = vm.City;
                            query.StreetAddress = vm.StreetAddress;
                            query.CompanyName = vm.CompanyName;
                            query.ZipCode = vm.ZipCode;

                            query.DepartmentNumber = vm.DepartmentNumber;
                            query.SeasonZhName = vm.SeasonZhName;
                            query.SelectCustomer = vm.SelectCustomer;

                            query.DT_MODIFYDATE = DateTime.Now;
                            query.ST_MODIFYUSER = currentUser.UserID;
                            query.IPAdress = CommonCode.GetIP();

                            int length = context.SaveChanges();
                            if (length > 0)
                            {
                                result = DBOperationStatus.Success;
                            }
                            else
                            {
                                result = DBOperationStatus.Failed;
                            }
                        }
                        #endregion
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }

            return result;
        }

        //#region  添加类型名称

        //public DBOperationStatus AddAttrName(DTODataDictionary DTO, int user)
        //{
        //    DBOperationStatus result = default(DBOperationStatus);
        //    using (ERPEntitiesNew context = new ERPEntitiesNew())
        //    {
        //        var query = context.Com_DataDictionary.Where(n => n.AttrName == DTO.AttrName);
        //        if (query.Count() > 0)
        //        {
        //            result = DBOperationStatus.Failed;
        //        }
        //        else
        //        {
        //            DAL.Com_DataDictionary data = new Com_DataDictionary();
        //            //判断是否有信息，如果有信息的话，得到最大的code与最大的tabkind，如果没有信息，code与tabkind分别从1，100开始
        //            //判断是否有信息
        //            var CountRows = context.Com_DataDictionary.Where(n => true);
        //            if (CountRows.Count() > 0)
        //            {
        //                var Tab = context.Com_DataDictionary.Max(n => n.TableKind);
        //                var Co = context.Com_DataDictionary.Max(n => n.Code);
        //                data.AttrName = DTO.AttrName;
        //                data.TableKind = Tab + 1;
        //                data.Name = DTO.Name;
        //                data.Code = Co + 1;
        //                data.Alias = DTO.Alias;
        //                data.DT_CREATEDATE = DateTime.Now;
        //                data.ST_CREATEUSER = user;
        //                data.DT_MODIFYDATE = DateTime.Now;
        //                data.IPAdress = CommonCode.GetIP();
        //                data.IsDelete = 0;

        //                data.Country = DTO.Country;
        //                data.Province = DTO.Province;
        //                data.City = DTO.City;
        //                data.StreetAddress = DTO.StreetAddress;

        //                context.Com_DataDictionary.Add(data);
        //                int length = context.SaveChanges();
        //                if (length == 0)
        //                {
        //                    result = DBOperationStatus.Failed;
        //                }
        //                else
        //                {
        //                    result = DBOperationStatus.Success;
        //                }
        //            }
        //            else
        //            {
        //                data.AttrName = DTO.AttrName;
        //                data.TableKind = 100;
        //                data.Name = DTO.Name;
        //                data.Code = 1;
        //                data.Alias = DTO.Alias;
        //                data.DT_CREATEDATE = DateTime.Now;
        //                data.ST_CREATEUSER = user;
        //                data.DT_MODIFYDATE = DateTime.Now;
        //                data.IPAdress = CommonCode.GetIP();
        //                data.IsDelete = 0;

        //                data.Country = DTO.Country;
        //                data.Province = DTO.Province;
        //                data.City = DTO.City;
        //                data.StreetAddress = DTO.StreetAddress;

        //                context.Com_DataDictionary.Add(data);
        //                int length = context.SaveChanges();
        //                if (length == 0)
        //                {
        //                    result = DBOperationStatus.Failed;
        //                }
        //                else
        //                {
        //                    result = DBOperationStatus.Success;
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        //#endregion



        //public DBOperationStatus UpdateAttrName(DTODataDictionary dto, int user)
        //{
        //    DBOperationStatus result = default(DBOperationStatus);
        //    using (ERPEntitiesNew context = new ERPEntitiesNew())
        //    {
        //        try
        //        {
        //            var quer = context.Com_DataDictionary.Where(n => n.AttrName == dto.AttrName);
        //            if (quer.Count() > 0)
        //            {
        //                var attrname = context.Com_DataDictionary.Where(n => n.AttrName == dto.AttrName).FirstOrDefault().TableKind;
        //                if (attrname == dto.TableKind)
        //                {
        //                    var queryy = context.Com_DataDictionary.Where(n => n.TableKind == dto.TableKind);
        //                    List<int> listt = new List<int>();
        //                    foreach (var item in queryy)
        //                    {
        //                        listt.Add(item.ID);
        //                    }
        //                    foreach (var item in listt)
        //                    {
        //                        var qq = context.Com_DataDictionary.Find(item);
        //                        qq.AttrName = dto.AttrName;
        //                        qq.ST_MODIFYUSER = user;
        //                        qq.DT_MODIFYDATE = DateTime.Now;
        //                        qq.IPAdress = CommonCode.GetIP();
        //                    }
        //                    int lenght = context.SaveChanges();
        //                    if (lenght > 0)
        //                    {
        //                        result = DBOperationStatus.Success;
        //                    }
        //                    else
        //                    {
        //                        result = DBOperationStatus.Failed;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var query = context.Com_DataDictionary.Where(n => n.TableKind == dto.TableKind);
        //                List<int> list = new List<int>();
        //                foreach (var item in query)
        //                {
        //                    list.Add(item.ID);
        //                }
        //                foreach (var item in list)
        //                {
        //                    var q = context.Com_DataDictionary.Find(item);
        //                    q.AttrName = dto.AttrName;
        //                    int lenght = context.SaveChanges();
        //                    q.ST_MODIFYUSER = user;
        //                    q.DT_MODIFYDATE = DateTime.Now;
        //                    q.IPAdress = CommonCode.GetIP();
        //                }
        //                int lenghtt = context.SaveChanges();
        //                if (lenghtt > 0)
        //                {
        //                    result = DBOperationStatus.Success;
        //                }
        //                else
        //                {
        //                    result = DBOperationStatus.Failed;
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            LogHelper.WriteError(e);
        //        }
        //    }

        //    return result;
        //}


        ///// <summary>
        ///// 添加数据信息
        ///// </summary>
        ///// <param name="dto"></param>
        ///// <returns></returns>
        //public DBOperationStatus AddName(DTODataDictionary dto, int user)
        //{
        //    DBOperationStatus result = default(DBOperationStatus);
        //    using (ERPEntitiesNew context = new ERPEntitiesNew())
        //    {
        //        try
        //        {
        //            //判断数据库是否有信息,不管这条信息是否为假删除，如果有信息，执行下面操作，判断是在同类型中是否存在此信息，并且这条信息的状态没有被删除，如果存在，则添加失败，如果不存在，获得最大的code+1,
        //            var queryNum = context.Com_DataDictionary.Where(n => true);
        //            //如果数据库有信息
        //            if (queryNum.Count() > 0)
        //            {
        //                if (dto.TableKind != (int)DictionaryTableKind.Season)
        //                {
        //                    ////先判断数据库中，同类型中有没有相同的信息,并且isdelete的值不为1，（为1就是已经删除的信息状态）
        //                    var qu = context.Com_DataDictionary.Where(n => n.Name == dto.Name && n.TableKind == dto.TableKind && n.IsDelete != 1);
        //                    //判断类型中，存在此Name,并且状态不为假删除，如果条目大于0，返回，已存在此数据信息
        //                    if (qu.Count() > 0)
        //                    {
        //                        result = DBOperationStatus.Failed;
        //                        return result;
        //                    }
        //                }

        //                //查找最大的code
        //                var code = context.Com_DataDictionary.Max(n => n.Code);
        //                DAL.Com_DataDictionary dom = new Com_DataDictionary();
        //                dom.Name = dto.Name;
        //                dom.Alias = dto.Alias;
        //                dom.DataFlag = dto.DataFlag;
        //                dom.IPAdress = CommonCode.GetIP();
        //                dom.DT_CREATEDATE = DateTime.Now;
        //                dom.ST_CREATEUSER = user;
        //                dom.DT_MODIFYDATE = DateTime.Now;
        //                dom.IPAdress = CommonCode.GetIP();
        //                dom.TableKind = dto.TableKind;
        //                //根据传进来的id值，查找出名称
        //                dom.AttrName = GetOutContract(dto.TableKind);
        //                //数据库有信息时，code从数据库已有的最大值加一
        //                dom.Code = code + 1;
        //                dom.IsDelete = 0;
        //                context.Com_DataDictionary.Add(dom);
        //                int a = context.SaveChanges();
        //                if (a > 0)
        //                {
        //                    result = DBOperationStatus.Success;
        //                }
        //                else
        //                {
        //                    result = DBOperationStatus.Failed;
        //                }

        //            }
        //            else
        //            {
        //                DAL.Com_DataDictionary dom = new Com_DataDictionary();
        //                dom.Name = dto.Name;
        //                dom.Alias = dto.Alias;
        //                dom.DataFlag = dto.DataFlag;
        //                dom.IPAdress = CommonCode.GetIP();
        //                dom.DT_CREATEDATE = DateTime.Now;
        //                dom.ST_CREATEUSER = user;
        //                dom.DT_MODIFYDATE = DateTime.Now;
        //                dom.IPAdress = CommonCode.GetIP();
        //                dom.TableKind = dto.TableKind;
        //                //根据传进来的id值，查找出名称
        //                dom.AttrName = GetOutContract(dto.TableKind);
        //                //没有信息时，code从1开始
        //                dom.Code = 1;
        //                dom.IsDelete = 0;
        //                context.Com_DataDictionary.Add(dom);
        //                int a = context.SaveChanges();
        //                if (a > 0)
        //                {
        //                    result = DBOperationStatus.Success;
        //                }
        //                else
        //                {
        //                    result = DBOperationStatus.Failed;
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            LogHelper.WriteError(e);
        //        }
        //    }

        //    return result;
        //}

        public DBOperationStatus BatchDelete(List<int> id)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    foreach (var item in id)
                    {
                        var query = context.Com_DataDictionary.Find(item);
                        query.IsDelete = 1;
                    }
                    int lenth = context.SaveChanges();
                    if (lenth == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        result = DBOperationStatus.Success;
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteError(e);
                }
            }
            return result;
        }

        public DBOperationStatus Delete(int id)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    var query = context.Com_DataDictionary.Find(id);
                    query.IsDelete = 1;
                    int length = context.SaveChanges();
                    if (length == 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        result = DBOperationStatus.Success;
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteError(e);
                }
            }
            return result;
        }

        /// <summary>
        /// 查找全部港口
        /// </summary>
        public static List<DTODataDictionary> selectPotyName()
        {
            List<DTODataDictionary> LIST = new List<DTODataDictionary>();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    LIST = (from fa in context.Com_DataDictionary
                            where fa.TableKind == (int)DictionaryTableKind.OutPort && fa.IsDelete == 0
                            select new DTODataDictionary
                            {
                                ID = fa.ID,
                                Alias = fa.Alias,
                            }).ToList();
                }
                catch (Exception e)
                {
                    LogHelper.WriteError(e);
                }
            }
            return LIST;
        }

    }
}