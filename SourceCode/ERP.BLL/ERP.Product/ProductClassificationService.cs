using ERP.BLL.Consts;
using ERP.DAL;
using ERP.Models.AdminUser;
using ERP.Models.CustomEnums;
using ERP.Models.Product;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Product
{
    public class ProductClassificationService
    {
        public List<DTOProductClassifications> GetAll(VMERPUser currentUser, int? parentID, bool? isShow = null)
        {
            List<DTOProductClassifications> productClassifications = new List<DTOProductClassifications>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Classifies.Include("Classify1").Where(p => p.IsDelete == 0 && p.ParentID == parentID);

                    if (isShow.HasValue)
                    {
                        query = query.Where(p => p.Show == isShow.Value);
                    }
                    if (query != null)
                    {
                        foreach (var c in query.OrderBy(d=>d.Name))
                        {
                            bool existChildren = c.Classify1.Where(d => d.IsDelete == 0).Count() > 0;
                            productClassifications.Add(new DTOProductClassifications()
                            {
                                ID = c.ID,
                                Text = c.Name + "(" + context.UP_GetViewProductList(c.ID).Count() + ")",
                                Children = existChildren
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return productClassifications;
        }

        /// <summary>
        /// 产品页面用到了
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="isShow"></param>
        /// <returns></returns>
        public List<DTOProductClassifications> GetAllLeafNodes(VMERPUser currentUser, bool? isShow = null)
        {
            List<DTOProductClassifications> productClassifications = new List<DTOProductClassifications>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Classifies.Where(p => p.IsDelete == 0);

                    if (isShow.HasValue)
                    {
                        //query = query.Where(p => p.Show == isShow.Value && p.Classify2.Show == isShow.Value);
                    }
                    if (query != null)
                    {
                        foreach (var c in query)
                        {
                            int recursiveMaxDepth = AdminConsts.RECURSIVE_MAX_DEPTH;
                            productClassifications.Add(new DTOProductClassifications()
                            {
                                ID = c.ID,
                                Text = GetName(c, ref recursiveMaxDepth, isShow)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return productClassifications;
        }

        private string GetName(Classify current, ref int recursiveDepth, bool? isShow = null)
        {
            if (recursiveDepth < 0)
            {
                throw new Exception("递归次数超过了最大限制：" + AdminConsts.RECURSIVE_MAX_DEPTH);
            }

            if (current != null && current.Classify2 != null)
            {
                recursiveDepth--;
                if (isShow.HasValue && (isShow.Value == current.Classify2.Show))
                {
                    return GetName(current.Classify2, ref recursiveDepth, isShow) + " - " + current.Name;
                }
                else
                {
                    return current.Name;
                }
            }
            return current.Name;
        }

        public DTOProductClassificationMoreInfo GetMoreInfo(VMERPUser currentUser, int ID, bool? isShow = null)
        {
            DTOProductClassificationMoreInfo moreInfo = null;
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Classifies.Include("Classify2").Where(p => p.IsDelete == 0 && p.ID == ID);
                    if (isShow.HasValue)
                    {
                        query = query.Where(p => p.Show == isShow.Value && p.Classify2.Show == isShow.Value);
                    }
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        moreInfo = new DTOProductClassificationMoreInfo();
                        moreInfo.ID = dataFromDB.ID;
                        moreInfo.Name = dataFromDB.Name;
                        moreInfo.ParentID = dataFromDB.ParentID;
                        moreInfo.ParentName = dataFromDB.Classify2 == null ? null : dataFromDB.Classify2.Name;
                        moreInfo.Image = dataFromDB.Image;
                        moreInfo.Show = dataFromDB.Show;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return moreInfo;
        }

        public DBOperationStatus Update(VMERPUser currentUser, DTOProductClassificationMoreInfo model)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Classifies.Where(p => p.ID == model.ID && p.IsDelete == 0);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        dataFromDB.Name = model.Name;
                        dataFromDB.Image = model.Image;
                        dataFromDB.Show = model.Show;
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

        public int Create(VMERPUser currentUser, DTOProductClassificationMoreInfo model)
        {
            int newrowID = 0;

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    Classify classify = new Classify()
                    {
                        Name = model.Name,
                        ParentID = model.ParentID == -1 ? null : model.ParentID,
                        CreateUserID = currentUser.UserID,
                        Show = true,
                    };
                    context.Classifies.Add(classify);

                    int affectRows = context.SaveChanges();
                    newrowID = classify.ID;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }

            return newrowID;
        }

        public DBOperationStatus Rename(VMERPUser currentUser, DTOProductClassificationMoreInfo model)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Classifies.Where(p => p.ID == model.ID && p.IsDelete == 0);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        dataFromDB.Name = model.Name;
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

        public DBOperationStatus Move(VMERPUser currentUser, DTOProductClassificationMoreInfo model)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Classifies.Where(p => p.ID == model.ID && p.IsDelete == 0);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        if (model.ParentID == -1)
                        {
                            dataFromDB.ParentID = null;
                        }
                        else
                        {
                            dataFromDB.ParentID = model.ParentID;
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

        public DBOperationStatus Delete(VMERPUser currentUser, DTOProductClassificationMoreInfo model)
        {
            DBOperationStatus result = default(DBOperationStatus);

            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    if (context.UP_GetViewProductList(model.ID).Count() > 0)//该分类存在产品
                    {
                        return DBOperationStatus.ExistingRecord;
                    }
                    var query = context.Classifies.Where(p => p.ID == model.ID && p.IsDelete == 0);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        dataFromDB.IsDelete = 1;
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

        public List<VMViewProductList> GetViewProductList(VMERPUser currentUser, int id)
        {
            List<VMViewProductList> list_vm = new List<VMViewProductList>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var dataFromDB = context.UP_GetViewProductList(id);
                    if (dataFromDB != null)
                    {
                        foreach (var item in dataFromDB)
                        {
                            list_vm.Add(new VMViewProductList()
                            {
                                No = item.No,
                                Image = item.Image,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return list_vm;
        }

        public DBOperationStatus IsNameExist(VMERPUser currentUser, int currentID, string name, int? parentID)
        {
            DBOperationStatus result = default(DBOperationStatus);
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Classifies.Where(p => p.Name == name && p.IsDelete == 0 && p.ID != currentID && p.ParentID == parentID.Value);
                    var dataFromDB = query.FirstOrDefault();
                    if (dataFromDB != null)
                    {
                        result = DBOperationStatus.ExistingRecord;
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
}