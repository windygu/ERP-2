using ERP.DAL;
using ERP.Models.CustomEnums;
using ERP.Models.Dictionary;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Dictionary
{
    public class DictionaryServices
    {
        public List<DTODictionary> GetDictionaryInfos(int userID, DictionaryTableKind? tableKind)
        {
            List<DTODictionary> customerKeyinfo = new List<DTODictionary>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Com_DataDictionary.Where(p => p.IsDelete == 0);
                    if (tableKind.HasValue)
                    {
                        query = query.Where(p => p.TableKind == (int)tableKind);
                    }
                    customerKeyinfo = (from entity in query
                                       select new DTODictionary
                                       {
                                           Alias = entity.Alias,
                                           AttrName = entity.AttrName,
                                           Code = entity.Code,
                                           DT_CREATEDATE = entity.DT_CREATEDATE,
                                           DT_MODIFYDATE = entity.DT_MODIFYDATE,
                                           ID = entity.ID,
                                           IPAdress = entity.IPAdress,
                                           IsDelete = entity.IsDelete,
                                           Name = entity.Name,
                                           ST_CREATEUSER = entity.ST_CREATEUSER,
                                           ST_MODIFYUSER = entity.ST_MODIFYUSER,
                                           TableKind = entity.TableKind,
                                           DataFlag = entity.DataFlag,
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
        /// 获取未删除的数据字典列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<DAL.Com_DataDictionary> GetList(List<DAL.Com_DataDictionary> list)
        {
            return list.Where(d => d.IsDelete == 0).ToList();
        }

        #region 获取数据字典的数据

        /// <summary>
        /// 是否存在Code
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        private static bool IsExistCode(int? Code, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return list_Com_DataDictionary.Where(d => d.Code == Code).Count() > 0;
        }

        /// <summary>
        /// 获取数据字典名称
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionaryByName(int? Code, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            if (Code.HasValue)
            {
                if (IsExistCode(Code, list_Com_DataDictionary))
                {
                    return list_Com_DataDictionary.Where(d => d.Code == Code).FirstOrDefault().Name;
                }
            }
            return "";
        }

        /// <summary>
        /// 获取数据字典别名
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionaryByAlias(int? Code, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            if (Code.HasValue)
            {
                if (IsExistCode(Code, list_Com_DataDictionary))
                {
                    return list_Com_DataDictionary.Where(d => d.Code == Code).FirstOrDefault().Alias;
                }
            }
            return "";
        }

        /// <summary>
        /// 获取数据字典别名
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionaryByDepartmentNumber(int? Code, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            if (Code.HasValue)
            {
                if (IsExistCode(Code, list_Com_DataDictionary))
                {
                    return list_Com_DataDictionary.Where(d => d.Code == Code).FirstOrDefault().DepartmentNumber;
                }
            }
            return "";
        }

        /// <summary>
        /// 获取数据字典名称
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_SeasonZhName(int? Code, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            if (Code.HasValue)
            {
                if (IsExistCode(Code, list_Com_DataDictionary))
                {
                    return list_Com_DataDictionary.Where(d => d.Code == Code).FirstOrDefault().SeasonZhName;
                }
            }
            return "";
        }

        /// <summary>
        /// 获取数据字典名称
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionaryByName2(int? ID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            if (ID.HasValue)
            {
                var query = list_Com_DataDictionary.Where(d => d.ID == ID);
                if (query != null && query.Count() >= 0 && query.FirstOrDefault() != null)
                {
                    return query.FirstOrDefault().Name;
                }
            }
            return "";
        }

        /// <summary>
        /// 获取数据字典别名
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionaryByAlias2(int? ID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            if (ID.HasValue)
            {
                var query = list_Com_DataDictionary.Where(d => d.ID == ID);
                if (query != null && query.Count() >= 0 && query.FirstOrDefault() != null)
                {
                    return query.FirstOrDefault().Alias;
                }
            }
            return "";
        }
        /// <summary>
        /// 获取数据字典的单位名称
        /// </summary>
        /// <param name="UnitID"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_UnitName(int? UnitID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByName(UnitID, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的货币名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_CurrencyName(int? CurrencyType, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByName(CurrencyType, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的货币符号
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_CurrencySign(int? CurrencyType, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByAlias(CurrencyType, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的出运港名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_PortName(int? PortID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByName(PortID, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的出运港名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_PortEnName(int? PortID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByAlias(PortID, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的目的港名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_DestinationPortName(int? DestinationPortID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByName(DestinationPortID, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的目的港名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_DestinationPortEnName(int? DestinationPortID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByAlias(DestinationPortID, list_Com_DataDictionary);
        }



        /// <summary>
        /// 获取数据字典的款式名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_StyleName(int? StyleID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByName(StyleID, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的款式名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_StyleNumber(int? StyleID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByAlias(StyleID, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的包装资料名称（中文）
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_PackingMannerZhName(int? PackingMannerZhID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByName(PackingMannerZhID, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的包装资料名称（英文）
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_PackingMannerEnName(int? PackingMannerEnID, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByAlias(PackingMannerEnID, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的季节名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_AllSeasonName(int? Season, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            string name = GetDictionaryByName(Season, list_Com_DataDictionary);
            string name2 = GetDictionaryByAlias(Season, list_Com_DataDictionary);

            if (string.IsNullOrEmpty(name2))
            {
                return name;
            }
            else
            {
                return name + " - " + name2;
            }
        }

        /// <summary>
        /// 获取数据字典的季节别名
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_SeasonAlias(int? Season, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByAlias(Season, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的季节名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_SeasonName(int? Season, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByName(Season, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的季节部门名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_SeasonDepartmentNumber(int? Season, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByDepartmentNumber(Season, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的部门名称
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_DepartmentName(int? Department, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByName2(Department, list_Com_DataDictionary);
        }

        /// <summary>
        /// 获取数据字典的部门别名
        /// </summary>
        /// <param name="CurrencyType"></param>
        /// <param name="list_Com_DataDictionary"></param>
        /// <returns></returns>
        public string GetDictionary_DepartmentAlias(int? Season, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetDictionaryByAlias2(Season, list_Com_DataDictionary);
        }


        #endregion 获取数据字典的数据

        #region 根据数据字典名称获取ID

        private static bool IsExistName(string Name, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return list_Com_DataDictionary.Where(d => d.Name == Name).Count() > 0;
        }

        private static int GetCodeByName(string Name, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                if (IsExistName(Name, list_Com_DataDictionary))
                {
                    return list_Com_DataDictionary.Where(d => d.Name == Name).FirstOrDefault().Code;
                }
            }
            return -1;
        }

        private static int GetCodeByAlias(string Alias, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return list_Com_DataDictionary.Where(d => d.Alias == Alias).FirstOrDefault().Code;
        }

        public int GetCode_StyleID(string StyleName, List<DAL.Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetCodeByName(StyleName, list_Com_DataDictionary);
        }

        public int GetCode_UnitID(string UnitName, List<Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetCodeByName(UnitName, list_Com_DataDictionary);
        }

        public int GetCode_PortID(string PortName, List<Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetCodeByName(PortName, list_Com_DataDictionary);
        }

        public int GetCode_PackingMannerZhID(string PackingMannerZhName, List<Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetCodeByName(PackingMannerZhName, list_Com_DataDictionary);
        }

        public int GetCode_CurrencyType(string CurrencyName, List<Com_DataDictionary> list_Com_DataDictionary)
        {
            return GetCodeByName(CurrencyName, list_Com_DataDictionary);
        }

        #endregion 根据数据字典名称获取ID
    }
}