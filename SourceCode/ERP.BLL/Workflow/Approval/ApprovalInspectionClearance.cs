﻿using ERP.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Workflow.Approval
{
    public class ApprovalInspectionClearance : BaseApproval
    {
        public ApprovalInspectionClearance(ApprovalInfo info)
             : base(info)
        {
        }

        private Inspection_InspectionClearance _item;

        public override object ItemInfo
        {
            get
            {
                if (_item == null)
                    _item = context.Inspection_InspectionClearance.Where(i => i.InspectionClearanceID == approvalInfo.IdentityID).FirstOrDefault();

                return _item;
            }
        }

        public override int ItemCreateUserID
        {
            get { return _item.ST_CREATEUSER; }
        }

        public override int ItemStatus
        {
            get
            {
                return _item.StatusID;
            }
            set
            {
                _item.StatusID = value;
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
            var history = approvalInfo.LogMethod() as Inspection_InspectionClearanceHis;
            if (history != null)
                _item.Inspection_InspectionClearanceHis.Add(history);
        }

        protected override void DoSaveItemInfo()
        {
            var entry = context.Entry(_item);
            entry.State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }
    }
}