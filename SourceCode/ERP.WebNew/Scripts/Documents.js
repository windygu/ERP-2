//单证用到的

$(function () {
    $('#Tabs').tabs({
        border: false,
        pill: true,
        onSelect: function (title, index) {
            if (endEditingInfo()) {
                editIndex = undefined;
            }
        }
    });

});


function getRowIndex(target) {
    var tr = $(target).closest('tr.datagrid-row');
    return parseInt(tr.attr('datagrid-row-index'));
}

var editIndex = undefined;
var editGrid = "";

function onBeforeEdit(index, row) {
    row.editing = true;
    updateActionsInfo(index);
    //UpdateExpander();
}

function onAfterEdit(index, row) {
    row.editing = false;
    updateActionsInfo(index);

    //UpdateExpander();

    BindOtherDatagrid(index);
}
function onCancelEdit(index, row) {
    row.editing = false;
    updateActionsInfo(index);
}
function endEditingInfo() {
    if (editIndex == undefined) { return true }
    if ($(editGrid).datagrid('validateRow', editIndex)) {
        $(editGrid).datagrid('endEdit', editIndex);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}
function updateActionsInfo(index) {
    $(editGrid).datagrid('refreshRow', index);
    InitPopover();

}

function acceptInfo(target) {
    editIndex = getRowIndex(target);
    if (endEditingInfo()) {
        $(editGrid).datagrid('acceptChanges');
    }
}

function onClickCell(index, field) {
    if (editGrid != "#" + $(this)[0].id && endEditingInfo()) {
        editIndex = undefined;
    }

    editGrid = "#" + $(this)[0].id;
    if (editIndex != index) {
        if (endEditingInfo()) {
            $(editGrid).datagrid('selectRow', index).datagrid('beginEdit', index);
            var ed = $(editGrid).datagrid('getEditor', { index: index, field: field });
            if (ed) {
                ($(ed.target).data('textbox') ? $(ed.target).textbox('textbox') : $(ed.target)).focus();
            }
            editIndex = index;
        } else {
            setTimeout(function () {
                $(editGrid).datagrid('selectRow', editIndex);
            }, 0);
        }
    }
    RemovePopover();
}

function productNoFormatter(val, row, index) {
    return jav.GetProductHtml(row.Image, '@(Url.Content("~/Product/Details/"))' + row.ProductID, row.No);
}

function onBeginEdit(index, row) {
    $(".datagrid-editable-input,.validatebox-text").on("keypress", function (e) {
        if (e.keyCode == 13) {
            acceptInfo(row);
            //UpdateSubDatagrid(index,row);
        }
    });
}

function emptyFunc() {

}

function onLoadSuccess() {
    InitPopover();
}


function GetUpLoadFile(tableID) {
    var UpLoadFileList = [];
    var HasUploadFile = false;
    if ($(tableID + " tbody tr .ServerFileName").length > 0) {
        $(tableID + " tbody tr").each(function () {
            var $this = $(this);
            var UpLoadFileListID = $this.find(".UpLoadFileListID").val();
            var DisplayFileName = $this.find(".DisplayFileName").val();
            var ServerFileName = $this.find(".ServerFileName").val();
            var DT_CREATEDATE = $this.find(".DT_CREATEDATE").val();
            var IsDelete = $this.find(".IsDelete").val();

            if (ServerFileName != "undefined") {
                UpLoadFileList.push({
                    ID: UpLoadFileListID,
                    DisplayFileName: DisplayFileName,
                    ServerFileName: ServerFileName,
                    DT_CREATEDATE: DT_CREATEDATE,
                    IsDelete: IsDelete
                });
            }
            if (IsDelete == "false") {
                HasUploadFile = true;
            }
        });
    }
    return [UpLoadFileList, HasUploadFile];
}

function GetInputValue_ByName(index, name, type) {
    if (type == null) {
        type = "input";
    }
    var val = $(type + "[name='ThisModel[" + index + "]." + name + "']").val();
    return val;
}
