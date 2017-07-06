/*----------------------------------------报价单页面用到了，其他页面不需要引用---------------------------------------------*/

//内容改变
function CalculateRow(row, editIndex, field) {
    //debugger;
    //console.log(editIndex);

    var tempELC = row.ELC;
    var tempMU = row.MU;
    var tempRetail = row.Retail;

    var isCostChange = false;
    if (field == "Cost") {
        isCostChange = true;
    }
    else if (field == "Retail") {
        if (row.Retail == 0) {
            row.MU = 0;
        } else {
            row.MU = 1 - row.ELC / row.Retail;
        }
        row.MU = NumberToRound(row.MU * 100, 2);
        return;
    }
    else if (field == "MU") {
        row.Retail = NumberToRound(row.ELC / (1 - row.MU / 100), 0) - 0.01;
        return;
    }

    //从客户资料里面取
    var CommissionPercent = NumberToRound(row.Commission, 2) / 100;
    var AllowancePercent = NumberToRound(row.Allowance, 2) / 100;
    var PalletPc = NumberToRound(row.PalletPc, 2);
    var MiscImportLoadPercent = NumberToRound(row.MiscImportLoad, 2) / 100;
    var ELCFill = NumberToRound(row.ELCFill, 2) / 100;
    var QuoteTemplateFileName = row.QuoteTemplateFileName;

    var termsID = parseInt(row.TermsID);

    var CurrencySign = row.CurrencySign;
    var Rate = NumberToRound(row.Rate, GetRate_RoundNumber(QuoteTemplateFileName));//已经计算过 Rate 。如果产品的工厂价格是人民币，取人民币的换汇。如果产品的工厂价格是美元，取美元的换汇。
    var Cost = NumberToRound(row.Cost, 4);
    var MUPercent = NumberToRound(row.MU, 2) / 100;

    //已经计算过  Fobfty($)。当工厂价格是美金报价，Fobfty=工厂价格。当工厂价格是人民币报价，Fobfty=工厂价格 / 汇率。
    var FOBFTY = NumberToRound(row.FOBFTY, GetFobFty_RoundNumber(QuoteTemplateFileName));
    var PriceFactory = NumberToRound(row.PriceFactory, 2);
    var DutyPercent = NumberToRound(row.DutyPercent, 2) / 100;

    var PortID = row.PortID;
    row.FreightRate = $("#hiddenPortID option[value=" + PortID + "]").attr("freightRate");
    row.OuterVolume = NumberToRound(row.OuterVolume, GetOuterVolume_RoundNumber(QuoteTemplateFileName));
    var FreightRate = row.FreightRate;
    var FreightAmount = FreightRate * row.OuterVolume / row.OuterBoxRate;//已经计算过 Freight Amount = Freight rate*外箱材积/外箱率
    
    var AgentPercent = NumberToRound(row.Agent, 2) / 100;
    var AgentAmount = 0;

    var FinalFOB = 0;
    var DutyAmount = 0;
    var MiscImportLoadAmount = 0;
    var ELC = 0;
    var Retail = 0;

    var dataflag = 0;

    switch (termsID) {
        case 1://FOB CN
            var tempRate = Rate;
            if (CurrencySign == "$") {
                tempRate = 1 - Rate;
            }

            if (QuoteTemplateFileName == "DG" || QuoteTemplateFileName == "S220") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost;
                } else {
                    Cost = PriceFactory / tempRate;
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount + MiscImportLoadAmount + AgentAmount;
            }

            else if (QuoteTemplateFileName == "F20") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost;
                } else {
                    Cost = PriceFactory / tempRate;
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount;
            }

            else if (QuoteTemplateFileName == "Form1") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent) / (1 - AllowancePercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent) / (1 - AllowancePercent);
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount + MiscImportLoadAmount;
            }

            else if (QuoteTemplateFileName == "S13") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost;
                } else {
                    Cost = PriceFactory / tempRate;
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount;
            }

            else if (QuoteTemplateFileName == "S164") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent) / (1 - MiscImportLoadPercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent) / (1 - MiscImportLoadPercent);
                }

                DutyAmount = DutyPercent * FOBFTY;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount + MiscImportLoadAmount;
            }

            else if (QuoteTemplateFileName == "S188") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent) / (1 - AllowancePercent) / (1 - MiscImportLoadPercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent) / (1 - AllowancePercent) / (1 - MiscImportLoadPercent);
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount;

                var OuterVolume = NumberToRound(row.OuterVolume, 4);
                row.OuterVolume = OuterVolume;
            }

            else if (QuoteTemplateFileName == "S235") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent) / (1 - AllowancePercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent) / (1 - AllowancePercent);
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount;
            }

            else if (QuoteTemplateFileName == "S237") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent) / (1 - AllowancePercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent) / (1 - AllowancePercent);
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount + MiscImportLoadAmount;
            }

            else if (QuoteTemplateFileName == "S239") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent);
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount + MiscImportLoadAmount;
            }

            else if (QuoteTemplateFileName == "S288") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent) / (1 - MiscImportLoadPercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent) / (1 - MiscImportLoadPercent);
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = (Cost * (1 - MiscImportLoadPercent + ELCFill)) + FreightAmount + DutyAmount;
            }

            else if (QuoteTemplateFileName == "S56") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent);
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount + MiscImportLoadAmount;
            }

            else if (QuoteTemplateFileName == "S60") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent) / (1 - AgentPercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent) / (1 - AgentPercent);
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount + MiscImportLoadAmount;
            }
            else if (QuoteTemplateFileName == "DT") {
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent) / (1 - MiscImportLoadPercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent) / (1 - MiscImportLoadPercent);
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount + MiscImportLoadAmount;
            } else {
                //公用的公式
                if (isCostChange) {
                    tempRate = PriceFactory / Cost / (1 - CommissionPercent) / (1 - MiscImportLoadPercent);
                } else {
                    Cost = PriceFactory / tempRate / (1 - CommissionPercent) / (1 - MiscImportLoadPercent);
                }

                DutyAmount = DutyPercent * Cost;
                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;
                ELC = Cost + DutyAmount + FreightAmount + MiscImportLoadAmount;

            }

            //debugger;
            if (tempELC != NumberToRound(ELC, GetELC_RoundNumber(QuoteTemplateFileName)) || tempMU != row.MU) {
                Retail = NumberToRound(ELC / (1 - MUPercent), 0) - 0.01;
            } else {
                Retail = tempRetail;
            }

            Rate = tempRate;
            if (CurrencySign == "$") {
                Rate = 1 - tempRate;
            }
            dataflag = 1;
            break;
        case 3://FOB US
            //暂时不设置公式

            if (isCostChange) {
            } else {
                Cost = 0;
            }
            break;
        case 4://POE
        case 5://DDP

            if (termsID == 5 && QuoteTemplateFileName == "S164") {
                DutyAmount = DutyPercent * FOBFTY;
                if (isCostChange) {
                    Rate = 1 - (FOBFTY + DutyAmount + FreightAmount + PalletPc) / (1 - CommissionPercent) / (1 - AllowancePercent) / (1 - AgentPercent) / Cost;
                } else {
                    Cost = (FOBFTY + DutyAmount + FreightAmount + PalletPc) / (1 - CommissionPercent) / (1 - AllowancePercent) / (1 - AgentPercent) / (1 - Rate);
                }

                FinalFOB = Cost - DutyAmount - FreightAmount;//只有当客户的报价单模板是S164、Terms选中DDP时才用到
                FinalFOB = NumberToRound(FinalFOB, 2);

                MiscImportLoadAmount = MiscImportLoadPercent * Cost;
                AgentAmount = AgentPercent * Cost;

                ELC = Cost + DutyAmount + FreightAmount + MiscImportLoadAmount;
                Retail = NumberToRound(Cost / (1 - MUPercent), 0) - 0.01;
            }

            break;
        default:
            break;
    }

    var CommissionAmount = CommissionPercent * Cost;

    Rate = NumberToRound(Rate, GetRate_RoundNumber(QuoteTemplateFileName));
    CommissionAmount = NumberToRound(CommissionAmount, 2);

    AgentAmount = NumberToRound(AgentAmount, 2);

    DutyAmount = NumberToRound(DutyAmount, GetDutyAmount_RoundNumber(QuoteTemplateFileName));
    FreightAmount = NumberToRound(FreightAmount, GetFreightAmount_RoundNumber(QuoteTemplateFileName));
    MiscImportLoadAmount = NumberToRound(MiscImportLoadAmount, GetMiscAmount_RoundNumber(QuoteTemplateFileName));
    Cost = NumberToRound(Cost, GetCost_RoundNumber(QuoteTemplateFileName));
    ELC = NumberToRound(ELC, GetELC_RoundNumber(QuoteTemplateFileName));

    //console.log("计算之后————————————————————————————————————");
    //console.log("工厂价:" + PriceFactory);
    //console.log("币种:" + CurrencySign);
    //console.log("外箱率:" + row.OuterBoxRate);
    //console.log("外箱材积:" + row.OuterVolume);
    //console.log("Duty%:" + DutyPercent);
    //console.log("DutyAmount:" + DutyAmount);
    //console.log("FreightRate:" + FreightRate);
    //console.log("FreightAmount:" + FreightAmount);
    //console.log("CommissionPercent:" + CommissionPercent);
    //console.log("AllowancePercent:" + AllowancePercent);
    //console.log("MiscImportLoadPercent:" + MiscImportLoadPercent);
    //console.log("MiscImportLoadAmount:" + MiscImportLoadAmount);
    //console.log("AgentPercent:" + AgentPercent);
    //console.log("AgentAmount:" + AgentAmount);
    //console.log("FOBFTY:" + FOBFTY);
    //console.log("ELC:" + ELC);
    //console.log("Rate:" + Rate);
    //console.log("Cost:" + Cost);

    //var $PortID = row.PortID;
    //$PortID.find("option").show();
    //$PortID.find("option").each(function () {
    //    var $this = $(this);
    //    if ($this.data("dataflag") == dataflag) {
    //        $this.hide();//隐藏英文的出运港
    //    }
    //});
    //row.FreightRate = FreightRate;
    row.Freight = FreightAmount;

    row.FOBFTY = FOBFTY;
    row.Rate = Rate;
    row.FinalFOB = FinalFOB;

    row.Duty = DutyAmount;
    row.CommissionAmount = CommissionAmount;
    row.MiscImportLoadAmount = MiscImportLoadAmount;
    row.AgentAmount = AgentAmount;
    row.ELC = ELC;
    row.Retail = Retail;

    if (isCostChange) {
        row.Cost = Cost;
    } else {
        if (termsID != 3) {//如果选中的不是FOB US
            row.Cost = Cost;
        }
    }

    //var $thisRow = $(".datagrid-view2 .datagrid-body tr[datagrid-row-index=" + editIndex + "]");
    //$thisRow.find("td[field='FreightRate']").children().text(FreightRate);
    //$thisRow.find("td[field='Freight']").children().text(FreightAmount);
    //$thisRow.find("td[field='FinalFOB']").children().text(FinalFOB);
    //$thisRow.find("td[field='Rate']").find("input").eq(0).val(Rate);
    //$thisRow.find("td[field='Cost']").find("input").eq(0).val(Cost);
    //$thisRow.find("td[field='Retail']").find("input").eq(0).val(Retail);
    //$thisRow.find("td[field='Duty']").children().text(DutyAmount);
    //$thisRow.find("td[field='CommissionAmount']").children().text(CommissionAmount);
    //$thisRow.find("td[field='MiscImportLoadAmount']").children().text(MiscImportLoadAmount);
    //$thisRow.find("td[field='AgentAmount']").children().text(AgentAmount);
    //$thisRow.find("td[field='ELC']").children().text(ELC);
}

$("#btnSelectProduct").click(function () {
    //验证
    if ($('#CustomerID').val() == "") {
        $.messager.alert("提示", "请选择客户！", "info", function () {
            $("#CustomerID").focus();
        });
        return;
    }

    if ($('#ExchangeRate').val() == "") {
        $.messager.alert("提示", "请输入预期市场汇率！", "info", function () {
            $("#ExchangeRate").focus();
        });
        return;
    }
    if ($('#CurrencyExchangeUSD').val() == "") {
        $.messager.alert("提示", "请输入美元换汇！", "info", function () {
            $("#CurrencyExchangeUSD").focus();
        });
        return;
    }
    if ($('#CurrencyExchangeRMB').val() == "") {
        $.messager.alert("提示", "请输入人民币换汇！", "info", function () {
            $("#CurrencyExchangeRMB").focus();
        });
        return;
    }

    var CurrencyExchangeRMB = $("#CurrencyExchangeRMB").val();
    if (CurrencyExchangeRMB != parseFloat(CurrencyExchangeRMB) || parseFloat(CurrencyExchangeRMB) <= 0) {
        $.messager.alert("提示", "请输入大于0的数字！", "info", function () {
            $("#CurrencyExchangeRMB").focus();
        });
        return;
    }

    var CurrencyExchangeUSD = $("#CurrencyExchangeUSD").val();
    if (CurrencyExchangeUSD != parseFloat(CurrencyExchangeUSD) || parseFloat(CurrencyExchangeUSD) <= 0) {
        $.messager.alert("提示", "请输入大于0的数字！", "info", function () {
            $("#CurrencyExchangeUSD").focus();
        });
        return;
    }

    var ExchangeRate = $("#ExchangeRate").val();
    if (ExchangeRate != parseFloat(ExchangeRate) || parseFloat(ExchangeRate) <= 0) {
        $.messager.alert("提示", "请输入大于0的数字！", "info", function () {
            $("#ExchangeRate").focus();
        });
        return;
    }

    if ($('#TermsID').val() == "") {
        $.messager.alert("提示", "请选择Terms！", "info", function () {
            $("#TermsID").focus();
        });
        return;
    }

    if ($('#Port').val() == "") {
        $.messager.alert("提示", "请选择Port！", "info", function () {
            $("#Port").focus();
        });
        return;
    }

    //loadGrid();
    $('#myModal1').modal({
        keyboard: true
    });
});