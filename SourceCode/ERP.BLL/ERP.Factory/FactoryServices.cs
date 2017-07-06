using ERP.BLL.ERP.Dictionary;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Country;
using ERP.Models.CustomEnums;
using ERP.Models.Factory;
using ERP.Tools;
using ERP.Tools.EnumHelper;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Factory
{
    public class FactoryServices
    {
        private DictionaryServices _dictionaryServices = new DictionaryServices();


        /// <summary>
        /// 工厂枚举
        /// </summary>
        /// <returns></returns>

        public static Dictionary<int, string> GetContractStatus()
        {
            Dictionary<int, string> di = EnumHelper.GetCustomEnums<int>(typeof(FactoryDataFlagEnum));
            return di;
        }

        /// <summary>
        /// 获取工厂列表
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetAllFactoryKeyInfo2()
        {

            Dictionary<int, string> di = new Dictionary<int, string>();
            List<DTOFactory> list = GetAllFactoryKeyInfo();
            foreach (var item in list)
            {
                di.Add(item.ID, item.Abbreviation + "(" + item.CurrencyName + ")");
            }
            return di;
        }



        public List<DTOFactory> GetAllFactoryKeyInfo()
        {
            List<DTOFactory> customerKeyinfo = new List<DTOFactory>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    customerKeyinfo = (from fa in context.Factories
                                       where fa.IsDelete == 0
                                       orderby fa.Abbreviation descending
                                       select new DTOFactory
                                       {
                                           ID = fa.ID,
                                           Name = fa.Name,
                                           Abbreviation = fa.Abbreviation,
                                           CurrencyType = fa.CurrencyType,
                                           CurrencyName = (from c in context.Com_DataDictionary
                                                           where c.IsDelete == 0 && c.ID == fa.CurrencyType
                                                           select c.Name).FirstOrDefault()
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
        /// 按照数据分类查询工厂、代印公司基本信息列表
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="dataFlag">数据分类：1=工厂；2=代印公司</param>
        /// <returns></returns>
        public List<DTOFactory> GetSupplierSelectList(int userID, int dataFlag)
        {
            List<DTOFactory> customerKeyinfo = new List<DTOFactory>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    customerKeyinfo = (from fa in context.Factories
                                       where fa.DataFlag == dataFlag && fa.IsDelete == 0
                                       select new DTOFactory
                                       {
                                           ID = fa.ID,
                                           Name = fa.Name,
                                           Abbreviation = fa.Abbreviation,
                                           CurrencyType = fa.CurrencyType,
                                           CurrencyName = (from c in context.Com_DataDictionary
                                                           where c.IsDelete == 0 && c.ID == fa.CurrencyType
                                                           select c.Name).FirstOrDefault()
                                       }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return customerKeyinfo;
        }

        public List<DTOFactory> SelectAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, string callpeople, string Name, string Flag)
        {
            List<DTOFactory> listModel = new List<DTOFactory>();
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Factories.Where(n => n.IsDelete != 1);
                    //query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForFactory);

                    #region
                    if (!string.IsNullOrEmpty(callpeople))
                    {
                        query = query.Where(n => n.CallPeople.Contains(callpeople));
                    }
                    if (!string.IsNullOrEmpty(Name))
                    {
                        query = query.Where(n => n.Abbreviation.Contains(Name));
                    }
                    if (!string.IsNullOrEmpty(Flag))
                    {
                        int angle = int.Parse(Flag);
                        query = query.Where(n => n.DataFlag == angle);
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
                    List<DAL.Com_DataDictionary> list_Com_DataDictionary = _dictionaryServices.GetList(context.Com_DataDictionary.ToList());

                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();//分页

                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        listModel = new List<DTOFactory>();

                        foreach (var item in dataFromDB)
                        {
                            listModel.Add(new DTOFactory()
                            {
                                ID = item.ID,
                                Name = item.Name,
                                Abbreviation = item.Abbreviation,
                                AllAdress = item.AllAdress,
                                City = item.City,
                                CallPeople = item.CallPeople,
                                Telephone = item.Telephone,
                                Cellphone = item.Cellphone,
                                Fax = item.Fax,
                                EmailAdress = item.EmailAdress,
                                Duty = item.Duty,
                                CurrencyName = _dictionaryServices.GetDictionary_CurrencyName(item.CurrencyType, list_Com_DataDictionary),
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
        /// 查询中国所有的省(省份id,省份名称)
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        ///
        public List<ChinseProvince> selectProvince()
        {
            List<ChinseProvince> list = new List<ChinseProvince>();

            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                //省份
                try
                {
                    var query = context.Reg_Area.Where(n => n.COID == 2);
                    foreach (var item in query)
                    {
                        ChinseProvince province = new ChinseProvince();
                        province.ARID = item.ARID;//省份id
                        province.ProvinceName = item.AreaName;//省份名称
                        list.Add(province);
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteError(e);
                }
            }

            return list;
        }

        /// <summary>
        /// 查询所有的市
        /// </summary>
        /// <param name="idd">传进来省份id</param>
        /// <returns></returns>
        public List<ChinseCity> selectCity(int idd)
        {
            List<ChinseCity> list = new List<ChinseCity>();

            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    var query = context.Reg_City.Where(n => n.ARID == idd);

                    foreach (var item in query)
                    {
                        ChinseCity ch = new ChinseCity();
                        ch.CIID = item.CIID;//市id
                        ch.cityName = item.AreaName;//名称
                        list.Add(ch);
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteError(e);
                }
            }
            return list;
        }

        /// <summary>
        /// 查询所有的县
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ChinseArea> selectArea(int id)
        {
            List<ChinseArea> list = new List<ChinseArea>();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    var query = context.Reg_District.Where(n => n.CIID == id);
                    foreach (var item in query)
                    {
                        ChinseArea area = new ChinseArea();
                        area.DIID = item.DIID;//县id
                        area.AreaName = item.AreaName;//县名称
                        list.Add(area);
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteError(e);
                }
            }
            return list;
        }

        public DBOperationStatus AddFactory(DTOFactory dto, int user)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    //现根据传进来的工厂名称查询是否已存在,并且没有删除掉
                    var query = context.Factories.Where(n => n.Name == dto.Name && n.CurrencyType == dto.CurrencyType && n.IsDelete != 1);
                    //已存在
                    if (query.Count() > 0)
                    {
                        result = DBOperationStatus.NoAffect;
                    }
                    else
                    {
                        //添加
                        DAL.Factory factory = new DAL.Factory();
                        factory.Name = dto.Name;
                        factory.Abbreviation = dto.Abbreviation;
                        factory.Province = context.Reg_Area.Find(int.Parse(dto.Province)).AreaName;
                        factory.City = factory.Province;
                        if (dto.CityArea != "")
                        {
                            factory.Area = context.Reg_District.Find(int.Parse(dto.CityArea)).AreaName;
                        }
                        else
                        {
                            factory.Area = "";
                        }
                        if (dto.Area != "")
                        {
                            factory.CityArea = context.Reg_City.Find(int.Parse(dto.Area)).AreaName;
                        }
                        else
                        {
                            factory.CityArea = "";
                        }
                        factory.CallPeople = dto.CallPeople;
                        factory.Telephone = dto.Telephone;
                        factory.Cellphone = dto.Cellphone;
                        factory.EmailAdress = dto.EmailAdress;
                        factory.F_Address = dto.F_Address;
                        factory.AllAdress = factory.Province + factory.CityArea + factory.Area + factory.F_Address;
                        factory.Fax = dto.Fax;
                        factory.Duty = dto.Duty;
                        factory.ST_CREATEUSER = user;
                        factory.Hierarchy = dto.Hierachy;
                        factory.DataFlag = dto.DataFlag;

                        factory.RegisterFees = dto.RegisterFees;
                        factory.EnglishName = dto.EnglishName;
                        factory.EnglishAddress = dto.EnglishAddress;
                        factory.CurrencyType = dto.CurrencyType;

                        factory.DT_CREATEDATE = DateTime.Now;
                        factory.DT_MODIFYDATE = DateTime.Now;
                        factory.IPAdress = CommonCode.GetIP();
                        factory.IsDelete = 0;
                        context.Factories.Add(factory);
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
                }
                catch (Exception e)
                {
                    LogHelper.WriteError(e);
                }
            }

            return result;
        }

        /// <summary>
        /// 删除，就是将isdelete的状态改为1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DBOperationStatus DeleteFactory(List<int> id)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                //现根据传进来的工厂名称查询是否已存在,并且没有删除掉

                try
                {
                    foreach (var item in id)
                    {
                        var query = context.Factories.Find(item);
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

        /// <summary>
        /// 根据传进来的id值查询相应信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<DTOFactory> selectByID(int ID)
        {
            List<DTOFactory> list = new List<DTOFactory>();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Factories.Where(n => n.ID == ID);
                foreach (var item in query)
                {
                    DTOFactory dto = new DTOFactory();
                    dto.ID = item.ID;
                    dto.Name = item.Name;
                    dto.Abbreviation = item.Abbreviation;
                    dto.AllAdress = item.AllAdress;

                    dto.ProvinceID = ((context.Reg_Area.Where(n => n.AreaName == item.Province).FirstOrDefault().ARID));
                    dto.CityArea = item.Area;
                    dto.Area = item.CityArea;
                    if (item.CityArea == "")
                    {
                        dto.AreaID = 0;
                    }
                    else
                    {
                        //市id
                        dto.AreaID = ((context.Reg_City.Where(n => n.AreaName == item.CityArea).FirstOrDefault().CIID));
                    }
                    if (item.Area == "")
                    {
                        dto.CityAreaID = 0;
                    }
                    else
                    {
                        //县id
                        dto.CityAreaID = ((context.Reg_District.Where(n => n.AreaName == item.Area).FirstOrDefault().DIID));
                    }
                    dto.Hierachy = item.Hierarchy;
                    dto.CallPeople = item.CallPeople;
                    dto.Telephone = item.Telephone;
                    dto.Cellphone = item.Cellphone;
                    dto.Fax = item.Fax;
                    dto.EmailAdress = item.EmailAdress;
                    dto.Duty = item.Duty;
                    dto.DataFlag = (item.DataFlag);
                    dto.F_Address = item.F_Address;

                    dto.RegisterFees = item.RegisterFees;
                    dto.EnglishName = item.EnglishName;
                    dto.EnglishAddress = item.EnglishAddress;
                    dto.CurrencyType = item.CurrencyType;
                    list.Add(dto);
                }
            }
            return list;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public DBOperationStatus UpdateFactory(DTOFactory dto, int user)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    var query = context.Factories.Find(dto.ID);

                    //首先判断一下，是否存在此Name,如果Name存在，判读是否是此条id
                    var qu = context.Factories.Where(n => n.Name == dto.Name && n.CurrencyType == dto.CurrencyType && n.IsDelete == 0);//同一工厂的结算币种只能有一个
                    //如果存在此Name
                    if (qu.Count() > 0)
                    {
                        //判断id是否是id
                        var cc = context.Factories.Where(n => n.Name == dto.Name && n.CurrencyType == dto.CurrencyType && n.IsDelete == 0).FirstOrDefault().ID;
                        if (cc == dto.ID)
                        {
                            query.Name = dto.Name;
                            query.Abbreviation = dto.Abbreviation;
                            query.CallPeople = dto.CallPeople;
                            query.Cellphone = dto.Cellphone;
                            query.Telephone = dto.Telephone;
                            query.Duty = dto.Duty;
                            query.Fax = dto.Fax;
                            query.F_Address = dto.F_Address;
                            query.EmailAdress = dto.EmailAdress;
                            query.Duty = dto.Duty;
                            query.Hierarchy = dto.Hierachy;
                            query.Province = context.Reg_Area.Where(n => n.ARID == dto.ProvinceID).FirstOrDefault().AreaName;
                            //有问题
                            if (dto.CityAreaID == 0)
                            {
                                query.Area = "";
                            }
                            else
                            {
                                query.Area = context.Reg_District.Where(n => n.DIID == dto.CityAreaID).FirstOrDefault().AreaName;
                            }
                            if (dto.AreaID == 0)
                            {
                                query.CityArea = "";
                            }
                            else
                            {
                                query.CityArea = context.Reg_City.Where(n => n.CIID == dto.AreaID).FirstOrDefault().AreaName;
                            }

                            query.City = query.Province;
                            query.AllAdress = query.Province + query.CityArea + query.Area + query.F_Address;

                            query.RegisterFees = dto.RegisterFees;
                            query.EnglishName = dto.EnglishName;
                            query.EnglishAddress = dto.EnglishAddress;
                            query.CurrencyType = dto.CurrencyType;

                            query.DT_MODIFYDATE = DateTime.Now;
                            query.ST_MODIFYUSER = user;
                            query.DataFlag = dto.DataFlag;
                            int lenth = context.SaveChanges();
                            if (lenth == 0)
                            {
                                result = DBOperationStatus.Failed;
                            }
                            else
                            {
                                result = DBOperationStatus.Success;
                            }
                        }
                        else
                        {
                            result = DBOperationStatus.Failed;
                        }
                    }
                    else
                    {
                        query.Name = dto.Name;
                        query.Abbreviation = dto.Abbreviation;
                        query.CallPeople = dto.CallPeople;
                        query.Cellphone = dto.Cellphone;
                        query.Telephone = dto.Telephone;
                        query.Duty = dto.Duty;
                        query.Fax = dto.Fax;
                        query.Hierarchy = dto.Hierachy;
                        query.F_Address = dto.F_Address;
                        query.EmailAdress = dto.EmailAdress;
                        query.Duty = dto.Duty;
                        query.Province = context.Reg_Area.Where(n => n.ARID == dto.ProvinceID).FirstOrDefault().AreaName;
                        //有问题
                        if (dto.CityAreaID == 0)
                        {
                            query.Area = "";
                        }
                        else
                        {
                            query.Area = context.Reg_District.Where(n => n.DIID == dto.CityAreaID).FirstOrDefault().AreaName;
                        }
                        if (dto.AreaID == 0)
                        {
                            query.CityArea = "";
                        }
                        else
                        {
                            query.CityArea = context.Reg_City.Where(n => n.CIID == dto.AreaID).FirstOrDefault().AreaName;
                        }
                        query.City = query.Province;
                        query.AllAdress = query.Province + query.CityArea + query.Area + query.F_Address;
                        query.DT_MODIFYDATE = DateTime.Now;
                        query.ST_MODIFYUSER = user;
                        query.DataFlag = dto.DataFlag;

                        query.RegisterFees = dto.RegisterFees;
                        query.EnglishName = dto.EnglishName;
                        query.EnglishAddress = dto.EnglishAddress;
                        query.CurrencyType = dto.CurrencyType;

                        int lenth = context.SaveChanges();
                        if (lenth == 0)
                        {
                            result = DBOperationStatus.Failed;
                        }
                        else
                        {
                            result = DBOperationStatus.Success;
                        }
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteError(e);
                }
            }

            return result;
        }
    }
}