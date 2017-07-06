using ERP.BLL.Consts;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.CustomEnums;
using ERP.Models.Dictionary;
using ERP.Models.HS;
using ERP.Tools;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.HS
{
    /// <summary>
    /// 海关税则编码管理
    /// </summary>
    public class HSService
    {
        /// <summary>
        /// HS CODE下拉框
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="dataFlag">类型（1：报关的HSCODE类型[HSCode]，2：进口HSCODE类型[HTS]）</param>
        /// <returns></returns>
        public List<DTOHSContract> GetHSCodeSelectList(int userID, int dataFlag)
        {
            List<DTOHSContract> customerKeyinfo = new List<DTOHSContract>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    customerKeyinfo = (from a in context.HarmonizedSystems
                                       where a.DataFlag == dataFlag && !a.IsDelete
                                       select new DTOHSContract
                                       {
                                           ID = a.ID,
                                           HSCode = a.HSCode,
                                           CodeName = a.CodeName,
                                           CodeEngName = a.CodeEngName,
                                       }).OrderBy(p => p.HSCode).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return customerKeyinfo;
        }

        /// <summary>
        /// 查询全部的信息
        /// </summary>
        /// <returns></returns>
        public List<DTOHSContract> GetAll(VMERPUser currentUser, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows,
            VMHSContract vm)
        {
            List<DTOHSContract> listModel = new List<DTOHSContract>();
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.HarmonizedSystems.Where(p => !p.IsDelete);
                    //query = query.SetDataPermissionConditions(currentUser, DataPermissionModules.ForPurchase, "ST_CREATEUSER");

                    #region 筛选条件

                    if (!string.IsNullOrEmpty(vm.HSCode))
                    {
                        query = query.Where(d => d.HSCode.Contains(vm.HSCode));
                    }
                    if (!string.IsNullOrEmpty(vm.IsCheck))
                    {
                        int ddd = Utils.StrToInt(vm.IsCheck, 1);

                        var exceptionList = context.HS_Child.Select(e => e.HSID);
                        var allList = context.HarmonizedSystems.Select(e => e.ID);
                        if (ddd == 1)//是
                        {
                            var idlist = allList.Intersect(exceptionList);
                            query = query.Where(d => idlist.Contains(d.ID));
                        }
                        else
                        {
                            var idlist = allList.Except(exceptionList);
                            query = query.Where(d => idlist.Contains(d.ID));
                        }
                    }
                    if (vm.DataFlag != 0)
                    {
                        query = query.Where(d => d.DataFlag == vm.DataFlag);
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
                    var dataFromDB = query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                    totalRows = query.Count();


                    #region 给Model赋值

                    if (dataFromDB != null && dataFromDB.Count > 0)
                    {
                        foreach (var pc in dataFromDB)
                        {
                            listModel.Add(new DTOHSContract
                            {
                                ID = pc.ID,
                                HSCode = pc.HSCode,
                                Cess = pc.Cess,
                                DutyPercentList = pc.DutyPercentList,
                                CodeName = pc.CodeName,
                                CodeEngName = pc.CodeEngName,
                                IsCheck = IsCheck(pc.HS_Child),
                                ProjectName = CombinePacksName(pc.HS_Child),
                                DataFlag = pc.DataFlag ?? 0,
                                DT_MODIFYDATE = Utils.DateTimeToStr2(pc.DT_MODIFYDATE),
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

        ///查询所有的报检项目（没有删除掉的）
        public List<DTODictionary> selectAllDictionary()
        {
            List<DTODictionary> listmodel = new List<DTODictionary>();

            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    //查询出所有的报检项目，并且没有被删掉的
                    var query = context.Com_DataDictionary.Where(n => n.IsDelete != 1 && n.AttrName == "报检项目");
                    //如果存在数据，将所有的东西添加到
                    if (query.Count() > 0)
                    {
                        int length = query.Count();
                        listmodel = (from quer in query
                                     where quer.IsDelete != 1 && quer.AttrName == "报检项目"
                                     select new DTODictionary
                                     {
                                         Code = quer.Code,
                                         Name = quer.Name
                                     }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }
            return listmodel;
        }

        /// <summary>
        /// 根据id查找出信息，查询出是否报检，以及报检的项目id(tagid)
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DTOHSContract selectByID(int ID)
        {
            DTOHSContract con = new DTOHSContract();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.HarmonizedSystems.Where(n => n.ID == ID);
                foreach (var item in query)
                {
                    con.ID = item.ID;
                    con.HSCode = item.HSCode;
                    con.Cess = item.Cess;
                    con.DutyPercentList = item.DutyPercentList;
                    con.CodeName = item.CodeName;
                    con.CodeEngName = item.CodeEngName;
                    //是否报检
                    con.IsCheck = ISChecked(item.ID);
                    //报检ID
                    con.ProjectName = Tagid(item.ID);
                }
            }
            return con;
        }

        /// <summary>
        /// 修改，根据id
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public DBOperationStatus UpdateResult(DTOHSContract dto, int user, string codelist)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    if (codelist == "")
                    {
                        //首先根据传进来的codename判断，存不存在此codename,如果存在，查询id
                        var one = context.HarmonizedSystems.Where(n => n.HSCode == dto.HSCode & !n.IsDelete & n.DataFlag == dto.DataFlag);
                        //如果存在，查询id是否是本条信息
                        if (one.Count() > 0)
                        {
                            //TODO:加dataflag的条件
                            var two = context.HarmonizedSystems.Where(n => n.HSCode == dto.HSCode & !n.IsDelete & n.DataFlag == dto.DataFlag).FirstOrDefault().ID;
                            //是本条信息
                            if (two == dto.ID)
                            {
                                var three = context.HarmonizedSystems.Find(two);
                                three.HSCode = dto.HSCode;
                                three.Cess = dto.Cess;
                                three.DutyPercentList = dto.DutyPercentList;
                                three.DT_MODIFYDATE = DateTime.Parse(DateTime.Now.ToString());
                                three.IPAddress = CommonCode.GetIP();
                                three.CodeName = dto.CodeName;
                                three.CodeEngName = dto.CodeEngName;
                                int affectRows = context.SaveChanges();
                                if (affectRows == 0)
                                {
                                    result = DBOperationStatus.NoAffect;
                                }//如果修改成功
                                else
                                {
                                    var four = context.HS_Child.Where(n => n.HSID == dto.ID);
                                    if (four.Count() > 0)
                                    {
                                        var item = context.HS_Child.Where(n => n.HSID == dto.ID).FirstOrDefault().ID;
                                        var us = context.HS_Child.Find(item);
                                        context.HS_Child.Remove(us);

                                        int lenOne = context.SaveChanges();
                                        if (lenOne == 0)
                                        {
                                            result = DBOperationStatus.NoAffect;
                                        }
                                        else
                                        {
                                            result = DBOperationStatus.Success;
                                        }
                                    }
                                    else
                                    {
                                        result = DBOperationStatus.Success;
                                    }
                                }
                            }
                            else
                            {
                                result = DBOperationStatus.Failed;
                            }
                        }
                        //如果不存在此code,直接添加
                        else
                        {
                            //如果id
                            var quu = context.HarmonizedSystems.Find(dto.ID);
                            quu.HSCode = dto.HSCode;
                            quu.Cess = dto.Cess;
                            quu.DutyPercentList = dto.DutyPercentList;
                            quu.DT_MODIFYDATE = DateTime.Parse(DateTime.Now.ToString());
                            quu.IPAddress = CommonCode.GetIP();
                            quu.CodeName = dto.CodeName;
                            quu.CodeEngName = dto.CodeEngName;
                            int affectRows = context.SaveChanges();
                            if (affectRows == 0)
                            {
                                result = DBOperationStatus.NoAffect;
                            }//如果修改成功
                            else
                            {
                                var four = context.HS_Child.Where(n => n.HSID == dto.ID);
                                if (four.Count() > 0)
                                {
                                    var item = context.HS_Child.Where(n => n.HSID == dto.ID).FirstOrDefault().ID;
                                    var us = context.HS_Child.Find(item);
                                    context.HS_Child.Remove(us);
                                    context.SaveChanges();
                                    int onttwo = context.SaveChanges();
                                    if (onttwo == 0)
                                    {
                                        result = DBOperationStatus.NoAffect;
                                    }
                                    else
                                    {
                                        result = DBOperationStatus.Success;
                                    }
                                }
                                else
                                {
                                    result = DBOperationStatus.Success;
                                }
                            }
                        }
                    }
                    //存在code
                    else
                    {
                        //首先根据传进来的codename判断，存不存在此codename,如果存在，查询id,不存在，
                        var one = context.HarmonizedSystems.Where(n => n.HSCode == dto.HSCode & !n.IsDelete && n.DataFlag == dto.DataFlag);
                        //如果存在，查询id是否是本条信息
                        if (one.Count() > 0)
                        {
                            var two = context.HarmonizedSystems.Where(n => n.HSCode == dto.HSCode & !n.IsDelete && n.DataFlag == dto.DataFlag).FirstOrDefault().ID;
                            //是本条信息

                            if (two == dto.ID)
                            {
                                var three = context.HarmonizedSystems.Find(two);
                                three.HSCode = dto.HSCode;
                                three.Cess = dto.Cess;
                                three.DutyPercentList = dto.DutyPercentList;
                                three.CodeName = dto.CodeName;
                                three.CodeEngName = dto.CodeEngName;
                                three.DT_MODIFYDATE = DateTime.Parse(DateTime.Now.ToString());
                                three.IPAddress = CommonCode.GetIP();

                                int affectRows = context.SaveChanges();
                                if (affectRows == 0)
                                {
                                    result = DBOperationStatus.NoAffect;
                                }//如果修改成功
                                else
                                {
                                    //如果已经存在ciHsID信息，可直接修改，如果不存在，就添加
                                    var four = context.HS_Child.Where(n => n.HSID == dto.ID);
                                    if (four.Count() > 0)
                                    {
                                        int item = context.HS_Child.Where(n => n.HSID == dto.ID).FirstOrDefault().ID;
                                        DAL.HS_Child oneChild = context.HS_Child.Find(item);
                                        oneChild.HSID = dto.ID;
                                        oneChild.IsCheck = true;
                                        oneChild.TagID = codelist;
                                        //context.SaveChanges();
                                        int lengone = context.SaveChanges();
                                        if (lengone == 0)
                                        {
                                            result = DBOperationStatus.NoAffect;
                                        }
                                        else
                                        {
                                            result = DBOperationStatus.Success;
                                        }
                                    }
                                    else
                                    {
                                        DAL.HS_Child hs = new HS_Child();
                                        hs.HSID = dto.ID;
                                        hs.IsCheck = true;
                                        hs.TagID = codelist;
                                        context.HS_Child.Add(hs);
                                        context.SaveChanges();
                                        int lenthree = context.SaveChanges();
                                        if (lenthree == 0)
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
                            else
                            {
                                result = DBOperationStatus.Failed;
                            }
                        }//存在code，但是并不是相同的hscode
                        else
                        {
                            //直接修改，修改成功后，修改hss_chlid表
                            var c = context.HarmonizedSystems.Find(dto.ID);
                            c.CodeName = dto.CodeName;
                            c.CodeEngName = dto.CodeEngName;
                            c.HSCode = dto.HSCode;
                            c.Cess = dto.Cess;
                            c.DutyPercentList = dto.DutyPercentList;
                            c.DT_MODIFYDATE = DateTime.Parse(DateTime.Now.ToString());
                            c.IPAddress = CommonCode.GetIP();

                            int lengtho = context.SaveChanges();
                            if (lengtho == 0)
                            {
                                result = DBOperationStatus.NoAffect;
                            }
                            else
                            {
                                var quone = context.HS_Child.Where(n => n.HSID == dto.ID);
                                if (quone.Count() > 0)
                                {
                                    int id = context.HS_Child.Where(n => n.HSID == dto.ID).FirstOrDefault().ID;
                                    var use = context.HS_Child.Find(id);
                                    use.HSID = dto.ID;
                                    use.IsCheck = true;
                                    use.TagID = codelist;

                                    int tagoneOO = context.SaveChanges();
                                    if (tagoneOO == 0)
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
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
                return result;
            }
        }

        /// <summary>
        /// 删除，就是将它的状态改为true
        /// </summary>
        /// <returns></returns>
        public DBOperationStatus Delete(int id)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    var query = context.HarmonizedSystems.Find(id);
                    query.IsDelete = true;
                    context.SaveChanges();
                    int affectRows = context.SaveChanges();
                    result = DBOperationStatus.Success;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }
            return result;
        }

        /// <summary>
        ///添加海关税则编码（添加海关税则编码及税率）
        /// </summary>
        /// <returns></returns>
        public DBOperationStatus AddCode(DTOHSContract dto, int user, string codelist)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    //首先判断传进来的海关税则编码是否存在,并且没有被删除
                    var query = context.HarmonizedSystems.Where(n => n.HSCode == dto.HSCode && !n.IsDelete && n.DataFlag == dto.DataFlag);
                    if (query.Count() > 0)//存在的
                    {
                        result = DBOperationStatus.ExistingRecord;
                    }
                    else
                    {
                        DAL.HarmonizedSystem model = new DAL.HarmonizedSystem()
                        {
                            HSCode = dto.HSCode,
                            Cess = dto.Cess,
                            DutyPercentList = dto.DutyPercentList,
                            CodeName = dto.CodeName,
                            CodeEngName = dto.CodeEngName,
                            DT_CREATEDATE = DateTime.Parse(DateTime.Now.ToString()),
                            ST_CREATEUSER = user,
                            DT_MODIFYDATE = DateTime.Parse(DateTime.Now.ToString()),
                            ST_MODIFYUSER = user,
                            IsDelete = false,
                            IPAddress = CommonCode.GetIP(),
                            DataFlag = dto.DataFlag
                        };

                        context.HarmonizedSystems.Add(model);
                        int affectRows = context.SaveChanges();
                        if (affectRows == 0)
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                        else
                        {
                            if (codelist != "")
                            {
                                //如果没有，就是为1个
                                DAL.HS_Child child = new HS_Child()
                                {
                                    TagID = (codelist),
                                    HSID = context.HarmonizedSystems.Max(d => d.ID),
                                    IsCheck = true
                                };
                                context.HS_Child.Add(child);
                                context.SaveChanges();
                            }
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
        }

        /// <summary>
        /// 把HSCoe表与HS_Child表对应起来得到需要的报检项目
        /// </summary>
        /// <param name="pp"></param>
        /// <param name="linkMark"></param>
        /// <returns></returns>
        private string CombinePacksName(ICollection<HS_Child> pp)
        {
            using (ERPEntitiesNew Entity = new ERPEntitiesNew())
            {
                string sR = string.Empty;
                foreach (var p in pp)
                {
                    if (p.IsCheck == true)
                    {
                        //sR += linkMark + Entity.Com_DataDictionary.Where(n => n.Code == p.TagID).FirstOrDefault().Name;

                        //如果有逗号隔开，两个及以上
                        if (p.TagID.Contains(","))
                        {
                            string[] arr = p.TagID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            int arrlen = arr.Length;
                            for (int i = 0; i < arr.Length; i++)
                            {
                                int a = int.Parse(arr[i]);
                                var ab = Entity.Com_DataDictionary.Where(n => n.Code == a);
                                foreach (var item in ab)
                                {
                                    sR += item.Name + ",";
                                }
                            }
                            sR = sR.Substring(0, sR.Length - 1);
                        }
                        //如果没有逗号隔开，就说明只有一个tagid,不需要用到split这些
                        else
                        {
                            int a = int.Parse(p.TagID);

                            var query = Entity.Com_DataDictionary.Where(n => n.Code == a);
                            foreach (var item in query)
                            {
                                sR = item.Name;
                            }
                        }
                    }
                }

                //if (sR != string.Empty)
                //{
                //    sR = sR.Remove(0, 1);
                //}
                return sR;
            }
        }

        /// <summary>
        /// 是否报检
        /// </summary>
        /// <param name="pp"></param>
        /// <returns></returns>
        private string IsCheck(ICollection<HS_Child> pp)
        {
            string Sr = "";
            foreach (var item in pp)
            {
                if (item.IsCheck)
                {
                    Sr = "是";
                }
            }
            if (Sr == "")
            {
                Sr = "否";
            }
            else
            {
                Sr = "是";
            }
            return Sr;
        }

        /// <summary>
        /// 根据id查询是否报检
        /// </summary>
        /// <returns></returns>
        private string ISChecked(int id)
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                string Sr = "";
                var query = context.HS_Child.Where(n => n.HSID == id);
                if (query.Count() > 0)
                {
                    foreach (var item in query)
                    {
                        if (item.IsCheck)
                        {
                            Sr = "是";
                        }
                    }
                    if (Sr == "")
                    {
                        Sr = "否";
                    }
                    else
                    {
                        Sr = "是";
                    }
                }
                else
                {
                    Sr = "否";
                }
                return Sr;
            }
        }

        /// <summary>
        /// 根据id查找tagid
        /// </summary>
        /// <returns></returns>
        private string Tagid(int id)
        {
            string Sr = "";
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.HS_Child.Where(n => n.HSID == id);
                //如果记录等于1
                if (query.Count() == 1)
                {
                    foreach (var item in query)
                    {
                        Sr += item.TagID;
                    }
                }//如果记录大于1,就已逗号隔开，并且将字符串最后一位去掉（,）
                else if (query.Count() > 1)
                {
                    foreach (var item in query)
                    {
                        Sr += item.TagID + ",";
                    }

                    Sr = Sr.Substring(0, Sr.Length - 1);
                }
            }

            return Sr;
        }

        public List<decimal> GetDutyPercentList(int ID)
        {
            List<decimal> list = new List<decimal>();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.HarmonizedSystems.Find(ID);
                if (query != null)
                {
                    string DutyPercentList = query.DutyPercentList;
                    if (!string.IsNullOrEmpty(DutyPercentList))
                    {
                        string[] list_temp = DutyPercentList.Split(';');
                        foreach (var item in list_temp)
                        {
                            if (Utils.IsDecimal(item))
                            {
                                list.Add(Utils.StrToDecimal(item,0));
                            }
                        }
                    }
                }
            }
            return list;
        }
    }
}