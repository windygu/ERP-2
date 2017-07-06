using ERP.Models.Common;
using ERP.Models.CustomEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.ProductFitting
{
    public class VMProductFittingInfo
    {
        public int ID { get; set; }

        public int FactoryID { get; set; }

        public string No { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public decimal? Length { get; set; }

        public decimal? LengthIN { get; set; }

        public decimal? Height { get; set; }

        public decimal? HeightIN { get; set; }

        public decimal? Width { get; set; }

        public decimal? WidthIN { get; set; }

        public decimal? PriceFactory { get; set; }

        public string Comment { get; set; }

        public bool? Deleted { get; set; }

        public int CurrencyType { get; set; }

        public DateTime? DT_CREATEDATE { get; set; }

        public int? ST_CREATEUSER { get; set; }

        public string DT_MODIFYDATE { get; set; }

        public string FactoryName { get; set; }

        public string CurrencyName { get; set; }

        public string CurrencySign { get; set; }

        public int ProductID { get; set; }
        public int? Qty { get; set; }

        /// <summary>
        /// 数据记录对应的用户部门
        /// </summary>
        public int UserHierachyID { get; set; }

        public PageTypeEnum PageType { get; set; }
        public string PackageName { get; set; }
        public string PriceFactoryFormatter { get; set; }
        public int? ParentID { get; set; }
        public int? RootID { get; set; }

        /// <summary>
        /// 跟单费用比例(%)
        /// </summary>
        public decimal? FeesRate { get; set; }
    }
}