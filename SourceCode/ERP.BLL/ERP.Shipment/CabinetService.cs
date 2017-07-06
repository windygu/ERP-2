using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.Cabinet;
using ERP.Models.CustomEnums;
using ERP.Models.Shipment;
using ERP.Tools;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Shipment
{
    public class CabinetService
    {
        /// <summary>
        /// 查找全部的柜型和尺寸
        /// </summary>
        /// <returns></returns>
        public List<DTOCabinet> selectList()
        {
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    var query = (from Query in context.Shipment_Cabinet
                                 where Query.IsDelete != 1
                                 select new DTOCabinet
                                 {
                                     ID = Query.ID,
                                     Name = Query.Name,
                                     Size = Query.Size
                                 }).ToList();
                    return query;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }

            return null;
        }

        /// <summary>
        /// 查找全部的未删除的信息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="sortColumnsNames"></param>
        /// <param name="sortColumnOrders"></param>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRows"></param>
        /// <returns></returns>
        public List<DTOCabinet> selectALL(int userID, List<string> sortColumnsNames, List<string> sortColumnOrders, int currentPage, int pageSize, out int totalRows, string Name, string Size)
        {
            List<DTOCabinet> listModel = new List<DTOCabinet>();
            totalRows = 0;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Shipment_Cabinet.Where(n => n.IsDelete != 1);
                    #region
                    if (!string.IsNullOrEmpty(Name))
                    {
                        query = query.Where(n => n.Name.Contains(Name));
                    }
                    if (!string.IsNullOrEmpty(Size))
                    {
                        query = query.Where(n => n.Size.Contains(Size));
                    }
                    //if (!string.IsNullOrEmpty(Flag))
                    //{
                    //    int angle = int.Parse(Flag);
                    //    query = query.Where(n => n.DataFlag == angle);
                    //}
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

                    query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);//分页
                    listModel = (from quer in query
                                 where quer.IsDelete != 1
                                 select new DTOCabinet
                                 {
                                     ID = quer.ID,
                                     Name = quer.Name,
                                     Size = quer.Size,
                                 }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return listModel;
        }

        /// <summary>
        /// 添加柜型
        /// </summary>
        /// <returns></returns>
        public DBOperationStatus AddCacinet(DTOCabinet dto, int user)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    //先判断传进来的名称是否已经存在，如果存在，不能添加
                    var query = context.Shipment_Cabinet.Where(n => n.Name == dto.Name && n.IsDelete != 1);
                    if (query.Count() > 0)
                    {
                        result = DBOperationStatus.Failed;
                    }
                    else
                    {
                        DAL.Shipment_Cabinet dal = new Shipment_Cabinet();
                        dal.Name = dto.Name;
                        dal.Size = dto.Size;
                        dal.DT_CREATEDATE = DateTime.Now;
                        dal.DT_MODIFYDATE = DateTime.Now;
                        dal.ST_CREATEUSER = user;
                        dal.IPAddress = CommonCode.GetIP();
                        dal.Length = decimal.Parse(dto.Length);
                        dal.Width = decimal.Parse(dto.Width);
                        dal.Height = decimal.Parse(dto.Height);
                        context.Shipment_Cabinet.Add(dal);
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
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }
            return result;
        }

        /// <summary>
        /// 删除柜型(单个删除)
        /// </summary>
        /// <returns></returns>
        public DBOperationStatus DeleteCacinet(int id)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Shipment_Cabinet.Find(id);
                query.IsDelete = 1;
                int length = context.SaveChanges();
                if (length > 0)
                {
                    result = DBOperationStatus.Success;
                }
                else
                {
                    result = DBOperationStatus.NoAffect;
                }
            }
            return result;
        }

        /// <summary>
        /// 删除柜型(批量删除)
        /// </summary>
        /// <returns></returns>
        public DBOperationStatus DeleteCacinet(List<int> id)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                foreach (var item in id)
                {
                    var query = context.Shipment_Cabinet.Find(item);
                    query.IsDelete = 1;
                }
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
            return result;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public DBOperationStatus UpdateCacinet(DTOCabinet dto, int user)
        {
            DBOperationStatus result = default(DBOperationStatus);
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                try
                {
                    //先判断传进来的名称是否已经存在，如果存在，不能添加
                    var query = context.Shipment_Cabinet.Where(n => n.Name == dto.Name && n.IsDelete != 1);
                    if (query.Count() > 0)
                    {
                        var caid = context.Shipment_Cabinet.Where(n => n.Name == dto.Name && n.IsDelete != 1).FirstOrDefault().ID;
                        if (caid == dto.ID)
                        {
                            var quer = context.Shipment_Cabinet.Find(caid);
                            quer.Name = dto.Name;
                            quer.Size = dto.Size;
                            quer.Length = decimal.Parse(dto.Length);
                            quer.Width = decimal.Parse(dto.Width);
                            quer.Height = decimal.Parse(dto.Height);
                            quer.IPAddress = CommonCode.GetIP();
                            quer.DT_MODIFYDATE = DateTime.Now;
                            quer.ST_MODIFYUSER = user;

                            int length = context.SaveChanges();
                            if (length > 0)
                            {
                                result = DBOperationStatus.Success;
                            }
                            else
                            {
                                result = DBOperationStatus.NoAffect;
                            }
                        }
                        else
                        {
                            result = DBOperationStatus.Failed;
                        }
                    }
                    else
                    {
                        var quer = context.Shipment_Cabinet.Find(dto.ID);
                        quer.Name = dto.Name;
                        quer.Size = dto.Size;
                        quer.Length = decimal.Parse(dto.Length);
                        quer.Width = decimal.Parse(dto.Width);
                        quer.Height = decimal.Parse(dto.Height);
                        quer.IPAddress = CommonCode.GetIP();
                        quer.DT_MODIFYDATE = DateTime.Now;
                        quer.ST_MODIFYUSER = user;
                        int length = context.SaveChanges();
                        if (length > 0)
                        {
                            result = DBOperationStatus.Success;
                        }
                        else
                        {
                            result = DBOperationStatus.NoAffect;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.WriteError(ex);
                }
            }
            return result;
        }

        /// <summary>
        /// 根据传进来的id查找信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<DTOCabinet> SelectById(int id)
        {
            List<DTOCabinet> list = new List<DTOCabinet>();
            using (ERPEntitiesNew context = new ERPEntitiesNew())
            {
                var query = context.Shipment_Cabinet.Find(id);
                DTOCabinet dal = new DTOCabinet();
                dal.Name = query.Name;
                if (query.Size.Contains("~"))
                {
                    string val = query.Size;

                    String[] sss = val.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
                    dal.Size = sss[0];
                    dal.Sizetwo = sss[1];
                }
                else
                {
                    dal.Size = query.Size;
                    dal.Sizetwo = "";
                }
                dal.Length = query.Length.ToString();
                dal.Width = query.Width.ToString();
                dal.Height = query.Height.ToString();
                list.Add(dal);
            }
            return list;
        }

        public List<DTOCabinet> GetAll()
        {
            List<DTOCabinet> listModel = new List<DTOCabinet>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Shipment_Cabinet.Where(n => n.IsDelete != 1);
                    listModel = (from quer in query
                                 select new DTOCabinet
                                 {
                                     ID = quer.ID,
                                     Name = quer.Name,
                                     Size = quer.Size,
                                 }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return listModel;
        }
    }
}