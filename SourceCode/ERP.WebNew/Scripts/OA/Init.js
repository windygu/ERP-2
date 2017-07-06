// JScript 文件

/**********************
需要在显示时间/日期范围的对象中加入
daterange 日期范围
datetimerange 日期时间范围
min 最小值
max 最大值
step 此处表示若类别为datetime, 分钟的步长值/间隔值

$t.SetDateRange(obj,name,rel) 可以设置时间范围的值

此段已经顺利完成

*******************/
//$(function () {
//    if ($.fn.daterangepicker)
//        $.fn.daterangepicker.defaultRanges = {
//            '今天': [moment(), moment()],
//            '前天': [moment().subtract('days', 2), moment().subtract('days', 2)],
//            '昨天': [moment().subtract('days', 1), moment().subtract('days', 1)],
//            '上月': [moment().subtract('months', 1).startOf('month'), moment().subtract('months', 1).endOf('month')],
//            '上周': [moment().subtract('weeks', 1).startOf('week'), moment().subtract('weeks', 1).endOf('week')],
//            '本周': [moment().subtract('weeks', 0).startOf('week'), moment().subtract('weeks', 0).endOf('week')],
//            '本月': [moment().startOf('month'), moment().endOf('month')],
//            '明天': [moment().subtract('days', -1), moment().subtract('days', -1)],
//            '后天': [moment().subtract('days', -2), moment().subtract('days', -2)],
//            '下周': [moment().subtract('weeks', -1).startOf('week'), moment().subtract('weeks', -1).endOf('week')],
//            '下月': [moment().subtract('months', -1).startOf('month'), moment().subtract('months', -1).endOf('month')],
//        };
//}
//);

function InitDateTimeRange(filter) {
    $(filter).each(function () {
        var $this = $(this);
        var format = $(this).attr("format");
        var $fnCall = OA.isNull($(this).attr("fnCall"));
        if (typeof (format) != "string") format = "";
        if ($.trim(format) == "")
            format = $(this).hasClass("datetimerange") ? "YYYY-MM-DD HH:mm:ss" : "YYYY-MM-DD";
        $("#" + $this.attr("timediffrel")).val(0);
        $this.attr("format", format);
        $this.attr("readonly", "readonly");
        $this.daterangepicker
        (
            {
                format: format,
                startDate: $this.attr("min"),
                endDate: $this.attr("max"),
                opens: $this.attr("at") != "right" ? "left" : "right",
                timePicker: $this.hasClass("datetimerange"),
                timePickerIncrement: $this.attr("step")
            },
            function (start, end, obj) {
                var format = $this.attr("format");
                var s = "";
                if (start == null || end == null) {
                    $("#" + $this.attr("startrel")).val("");
                    $("#" + $this.attr("endrel")).val("");
                    $this.val("");
                    $("#" + $this.attr("timediffrel")).val(0);

                    return;
                }

                //if(typeof(format)=="undefined" || format==null|| $.trim(format)=="") format="YYYY-MM-DD";
                start = moment(start.format(format));
                end = moment(end.format(format)).add("seconds", 1);
                $("#" + $this.attr("startrel")).val(start.format(format));
                $("#" + $this.attr("endrel")).val(end.format(format));
                var curVal = null;
                if ($this.hasClass("datetimerange")) {
                    days = end.diff(start, "days");
                    hours = end.diff(start, "hours") % 24;
                    minutes = end.diff(start, "minutes") % 60;
                    curVal = end.diff(start, "minutes");
                }
                else {
                    days = end.diff(start, "days") + 1;
                    hours = 0;
                    minutes = 0;
                    curVal = days;
                }

                if (hours == 0 && days == 0 && minutes == 0)
                    s = "（相同）";
                else
                    s = "（" + (days > 0 ? days + "天" : "")
                     + (hours > 0 ? hours + "小时" : "")
                     + (minutes > 0 ? minutes + "分钟" : "") + "）";
                $("#" + $this.attr("timediffrel")).val(curVal);
                $this.val(start.format(format) + " - " + end.format(format) + s);
                if ($fnCall != null) {
                    try {
                        eval($fnCall + "(curVal)");
                    }
                    catch (te) {
                        alert("回调出错, 错误信息 = " + te.description);
                    }
                }
            }
        );
    });
}

function InitColorPicter(filter) {
    $(filter).colorpicker();
}

function InitSelect(filter) {
    $(filter).selectpicker();
}

function InitDateTime(filter) {
    /***此段已经ＯＫ***/
    $(filter).each(function () {
        var $this = $(this);
        var options = {};
        switch ($(this).attr("xtype")) {
            case "date":
            case "date1":
                options = {
                    startView: 2,
                    minView: 2,
                    format: "yyyy-mm-dd"
                };
                break;
            case "time":
            case "time1":
            case "time2":
                options = {
                    startView: 1,
                    minView: 0,
                    maxView: 1,
                    format: "hh:ii:ss",
                    showMeridian: 1
                };
                break;
            case "datetime":
            case "datetime1":
                options = {
                    startView: 2,
                    showMeridian: 1,
                    format: "yyyy-mm-dd hh:ii:ss"
                };
                break;

            case "date2":
                options = {
                    startView: 2,
                    minView: 2,
                    format: "yyyy/mm/dd"
                };
                break;
            case "datetime2":
                options = {
                    startView: 2,
                    showMeridian: 1,
                    format: "yyyy/mm/dd hh:ii:ss"
                };
                break;

            case "date3":
                options = {
                    startView: 2,
                    minView: 2,
                    format: "yyyymmdd"
                };
                break;
            case "time3":
                options = {
                    startView: 1,
                    minView: 0,
                    maxView: 1,
                    format: "hhiiss",
                    showMeridian: 1
                };
                break;
            case "datetime3":
                options = {
                    startView: 2,
                    showMeridian: 1,
                    format: "yyyymmddhhiiss"
                };
                break;

            case "date4":
                options = {
                    startView: 2,
                    minView: 2,
                    format: "yyyy年mm月dd日"
                };
                break;
            case "time4":
                options = {
                    startView: 1,
                    minView: 0,
                    maxView: 1,
                    format: "hh时ii分ss秒",
                    showMeridian: 1
                };
                break;
            case "datetime4":
                options = {
                    startView: 2,
                    showMeridian: 1,
                    format: "yyyy年mm月dd日hh时ii分ss秒"
                };
                break;
            default:
                options = {
                    startView: 2,
                    showMeridian: 1,
                    format: "yyyy-mm-dd hh:ii:ss"
                };
                break;
        }
        options = $.extend({},
           {
               language: 'zh-CN',
               weekStart: 1,
               todayBtn: 1,
               autoclose: 1,
               todayHighlight: 1,
               forceParse: 0,
               pickerPosition: "bottom-left"
           },
            options);
        $this.datetimepicker(options);
    });
}