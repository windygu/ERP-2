using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalShipping : BaseApproval
    {
        public ApprovalShipping(ApprovalInfo info)
            : base(info)
        {

        }

        private Delivery_Encasements _item;
        public override object ItemInfo
        {
            get
            {
                if (_item == null)
                {
                    _item = context.Delivery_Encasements.Where(q => q.EncasementsID == approvalInfo.IdentityID).FirstOrDefault();
                }
                return _item;
            }
        }

        public override int ItemCreateUserID
        {
            get { return _item.CreateUserID; }
        }

        public override int ItemStatus
        {
            get
            {
                return _item.EncasementsStatus;
            }
            set
            {
                _item.EncasementsStatus = value;
            }
        }

        public override int? ItemApproverIndex
        {
            get
            {
                return _item.ApproverIndex;
            }
            set
            {
                _item.ApproverIndex = value;
            }
        }

        protected override void DoLogMethod()
        {
            //Purchase_ThreeTimesQCHistory t = approvalInfo.LogMethod() as Purchase_ThreeTimesQCHistory;
            //if (t != null)
            //{
            //    _item.Purchase_ThreeTimesQCHistory.Add(t);
            //}
        }

        protected override void DoSaveItemInfo()
        {
                var entry = context.Entry(_item);
                entry.State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
        }
    }
}
