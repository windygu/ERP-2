using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.Contacts
{
    public class DTOContacts
    {
        /// <summary>
        /// OLID
        /// </summary>
        public int OLID { get; set; }

        /// <summary>
        /// OCID
        /// </summary>
        public int OCID { get; set; }

        /// <summary>
        /// FirstName
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// LastName
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// FullName
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Duty
        /// </summary>
        public string Duty { get; set; }

        /// <summary>
        /// MobilePhone
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// TelPhone
        /// </summary>
        public string TelPhone { get; set; }

        /// <summary>
        /// Fax
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// IsDefault
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// CreateDate
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}