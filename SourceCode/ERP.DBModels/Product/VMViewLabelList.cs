using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Product
{
    public class VMViewLabelList:IndexPageBaseModel
    {
        public int ProductID { get; set; }

        public string No { get; set; }

        public string StyleName { get; set; }

        public int? InnerBoxRate { get; set; }

        public int? OuterBoxRate { get; set; }

        public decimal? OuterVolume { get; set; }

        public decimal? LengthIN { get; set; }

        public decimal? WidthIN { get; set; }

        public decimal? HeightIN { get; set; }

        public decimal PriceFactory { get; set; }

        public string CurrencySign { get; set; }

        public int StyleNumber { get; set; }
    }
}