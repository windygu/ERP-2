using ERP.DAL;
using ERP.Models.Address;
using ERP.Tools.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.ERP.Address
{
    public class AddressServices
    {
        /// <summary>
        /// 获取地区
        /// </summary>
        /// <param name="countryID"></param>
        /// <returns></returns>
        public List<VMArea> GetAllAreaByCountryID(int? countryID)
        {
            List<VMArea> areaInfos = new List<VMArea>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Reg_Area.Where(p => true);
                    if (countryID.HasValue)
                    {
                        query = query.Where(p => p.COID == countryID.Value).OrderBy(d => d.AreaName);
                    }

                    areaInfos = (from a in query
                                 select new VMArea
                                 {
                                     COID = a.COID,
                                     ARID = a.ARID,
                                     Abbreviation = a.Abbreviation,
                                     AbbreviationA = a.AbbreviationA,
                                     AreaName = a.AreaName
                                 }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return areaInfos;
        }

        /// <summary>
        /// 获取所有的国家
        /// </summary>
        /// <returns></returns>
        public List<VMCountry> GetAllCountry()
        {
            List<VMCountry> list = new List<VMCountry>();
            try
            {
                using (ERPEntitiesNew context = new ERPEntitiesNew())
                {
                    var query = context.Reg_Country.Where(p => true);

                    list = (from a in query
                            select new VMCountry
                            {
                                COID = a.COID,
                                CountryName = a.CountryName,
                                Abbreviation = a.Abbreviation,
                                AbbreviationA = a.AbbreviationA,
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteError(ex);
            }
            return list;
        }



    }
}
