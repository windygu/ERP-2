var blockUIStyle = {
    minWidth: '450px',
    padding: '20px',
    fontSize: "16px",
    border: '1px solid black'
}

var jav = {
    //增加easyui的单元格的简单tooltips
    initGridCellTooltips: function (excludes) {
        $("tr.datagrid-row td[field]").each(function (i, o) {
            var text = $(this).text().trim();
            if (text != "") {
                if (!arrayContains(excludes, $(this).attr("field"))) {
                    $(this).tooltip({
                        position: 'bottom',
                        content: '<span style="color:#fff">' + text + '</span>',
                        onShow: function () {
                            $(this).tooltip('tip').css({
                                backgroundColor: '#666',
                                borderColor: '#666',
                                maxWidth: '40%',
                                wordWrap: 'break-word'
                            });
                        }
                    });
                }
            }
        });
    },

    regx: {
        // 数值验证，两位小数
        numValid: {
            regexp: /^\d+(\.\d{0,2})?$/,
            message: '请输入有效的数字'
        },
    },
    emailValid: {
        regexp: /^([\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]+;?)+$/,
        message: '请输入有效的邮箱'
    },
    gridColumnVisibilityMenus: {
        menuLeft: 0,
        menuTop: 0,
        cmenu: null
    },

    initGridColumnVisibilityMenus: function (gridID, userID, pageID, pageX, pageY, columnsVisible) {
        if (!jav.gridColumnVisibilityMenus.cmenu) {
            jav.createColumnMenu(gridID, userID, pageID, columnsVisible);
        }
        jav.gridColumnVisibilityMenus.cmenu.menu('show', {
            left: pageX,
            top: pageY
        });

        jav.gridColumnVisibilityMenus.menuLeft = pageX;
        jav.gridColumnVisibilityMenus.menuTop = pageY;
    },

    createColumnMenu: function (gridID, userID, pageID, columnsVisible) {
        jav.gridColumnVisibilityMenus.cmenu = $('<div/>').appendTo('body');
        jav.gridColumnVisibilityMenus.cmenu.menu({
            onClick: function (item) {
                if (item.iconCls == 'icon-ok') {
                    $('#' + gridID).datagrid('hideColumn', item.name);
                    jav.gridColumnVisibilityMenus.cmenu.menu('setIcon', {
                        target: item.target,
                        iconCls: 'icon-empty'
                    });
                } else {
                    $('#' + gridID).datagrid('showColumn', item.name);
                    jav.gridColumnVisibilityMenus.cmenu.menu('setIcon', {
                        target: item.target,
                        iconCls: 'icon-ok'
                    });
                }
                jav.gridColumnVisibilityMenus.cmenu.menu('show', {
                    left: jav.gridColumnVisibilityMenus.menuLeft,
                    top: jav.gridColumnVisibilityMenus.menuTop
                });

                // 保存起来
                var fields = $('#' + gridID).datagrid('getColumnFields');
                var fieldToHide = [];
                $.each(fields, function (i, o) {
                    var col = $('#' + gridID).datagrid('getColumnOption', o);
                    if (!col.hidden) {
                        fieldToHide.push(o);
                    }
                });
                $.ajax({
                    url: "/Common/SaveOrUpdateUserGridSettings",
                    data: JSON.stringify({ userID: userID, pageID: pageID, settings: fieldToHide }),
                    type: 'post',
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (data) {
                    },
                    error: function () {
                        $.messager.alert("提示", "保存失败");
                    }
                });
            },
            hideOnUnhover: false
        });
        var fields = $('#' + gridID).datagrid('getColumnFields');
        var columnsVisible = columnsVisible;
        for (var i = 0; i < fields.length; i++) {
            var field = fields[i];
            var col = $('#' + gridID).datagrid('getColumnOption', field);
            jav.gridColumnVisibilityMenus.cmenu.menu('appendItem', {
                text: col.title,
                name: field,
                iconCls: !columnsVisible ? 'icon-ok' : (arrayContains(columnsVisible, field) ? 'icon-ok' : 'icon-empty')
            });
        }
    },
    slideToogle: function (tog) {
        //左侧菜单栏展开收缩
        if (tog == "1") {
            $("#page_left").stop(true, true).css("display", "none");
            $("#page_right").parent().css("cssText", "margin-left: 0px!important;");
            $("#sliderbarToogler").attr("title", "展开").show();
            $("#sliderbarToogler span").removeClass("arr_l").addClass("arr_r");
        }
        else {
            $("#page_left").stop(true, true).css("display", "block");
            $("#page_right").parent().css("margin-left", "200px");
            $("#sliderbarToogler").attr("title", "收缩").show();
            $("#sliderbarToogler span").removeClass("arr_r").addClass("arr_l");
        }
    },
    SetCookie: function (name, value, days) {
        //写入cookie
        var exp = new Date();    //new Date("December 31, 9998");
        exp.setTime(exp.getTime() + days * 24 * 60 * 60 * 1000);
        document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString() + ";path=/";
    },
    GetCookie: function (name) {
        //读取cookie
        var arr = document.cookie.match(new RegExp("(^| )" + name + "=([^;]*)(;|$)"));
        if (arr != null) return unescape(arr[2]); return null;
    },
    GetProductHtml: function (image, href, no) {
        return "<a winType='idialog' class='data-content' winSize='1200,800,yes' data-toggle='popover' data-content='<div class=&quot;popoverDiv&quot;><img class=&quot;popoverImg&quot; src=&quot;" + image + "&quot; /></div>' onclick='return OA.i(this)' href='" + href + "' target='_blank'>" + no + "</a>";
    },
    IsInt: function (number) {
        //是否是整数 true：是整数，false：不是整数。
        if (number != parseInt(number)) {
            return false;
        }
        return true;
    },
    IsFloat: function (number) {
        //是否是数字 true：是数字，false：不是数字。
        if (number != parseFloat(number)) {
            return false;
        }
        return true;
    },
    //判断是否为数字,是数字返回true，否则返回false
    IsFigure: function (val) {
        if (val != null && val != "") {
            return !isNaN(val);
        } else {
            return false;
        }
    },
    //价格等于0的客户
    list_ContainZeroPrice_Customer:new Array("S135"),
};

Array.prototype.contains = function (obj) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == obj) {
            return true;
        }
    }
    return false;
}

Array.prototype.remove = function (obj) {
    var arr = [];
    for (var i = 0; i < this.length; i++) {
        if (this[i] != obj) {
            arr.push(this[i]);
        }
    }
    return arr;
}

function arrayContains(a, obj) {
    if (!a) return false;
    var i = a.length;
    while (i--) {
        if (a[i] === obj) {
            return true;
        }
    }
    return false;
}

function InitPopover() {
    /// <summary>
    /// 初始化鼠标移入显示图片
    /// </summary>
    $('[data-toggle="popover"]').popover({
        container: 'body',
        trigger: 'hover',
        html: true,
    });
}

function RemovePopover() {
    /// <summary>
    /// 移除鼠标移入显示图片
    /// </summary>
    $(".popover").remove();
}

function checkEmpty(inputName) {
    /// <summary>
    /// 判断是否为空
    /// </summary>
    var $input = $(inputName);
    if ($input.val() == "") {
        $.messager.alert("提示", $input.closest("li").find(".text-danger").html() + "不能为空！");
        $input.focus();
        return false;
    }
    return true;
}

function getSelections(tableName, fieldName) {
    /// <summary>
    /// 获取表格中所有选中的ID
    /// </summary>
    var idArray = [];
    var rows = $(tableName).datagrid('getSelections');
    for (var i = 0; i < rows.length; i++) {
        idArray.push(rows[i][fieldName]);
    }
    return idArray;
}

function clearAll(e) {
    /// <summary>
    /// 清空所有搜索条件
    /// </summary>
    $(e).closest(".search_Condition").find("input").val("");
    $(e).closest(".search_Condition").find("select").each(function () {
        $(this)[0].selectedIndex = 0;
    });
}

function formateDate(date) {
    var timestamp = Date.parse(date);
    if (timestamp) {
        var d = new Date(timestamp);
        var year = d.getFullYear();
        var month = fixNumberSize(d.getMonth() + 1);
        var day = fixNumberSize(d.getDate());
        return year + '/' + month + '/' + day + " " + fixNumberSize(d.getHours()) + ":" + fixNumberSize(d.getMinutes()) + ":" + fixNumberSize(d.getSeconds());
    } else {
        return '';
    }
}

function formateDate2(date) {
    var timestamp = Date.parse(date);
    if (timestamp) {
        var d = new Date(timestamp);
        var year = d.getFullYear();
        var month = fixNumberSize(d.getMonth() + 1);
        var day = fixNumberSize(d.getDate());
        return year + '-' + month + '-' + day + " " + fixNumberSize(d.getHours()) + ":" + fixNumberSize(d.getMinutes()) + ":" + fixNumberSize(d.getSeconds());
    } else {
        return '';
    }
}

function formateDate3(date, addyear, addmonth) {
    var timestamp = Date.parse(date);
    if (timestamp) {
        var d = new Date(timestamp);
        var year = d.getFullYear() + addyear;
        var month = d.getMonth() + 1 + addmonth;
        var day = d.getDate();
        if (month > 12) {
            year += Math.floor(month / 12);
            month = Math.round(month % 12);
        }
        return year + '-' + fixNumberSize(month) + '-' + fixNumberSize(day);
    } else {
        return '';
    }
}

function fixNumberSize(value, fixChar) {
    if (value < 10) {
        return (fixChar || '0') + value;
    }
    return value;
}

//四舍五入
function NumberToRound(num, v) {
    /// <summary>
    /// 四舍五入。第二个参数是小数点后面的位数
    /// </summary>
    var vv = Math.pow(10, v);
    return Math.round(num * vv) / vv;
}

//列表页面的表格刷新(为了兼容IE，加上后面的随机数)
function parentGridReload(tableName) {
    if (tableName == null) {
        tableName = "#MyGrid";
    }
    var options_url = parent.$(tableName).datagrid("options").url;
    if (options_url.indexOf("&randomNumber") > 0) {
        var startNumber = options_url.indexOf("&randomNumber");
        options_url = options_url.substr(0, startNumber);
    }
    options_url += "&randomNumber=" + Math.random();
    //console.log(options_url);
    parent.$(tableName).datagrid("options").url = options_url;
    parent.$(tableName).datagrid('reload');
}

//不是混装产品隐藏前面的加号
function UpdateExpander() {
    //每次加载
    var rows = $('#MyPopGrid').datagrid("getRows");
    if (rows && rows.length > 0) {
        for (var i = 0; i < rows.length; i++) {
            //console.log("1."+rows[i].IsProductMixed);
            if (rows[i].IsProductMixed == "False" || rows[i].IsProductMixed == "false" || rows[i].IsProductMixed == false) {
                $("#MyPopGrid").parent().find("table.datagrid-btable tr.datagrid-row:eq(" + i + ") .datagrid-row-expander").css("visibility", "hidden");
            }
        }
    }
}