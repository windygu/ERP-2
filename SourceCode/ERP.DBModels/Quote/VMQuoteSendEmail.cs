using ERP.Models.Common;
using ERP.Models.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Quote
{
    public class VMQuoteSendEmail
    {
        public int ID { get; set; }
        public int StatusID { get; set; }

        public List<DTOContacts> Contacts { get; set; }

        public VMSendEmail SendEmail { get; set; }
    }
}