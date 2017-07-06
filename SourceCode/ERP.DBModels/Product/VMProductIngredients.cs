using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Product
{

    /// <summary>
    /// 产品成分
    /// </summary>
    [Serializable]
    public class VMProductIngredients
    {
        public int ID { get; set; }

        /// <summary>
        /// 产品成分构成
        /// </summary>
        public string IngredientName { get; set; }

        /// <summary>
        /// 百分比(%)
        /// </summary>
        public decimal? IngredientPercent { get; set; }

        public int ProductID { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public int ModuleType { get; set; }
    }
}
