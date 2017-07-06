using ERP.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.BLL.Consts
{
    public class PurchaseContractHelper
    {
        /// <summary>
        /// 新建采购合同时，绑定采购合同条款
        /// </summary>
        /// <param name="CurrentSign">币种</param>
        /// <param name="AllAmount">总金额</param>
        /// <param name="list_OuterVolume">XXXX的单个外箱箱规不得超过XXXXCBM</param>
        /// <param name="SelectCustomer">选择的客户</param>
        /// <param name="RegisterFees">工厂拉柜费</param>
        /// <param name="str_ProductCopyRight_Factory_No">工厂专有设计的产品</param>
        /// <param name="str_ProductCopyRight_OurCompany_No">我司专有设计的产品</param>
        /// <param name="str_Customs">本合同XXXX产品报关品名XXXX，出运前须办理XXXX。</param>
        /// <param name="str_SumOuterVolume">总体积</param>
        /// <param name="PortName">销售订单的出运港</param>
        /// <param name="IsInnerBoxRate">是否有内盒</param>
        /// <param name="str_ProductPackingMethod">产品包装方式</param>
        /// <param name="list_OuterVolume_S60">XXXX的单个外箱箱规不得超过或小于长宽高/外箱材积</param>
        /// <param name="CustomerCode">客户编号</param>
        /// <param name="list_Inner_S135">内核率只/邮购内盒/内核长X宽X高CM</param>
        /// <param name="list_Outer_S135">外箱率只/外箱/外箱长X宽X高CM</param>
        /// <param name="str_Customs_S10">本合同产品出货前请办理XXXX，报检品名为XXXX，HS编码为XXXX。</param>
        /// <param name="list_OuterVolume_S164">XXXX的单个外箱箱柜不得超过长X宽X高CM/外箱材积'/净重LBS</param>
        /// <returns></returns>
        public static StringBuilder Bind_PurchaseContract(string CurrentSign, decimal AllAmount, List<string> list_OuterVolume, string SelectCustomer, decimal RegisterFees, string str_ProductCopyRight_Factory_No, string str_ProductCopyRight_OurCompany_No, string str_Customs, string str_SumOuterVolume, string PortName, bool IsInnerBoxRate, string str_ProductPackingMethod, List<string> list_OuterVolume_S60, string CustomerCode, List<string> list_Inner_S135, List<string> list_Outer_S135, string str_Customs_S10, List<string> list_OuterVolume_S164)
        {
            string str_PurchaseDate = "";//采购合同下单日期-10天

            StringBuilder sb = new StringBuilder();

            #region S188

            if (SelectCustomer == "S188")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-10));

                sb.Append("1." + str_PurchaseDate + "前提供六套含完整包装的大货签字样，其中一套寄客人美国IC，四套寄客人指定的第三方检测公司，两套寄我司做签字样。待样品确认后，检测公司会将其中一套已确认的样品密封后退回工厂做大货验货用，工厂不可擅自拆开封样。客人要求确认样须含完整包装，若包装排版在打样确认中，则先安排样品，待包装排版确认后再补齐完整的吊卡及PDQ等包装。<br/>");
                sb.Append("签字样要求：有PDQ的货号为2完整PDQ样品 + 4套完整出货吊卡或标签包装的样品 + 4只PDQ；<br/>");
                sb.Append("无PDQ的货号为6套含完整出货吊卡或标签包装的大货签字样。<br/>");

                sb.Append("2.签字样必须同原确认留底样，若因签字样与客人原样不符导致检测公司拒签，重签费USD165 / 货号 / 次及其他一切后果须由工厂承担。<br/>");

                sb.Append("3.所有产品须符合美国法定环保标准，产品及产品表面含铅量不得超过90PPM（儿童产品不得超过40PPM），并能通过客人指定的第三方检测公司的检测。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。其余相关检测要求参见我司另行通知的产品对应的检测标准。<br/>");

                sb.Append("4.所有产品须通过由客人指定的第三方检测公司检测，检测样须由检测公司人员到工厂现场抽检后寄至检测公司，并根据客人的检测标准进行检测。首次抽检及检测的费用由我司承担，如果因工厂的检测样没有通过检测而产生重新抽检、检测、罚款或者订单取消，须由工厂承担全部责任和损失。工厂如需做产品自测，须寄六套自测样到检测公司按客人标准做自测，自测费用须由工厂承担。<br/>");

                sb.Append("5.大货包装数量至少完成20 % 后方可安排抽检，工厂须提前至少一周通知我司可抽检的准确时间。所有检测样需一次性准备好，检测公司现场抽检数量一般为每个货号12套，多余的数量将来可放入大货。<br/>");

                sb.Append("6.所有大货须由客人指定的第三方检测公司及我司验货通过后方可出运。首次验货的费用由我司承担，如果因为验货不通过而产生的重验费及一切责任和损失须由工厂承担。大货生产全部完成，且包装至少完成80 % 后方可申请尾期验货。<br/>");

                sb.Append("7.所有大货质量须与检测样及签字样相同，如果因为产品质量问题而引起任何后果，一切责任和损失须由工厂承担。<br/>");

                sb.Append("8.出货前15天工厂须提供三套含完整包装的出货样给我司，有PDQ的货号，三套为三整套PDQ。两套寄客人，一套我司留底。<br/>");

                sb.Append("9.工厂必须通过由客人指定的第三方验厂公司的WCA验厂，并承担相应的验厂费用。客人接受审核工厂已通过的、有效期在六个月以内的、由BV / ITS / UL / SGS / SERCURA出具给其他客人的WCA验厂报告。若报告审核不通过，产生的重验厂及整改报告审核费用均由工厂承担。<br/>");
                sb.Append("工厂可以选择以XXXX的名义出货，但是需要承担订单金额的2%（即" + (AllAmount * 0.02m) + "元）作为对XXXX工厂的补贴，此费用将从货款中直接扣除。如果工厂使用其他工厂名义出货，工厂须按照要求将指定数量的大货送到指定工厂验货，因此而产生的仓储及装卸等费用按实际收取。由指定工厂转运至港口的运费由工厂承担（如果散货进仓，以实际产生费用为准；如果工厂拖柜，运费按" + RegisterFees + "元/立方收取）。如果由于装柜的需要或者工厂要求运回大货，回程的运费将由工厂自行承担。<br/>");

                sb.Append("10.所有产品的彩色标贴、吊卡等排版需由客人指定的包装设计公司提供，我司收到排版后即通知工厂，工厂收到通知后须先做电脑排版给我司确认，排版确认后一周内须完成样品制作并寄至客人指定的包装设计公司确认，样品确认且第三方检测公司检测通过后，工厂方可大货印刷。<br/>");

                sb.Append("11.所有的彩色标贴、吊卡、PDQ等包装材料需由客人指定的GMI认证工厂生产。如需我司统一代印，相关代印费用及运费均由工厂承担。若因代印打样晚递交或拒签后二次确认样品而产生的任何客人罚款，均需由工厂承担。<br/>");

                sb.Append("12.本合同箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。所有排版须经我司确认并由客人指定第三方检测公司检测通过后，工厂方可开始大货印制。<br/>");

                sb.Append("13.产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。若塑料袋或汽泡袋开口大于5\"，还须打孔。可转运塑料袋内包装需寄样给我司确认后方可大货生产。<br/>");

                sb.Append("14.本合同项下如有易碎产品（如玻璃、陶瓷、树脂等），外箱箱唛须印有易碎标。<br/>");

                sb.Append("15.内盒需要转运安全。外箱须为双层瓦楞纸箱，不可打钉，不可打包装带，须工字型封箱。外箱须通过一点三线六面90CM摔箱测试。安全包装的细节请与我司QC确认。<br/>");

                sb.Append("16." + CommonCode.ListToString(list_OuterVolume) + "，订单总体积不得超过" + str_SumOuterVolume + "。若因体积超出合同要求而产生客人索赔，一切索赔费用及责任须由工厂承担。<br/>");

                sb.Append("17.");
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append("本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append("本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，则由我司安排指定货代拉柜，工厂需退还我司￥" + RegisterFees + "元/立方的拉柜费，提箱费实报实销由我司承担。<br/>");
                }
                sb.Append(str_Customs + "。<br/>");

                sb.Append("18.本合同不允许提前或者延迟出运，否则因此而产生的罚款，订单取消或空运等一切责任将由工厂承担。<br/>");

                sb.Append("19.");
                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    sb.Append("本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    sb.Append("本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No) || !string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    sb.Append("20.");
                }
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append("本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append("本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No) || !string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    sb.Append("21.");
                }
                else
                {
                    sb.Append("20.");
                }
                sb.Append("请于收到合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");
            }

            #endregion S188

            #region S288

            else if (SelectCustomer == "S288")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(+7));//下单日期+7天

                int i = 0;

                i++;
                sb.Append(i + "." + str_PurchaseDate + "前提供一套确认样给我司，我司将作为签样签字并退还给贵司做大货生产依据。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".大货生产及验货必须以我司提供的签字样为准。产品装配时须注意产品的稳定性和牢固性。拆装的产品需要配客人确认的装配说明书。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".所有产品须符合美国法定环保标准，产品及产品表面含铅量不得超过90PPM（儿童产品不得超过40PPM）。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".出货前须提供2套含完整包装的出货样给我司，我司寄给客人确认，待客人确认后方可安排大货包装及出运。产品质量需与签字样相符。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".所有大货须由我司QC验货通过后方可出运。所有大货质量须与签字样相同，如果因为产品质量问题而引起任何后果，一切责任和损失须由工厂承担。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".如果工厂委托我司代印包装材料，费用以与我司合作的印刷厂的报价为准。代印费用由我司直接垫付给印刷厂，并从货款中扣除此代印费用。代印包装材料的运费由工厂自行承担。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".本合同箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。所有排版须经我司确认后，工厂方可开始大货印制。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。若塑料袋或汽泡袋开口大于5英寸，还须打孔。<br />");
                //固定条款8

                i++;
                sb.Append(i + ".本合同项下如有易碎产品（如玻璃、陶瓷、树脂等），外箱箱唛须印有易碎标。<br/>");
                //固定条款9

                i++;
                sb.Append(i + ".外箱须为双层瓦楞纸箱，不可打钉，不可打包装带，须工字型封箱。外箱须通过一点三线六面90CM摔箱测试。安全包装的细节请与我司QC确认。<br/>");
                //固定条款10

                i++;
                sb.Append(i + ".订单总体积不得超过" + str_SumOuterVolume + "。若因体积超出合同要求而导致柜子装不下或者客人索赔，一切责任及后果由工厂承担。<br/>");
                //变动条款1

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，则由我司安排指定货代拉柜，工厂需退还我司￥" + RegisterFees + "元/立方的拉柜费，提箱费实报实销由我司承担。<br/>");
                }

                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在
                 //变动条款2

                i++;
                sb.Append(i + ".本合同不允许提前或者延迟出运，否则因此而产生的罚款、订单取消或空运等一切责任将由工厂承担。客人罚款金额不少于客人定单金额的50%，最低罚款金额为500美金。<br/>");
                //固定条款11

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款3

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款4

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");
            }

            #endregion S288

            #region S60

            else if (SelectCustomer == "S60")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-5));//下单日期-5天
                string str_InnerPK = "";

                int i = 0;
                if (IsInnerBoxRate)
                {
                    str_InnerPK = "和内盒";
                }

                i++;
                sb.Append(i + "." + str_PurchaseDate + "前提供2套产前样给我司，待客人确认后，我司将回签其中一套样品并退还工厂做大货生产及验货依据。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".所有产品须符合美国法定环保标准，产品及产品表面含铅量不得超过90PPM。所有包装材料（吊卡、外箱等）材料上的油墨，UV油、膜等均须使用环保材料，其重金属含量不得超过100PPM。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".每个产品上要挂/贴" + str_ProductPackingMethod + "。需先做" + str_ProductPackingMethod + "排版给客人确认，待排版确认后安排打样张给客人确认，待样张确认后才能大货印制。为确保顺利及时完成包装确认，减少贵司多次打样产生额外费用。我司将统一代做" + str_ProductPackingMethod + "，所产生的费用须由贵司自行承担（内容及费用将另行通知）。印制完成后由我司安排快递到付给贵司。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".外箱" + str_InnerPK + "箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。所有排版须经我司确认后，工厂方可开始大货印制。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。若塑料袋或汽泡袋开口大于5\"，还须打孔。可转运塑料袋内包装需寄样给我司确认后方可大货生产。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".产品包装确认后10个工作日内需提供2套含完整包装的拍照样给客人，客人确认后方可出货。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".所有产品须通过由客人指定的第三方检测公司检测，检测样由工厂提供给客人指定的第三方检测公司根据客人的检测标准进行检测。首次检测的费用由我司承担，如果因工厂的检测样没有通过检测而产生重新检测、罚款和订单取消，须由工厂承担全部责任和损失。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".该客人会根据产品性质安排第三方检测行进行产中抽验。第一次抽验的检测费用由我司承担，贵司须确保抽验检测顺利通过。如果不通过，需安排重测，则贵司承担重测费用。产中抽验的具体要求稍后邮件另行通知。<br/>");
                //固定条款8

                i++;
                sb.Append(i + ".所有外箱必须为双瓦无钉箱，工字型封箱，不得使用打包带。所有外箱都要达到客人要求的纸箱标准：“每平方英寸的承重力不小于275磅”。外箱须过客人六面摔箱测试，摔箱高度为90CM。安全包装细节请与我司QC确认。<br/>");
                //固定条款9

                i++;
                sb.Append(i + "." + CommonCode.ListToString(list_OuterVolume_S60, "；") + "。订单总体积不得超过" + str_SumOuterVolume + "。否则客人有权就超过部分向贵厂索赔海运费差额。<br/>");
                //变动条款1

                i++;
                sb.Append(i + ".所有大货须由我司QC验货通过后方可出运。出货前须提供1套含完整包装的出货样给我司。所有大货质量须与签字样及检测样相同，如果因为产品质量问题而引起任何后果，一切责任和损失须由贵司承担。<br/>");
                //固定条款10

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，工厂需承担￥" + RegisterFees + "元/立方的拉柜费。<br/>");
                }

                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在
                 //变动条款2

                i++;
                sb.Append(i + ".该合同须通过客人的验厂和验货方可出货。<br/>");
                //固定条款11

                i++;
                sb.Append(i + ".该客人要求所有工厂均通过亚检的GSV和WCA验厂。为了节约成本，一个地区只安排一家工厂验厂。产生的验厂费用根据验厂报告有效期年度内实际下单的工厂数量以及订单金额按比例分摊。具体的会邮件另行通知。<br/>");
                //固定条款12

                i++;
                sb.Append(i + ".请按时出货。客人将对延迟出运予以一定的罚款并有权取消订单：<br/>");
                sb.Append("A）延迟出运1-7天，罚款金额不少于客人订单金额的4％。<br/>");
                sb.Append("B）延迟出运8-14天，罚款金额不少于客人订单金额的7％。<br/>");
                sb.Append("C）延迟出运15天或以上，罚款金额不少于客人订单金额的10％。<br/>");
                //固定条款13

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款3

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款4

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");
                //固定条款14
            }

            #endregion S60

            #region DG

            else if (SelectCustomer == "DG")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-5));//下单日期-5天

                string str_InnerPK = "";

                int i = 0;
                if (IsInnerBoxRate)
                {
                    str_InnerPK = "和内核";
                }

                i++;
                sb.Append(i + ".大货生产及验货必须以我司提供的签字样为准。产品装配时须注意产品的稳定性和牢固性。拆装的产品需要配客人确认的装配说明书。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".工厂在生产前需安排一套样品寄至客人指定的第三方检测公司做PPR（即产前产品审核），PPR费用由工厂承担。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".工厂在生产前须安排三套样品寄至客人指定的第三方检测公司按照DG的检测标准做自测，自测费用由工厂自行承担。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".工厂须提供一套SMSB拍照样，二套POG销售样给客人，具体时间及包装要求我司将另行邮件通知。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".所有产品须符合美国法定环保标准，产品及产品表面含铅量不得超过50PPM（儿童产品不得超过40PPM），并能通过客人指定的第三方检测公司的检测。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。其余相关检测要求参见我司另行通知的产品对应的检测标准。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".所有产品须通过由客人指定的第三方检测公司检测，检测样须由检测公司人员到工厂现场抽检后寄至检测公司，并根据客人的检测标准进行检测。首次抽检及检测的费用由我司承担，如果因工厂的检测样没有通过检测而产生重新抽检、检测、罚款或者订单取消，须由工厂承担全部责任和损失。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".大货包装至少完成15%后方可安排中期抽检，工厂须提前至少一周通知我司可抽检的准确时间。中期抽检时将抽取如下样品并现场封样，除客人验货人员外任何人不得擅自拆封：<br/>");
                sb.Append("12套测试样，寄至第三方检测公司做检测<br/>");
                sb.Append("1套验货样，做为尾期验货的标准<br/>");
                sb.Append("2套大货样，寄给客人与之前的产前确认样对比，如果完全一致，则大货样确认OK；如果跟确认样不一样，工厂须修改大货，重新做抽检申请。重新抽检时仍需抽测试样、验货样及大货样并现场封样。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".所有大货须由客人指定的第三方检测公司及我司验货通过后方可出运。首次验货的费用由我司承担，如果因为验货不通过而产生的重验费及一切责任和损失须由工厂承担。大货生产全部完成，且包装至少完成80%后方可申请尾期验货。<br/>");
                //固定条款8

                i++;
                sb.Append(i + ".所有大货质量须与签字样及检测样相同，如果因为产品质量问题而引起任何后果，一切责任和损失须由工厂承担。<br/>");
                //固定条款9

                i++;
                sb.Append(i + ".出货前须提供一整箱含完整包装的出货样给我司，产品质量需与签字样相符。<br/>");
                //固定条款10

                i++;
                sb.Append(i + ".工厂必须通过由客人指定的第三方验厂公司的WCA及SQP验厂。SQP验厂的分数要求不低于71分，如果在85分或以上，则SQP验厂结果永久有效，SQP验厂及重验的费用均由工厂承担。WCA验厂的分数要求不低于85分，有效期为一年，如果首次验厂的分数达到85分或以上，验厂费由我司承担，如果分数在85以下，需要整改后重新验厂，首次验厂及重验厂的费用均由工厂承担。自2013年11月1日开始，不再由供应商向验厂公司提前申请具体的验厂时间，验厂公司会在上次年审的有效期前65天之内的任意时间随时到访工厂验厂，工厂需提前做好随时被验厂准备。<br/>");
                sb.Append("工厂可以选择以XXXX的名义出货，但是需要承担订单金额的2%（即" + (AllAmount * 0.02m) + "元）作为对XXXX工厂的补贴，此费用将从货款中直接扣除。如果工厂使用其他工厂名义出货，工厂须按照要求将指定数量的大货送到指定工厂验货，因此而产生的仓储及装卸等费用按实际收取。由指定工厂转运至港口的运费由工厂承担（如果散货进仓，以实际产生费用为准；如果工厂拖柜，运费按" + RegisterFees + "元/立方收取）。如果由于装柜的需要或者工厂要求运回大货，回程的运费将由工厂自行承担。<br/>");
                //固定条款11

                i++;
                sb.Append(i + ".所有产品的彩色吊卡、PDQ等排版需由客人指定的包装设计公司提供，工厂提供刀线图给我司，我司安排设计公司排版，完成之后发给工厂确认。第一轮图稿确认之后将不允许做任何更改，否则产生的费用由工厂承担。在收到设计公司正式图稿及光盘后工厂须马上打样并寄样张给设计公司确认。供应商共有两次机会确认样张，如果第二次还没有被确认，设计公司将要求到工厂现场指导印刷，因此产生的费用由工厂承担。样张确认后工厂方可大货印刷。<br/>");
                //固定条款12

                i++;
                sb.Append(i + ".如果工厂委托我司代印包装材料，费用以与我司合作的印刷厂的报价为准。代印费用由我司直接垫付给印刷厂，并从货款中扣除此代印费用。代印包装材料的运费由工厂自行承担。<br/>");
                //固定条款13

                i++;
                sb.Append(i + ".本合同箱唛及相关包装要求以我司邮件通知的包装资料为准。包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。所有排版须经我司确认后，工厂方可开始大货印制。<br/>");
                //固定条款14

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。若塑料袋或汽泡袋开口大于5\"，还须打孔。可转运塑料袋内包装需寄样给我司确认后方可大货生产。<br/>");
                //固定条款15

                i++;
                sb.Append(i + ".本合同项下如有易碎产品（如玻璃、陶瓷、树脂等），外箱箱唛须印有易碎标。<br/>");
                //固定条款16

                i++;
                sb.Append(i + ".PDQ材质最低要求为250g灰底铜板卡+B6瓦楞纸板。内盒需要转运安全。外箱须为双层瓦楞纸箱，不可打钉，不可打包装带，须工字型封箱。外箱须通过一点三线六面90CM摔箱测试，且能通过DG客人要求的200磅压力测试。安全包装的细节请与我司QC确认。<br/>");
                //固定条款17

                i++;
                sb.Append(i + "." + CommonCode.ListToString(list_OuterVolume_S60, "；") + "。订单总体积不得超过" + str_SumOuterVolume + "。若因体积超出合同要求而产生客人索赔，一切索赔费用及责任须由工厂承担。<br/>");
                //变动条款1

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，工厂需承担￥" + RegisterFees + "元/立方的拉柜费。<br/>");
                }

                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在,是否需要报检和报检品名的条款
                //变动条款2

                i++;
                sb.Append(i + ".本合同不允许提前或者延迟出运，否则因此而产生的罚款，订单取消或空运等一切责任将由工厂承担。<br/>");
                //固定条款18

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款3

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款4

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");
                //固定条款14
            }

            #endregion DG

            #region S05 + S235

            else if (SelectCustomer == "S05" || SelectCustomer == "S235")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(+5));//下单日期-5天
                string str_InnerPK = "";

                int i = 0;
                if (IsInnerBoxRate)
                {
                    str_InnerPK = "内核需转运安全。";
                }

                i++;
                sb.Append(i + ".请于" + str_PurchaseDate + "前提供2套产前样给我司，待客人确认后，我司将回签其中一套样品并退还工厂做大货生产及验货依据。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".大货生产及验货必须以我司提供的签字样为准。产品装配时须注意产品的稳定性和牢固性。拆装的产品需要配客人确认的装配说明书。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".所有产品须符合美国法定环保标准，产品及产品表面含铅量不得超过90PPM（儿童产品不得超过40PPM）。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。其余相关检测要求参见我司另行通知的产品对应的检测标准。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".出货前两周需提供产品详细包装照片给客人，待客人确认后方可出货。<br/>");
                //固定条款4

                i++;
                int x = 2;
                sb.Append(i + ".出货前须提供" + x + "套含完整包装的出货样给我司，产品质量需与签字样相符。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".所有大货须由我司QC验货通过后方可出运。所有大货质量须与签字样及检测样相同，如果因为产品质量问题而引起任何后果，一切责任和损失须由工厂承担。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".如果工厂委托我司代印包装材料，费用以与我司合作的印刷厂的报价为准。代印费用由我司直接垫付给印刷厂，并从货款中扣除此代印费用。代印包装材料的运费由工厂自行承担。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".本合同箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。所有排版须经我司确认后，工厂方可开始大货印制。<br/>");
                //固定条款9

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。若塑料袋或汽泡袋开口大于5\"，还须打孔。可转运塑料袋内包装需寄样给我司确认后方可大货生产。<br/>");
                //固定条款10

                i++;
                sb.Append(i + ".本合同项下如有易碎产品（如玻璃、陶瓷、树脂等），外箱箱唛须印有易碎标。<br/>");
                //固定条款11

                i++;
                sb.Append(i + "." + str_InnerPK + "外箱须为双层瓦楞纸箱，不可打钉，不可打包装带，须工字型封箱。外箱须通过一点三线六面90CM摔箱测试。安全包装的细节请与我司QC确认。<br/>");
                //固定条款12

                i++;
                sb.Append(i + "." + CommonCode.ListToString(list_OuterVolume_S60, "；") + "。订单总体积不得超过" + str_SumOuterVolume + "。若因体积超出合同要求而导致柜子装不下或者客人索赔，一切责任及后果由工厂承担。<br/>");
                //变动条款1

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，工厂需承担￥" + RegisterFees + "元/立方的拉柜费。<br/>");
                }

                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在
                //变动条款2

                i++;
                sb.Append(i + ".本合同不允许提前或者延迟出运，否则因此而产生的罚款，订单取消或空运等一切责任将由工厂承担。<br/>");
                //固定条款13

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款3

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款4

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");
                //固定条款14
            }

            #endregion S05 + S235

            #region S13

            else if (SelectCustomer == "S13")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(+5));//下单日期-5天
                string str_InnerPK = "";

                int i = 0;
                if (IsInnerBoxRate)
                {
                    str_InnerPK = "内核需转运安全。";
                }

                i++;
                sb.Append(i + ".请在下单后一周内提供2套产前样给我司，待客人确认后，我司回签一套样品并退还工厂做大货生产及验货依据。客人确认样品之后工厂才可以安排大货生产，否则由此引起的一切及后果将由工厂自行承担。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".客人确认产前样后一周内根据我司邮件通知的检测要求准备相应数量的检测样，工厂必须保证样品能够一次性通过测试。如果翻单产品有不通过测试的记录，或者新产品测试不通过，工厂必须就不通过项目先行自测，通过之后方可进行正式测试。首次检测的费用由我司承担，因测试不通过而导至的自测和重测费用由工厂承担，自测和重测的费用由我司代付，并会在货款中扣除。工厂必须确保大货与测试样品质相同，且都能通过测试，并符合美国法律及法规的要求。工厂必须在测试通过之后方可安排大货生产，否则由此而造成的任何损失及后果，将由工厂自行承担。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".请在测试通过之后一周内提供一套与签样及测试样相同颜色效果及质量相同的大货样寄给客人，并确保大货样可以一次性被确认，否则由此引起的任何后责任及后果将由工厂承担。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".大货生产及验货必须以我司提供的签字样为准。产品装配时须注意产品的稳定性和牢固性。拆装的产品需要配客人确认的装配说明书。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".所有产品须符合美国法定环保标准，产品及产品表面含铅量不得超过90PPM（儿童产品不得超过40PPM）。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。其余相关检测要求参见我司另行通知的产品对应的检测标准。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".出货前两周需提供产品详细包装照片给客人，待客人确认后方可出货。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".出货前须提供1套含完整包装的出货样给我司，产品质量需与签字样相符。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".所有大货须由我司QC验货通过后方可出运。所有大货质量须与签字样及检测样相同，如果因为产品质量问题而引起任何后果，一切责任和损失须由工厂承担。<br/>");
                //固定条款8

                i++;
                sb.Append(i + ".如果工厂委托我司代印包装材料，费用以与我司合作的印刷厂的报价为准。代印费用由我司直接垫付给印刷厂，并从货款中扣除此代印费用。代印包装材料的运费由工厂自行承担。<br/>");
                //固定条款9

                i++;
                sb.Append(i + ".本合同箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。所有排版须经我司确认后，工厂方可开始大货印制。<br/>");
                //固定条款10

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。若塑料袋或汽泡袋开口大于5\"，还须打孔。可转运塑料袋内包装需寄样给我司确认后方可大货生产。<br/>");
                //固定条款11

                i++;
                sb.Append(i + ".本合同项下如有易碎产品（如玻璃、陶瓷、树脂等），外箱箱唛须印有易碎标。<br/>");
                //固定条款12

                i++;
                sb.Append(i + "." + str_InnerPK + "外箱须为双层瓦楞纸箱，不可打钉，不可打包装带，须工字型封箱。外箱须通过一点三线六面90CM摔箱测试。安全包装的细节请与我司QC确认。<br/>");
                //固定条款13

                i++;
                sb.Append(i + "." + CommonCode.ListToString(list_OuterVolume_S60, "；") + "。订单总体积不得超过" + str_SumOuterVolume + "。若因体积超出合同要求而导致柜子装不下或者客人索赔，一切责任及后果由工厂承担。<br/>");
                //变动条款14

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，工厂需承担￥" + RegisterFees + "元/立方的拉柜费。<br/>");
                }
                //变动条款15

                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在
                //变动条款16

                i++;
                sb.Append(i + ".本合同不允许提前或者延迟出运，否则因此而产生的罚款，订单取消或空运等一切责任将由工厂承担。<br/>");
                //固定条款17

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款18

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款19

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");
                //固定条款20
            }

            #endregion S13

            #region S220

            else if (SelectCustomer == "S220")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-5));//下单日期-5天
                string str_InnerPK = "";

                int i = 0;
                if (IsInnerBoxRate)
                {
                    str_InnerPK = "内核需转运安全。";
                }

                i++;
                sb.Append(i + ".请于" + str_PurchaseDate + "前提供2套含完整包装的产前样给我司，待客人确认后，我司将回签其中一套样品并退还工厂做大货生产及验货依据。大货生产及验货必须以我司提供的签字样为准。产品装配时须注意产品的稳定性和牢固性。拆装的产品需要配客人确认的装配说明书。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".所有产品须符合加拿大法定环保标准，产品及产品表面含铅量不得超过90PPM（儿童产品不得超过40PPM）。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。其余相关检测要求参见我司另行通知的产品对应的检测标准。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".所有产品须通过由客人指定的第三方检测公司检测，检测样由工厂提供给客人指定的第三方检测公司根据客人的检测标准进行检测。首次检测的费用由我司承担，如果因工厂的检测样没有通过检测而产生重新检测、罚款或者订单取消，须由工厂承担全部责任和损失。工厂如需做产品自测，须寄六套自测样到检测公司按客人标准做自测，自测费用须由工厂承担。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".大货包装数量至少完成80%后方可安排尾期验货，工厂须提前至少一周告知我司可验货的准确时间。我司QC人员将在大货中抽取样品做签样，提供给检测公司做验货依据。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".所有大货须由客人指定的第三方检测公司及我司验货通过后方可出运。首次验货的费用由我司承担，如果因为验货不通过而产生的重验费及一切责任和损失须由工厂承担。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".所有大货质量须与签字样及检测样相同，如果因为产品质量问题而引起任何后果，一切责任须由工厂承担。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".出货前须提供一整箱含完整包装的出货样给我司，产品质量需与签字样相符。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".工厂必须通过由客人指定的第三方验厂公司的WCA验厂，并承担相应的验厂费用。客人接受工厂已通过的、在出货时仍在有效期的、由ITS出具给其他客人的WCA验厂报告。<br/>");
                sb.Append(".工厂可以选择以XXXX的名义出货，但是需要承担订单金额的2%（即" + (AllAmount * 0.02m) + "元）作为对XXXX工厂的补贴，此费用将从货款中直接扣除。如果工厂使用其他工厂名义出货，工厂须按照要求将指定数量的大货送到指定工厂验货，因此而产生的仓储及装卸等费用按实际收取。由指定工厂转运至港口的运费由工厂承担（如果散货进仓，以实际产生费用为准；如果工厂拖柜，运费按" + RegisterFees + "元/立方收取）。如果由于装柜的需要或者工厂要求运回大货，回程的运费将由工厂自行承担。<br/>");
                //固定条款8

                i++;
                sb.Append(i + ".如果工厂委托我司代印包装材料，费用以与我司合作的印刷厂的报价为准。代印费用由我司直接垫付给印刷厂，并从货款中扣除此代印费用。代印包装材料的运费由工厂自行承担。<br/>");
                //固定条款9

                i++;
                sb.Append(i + ".本合同箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。所有排版须经我司确认后，工厂方可开始大货印制。<br/>");
                //固定条款10

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。若塑料袋或汽泡袋开口大于5\"，还须打孔。可转运塑料袋内包装需寄样给我司确认后方可大货生产。<br/>");
                //固定条款11

                i++;
                sb.Append(i + ".本合同项下如有易碎产品（如玻璃、陶瓷、树脂等），外箱箱唛须印有易碎标。<br/>");
                //固定条款12

                i++;
                sb.Append(i + "." + str_InnerPK + "外箱须为双层瓦楞纸箱，不可打钉，不可打包装带，须工字型封箱。外箱须通过一点三线六面90CM摔箱测试。安全包装的细节请与我司QC确认。<br/>");
                //固定条款13

                i++;
                sb.Append(i + "." + CommonCode.ListToString(list_OuterVolume_S60, "；") + "。订单总体积不得超过" + str_SumOuterVolume + "。若因体积超出合同要求而导致柜子装不下或者客人索赔，一切责任及后果由工厂承担。<br/>");
                //变动条款1

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，工厂需承担￥" + RegisterFees + "元/立方的拉柜费。<br/>");
                }

                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在
                //变动条款2

                i++;
                sb.Append(i + ".本合同不允许提前或者延迟出运，否则因此而产生的罚款，订单取消或空运等一切责任将由工厂承担。<br/>");
                //固定条款13

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款3

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款4

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");
                //固定条款14
            }

            #endregion S220

            #region F20

            else if (SelectCustomer == "F20")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-5));//下单日期-5天
                string str_InnerPK = "";

                int i = 0;
                if (IsInnerBoxRate)
                {
                    str_InnerPK = "内核需转运安全。";
                }

                i++;
                sb.Append(i + "产前样、产中样的寄样时间及数量以我司邮件通知为准。客人会回签一套确认的产前样给工厂做为生产及第三方验货公司的验货依据。产前样、产中样和大货的尺寸、颜色、质量等必须一致，如果因为样品的质量问题引起延期交货及客人索赔等一切后果，均由工厂承担。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".所有产品须符合美国法定环保标准，产品及产品表面含铅量不得超过50PPM（儿童产品不得超过40PPM），并能通过客人指定的第三方检测公司的检测。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。其余相关检测要求参见我司另行通知的产品对应的检测标准。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".所有产品须通过由客人指定的第三方检测公司检测，检测样须由检测公司人员到工厂现场抽检后寄至检测公司，并根据客人的检测标准进行检测。首次抽检及检测的费用由我司承担，如果因工厂的检测样没有通过检测而产生重新抽检、检测、罚款或者订单取消，须由工厂承担全部责任和损失。工厂如需做产品自测，须寄六套自测样到检测公司按客人标准做自测，自测费用须由工厂承担。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".大货包装数量至少完成15%后方可安排抽检及中期验货，工厂须提前至少一周通知我司可抽检的准确时间。所有检测样需一次性准备好，检测公司现场抽检数量一般为每个货号六套。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".所有大货须由客人指定的第三方检测公司及我司验货通过后方可出运。首次验货的费用由我司承担，如果因为验货不通过而产生的重验费及一切责任和损失须由工厂承担。  <br/>");
                //固定条款5

                i++;
                sb.Append(i + ".所有大货质量须与签字样及检测样相同，如果因为产品质量问题而引起任何后果，一切责任须由工厂承担。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".出货前须提供一整箱含完整包装的出货样给我司，产品质量需与签字样相符。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".工厂必须通过由客人指定的第三方验厂公司的WCA及SQP验厂，并承担相应的验厂费用。<br/>");
                sb.Append(".工厂可以选择以XXXX的名义出货，但是需要承担订单金额的2%（即" + (AllAmount * 0.02m) + "元）作为对XXXX工厂的补贴，此费用将从货款中直接扣除。如果工厂使用其他工厂名义出货，工厂须按照要求将指定数量的大货送到指定工厂验货，因此而产生的仓储及装卸等费用按实际收取。由指定工厂转运至港口的运费由工厂承担（如果散货进仓，以实际产生费用为准；如果工厂拖柜，运费按" + RegisterFees + "元/立方收取）。如果由于装柜的需要或者工厂要求运回大货，回程的运费将由工厂自行承担。<br/>");
                //固定条款8

                i++;
                sb.Append(i + ".大货包装的XXXX排版由我司提供，经客人确认后提供给工厂可以直接印刷的文件，工厂在收到文件后五天内须完成样品，待客人确认样品后方可开始大货印制。大货的XXXX质量必须和客人确认的样品一致，否则因此而引起的一切后果均由工厂承担。<br/>");
                //固定条款9

                i++;
                sb.Append(i + ".如果工厂委托我司代印包装材料，费用以与我司合作的印刷厂的报价为准。代印费用由我司直接垫付给印刷厂，并从货款中扣除此代印费用。代印包装材料的运费由工厂自行承担。<br/>");
                //固定条款10

                i++;
                sb.Append(i + ".本合同箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。所有排版须经我司及客户确认后，工厂方可开始大货印制。<br/>");
                //固定条款11

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。若塑料袋或汽泡袋开口大于5\"，还须打孔。可转运塑料袋内包装需寄样给我司确认后方可大货生产。<br/>");
                //固定条款12

                i++;
                sb.Append(i + ".本合同项下如有易碎产品（如玻璃、陶瓷、树脂等），外箱箱唛须印有易碎标。<br/>");
                //固定条款13

                i++;
                sb.Append(i + "." + str_InnerPK + "外箱须为双层瓦楞纸箱，不可打钉，不可打包装带，须工字型封箱。外箱须通过一点三线六面90CM摔箱测试。安全包装的细节请与我司QC确认。<br/>");
                //固定条款14

                i++;
                sb.Append(i + "." + CommonCode.ListToString(list_OuterVolume_S60, "；") + "。订单总体积不得超过" + str_SumOuterVolume + "。若因体积超出合同要求而导致柜子装不下或者客人索赔，一切责任及后果由工厂承担。<br/>");
                //变动条款1

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，工厂需承担￥" + RegisterFees + "元/立方的拉柜费。<br/>");
                }

                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在
                //变动条款2

                i++;
                sb.Append(i + ".本合同不允许提前或者延迟出运，否则因此而产生的罚款，订单取消或空运等一切责任将由工厂承担。<br/>");
                //固定条款15

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款3

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款4

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");
                //固定条款16
            }

            #endregion F20

            #region S135

            else if (SelectCustomer == "S135")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-14));//下单日期-14天
                string str_InnerPK = "";

                int i = 0;
                if (IsInnerBoxRate)
                {
                    str_InnerPK = "内核需转运安全。";
                }

                i++;
                sb.Append(i + ".翻单货号，请依据今年出货样或签字样做验货依据。产品装配时须注意产品的稳定性和牢固性。如不是翻单货号，此条款为：1.请于" + str_PurchaseDate + "前提供2套含完整包装的产前样给我司，其中一套我司将签字并退还工厂做大货生产及验货依据。大货生产及验货必须以我司提供的签字样为准。产品装配时须注意产品的稳定性和牢固性。拆装的产品需要配客人确认的装配说明书。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".标签排版给我司确认，要求亚光表面，到达A级扫描标准，贴于邮购内盒正面正中间位置。箱唛内容8/10前通知给贵厂，请先发排版确认，客人确认OK后方可大货印刷。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".所有产品须符合美国法定环保标准，产品及产品表面含铅量不得超过90PPM（儿童产品不得超过40PPM）。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。其余相关检测要求参见我司另行通知的产品对应的检测标准。所有产品须通过由客人指定的第三方检测公司检测，检测样由工厂提供给客人指定的第三方检测公司根据客人的检测标准进行检测。首次检测的费用由我司承担，如果因工厂的检测样没有通过检测而产生重新检测、罚款和订单取消，须由工厂承担全部责任和损失。工厂如需做产品自测，须寄自测样到检测公司按客人标准做自测，自测费用须由工厂承担。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".出货前两周需提供产品详细包装照片给客人，待客人确认后方可出货。出货前须提供一套含完整包装的出货样给我司，产品质量需与签字样相符。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".所有大货须由我司QC验货通过后方可出运。所有大货质量须与检测样及签字样相同，如果因为产品质量问题而引起任何后果，一切责任和损失须由工厂承担。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".本合同箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。所有排版须经我司确认后，工厂方可开始大货印制。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，若开口大于5\"须打孔，且须印有或贴有客人指定内容的警告语，警告语内容参见包装资料要求，排版须经我司确认后方可大货印刷。可转运塑料袋内包装需寄样给我司确认后方可大货生产。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".外箱须为双层瓦楞纸箱，邮购内盒和外箱均须 \"工\" 字型封箱不可打钉，不可打包装带。内盒和外箱须通过一点三线六面90CM摔箱测试。邮购内盒需要转运安全，安全包装的细节请与我司QC确认。<br/>");
                //固定条款8

                //9）内核率只/邮购内盒/内核长X宽X高CM，实际尺寸不得超过该尺寸，否则客人有权就超过部分向贵厂索赔美国境内快递费差额。
                //外箱率只/外箱/外箱长X宽X高CM, 订单总体积不得超过99.01M3，否则客人有权就超过部分向贵厂索赔海运费差额。

                i++;
                sb.Append(i + "." + CommonCode.ListToString(list_Inner_S135, "/") + "。实际尺寸不得超过该尺寸，否则客人有权就超过部分向贵厂索赔美国境内快递费差额。<br/>");
                sb.Append(CommonCode.ListToString(list_Outer_S135, "/") + "。订单总体积不得超过" + str_SumOuterVolume + "。否则客人有权就超过部分向贵厂索赔海运费差额。<br/>");

                //变动条款1
                //此处两个变量需要重新传值。内核率的值和外箱率的值，以及内核外箱的长宽高

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，工厂需承担￥" + RegisterFees + "元/立方的拉柜费。<br/>");
                }

                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在
                //变动条款2

                i++;
                sb.Append(i + ".本合同不允许提前或者延迟出运，否则因此而产生的罚款，订单取消或空运等一切责任将由工厂承担。<br/>");
                //固定条款15

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款3

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款4

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");
                //固定条款16
            }

            #endregion S135

            #region S135T

            //S135客户试单
            else if (CustomerCode == "S135T")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-5));//下单日期-5天
                string str_InnerPK = "";

                int i = 0;
                if (IsInnerBoxRate)
                {
                    str_InnerPK = "内核需转运安全。";
                }

                i++;
                sb.Append(i + "我司黄岩办会将之前留底样（含邮购内盒）回签给贵厂做验货依据。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".标签和箱唛内容" + str_PurchaseDate + "前通知给贵厂，贵厂需先发排版确认，我司确认后方可大货印刷。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".单个产品需贴有“MADE IN CHINA”产地标。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".质量要求依据我司签字样，产品装配时须注意产品的稳定性和牢固性。安全包装的细节请与我司黄岩办确认。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".纸箱均为双瓦质量。邮购内盒和外箱均须 \"工\" 字型封箱，不得使用打包带，不可以打钉。验货必须通过平臂90CM高，6面3线1点的安全试摔。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".产品及表面涂层含铅量不得超过90PPM。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".验货通过后，出货前须免费提供一套出货样给我司备案，产品质量须与签字样相符。如因产品质量问题造成客户索赔，一切费用和责任由贵厂承担。<br/>");
                //固定条款7

                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在
                //变动条款2

                i++;
                sb.Append(i + ".请按时出货。若因贵厂原因延期出货而产生的一切费用和责任，由贵厂承担。<br/>");
                //固定条款8

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，工厂需承担￥" + RegisterFees + "元/立方的拉柜费。<br/>");
                }

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款3

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款4

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");
                //固定条款16
            }

            #endregion S135T

            #region S10

            else if (SelectCustomer == "S10")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-5));//下单日期-5天

                int i = 0;

                i++;
                sb.Append(i + ".请于" + str_PurchaseDate + "前提供1套确认样给我司，我司将签字并退还给贵司做大货生产和验货的依据。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".所有产品须符合美国法定环保标准，产品及产品表面含铅量不得超过90PPM（儿童产品不得超过40PPM）。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".如果工厂委托我司代印包装材料，费用以与我司合作的印刷厂的报价为准。代印费用由我司直接垫付给印刷厂，贵司须退还我司。代印包装材料的运费由工厂自行承担。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。若塑料袋或汽泡袋开口大于5，还须打孔。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".本合同项下如有易碎产品（如玻璃、陶瓷、树脂等），外箱箱唛须印有易碎标。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".本合同箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。 所有排版须经我司确认后，工厂方可开始大货印制。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".外箱须为双层瓦楞纸箱，不可打钉，不可使用打包带，须工字型封箱。外箱须通过一点三线六面60-90CM摔箱测试。安全包装的细节请与我司QC确认。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".订单总体积不得超过" + str_SumOuterVolume + "。若因体积超出合同要求而导致柜子装不下或者客人索赔，一切责任及后果由工厂承担。<br/>");
                //固定条款8

                i++;
                sb.Append(i + ".出货前需将产品详细包装照片电邮我司，待客人确认后方能大货出运。同时需提供一套含完整包装的出货样给我司，产品质量需与签字样相符，若因质量问题造成客户索赔，一切费用和责任由贵厂承担。<br/>");
                //固定条款9

                i++;
                sb.Append(i + ".本合同不允许提前或者延迟出运，否则因此而产生的罚款、订单取消或空运等一切责任和费用将由工厂承担。<br/>");
                //固定条款10

                if (!string.IsNullOrEmpty(str_Customs_S10))
                {
                    i++;
                    sb.Append(i + "." + str_Customs_S10 + "。<br/>");
                }
                //变动条款11

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同为货交" + PortName + "仓库价格，如果将来改为拉柜，贵厂须按每立方米￥" + RegisterFees + "元退还运费给我司。<br/>");
                }
                //变动条款12

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，贵厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款13

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".付款方式为XXXX天内，由JOY KEY(HONG KONG) LTD. 全额付清。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为含税价，付款方式为XXXX天左右提供有效的增值税发票，由我司安排付款。<br/>");
                }
                //变动条款14

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021-61183771，否则视为自动放弃。<br/>");
                //固定条款15
            }

            #endregion S10

            #region S52

            else if (SelectCustomer == "S52")
            {
                int i = 0;

                i++;
                sb.Append(i + ".提供两套确认样，一套寄客人确认，另一套待客人确认后由我司签字退还贵厂做大货生产依据。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".所有产品都要符合美国法定环保标准，产品表面油漆含铅量不得超过90PPM。<br/>");
                //固定条款2
                //
                i++;
                sb.Append(i + ".每只产品需挂/贴" + str_ProductPackingMethod + "和产地标。" + str_ProductPackingMethod + "由我司代印，价标由客人提供，代印费用由贵厂承担。 产地标由贵厂自行负责。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".所有外箱需贴外箱标，外箱标内容以及贴的位置要求稍后通知，贵司需先做排版给我司确认，我司确认后方可大批量印制和大货包装。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".所有产品包装（" + str_ProductPackingMethod + "、产地标、外箱等）材料上的油墨，UV油、膜等均须使用环保材料，其重金属含量不得超过100PPM。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".所有" + str_ProductPackingMethod + "、外箱标的内容及贴/挂位置都需正确，所有条码都能扫描正确，否则将会引起客人严重罚款。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".客人要求所有的产品需通过美国标准的专业检测。首次检测费用由我司承担，贵厂须保证检测样能顺利通过检测。如果未获通过，需重测，则贵厂承担重测费（重测费依据上海ITS实验室 的收费标准来执行）所有产品须经ITS检测合格后大货方可出运。所有大货质量须与检测样相同。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，塑料袋或气泡袋上均要印有或贴有常规警告语，待我司确认警告语内容后方可大货印制。<br/>");
                //固定条款8

                i++;
                sb.Append(i + ".所有外箱必须为双瓦无钉箱，工字型封箱，不得使用打包带。所有外箱都要达到客人要求的纸箱标准：“每平方英寸的承重力不小于275磅”。内盒和外箱须过客人六面摔箱测试，摔箱高度为90CM。合同总体积不得超过或小于" + str_SumOuterVolume + "。否则客人有权就超过部分向贵厂索赔海运费差额，安全包装细节请与我司黄岩办确认。<br/>");
                //固定条款9

                i++;
                sb.Append(i + ".出货前须提供一套含完整包装的出货样给我司。产品质量须与确认样相符，若因质量问题造成客户索赔，须由贵厂负责。<br/>");
                //固定条款10

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".该合同为FOB" + PortName + "港的价格。贵司负责报关和出运。<br/>");
                }
                else
                {
                    sb.Append(i + ".该合同为货交" + PortName + "港仓库的价格。<br/>");
                }
                //变动条款11

                i++;
                sb.Append(i + ".请按时出货，客人将对延迟出运予以罚款并有权取消定单。<br/>");
                //固定条款12

                i++;
                sb.Append(i + ".该客人要求所有工厂均通过GSV和WCA验厂。为了节约成本，一个地区只安排一家工厂验厂。产生的验厂费用根据 验厂报告有效期年度内实际下单的工厂数量以及订单金额按比例分摊。具体的会邮件另行通知。<br/>");
                //固定条款13

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，贵厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款14

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".付款方式为XXXX天内，由JOY KEY(HONG KONG) LTD. 全额付清。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为含税价，付款方式为XXXX天左右提供有效的增值税发票，由我司安排付款。<br/>");
                }
                //变动条款16

                i++;
                sb.Append(i + ".请于收到该合同三个工作日内签字盖公章回传至021-61183771，否则视为自动放弃。<br/>");
                //固定条款16
            }

            #endregion S52

            #region S56

            else if (SelectCustomer == "S56")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-5));//下单日期-5天

                int i = 0;

                i++;
                sb.Append(i + ".请于" + str_PurchaseDate + "前提供一套确认样给我司，我司将做签样并退还给贵司做为大货生产和验货的依据。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".所有产品都要符合美国法定环保标准，产品及产品表面含铅量不得超过90PPM。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".每只产品必须先安全包装好后再入内盒，要避免产品之间相互接触、摩擦或者划伤。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".所有产品都需挂/贴" + str_ProductPackingMethod + "，" + str_ProductPackingMethod + "位置必须统一。我司将统一代做" + str_ProductPackingMethod + "所产生的费用由贵厂承担（内容及费用将另行通知）。印制完成后贵厂可去我司黄岩办自提，或由我司安排快递到付给贵厂。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".所有内盒和外箱上都印唛头，且外箱上都需贴外箱标，内盒唛头、外箱唛头和外箱标内容我司稍后通知，贵厂需做电脑排版电邮给我司确认，待我司确认电脑排版后方可安排大货印制。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".所有" + str_ProductPackingMethod + "、外箱标的内容及贴/挂位置都需正确，所有条码都能扫描正确，否则将会引起客人严重罚款。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".请于出货前30天提供两套出货样，我司寄给客人确认，待客人确认后方可出运。产品质量须与确认样相符，若因质量问题造成客户索赔，须由贵厂负责。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，塑料袋或气泡袋上均要印有或贴有常规警告语，待我司确认警告语内容后方可大货印制。<br/>");
                //固定条款8

                i++;
                sb.Append(i + ".所有外箱必须为双瓦无钉纸箱，工字型封箱，不得使用打包带。所有外箱都要达到客人要求的纸箱标准：“每平方英寸的承重力不小于275磅”。内盒和外箱须过客人六面摔箱测试，摔箱高度为60-90CM。订单总体积不得超过" + str_SumOuterVolume + "，否则客人有权就超过部分向贵司索赔海运费差额，具体包装细节请与我司QC确认。<br/>");
                //固定条款9

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同为货交" + PortName + "仓库价格，如果将来改为拉柜，贵厂须按每立方米￥" + RegisterFees + "元退还运费给我司。<br/>");
                }
                //固定条款10

                if (!string.IsNullOrEmpty(str_Customs_S10))
                {
                    i++;
                    sb.Append(i + "." + str_Customs_S10 + "。<br/>");
                }
                //变动条款11

                i++;
                sb.Append(i + ".请按时出货，客人将对延迟出运予以罚款并有权取消定单，罚款金额不少于客人定单金额的50%。<br/>");
                //固定条款12

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，贵厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而产生任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款13

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".付款方式为XXXX天内，由JOY KEY(HONG KONG) LTD. 全额付清。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为含税价，付款方式为XXXX天左右提供有效的增值税发票，由我司安排付款。<br/>");
                }
                //变动条款14

                i++;
                sb.Append(i + ".请收到本合同3个工作日内签字盖公章并回传至021-61183771，否则将视作自动放弃。<br/>");
                //固定条款15
            }

            #endregion S56

            #region S164

            else if (SelectCustomer == "S164")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-5));//下单日期-5天

                int i = 0;

                i++;
                sb.Append(i + ".请于" + str_PurchaseDate + "前提供一套确认样给我司黄岩办确认并签字后退还贵厂做生产依据。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".所有产品都要符合美国法定环保标准，产品基层及产品表面油漆含铅量不得超过90PPM。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".每只产品都需挂" + str_ProductPackingMethod + "，每个货号的吊卡需挂于产品的统一位置，吊卡的纸张质量要求用350克铜版纸。为确保顺利及时完成包装确认，减少贵厂多次打样产生额外费用。我司将统一代做吊卡，所产生的费用需由贵厂承担（内容及费用将另行通知）。印制完成后由我司安排快递到付给贵厂。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".内盒和外箱正面一面右下方和顶面一面右下方需各贴一只外箱标，离各边边缘不小于1.25\"处。内盒标和外箱标以及内盒/外箱唛头内容我司稍后通知,贵司需做电脑排版电邮给我司确认，待我司确认电脑排版后方可安排大货印制。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".所有电脑标的条码都必须能够正确扫描，所有箱唛内容必须印刷正确，否则将会引起客人严重罚款。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".外箱必须是双层瓦楞无钉纸箱，工字形封箱，不得使用打包带。外箱都须通过我司六面试摔测试， 试摔高度为90CM。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".此单需要打托出运，托盘由我司统一订购提供。贵厂负责将外箱合理地垒在托盘上，并缠膜固定好，再用叉车逐个装入集装箱内，需注意打托好后的整体稳定性，安全包装的细节请与我司黄岩办确认。<br/>");
                //固定条款7

                i++;
                sb.Append(i + "." + CommonCode.ListToString(list_OuterVolume_S164, "；") + "。否则客人有权就超过部分向贵厂索赔海运费差额。<br/>");
                //固定条款8

                i++;
                sb.Append(i + ".出货前须提供一套含完整包装的出货样给我司，出货样产品质量必须与大货质量相符，若因质量问题造成客户索赔， 须由贵方负责。<br/>");
                //固定条款9

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同为货交浙江仙居指定工厂的价格，我司负责打托/缠膜，贵司自行负责报关。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同为货交" + PortName + "仓库价格，如果将来改为拉柜，将根据实际拖柜费按照每家厂的货物立方数分摊，贵厂须将此拖柜费退还至我司指定账户。<br/>");
                }
                //固定条款10

                i++;
                sb.Append(i + ".请按时出货，客人将对延迟出运予以罚款并有权取消定单，罚款金额不少于客人定单金额的50%。<br/>");
                //固定条款11

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，贵厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而产生任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款12

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".付款方式为XXXX天内，由JOY KEY(HONG KONG) LTD. 全额付清。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为含税价，付款方式为XXXX天左右提供有效的增值税发票，由我司安排付款。<br/>");
                }
                //变动条款13

                i++;
                sb.Append(i + ".请收到该合同3个工作日内确认加盖公章并签字回传到021-61183771，否则将视作自动放弃。<br/>");
                //固定条款14
            }

            #endregion S164

            #region S259

            else if (SelectCustomer == "S259")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(+5));//下单日期-5天

                int i = 0;

                i++;
                sb.Append(i + ".请工厂在收到本合同一周内提供两套能够通过90CM高6面4线试摔的包装样给客人确认包装安全；包装样确认后一周内请工厂提供两套含完整包装的大货样给客人确认产品质量及效果；如果因为包装样或者大货样不确认而引起的费用及其他后果均由工厂承担。如果是翻单产品，我司会以邮件形式另行通知包装样及大货样的需求。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".我司留底样在工厂回签本合同后将寄给作为签字样，并作为生产和验货的依据。大货生产及验货必须以我司提供的签字样为准。产品装配时须注意产品的稳定性和牢固性。拆装的产品需要配客人确认的装配说明书。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".所有产品均需要符合全美国法律法规及环保标准，表面涂层的含铅量不得超过50PPM。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".本合同箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。所有排版须经我司确认后，工厂方可开始大货印制。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".木制产品如用到复合板材，如密度板，夹板等，所用材料须符合美国加州标准，且产品上必须要贴CARB标。CARB内容经我司确认后方可印刷。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".如果工厂委托我司代印包装材料，费用以与我司合作的印刷厂的报价为准。代印费用由我司直接垫付给印刷厂，并从货款中扣除此代印费用。代印包装材料的运费由工厂自行承担。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".邮购内盒质量要求：须通过常规90公分6面4边安全试摔。 外箱要求使用无钉箱，工字型封箱，且不可使用打包带。外箱包装须过6面90公分安全摔箱测试。安全包装的具体细节请与我司黄岩办确认。<br/>");
                //固定条款8

                i++;
                sb.Append(i + "." + CommonCode.ListToString(list_OuterVolume_S60, "；") + "。订单总体积不得超过" + str_SumOuterVolume + "。若因体积超出合同要求而导致柜子装不下或者客人索赔，一切责任及后果由工厂承担。<br/>");
                //固定条款9

                i++;
                sb.Append(i + ".所有大货须由我司QC验货通过后方可出运。所有大货质量必须与签字样以及包装样和大货样相同，如果因为产品质量问题而引起任何后果，一切责任和损失须由工厂承担。<br/>");
                //固定条款10

                i++;
                sb.Append(i + ".出货前，须提供产品照片、箱唛及包装照片供我司确认。<br/>");
                //固定条款11

                i++;
                sb.Append(i + ".请按时出货。若因贵厂原因延期出货而产生的一切费用和责任，由贵厂承担。<br/>");
                //固定条款12

                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在
                //变动条款13

                i++;
                sb.Append(i + ".送货入仓/集装箱前，请携带少量多余的正确无破损的空纸箱以备用，避免因纸箱包装破损引起产品受损，导致客人索赔。<br/>");
                //固定条款14

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".此价格为交货" + PortName + "仓库人民币含税价格。如果我司拖柜工厂装柜，工厂须按照￥" + RegisterFees + "元/立方米退还我司拖柜费用。<br/>");
                }
                //变动条款15

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，贵厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款16

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款17

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021 - 61183771，逾期视为自动放弃。<br/>");
                //固定条款18

            }

            #endregion S259


            #region DT

            else if (SelectCustomer == "DT")
            {
                str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(+7));//下单日期+7天

                int i = 0;

                i++;
                sb.Append(i + "." + str_PurchaseDate + "前提供四套不含出货包装的产前确认样，供我司寄客人确认后做签字样给工厂。待客人确认标签或吊卡排版及大货样张品质后，需再提供三套完整出货包装的最终大货样供客人确认，确认通过后方可出运。<br/>");
                //固定条款1

                i++;
                sb.Append(i + ".签字样必须同原确认留底样。<br/>");
                //固定条款2

                i++;
                sb.Append(i + ".所有产品须符合美国法定环保标准，产品及产品表面含铅量不得超过90PPM（儿童产品不得超过40PPM）。所有包装材料的涂层与基材均须为环保材料，其重金属总含量不得超过100PPM。其余相关检测要求参见我司另行通知的产品对应的检测标准。工厂如需做产品自测，须寄六套自测样到检测公司按客人标准做自测，自测费用须由工厂承担。<br/>");
                //固定条款3

                i++;
                sb.Append(i + ".此合同产品若需要检测，我司会另行邮件通知。检测样的提交方式分为自行寄样送检和客人指定检测公司人员到工厂现场抽检。通常复活节儿童类产品、与食物接触相关的产品、蜡烛产品以及相关的烛台类产品需要现场抽检。需要抽检的产品，现场抽检的次数为1-4次以客人订单要求为准，所有批次抽检的首次抽检及检测的费用由我司承担，如果因工厂的检测样没有通过检测而产生重新抽检、检测、罚款或者订单取消，须由工厂承担全部责任和损失。<br/>");
                //固定条款4

                i++;
                sb.Append(i + ".所有大货须由客人指定验货人员或者我司QC验货通过后方可出运。若有客人尾期验货，首次验货的费用由我司承担，如果因为验货不通过而产生的重验费及一切责任和损失须由工厂承担。大货生产全部完成，且包装至少完成80%后方可申请尾期验货。<br/>");
                //固定条款5

                i++;
                sb.Append(i + ".所有大货质量须与检测样及签字样相同，如果因为产品质量问题而引起任何后果，一切责任和损失须由工厂承担。<br/>");
                //固定条款6

                i++;
                sb.Append(i + ".出货前15天工厂须提供两套含完整包装的出货样给我司，有PDQ的货号，两套为两整PDQ。<br/>");
                //固定条款7

                i++;
                sb.Append(i + ".此合同若需要客人指定的第三方验厂公司的WCA验厂，我司将另行邮件通知。若要验厂，工厂可以选择以XXXX的名义出货，但是需要承担订单金额的2 %（即" + Math.Round(AllAmount * 0.02m, 4) + "元）作为对XXXX工厂的补贴，此费用将从货款中直接扣除。如果工厂使用其他工厂名义出货，工厂须按照要求将指定数量的大货送到指定工厂验货，因此而产生的仓储及装卸等费用按实际收取。由指定工厂转运至港口的运费由工厂承担（如果散货进仓，以实际产生费用为准；如果工厂拖柜，运费按" + RegisterFees + "元 / 立方收取）。如果由于装柜的需要或者工厂要求运回大货，回程的运费将由工厂自行承担。<br/>");
                //固定条款8

                i++;
                sb.Append(i + ".所有产品的彩色标贴、吊卡等排版须经过客人确认，排版确认后需寄彩印样张供客人确认，样张经客人确认后方可大货印刷。<br/>");
                //固定条款9

                i++;
                sb.Append(i + ".如果工厂委托我司代印包装材料，费用以与我司合作的印刷厂的报价为准。代印费用由我司直接垫付给印刷厂，我司将与工厂另行结算代印费用。代印包装材料的运费由工厂自行承担。<br/>");
                //固定条款10

                i++;
                sb.Append(i + ".本合同箱唛及相关包装要求以我司邮件通知的包装资料为准，包装资料为本合同不可分割的一部分，具有与本合同相同的法律效力。箱唛排版经我司核对确认后，工厂须印大货样箱供我司拍清楚的箱唛印刷照片并打印箱唛照片寄客人确认，待客人确认样品箱唛照片后，工厂方可开始大货印制。<br/>");
                //固定条款11

                i++;
                sb.Append(i + ".12.	产品如有用到塑料袋或汽泡袋包装，须印有或贴有警告语，警告语内容参见此货号的包装资料要求，排版须经我司确认后方可大货印刷。若塑料袋或汽泡袋开口大于5\"，还须打孔。可转运塑料袋内包装需寄样给我司确认后方可大货生产。<br/>");
                //固定条款12

                i++;
                sb.Append(i + ".本合同项下如有易碎产品（如玻璃、陶瓷、树脂等），外箱箱唛须印有易碎标。<br/>");
                //固定条款13

                i++;
                sb.Append(i + ".内盒需要转运安全。外箱须为双层瓦楞纸箱，不可打钉，不可打包装带，须工字型封箱。外箱须通过一点三线六面离地90CM摔箱测试。安全包装的细节请与我司QC确认。<br/>");
                //固定条款14

                i++;
                sb.Append(i + "." + CommonCode.ListToString(list_OuterVolume) + "，订单总体积不得超过" + str_SumOuterVolume + "。若因体积超出合同要求而产生客人索赔，一切索赔费用及责任须由工厂承担。<br/>");
                //固定条款15

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同价格为FOB " + PortName + "港美金价，工厂负责报关及出运，并承担所有的出运费用。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同价格为货交" + PortName + "港人民币含税价。如实际出运改为工厂拉柜，则由我司安排指定货代拉柜，工厂需退还我司￥" + RegisterFees + "元/立方的拉柜费（黄岩地区￥55元/立方，沈阳地区虹嘉/华辰￥50元/立方，瑞隆￥60元/立方），提箱费实报实销由我司承担。<br/>");
                }
                //变动条款16


                if (!string.IsNullOrEmpty(str_Customs))
                {
                    i++;
                    sb.Append(i + "." + str_Customs + "。<br/>");
                }//可能此条款不存在
                //变动条款17

                i++;
                sb.Append(i + ".本合同不允许提前或者延迟出运，否则因此而产生的罚款，订单取消或空运等一切责任将由工厂承担。<br/>");
                //固定条款18

                if (!string.IsNullOrEmpty(str_ProductCopyRight_OurCompany_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_OurCompany_No + "产品为我司专有设计，工厂不得通过任何方式销售给其他公司或客户，否则我司有权要求索赔。<br/>");
                }
                if (!string.IsNullOrEmpty(str_ProductCopyRight_Factory_No))
                {
                    i++;
                    sb.Append(i + ".本合同项下" + str_ProductCopyRight_Factory_No + "产品为工厂专有设计，如果因为版权问题而生产任何纠纷及责任均由工厂承担。<br/>");
                }
                //变动条款19

                i++;
                if (CurrentSign == Keys.USD_Sign)
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右由我司全额付清货款。<br/>");
                }
                else
                {
                    sb.Append(i + ".本合同付款方式为XXXX天左右提供有效的增值税发票，由我司全额付清货款。<br/>");
                }
                //变动条款20

                i++;
                sb.Append(i + ".请于收到合同三个工作日内签字盖公章回传至021-61183771，否则视为自动放弃。<br/>");
                //固定条款21

            }

            #endregion DT


            return sb;
        }

        /// <summary>
        /// 配件合同的条款
        /// </summary>
        /// <returns></returns>
        public static StringBuilder Bind_PurchaseContact_ProductFitting()
        {
            StringBuilder sb = new StringBuilder();

            string str_PurchaseDate = Utils.DateTimeToStr3(DateTime.Now.AddDays(-5));//下单日期-5天

            int i = 0;

            i++;
            sb.Append(i + ".请于" + str_PurchaseDate + "提供3套确认样给我司，1套作为签样退还贵司做大货生产依据，另2套做出货样用。<br/>");
            i++;
            sb.Append(i + ".所有产品需符合美国法定环保标准。产品及其基层的含铅量不得超过90PPM.。<br/>");
            i++;
            sb.Append(i + ".为了防止产品在运输过程中的破损，每次发货时须多提供2 % 的余量。<br/>");
            i++;
            sb.Append(i + ".发货时请使用硬度好的双层瓦楞纸箱，工字型封箱，外箱需通过我司60 - 90cm高摔箱测试。具体安全包装的细节请与我司QC确认。<br/>");
            i++;
            sb.Append(i + ".大货发货前3天需通知我司QC验货，验货合格后方可发货。<br/>");
            i++;
            sb.Append(i + ".本合同价格为含运费和含税价，交货地为“福建省福州市闽侯县旗山教师公寓11号楼1402”。<br/>");
            i++;
            sb.Append(i + ".本产品为贵厂提供设计，如有任何侵权和专利纠纷由贵厂负责，与我司无关。<br/>");
            i++;
            sb.Append(i + ".付款方式为收到货后凭贵厂开具有效的增值税发票由我司全额付款。<br/>");
            i++;
            sb.Append(i + ".请于收到该合同三个工作日内签字盖公章回传至021 - 61183771，否则视为自动放弃。<br/>");

            return sb;

        }
    }
}