using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Dictionary
{
    public class DTODictionary
    {
        public int ID { get; set; }

        public decimal Code { get; set; }

        public string Name { get; set; }

        public Nullable<decimal> TableKind { get; set; }

        public string AttrName { get; set; }

        public string Alias { get; set; }

        public Nullable<System.DateTime> DT_CREATEDATE { get; set; }

        public Nullable<int> ST_CREATEUSER { get; set; }

        public Nullable<System.DateTime> DT_MODIFYDATE { get; set; }

        public Nullable<int> ST_MODIFYUSER { get; set; }

        public string IPAdress { get; set; }

        public Nullable<int> IsDelete { get; set; }

        public Nullable<int> DataFlag { get; set; }
    }
}