using ERP.Models.Rep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Customer
{
    public class DTOOrderCustomers
    {
        public int OCID { get; set; }

        public string CustomerCode { get; set; }

        public string CustomerName { get; set; }

        public string ContactName { get; set; }

        public string Abbreviation { get; set; }

        public string AbbreviationA { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public Nullable<int> Region { get; set; }

        public Nullable<int> Province { get; set; }

        public Nullable<int> Country { get; set; }

        public string PostalCode { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 应该是MiscImportLoadPercent
        /// </summary>
        public decimal? MiscImportLoadAmount { get; set; }

        public decimal? Commission { get; set; }

        public decimal? Agent { get; set; }

        public decimal? Allowance { get; set; }

        public decimal? MU { get; set; }

        public decimal? FOBNET { get; set; }

        public decimal? FinalFOB { get; set; }

        public decimal? CtnsPallet { get; set; }

        public decimal? PcsPallet { get; set; }

        public decimal? Palletpc { get; set; }

        public string QuoteTemplateFileName { get; set; }

        public string Duty { get; set; }

        public string MobilePhone { get; set; }

        public string Telephone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string CustomerAddress { get; set; }

        public List<VMAcceptInformation> AcceptInformations { get; set; }

        public List<VMContact> Contacts { get; set; }

        public List<VMFreightRate> VMFreight { get; set; }

        public List<VMCommission> RepDataList { get; set; }

        public string Code { get; set; }

        /// <summary>
        /// ELC补差%
        /// </summary>
        public decimal? ELCFill { get; set; }

        public int? PaymentType { get; set; }
        public int? SeasonID { get; set; }

        public string SelectCustomer { get; set; }
        public string SeasonIDList { get; set; }
    }

    public class VMAcceptedInfo
    {
        public int OCID { get; set; }

        public int AIID { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string AreaName { get; set; }

        public string CountryName { get; set; }

        public string AcceptedDetail { get; set; }
        public string CompanyName { get; set; }
        public string PostalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}