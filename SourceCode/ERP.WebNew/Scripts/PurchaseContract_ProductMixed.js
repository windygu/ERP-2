
function UpdateSubDatagrid(index, row) {
    var $thisDatagrid = $("#ddv-" + index);
    var rows = $thisDatagrid.datagrid("getRows");
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {

            rows[i].Qty = parseInt(row.Qty) * parseInt(rows[i].Qty2);
            if (rows[i].InnerBoxRate != null && rows[i].InnerBoxRate != "") {
                rows[i].Qty = parseInt(row.Qty) * parseInt(rows[i].Qty2) * parseInt(row.OuterBoxRate) / parseInt(row.InnerBoxRate);
            }
            rows[i].ProductAmount = rows[i].PriceFactory * rows[i].Qty;
            rows[i].ProductAmountFormatter = rows[i].CurrencySign + rows[i].PriceFactory * rows[i].Qty;

            rows[i].PackageName = row.PackageName;
            rows[i].MixedMode = row.MixedMode;
            rows[i].OtherComment = row.OtherComment;
            rows[i].IsFragile = row.IsFragile;

            $thisDatagrid.datagrid('acceptChanges');
            $thisDatagrid.datagrid('refreshRow', i);
        }
    }

}


function BindSubDatagrid() {
    $('#MyPopGrid').datagrid({
        view: detailview,
        detailFormatter: function (index, row) {
            if ($("#ddv-" + index).html() != undefined) {
                return;
            }
            return '<div style="padding:2px"><table id="ddv-' + index + '"></table></div>';
        },
        onExpandRow: function (index, row) {
            if ($("#ddv-" + index).html() != "") {
                return;
            }

            $('#ddv-' + index).datagrid({
                url: '/Order/GetProducts_Mixed/' + row.OrderProductID,
                method: 'get',
                fitColumns: false,
                singleSelect: true,
                rownumbers: false,
                loadMsg: '',
                height: 'auto',
                frozenColumns: [[
                ]],
                columns: [[
                    { field: 'No', title: '产品货号', width: 100, align: 'center', formatter: productNoFormatter },
                    { field: 'Name', title: '品名', width: 150, align: 'center' },
                    { field: 'PackageName', title: '包装方式', width: 100, align: 'center', editor: { type: 'validatebox' } },
                    { field: 'PriceFactoryFormatter', title: '工厂价格', width: 60, align: 'center' },
                    { field: 'Qty', title: '数量', width: 60, align: 'center' },
                    { field: 'UnitName', title: '单位', width: 60, align: 'center' },
                    { field: 'ProductAmountFormatter', title: '金额', width: 60, align: 'center' },
                    { field: 'InnerBoxRate', title: '内盒率', width: 60, align: 'center' },
                    { field: 'OuterBoxRate', title: '外箱率', width: 60, align: 'center' },
                    { field: 'PDQPackRate', title: 'PDQ装箱率', width: 60, align: 'center' },
                    { field: 'StyleName', title: '款式', width: 60, align: 'center' },
                    { field: 'MixedMode', title: '混装方式', width: 100, align: 'center', editor: { type: 'validatebox' } },
                    { field: 'OtherComment', title: '产品其他要求', width: 100, align: 'center', editor: { type: 'validatebox' } },
                    {
                        field: 'IsFragile', title: '是否易碎品', width: 100, align: 'center', editor: {
                            type: 'combobox',
                            options: {
                                valueField: 'Value',
                                textField: 'Text',
                                data: [{ 'Selected': false, 'Text': '&nbsp;', 'Value': '0' }, { 'Selected': false, 'Text': '是', 'Value': '1' }, { 'Selected': false, 'Text': '否', 'Value': '2' }],
                                editable: false,
                            }
                        }, formatter: formatStatus
                    },

                    { field: 'ID', hidden: true },
                    { field: 'OrderProductID', hidden: true },
                    { field: 'ProductID', hidden: true },
                    { field: 'Image', hidden: true },
                    { field: 'PriceFactory', hidden: true },
                    { field: 'ProductAmount', hidden: true },
                    { field: 'IsProductMixed', hidden: true },
                    { field: 'Qty2', hidden: true },
                    { field: 'CurrencySign', hidden: true },

                ]],
                onResize: function () {
                    $('#MyPopGrid').datagrid('fixDetailRowHeight', index);
                },
                onLoadSuccess: function () {
                    setTimeout(function () {
                        $('#MyPopGrid').datagrid('resize');
                        $('#MyPopGrid').datagrid('fixDetailRowHeight', index);
                        UpdateSubDatagrid(index, row);
                    }, 0);
                }
            });
            $('#MyPopGrid').datagrid('fixDetailRowHeight', index);

        }
    });
}
