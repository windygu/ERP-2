// JScript 文件
//OA相关的系统函数库
//江龙 20140517
OA =
{
    Root: "/xoa.v2/",
    Loading: "glyphicon glyphicon-repeat fa-spin",
    DefaultTheme: "default",
    ThemeColors: {
        "blue": ["#00a1cb", "white"],
        "brown": ["#00a1cb", "white"],
        "default": ["#cc1d1d", "white"],
        "grey": ["#cc1d1d", "#white"],
        "light": ["#cc1d1d", "white"],
        "purple": ["#701584", "white"]
    },
    Types:
    {
        sms: { w: 900, h: 500, c: "发送短信" },
        down: { w: 0, h: 0, c: "文档下载" },
        view: { w: 900, h: 600, c: "在线文档预览" }
    },
    faIcons: "glass|music|search|envelope-o|heart|star|star-o|user|film|th-large|th|th-list|check|times|search-plus|search-minus|power-off|signal|gear|cog|trash-o|home|file-o|clock-o|road|download|arrow-circle-o-down|arrow-circle-o-up|inbox|play-circle-o|rotate-right|repeat|refresh|list-alt|lock|flag|headphones|volume-off|volume-down|volume-up|qrcode|barcode|tag|tags|book|bookmark|print|camera|font|bold|italic|text-height|text-width|align-left|align-center|align-right|align-justify|list|dedent|outdent|indent|video-camera|photo|image|picture-o|pencil|map-marker|adjust|tint|edit|pencil-square-o|share-square-o|check-square-o|arrows|step-backward|fast-backward|backward|play|pause|stop|forward|fast-forward|step-forward|eject|chevron-left|chevron-right|plus-circle|minus-circle|times-circle|check-circle|question-circle|info-circle|crosshairs|times-circle-o|check-circle-o|ban|arrow-left|arrow-right|arrow-up|arrow-down|mail-forward|share|expand|compress|plus|minus|asterisk|exclamation-circle|gift|leaf|fire|eye|eye-slash|warning|exclamation-triangle|plane|calendar|random|comment|magnet|chevron-up|chevron-down|retweet|shopping-cart|folder|folder-open|arrows-v|arrows-h|bar-chart-o|twitter-square|facebook-square|camera-retro|key|gears|cogs|comments|thumbs-o-up|thumbs-o-down|star-half|heart-o|sign-out|linkedin-square|thumb-tack|external-link|sign-in|trophy|github-square|upload|lemon-o|phone|square-o|bookmark-o|phone-square|twitter|facebook|github|unlock|credit-card|rss|hdd-o|bullhorn|bell|certificate|hand-o-right|hand-o-left|hand-o-up|hand-o-down|arrow-circle-left|arrow-circle-right|arrow-circle-up|arrow-circle-down|globe|wrench|tasks|filter|briefcase|arrows-alt|group|users|chain|link|cloud|flask|cut|scissors|copy|files-o|paperclip|save|floppy-o|square|navicon|reorder|bars|list-ul|list-ol|strikethrough|underline|table|magic|truck|pinterest|pinterest-square|google-plus-square|google-plus|money|caret-down|caret-up|caret-left|caret-right|columns|unsorted|sort|sort-down|sort-desc|sort-up|sort-asc|envelope|linkedin|rotate-left|undo|legal|gavel|dashboard|tachometer|comment-o|comments-o|flash|bolt|sitemap|umbrella|paste|clipboard|lightbulb-o|exchange|cloud-download|cloud-upload|user-md|stethoscope|suitcase|bell-o|coffee|cutlery|file-text-o|building-o|hospital-o|ambulance|medkit|fighter-jet|beer|h-square|plus-square|angle-double-left|angle-double-right|angle-double-up|angle-double-down|angle-left|angle-right|angle-up|angle-down|desktop|laptop|tablet|mobile-phone|mobile|circle-o|quote-left|quote-right|spinner|circle|mail-reply|reply|github-alt|folder-o|folder-open-o|smile-o|frown-o|meh-o|gamepad|keyboard-o|flag-o|flag-checkered|terminal|code|mail-reply-all|reply-all|star-half-empty|star-half-full|star-half-o|location-arrow|crop|code-fork|unlink|chain-broken|question|info|exclamation|superscript|subscript|eraser|puzzle-piece|microphone|microphone-slash|shield|calendar-o|fire-extinguisher|rocket|maxcdn|chevron-circle-left|chevron-circle-right|chevron-circle-up|chevron-circle-down|html5|css3|anchor|unlock-alt|bullseye|ellipsis-h|ellipsis-v|rss-square|play-circle|ticket|minus-square|minus-square-o|level-up|level-down|check-square|pencil-square|external-link-square|share-square|compass|toggle-down|caret-square-o-down|toggle-up|caret-square-o-up|toggle-right|caret-square-o-right|euro|eur|gbp|dollar|usd|rupee|inr|cny|rmb|yen|jpy|ruble|rouble|rub|won|krw|bitcoin|btc|file|file-text|sort-alpha-asc|sort-alpha-desc|sort-amount-asc|sort-amount-desc|sort-numeric-asc|sort-numeric-desc|thumbs-up|thumbs-down|youtube-square|youtube|xing|xing-square|youtube-play|dropbox|stack-overflow|instagram|flickr|adn|bitbucket|bitbucket-square|tumblr|tumblr-square|long-arrow-down|long-arrow-up|long-arrow-left|long-arrow-right|apple|windows|android|linux|dribbble|skype|foursquare|trello|female|male|gittip|sun-o|moon-o|archive|bug|vk|weibo|renren|pagelines|stack-exchange|arrow-circle-o-right|arrow-circle-o-left|toggle-left|caret-square-o-left|dot-circle-o|wheelchair|vimeo-square|turkish-lira|try|plus-square-o|space-shuttle|slack|envelope-square|wordpress|openid|institution|bank|university|mortar-board|graduation-cap|yahoo|google|reddit|reddit-square|stumbleupon-circle|stumbleupon|delicious|digg|pied-piper-square|pied-piper|pied-piper-alt|drupal|joomla|language|fax|building|child|paw|spoon|cube|cubes|behance|behance-square|steam|steam-square|recycle|automobile|car|cab|taxi|tree|spotify|deviantart|soundcloud|database|file-pdf-o|file-word-o|file-excel-o|file-powerpoint-o|file-photo-o|file-picture-o|file-image-o|file-zip-o|file-archive-o|file-sound-o|file-audio-o|file-movie-o|file-video-o|file-code-o|vine|codepen|jsfiddle|life-bouy|life-saver|support|life-ring|circle-o-notch|ra|rebel|ge|empire|git-square|git|hacker-news|tencent-weibo|qq|wechat|weixin|send|paper-plane|send-o|paper-plane-o|history|circle-thin|header|paragraph|sliders|share-alt|share-alt-square|bomb|angellist|area-chart|at|bell-slash|bell-slash-o|bicycle|binoculars|birthday-cake|bus|calculator|cc|cc-amex|cc-discover|cc-mastercard|cc-paypal|cc-stripe|cc-visa|copyright|eyedropper|futbol-o|google-wallet|ils|ioxhost|lastfm|lastfm-square|line-chart|meanpath|newspaper-o|paint-brush|paypal|pie-chart|plug|shekel|sheqel|slideshare|soccer-ball-o|toggle-off|toggle-on|trash|tty|twitch|wifi|yelp".split("|"),
    glyphIcons: "adjust|align-center|align-justify|align-left|align-right|arrow-down|arrow-left|arrow-right|arrow-up|asterisk|backward|ban-circle|barcode|bell|bold|book|bookmark|briefcase|bullhorn|calendar|camera|certificate|check|chevron-down|chevron-left|chevron-right|chevron-up|circle-arrow-down|circle-arrow-left|circle-arrow-right|circle-arrow-up|cloud|cloud-download|cloud-upload|cog|collapse-down|collapse-up|comment|compressed|copyright-mark|credit-card|cutlery|dashboard|download|download-alt|earphone|edit|eject|envelope|euro|exclamation-sign|expand|export|eye-close|eye-open|facetime-video|fast-backward|fast-forward|file|film|filter|fire|flag|flash|floppy-disk|floppy-open|floppy-remove|floppy-save|floppy-saved|folder-close|folder-open|font|forward|fullscreen|gbp|gift|glass|globe|hand-down|hand-left|hand-right|hand-up|hd-video|hdd|header|headphones|heart|heart-empty|home|import|inbox|indent-left|indent-right|info-sign|italic|leaf|link|list|list-alt|lock|log-in|log-out|magnet|map-marker|minus|minus-sign|move|music|new-window|off|ok|ok-circle|ok-sign|open|paperclip|pause|pencil|phone|phone-alt|picture|plane|play|play-circle|plus|plus-sign|print|pushpin|qrcode|question-sign|random|record|refresh|registration-mark|remove|remove-circle|remove-sign|repeat|resize-full|resize-horizontal|resize-small|resize-vertical|retweet|road|save|saved|screenshot|sd-video|search|send|share|share-alt|shopping-cart|signal|sort|sort-by-alphabet|sort-by-alphabet-alt|sort-by-attributes|sort-by-attributes-alt|sort-by-order|sort-by-order-alt|sound-5-1|sound-6-1|sound-7-1|sound-dolby|sound-stereo|star|star-empty|stats|step-backward|step-forward|stop|subtitles|tag|tags|tasks|text-height|text-width|th|th-large|th-list|thumbs-down|thumbs-up|time|tint|tower|transfer|trash|tree-conifer|tree-deciduous|unchecked|upload|usd|user|volume-down|volume-off|volume-up|warning-sign|wrench|zoom-in|zoom-out".split("|"),
    ToMiddleCenter: function (filter) {
        var width = $(filter).width();
        var size = OA.getWinSize();
        var height = $(filter).height();
        $(filter).css({ left: (size.width - width) / 2, top: (size.height - height) / 2 });
    },
    CurrentThemeColor: function () {
        var d = OA.isNull($.cookie('style_color'));

        if (d == null) d = OA.DefaultTheme;
        return d;
    },
    SetCookie: function (name, v) {
        $.cookie(name, v, { path: "/", expires: 365 });
    },
    AddValue: function (id, value, split, isFilter) {
        var ts = $.trim($(id).val());
        if (!split) split = ";";
        if (isFilter) isFilter = 1;
        else isFilter = 0;
        if (isFilter == 1 && ts.indexOf(value) >= 0) return;

        var strSplit = ts.substring(ts.length - 1, ts.length);
        if (strSplit != split && ts != "")
            ts += split;
        $(id).val(ts + value);
    },
    GetThemeColor: function (b) {
        if (b != 0) b = 1;
        var d = OA.CurrentThemeColor();
        return OA.ThemeColors[d][b];
    },
    TrimVal: function (o, nullValue) {
        if (typeof (nullValue) == "undefined") nullValue = null;
        if (typeof (o) == "undefined") return nullValue;
        if (o == null) return nullValue;
        o = $.trim(o);
        if (o == "") return nullValue;
        return o;
    },
    fmoney: function (s, n) {
        if (typeof (n) == "undefined" || n == null) n = 2;
        n = n > 0 && n <= 20 ? n : 2;
        s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
        var l = s.split(".")[0].split("").reverse(),
        r = s.split(".")[1];
        t = "";
        for (i = 0; i < l.length; i++) {
            t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
        }
        return t.split("").reverse().join("") + "." + r;
    },

    rmoney: function (s) {
        return parseFloat(s.replace(/[^\d\.-]/g, ""));
    },
    ParaVal: function (o, type, paraName) {
        paraName = OA.TrimVal(paraName);
        var url = OA.TrimVal($(o).attr("href"));
        if (url == null) url = OA.TrimVal($(o).attr("data-href"));
        if (url == null) //当前空值
            return paraName == null ? null : paraName + "=";
        var url = url.toLowerCase().replace(type.toLowerCase() + "://", "");
        url = OA.TrimVal(url);
        if (url == null) {
            var rel = OA.TrimVal($(o).attr('rel'));
            if (rel == null) rel = $(o);
            else {
                if (rel == "none")
                { return paraName != null ? paraName + "=" : null; }
                rel = $("#" + rel);
            }
            var relatt = OA.TrimVal($(o).attr('relAttr'));
            if (relatt == null) relatt = "val";

            switch (relatt) {
                case "val": url = rel.val(); break;
                case "text": url = rel.text(); break;
                case "html": url = rel.html(); break;
                default: url = rel.attr(relatt); break;
            }
            url = OA.TrimVal(url);
            return paraName == null ? url : paraName + "=" + escape(url == null ? "" : url);
        }
        else
            return paraName == null ? url :
                   paraName + "=" + url.replace("?", "&");
    },
    g: function (o) {
        if (OA.s(o)) {
            window.location.replace($(o).attr("href"));
        }
        return false;
    },
    s: function (o) {
        return OA.ShowSearchingStatus(o);
    },
    r: function (o) {
        var i = $(o).find("i:eq(0)");
        var oldClass = OA.TrimVal(i.attr("oldClass"));
        if (i.hasClass("glyphicon-repeat") && oldClass != null) {
            i.attr("class", oldClass); //i.attr("oldClass"));
        }
        return false;
    },
    ShowSearchingStatus: function (o) {
        var i = $(o).find("i:eq(0)");
        if (i.hasClass("glyphicon-repeat")) {
            i.attr("oldClass", i.attr("class"));
            //alert("不要重复点击，我还在工作呢！");
            return false;
        }
        else {  //var i=$(o).find("i:eq(0)");
            i.attr("oldClass", i.attr("class"));
            i.attr("class", OA.Loading);
            return true;
        }
    },

    switchUser: function (d) {
        window.top.location.replace(OA.Root + "admin/share/switchUser.aspx?userid=" + d[0].UserID);
        return false;
    },

    GoApp: function (appid, url) {
        appid = OA.isNull(appid, "");
        url = OA.isNull(url, "");
        if (url != "") url = "/" + url;
        window.open(OA.Root + "api/p2p_go.aspx?appid=" + escape(appid) + "&url=" + escape(url), "_blank");
        return false;
    },

    ReplaceAll: function (s, value, newValue) {
        var p = new RegExp(value, "ig");
        newValue = newValue ? newValue : "";
        if (s == null || s == "") return s;
        return s.replace(p, newValue);
    },
    Reload: function () {
        dsGrid.Reload();
        return false;
    },

    FormatAutoText: function (data, item, key) {
        var v = item;
        for (var d in data) {
            var val = $(data).attr(d);
            v = OA.ReplaceAll(v, "{" + d + "}", val);
            v = OA.ReplaceAll(v, "{\\$" + d + "}", OA.AddKeyValue(val, key));
        }

        return v;
    },

    ResizeFixedMenu: function (id) {
        var $t = $("#" + id);
        var ww = $(window).width();
        var hh = $(window).height();
        var n = (ww - 1000) / 2 + 1000 + 10;
        var h = (hh - $($t).height()) / 2;
        if (n + $($t).width() > ww)
            n = ww - $($t).width() - 10;
        $($t).css("left", n).css("top", h);
    }
,

    AddKeyValue: function (m, key) {
        return OA.ReplaceAll(m, key, '<font style="color:red">' + key + '</font>');
    },

    MainSearch: function () {
        var key = $.trim($('#mainSearchKeys').val());
        var url = $("#mainSearchType").attr("go-url");
        if (key == "" && url.indexOf("admin/address/list.aspx") >= 0)
            url = OA.Root + "admin/address/dept.aspx";
        else
            url = OA.Root + $("#mainSearchType").attr("go-url") + "&keys=" + escape($('#mainSearchKeys').val());
        OA.ShowSearchingStatus($("#mainSearchSubmit"));
        window.location.href = url;
        return false;
    },
    MainSearchTypeClick: function (obj) {
        var s = $(obj).text().split("：");
        var className = $(obj).find("i").attr("class");
        var gourl = OA.isNull($(obj).attr("go-url"));
        if (gourl == null) gourl = "#";
        $("#mainSearchType").html("<i style='font-size:14px;line-height:14px;' class='" + className + "'></i>" + s[0] + ":");
        $("#mainSearchType").attr("go-url", gourl);
        // $("#mainSearchType").attr("href",gourl);
        $("#mainSearchType").attr("data-format", $(obj).attr("data-format"));
        $("#mainSearchType").attr("data-val", $(obj).attr("data-val"));
        $("#mainSearchType").attr("data-url", $(obj).attr("data-url"));
        $("#mainSearchType").attr("data-width", $(obj).attr("data-width"));
        // $("#mainSearchTypeListMenu").hide();
        return false;
    },
    ShowAutoText: function (o) {
        var config = OA.isNull($(o).attr("config"), o);
        var url = $(config).attr("data-url");

        var format = $(config).attr("data-format");
        var valAttr = $(config).attr("data-val");
        var rel = OA.isNull($(o).attr("rel"), config);
        var key = $.trim($(o).val());

        var min = $(config).attr("data-min");
        if (isNaN(min)) min = 2;
        var width = $(config).attr("data-width");
        if (isNaN(width)) width = 300;
        var time = $(config).attr("data-time");
        if (isNaN(time)) time = 20;
        //   if(key.length<min)
        //   {  $(rel).hide();
        //      return;
        //   }
        $(rel).width(width);
        //$(rel).html("<i class='"+ OA.Loading+"'></i>");
        //$(rel).show();

        $.ajax({
            url: OA.Root + url,
            data: { key: key },
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data, status) {
                if (data.ok == 0) { $(rel).hide(); return; }
                data = data.data;
                var ts = "<ul class='SearchResult'>";
                for (var d in data) {
                    ts += "<li val='" + $(data[d]).attr(valAttr) + "'>" + OA.FormatAutoText(data[d], format, key) + "</li>";
                }
                ts += "<li><p align='right' onclick='$(\"" + rel + "\").hide();' style='cursor:pointer'><small>>></small> 关闭</p></li>";
                ts += "</ul>";
                $(rel).show();
                $(rel).html(ts);
                $(rel).find("li>a").click(function () {
                    $(o).val($(this).parent().attr("val"));
                    $(rel).hide();
                });
            },
            error: function () {
                $(rel).hide();
            }
        });
    },

    FormatValue: function (obj, attrList, formatItem) {
        if (attrList == null || $.trim(attrList) == "") return null;
        if (formatItem == null || $.trim(formatItem) == "") return null;
        var ts = attrList.split(",");
        for (var i = 0; i < ts.length; i++) {
            formatItem = OA.ReplaceAll(formatItem, "\\{" + ts[i] + "\\}", $(obj).attr(ts[i]));
        }
        return formatItem;
    },

    InsertText: function (o, value) {
        var obj = $(o).get(0);

        if (document.selection) {
            obj.focus();
            var sel = document.selection.createRange();
            sel.text = value;
        } else
            if (typeof (obj.selectionStart) === 'number' && typeof (obj.selectionEnd) === 'number') {
                var startPos = obj.selectionStart;
                var endPos = obj.selectionEnd;
                var tmpStr = obj.value;
                obj.value = tmpStr.substring(0, startPos) + value + tmpStr.substring(endPos, tmpStr.length);
            } else {
                obj.value += value;
            }
        return false;
    },

    LoadUserRealMsg: function () {
        setTimeout(function () {
            OA.GetUserNotice();
        }, 15000);

        return false;
        setTimeout(function () {
            OA.GetUserIEmail();
        }, 30000);
    },
    GetUserIEmail: function () {  //return false;
        $.get(OA.Root + "api/UserIEmail.aspx?_=" + OA.Random(),
            function (data) { //data=eval("("+data+")");
                if (data.ok == 0) return;
                data = data.data;
                var nNew = data.n;
                if (nNew == 0) return;
                //$('span.badge').text(nNew);
                //$('#mainImailCounter').text(nNew);
                $("span.UserNewEmailCount").text(nNew);
                $("#UserNewEmailList").html(data.html);
                $.extend($.gritter.options, { position: 'bottom-right' });
                var unique_id = $.gritter.add({
                    title: '内部邮件',
                    text: '你的内部邮件箱中有 ' + nNew + ' 封新邮件，注意查收！',
                    image1: OA.Root + 'admin/assets/img/avatar1.jpg',
                    sticky: true,
                    time: '',
                    class_name: 'my-sticky-class'
                });

                $.extend($.gritter.options, { position: 'bottom-right' });

                setTimeout(function () {
                    $.gritter.remove(unique_id, {
                        fade: true,
                        speed: 'slow'
                    });
                }, 7000);

                $('#header_inbox_bar').pulsate({
                    color: "#dd5131",
                    repeat: 10
                });
            });
    },

    GetUserNotice: function () {
        $.get(OA.Root + "api/UserNotice.aspx?_=" + OA.Random(),
            function (d) {
                //d=eval("("+d+")");
                if (d.ok == 0) return;
                //alert(d.data);
                data = d.data;
                var nNew = data.n;
                if (nNew == 0) return;

                //$('#header_notification_bar .badge').text(nNew);
                //$('#mainNoticeCounter').text(nNew);
                $("span.UserNewNoticeCount").text(nNew);
                $("#UserNewNoticeList").html(data.html);

                $.extend($.gritter.options, { position: 'bottom-right' });

                var unique_id = $.gritter.add({
                    title: '消息通知',
                    text: '你有 ' + nNew + ' 条新的消息，请注意查看。',
                    image1: OA.Root + 'admin/assets/img/image1.jpg',
                    sticky: true,
                    time: '',
                    class_name: 'my-sticky-class'
                });

                setTimeout(function () {
                    $.gritter.remove(unique_id, {
                        fade: true,
                        speed: 'slow'
                    });
                }, 7000);

                $.extend($.gritter.options, {
                    position: 'bottom-right'
                });

                $('#header_notification_bar').pulsate({
                    color: "#66bce6",
                    repeat: 10
                });
            });
    },

    GetRelObj: function (o) {
        var t = $($(o).attr("rel"));
        if (t.length == 0)
            return null;
        else
            return t;
    },
    Random: function () {
        return Math.random() * Math.random() * Math.random() * Math.random();
    },
    GetFileExt: function (fileName) {
        var n = fileName.lastIndexOf(".");
        if (n == -1) return null;
        return fileName.substring(n, fileName.length).toLowerCase();
    },
    ShowJson: function (obj) {
        alert(JSON.stringify(obj));
        return false;
    },
    getWinSize: function () {
        return {
            width: (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth),
            height: (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight)
        };
    },

    SelectDepts: function (ismuli, BmType, caption, rel, relName) {
        BmType = OA.isNull(BmType, "");
        caption = OA.isNull(caption, "部门选择");
        ismuli = OA.isNull(ismuli, "0");
        rel = OA.isNull(rel, "");
        relName = OA.isNull(relName, "");
        var w = 960;
        var h = 600;
        var deptList = OA.Dialog(caption, OA.Root + "admin/share/selectDepts.aspx?type=" + BmType + "&ismuli=" + ismuli + "&caption=" + escape(caption) + "&rel=" + escape(rel), w, h);
        if (deptList == null) return;
        if (rel != "" || relName != "") {
            var rs = "";
            var rsName = "";
            for (var i = 0; i < deptList.length; i++) {
                rs += deptList[i].DeptID + (i < deptList.length - 1 ? "," : "")
                rsName += deptList[i].DeptName + (i < deptList.length - 1 ? "," : "")
            }
            if (rs != "" && rel != "") $(rel).val(rs);
            if (relName != "" && rsName != "") { OA.InsertText(relName, rsName); $(relName).focus(); }
        }
        return deptList;
    },

    ToUp: function (filter) {
        var o = $("select" + filter);
        if (o.length == 0) return false;

        listObj = $(o).get(0);

        var selIndex = listObj.selectedIndex;
        if (selIndex < 0) {
            alert("请先选中一项！");
            return false;
        }
        if (selIndex == 0) {
            alert("已经移到第一位！");
            return false;
        }
        var selValue = listObj.options[selIndex].value;
        var selText = listObj.options[selIndex].text;
        listObj.options[selIndex].value = listObj.options[selIndex - 1].value;
        listObj.options[selIndex].text = listObj.options[selIndex - 1].text;
        listObj.options[selIndex - 1].value = selValue;
        listObj.options[selIndex - 1].text = selText;
        listObj.selectedIndex = selIndex - 1;
        return false;
    },
    SetObjAttrValue: function (rel, relAttr, data) {
        if (rel == null) return;
        switch (relAttr) {
            case "val":
            case "value": $(rel).val(data); break;
            case "html":
            case "htm": $(rel).html(data); break;
            case "text": $(rel).text(data); break;
            case "none": break;
            default: $(rel).attr(relAttr, data); break;
        }
    },
    ToTop: function (filter) {
        var o = $("select" + filter);
        if (o.length == 0) return false;

        listObj = $(o).get(0);

        var selIndex = listObj.selectedIndex;
        if (selIndex < 0) {
            alert("请先选中一项！");
            return false;
        }
        if (selIndex == 0) {
            alert("已经移到第一位！");
            return false;
        }

        for (var i = selIndex; i >= 1; i--) {
            var selValue = listObj.options[i].value;
            var selText = listObj.options[i].text;

            listObj.options[i].value = listObj.options[i - 1].value;
            listObj.options[i].text = listObj.options[i - 1].text;
            listObj.options[i - 1].value = selValue;
            listObj.options[i - 1].text = selText;
            //listObj.selectedIndex=selIndex-1;
        }
        listObj.selectedIndex = 0;
        return false;
    },

    ToDown: function (filter) {
        var o = $("select" + filter);
        if (o.length == 0) return false;
        listObj = $(o).get(0);
        var selIndex = listObj.selectedIndex;
        if (selIndex < 0) {
            alert("请先选中一项！");
            return false;
        }
        if (selIndex == listObj.options.length - 1) {
            alert("已经移到最后一位！");
            return false;
        }
        var selValue = listObj.options[selIndex].value;
        var selText = listObj.options[selIndex].text;
        listObj.options[selIndex].value = listObj.options[selIndex + 1].value;
        listObj.options[selIndex].text = listObj.options[selIndex + 1].text;
        listObj.options[selIndex + 1].value = selValue;
        listObj.options[selIndex + 1].text = selText;
        listObj.selectedIndex = selIndex + 1;
        return false;
    },

    ToBottom: function (filter) {
        var o = $("select" + filter);
        if (o.length == 0) return false;
        listObj = $(o).get(0);
        var selIndex = listObj.selectedIndex;
        if (selIndex < 0) {
            alert("请先选中一项！");
            return false;
        }
        if (selIndex == listObj.options.length - 1) {
            alert("已经移到最后一位！");
            return false;
        }
        for (var i = selIndex; i < listObj.options.length - 1; i++) {
            var selValue = listObj.options[i].value;
            var selText = listObj.options[i].text;
            listObj.options[i].value = listObj.options[i + 1].value;
            listObj.options[i].text = listObj.options[i + 1].text;
            listObj.options[i + 1].value = selValue;
            listObj.options[i + 1].text = selText;
        }
        listObj.selectedIndex = listObj.options.length - 1;
        return false
    },

    SelectUsers: function (ismuli, deptid, isLimit, caption, rel, relName) {
        caption = OA.isNull(caption, "用户选择");
        ismuli = OA.isNull(ismuli, 0);
        isLimit = OA.isNull(isLimit, 0);
        deptid = OA.isNull(deptid, "");
        rel = OA.isNull(rel, "");
        relName = OA.isNull(relName, "");
        var w = isLimit == 0 ? 960 : 900;
        var h = isLimit == 0 ? 600 : 550;
        var returnUsers = OA.Dialog(caption, OA.Root + "admin/share/selectDeptUser.aspx?deptid=" + deptid + "&ismuli=" + ismuli + "&islimit=" + isLimit + "&caption=" + escape(caption) + "&rel=" + escape(rel), w, h);
        if (returnUsers == null) return null;
        if (rel != "" || relName != "") {
            var rs = "";
            var rsName = "";
            for (var i = 0; i < returnUsers.length; i++) {
                rs += returnUsers[i].UserID + (i < returnUsers.length - 1 ? "," : "");
                rsName += returnUsers[i].UserName + (i < returnUsers.length - 1 ? "、" : "");
            }
            if (rel != "") $(rel).val(rs);
            if (relName != "") { OA.InsertText(relName, rsName); $(relName).focus(); }
        }
        return returnUsers;
    },
    SelectRoleUsers: function (ismuli, roleID, caption, rel, split, relName) {
        caption = OA.isNull(caption, "用户选择");
        rel = OA.isNull(rel, "");
        relName = OA.isNull(relName, "");
        roleID = OA.isNull(roleID, "bmUser");
        ismuli = OA.isNull(ismuli, 0);
        split = OA.isNull(split, ",");
        isLimit = 1;//OA.isNull(isLimit,0);

        //deptid =OA.isNull(deptid,"");
        var w = 960;
        var h = 700;
        var returnUsers = OA.Dialog(caption, OA.Root + "admin/share/selectRoleUser.aspx?roleid=" + roleID + "&ismuli=" + ismuli + "&islimit=" + isLimit + "&caption=" + escape(caption) + "&rel=" + escape(rel), w, h);
        if (returnUsers == null) return null;

        if (rel != "" || relName != "") {
            var rs = "";
            var rsName = "";
            for (var i = 0; i < returnUsers.length; i++) {
                rs += returnUsers[i].UserID + (i < returnUsers.length - 1 ? "," : "");
                rsName += returnUsers[i].UserName + (i < returnUsers.length - 1 ? "、" : "");
            }
            if (rel != "") $(rel).val(rs);
            if (relName != "") { OA.InsertText(relName, rsName); $(relName).focus(); }
        }
        return returnUsers;
    },

    AttachIsView: function (fileExt) {
        return ",.doc,.docx,.ppt,.pptx,.pdf,.xls,.xlsx,".indexOf("," + fileExt + ",") >= 0;
    },

    AttachInputGroup: function (obj) {
        var obj = $(obj).parents("div[GroupID][fileType][catelog]");
        if (obj.length == 0) {
            alert("没找到相关的文件上传附件");
            return null;
        }
        else
            return obj;
    },

    AttachRemoveJsonObj: function (group, AttachID) {
        var data = $(group).data("data");
        if (AttachID == null)
            AttachID = $(group).find("select option:selected").val();
        for (var i = 0; i < data.length; i++) {
            if (data[i].AttachID == AttachID) {
                data = data.slice(0, i).concat(data.slice(i + 1, data.length));
                $(group).data("data", data);
                return;
            }
        }
    },
    AttachGetJsonObjAttr: function (group, AttachID, attrName) {
        var d = AttachGetJsonObj(group, AttachID);
        if (d == null) return null;
        else
            return d[attrName];
    },
    AttachGetJsonObj: function (group, AttachID) {
        var data = $(group).data("data");
        if (data == null) return null;
        if (data.length == 0) return null;

        var isAttachs = group.hasClass("attachs");
        if (!isAttachs) {
            return data[0];
        }

        if (AttachID == null)
            AttachID = $(group).find("select option:selected").val();
        var obj = null;
        for (var i = 0; i < data.length; i++) {
            if (data[i].AttachID == AttachID) {
                return data[i];
            }
        }
        return null;
    },

    AttachSetValue: function (group) {
        var data = $(group).data("data");
        var rel = $(group).attr("rel");
        var isAttachs = $(group).hasClass("attachs");
        var s = "";

        if (data != null && data.length > 0) {
            if (isAttachs) {
                $(group).find("select option").each(function () {
                    s += $(this).val() + "|";
                });

                if (s != "") s = s.substring(0, s.length - 1);
            }
            else {
                s = data[0].AttachID;
            }
        }
        $(rel).val(s);
    },

    AttachAddJsonObj: function (group, f) {
        var data = $(group).data("data");
        if (typeof (data) == "undefined" || data == null)
            data = [];
        data.push(f);
        $(group).data("data", data);
    },

    AttachAttr: function (obj, attrName) {
        var group = OA.AttachInputGroup(obj);
        if (group == null) return;
        var data = $(group).data("data");
        var isAttachs = group.hasClass("attachs");
        var AttachID = null;
        if (isAttachs) {
            return OA.AttachGetJsonObj(group, null, attrName);
        }
        else {
            obj = $(group).find("input:text");
            if (data.length == 0)
                return null;
            else
                return data[0][attrName];
        }
    },

    AttachInit: function (group) {
        var isAttachs = $(group).hasClass("attachs");
        if (!isAttachs) {
            var attachid = OA.isNull($(group).attr("AttachID"));
            if (attachid == null)
                $(group).attr("AttachID", $(group).attr("GroupID"));
        }
        else {
            $(group).attr("AttachID", "");
        }

        var obj = $(group).find("input,select");
        OA.AttachReload(obj, false);
    },

    OAAttachJsonData: function (fileInfo, GroupID, catelog) {
        var p = 0;
        var n = fileInfo.indexOf("-", 0);
        var title = fileInfo.substring(n + 1);
        var d = {
            AttachID: fileInfo,
            GroupID: GroupID,
            FileExt: OA.GetFileExt(fileInfo),
            FileSize: -1,
            IsPrint: 1,
            IsView: 1,
            IsCopy: 1,
            IsDown: 1,
            Title: title,
            FileInfo: fileInfo,
            WebUrl: OA.Root + 'data/' + catelog + "/" + (p > 0 ? fileInfo.substring(1, p) + "/" : "") + fileInfo,
            DownUrl: OA.Root + 'data/' + catelog + "/" + (p > 0 ? fileInfo.substring(1, p) + "/" : "") + fileInfo,
            ViewUrl: OA.Root + 'data/' + catelog + "/" + (p > 0 ? fileInfo.substring(1, p) + "/" : "") + fileInfo
        };

        return d;
    },
    AttachData: function (GroupID) {
        var d = $($(GroupID)).data("data");
        if (!d) return [];
        else return d;
    },
    AttachReload: function (obj, bAlert) {
        if (typeof (bAlert) == "undefined") bAlert = true;
        var group = OA.AttachInputGroup(obj);
        if (group == null) return;
        var rel = OA.isNull($(group).attr("rel"));
        if (rel == '#') rel = null;
        var GroupID = $(group).attr("GroupID");
        var isAttachs = group.hasClass("attachs");
        $(group).removeData("data");
        //if(!isAttachs) return; //单选不支持刷新
        $(group).data("data", []); //空数组

        if (rel != null) {
            $(group).attr("isOldOA", 1);
            var s = OA.isNull($(rel).val());

            var data = [];
            if (s != null) {
                var ts = s.split("|");
                for (var i = 0; i < ts.length; i++) {
                    data.push(OA.OAAttachJsonData(ts[i], GroupID, $(group).attr("catelog")));
                }
            }
            // OA.ShowJson(data);

            $(group).data("data", data); //保存当前的值

            if (isAttachs) {
                $(group).find("select option").each(function () {
                    $(this).remove();
                });

                obj = $(group).find("select");
                for (var i = 0; i < data.length; i++) {
                    var f = data[i];
                    $(obj).append("<option value='" + f.AttachID + "'>" + f.Title + "</option>");
                }
            }
            else {
                if (data.length > 0) {
                    obj = $(group).find("input");
                    $(obj).val(data[0].Title);
                }
            }
            return;
        }
        $(group).attr("isOldOA", 0);
        var data = { GroupID: GroupID, AttachID: group.attr("AttachID"), _: OA.Random(), OnlyOkStatus: $(obj).attr("OnlyOkStatus") == "1" ? 1 : 0 };
        var url = OA.Root + "admin/share/Attach_Group.aspx";
        $.get(url, data, function (data) {
            //data=eval("("+data+")");
            if (data.ok == 0) {
                if (bAlert)
                    alert("刷新失败，错误信息 = " + data.msg);
                return;
            }

            if (data.data == null) data.data = [];

            $(group).data("data", data.data); //保存当前的值

            if (isAttachs) //多重附件
            {
                $(group).find("select option").each(function () {
                    $(this).remove();
                });

                obj = $(group).find("select");
                for (var i = 0; i < data.data.length; i++) {
                    var f = data.data[i];
                    $(obj).append("<option value='" + f.AttachID + "'>" + f.Title + "</option>");
                }
            }
            else {
                obj = $(group).find("input:text");
                $(obj).val(data.data.length > 0 ? data.data[0].Title : "");
            }
            OA.AttachSetValue(group);
        });

        return;
    },

    AttachRemove: function (obj) {
        var group = OA.AttachInputGroup(obj);
        if (group == null) return;

        var jsonData = OA.AttachGetJsonObj(group, null);
        if (jsonData == null) {
            alert("没有文件被选中,不能删除");
            return;
        }
        var isAttachs = group.hasClass("attachs");

        var AttachID = jsonData.AttachID;
        var GroupID = jsonData.GroupID;
        var OldTitle = jsonData.Title;

        if (!window.confirm("你真想删除文件“" + OldTitle + "”？")) return;

        if ($(group).attr("isOldOA") == "1") {
            select = $(group).find("select option:selected");
            OA.AttachRemoveJsonObj(group, AttachID);
            $(select).remove();
            OA.AttachSetValue(group);
            return;
        }
        var data = { action: "remove", attachID: AttachID, GroupID: GroupID };
        $.get(OA.Root + "admin/share/attach_action.aspx",
            data,
            function (data) {  //data=eval("("+data+")");
                if (data.ok == 0) {
                    alert("删除失败，错误信息 = " + data.msg);
                    return;
                }

                if (isAttachs) //多文件上传

                {
                    // data =$(group).data("data");

                    select = $(group).find("select option:selected");
                    OA.AttachRemoveJsonObj(group, AttachID);
                    $(select).remove();
                }
                else {
                    $(group).removeData("data");//移去原数据

                    select = $(group).find("input:text");
                    $(group).data("data", []);//空值

                    $(select).val("");
                }
                OA.AttachSetValue(group);
            }
       );

        return false;
    },

    AttachUpdate: function (obj) {
        var group = OA.AttachInputGroup(obj);
        if (group == null) return;
        if ($(group).attr("isOldOA") == "1") {
            alert("当前文件上传处在老版OA的编辑模式，不支持附件的修改！");
            return false;
        }
        var jsonData = OA.AttachGetJsonObj(group, null);
        if (jsonData == null) {
            alert("没有文件被选中,不能更改属性");
            return;
        }
        var isAttachs = group.hasClass("attachs");
        var AttachID = jsonData.AttachID;
        var GroupID = jsonData.GroupID;
        var OldTitle = jsonData.Title;
        var url = OA.Root + "admin/share/Attach_Update.aspx?isPrint=" + jsonData.IsPrint
        + "&isview=" + jsonData.IsView
        + "&iscopy=" + jsonData.IsCopy
        + "&isdown=" + jsonData.IsDown
        + "&attachid=" + AttachID
        + "&GroupID=" + GroupID
        + "&title=" + escape(OldTitle);
        var f = OA.Dialog("附件属性更改", url, 750, 380);
        if (f == null) return false;
        if (isAttachs) //多文件上传

        {
            select = $(group).find("select option:selected");
            select.text(f.Title);
        }
        else //单文件上传

        {
            select = $(group).find("input:text");
            $(select).val(f.Title);
        }
        jsonData.Title = f.Title;
        jsonData.IsPrint = f.IsPrint;
        jsonData.IsCopy = f.IsCopy;
        jsonData.IsView = f.IsView;
        jsonData.IsDown = f.IsDown;
        OA.AttachSetValue(group);
    },

    AttachDown: function (obj) {
        var group = OA.AttachInputGroup(obj);
        if (group == null) return;

        var jsonData = OA.AttachGetJsonObj(group, null);
        if (jsonData == null) {
            alert("没有文件被选中,不能下载");
            return;
        }
        window.open("/tools/down.aspx?s=" + escape(jsonData.WebUrl), "_blank");
        return false;
    },

    AttachInfo: function (obj) {
        var group = OA.AttachInputGroup(obj);
        if (group == null) return;
        if ($(group).attr("isOldOA") == "1") {
            alert("当前文件上传处在老版OA的编辑模式，不支持附件的详细属性查看！");
            return false;
        }

        var jsonData = OA.AttachGetJsonObj(group, null);
        if (jsonData == null) {
            alert("没有文件被选中,不能查看详情");
            return;
        }

        var AttachID = jsonData.AttachID;
        var GroupID = jsonData.GroupID;
        OA.Dialog("附件详情", OA.Root + "admin/share/Attach_Info.aspx?AttachID=" + AttachID + "&GroupID=" + GroupID + "&_" + OA.Random(), 800, 450);
        return false;
    },

    AttachView: function (obj) {
        var group = OA.AttachInputGroup(obj);
        if (group == null) return;
        var jsonData = OA.AttachGetJsonObj(group, null);

        if (jsonData == null) {
            alert("没有文件被选中,不能预览");
            return;
        }
        if (!OA.AttachIsView(jsonData.FileExt)) {
            alert("系统暂不支持扩展名为[" + jsonData.FileExt + "]的文件在线预览！");
            return false;
        }
        window.open("/tools/preview.aspx?s=" + jsonData.WebUrl, "_blank");
        return false;
    },

    AttachSort: function (obj) {
        var group = OA.AttachInputGroup(obj);
        if (group == null) return;
        var isAttachs = group.hasClass("attachs");
        var GroupID = $(group).attr("GroupID");
        var rel = $(group).attr("id");
        if (!isAttachs) {
            alert("单文件不支持排序");
            return false;
        }
        var n = $(group).find("select option");
        if (n.length < 2) {
            alert("设置排序时，需要2个以上附件才行!");
            return false;
        }
        var f = OA.Dialog("附件排序设置", OA.Root + "admin/share/Attach_OASort.aspx?groupid=" + GroupID + "&_=" + OA.Random() + "&rel=" + escape(rel), 800, 400);

        if (f == null) return;

        var t = f.split("|");
        var html = "";
        for (var i = 0; i < t.length; i++) {
            var o = $(group).find('select option[value="' + t[i] + '"]');
            if (o.length == 1)
                html += o.get(0).outerHTML;
        }
        $(group).find('select').html(html);
        OA.AttachSetValue(group);
    },
    AttachBigUpload: function (obj) {
        alert("系统暂不支持大文上传!");
        return false;
    },
    FileUpload: function (title, catelog, fileType, GroupID, AttachID) {
        AttachID = (AttachID ? AttachID : "");
        return OA.Dialog(title, OA.Root + "admin/share/upload.aspx?AttachID=" + AttachID + "&groupid=" + GroupID + "&filetype=" + fileType + "&catelog=" + catelog, 750, 380);
    },
    AttachUpload: function (obj) { //$(window).dropdown('toggle');
        var group = OA.AttachInputGroup(obj);
        if (group == null) return;
        var isAttachs = group.hasClass("attachs");
        var GroupID = $(group).attr("GroupID");
        var isOldOA = $(group).attr("isOldOA");
        var fileType = OA.isNull($(group).attr("fileType"), "");
        var catelog = $(group).attr("catelog");
        var AttachID = OA.isNull($(group).attr("AttachID"));
        if (AttachID == null) AttachID = "";
        //alert(AttachID);
        var f = OA.Dialog("附件上传", OA.Root + "admin/share/upload.aspx?AttachID=" + AttachID + "&groupid=" + GroupID + "&filetype=" + fileType + "&catelog=" + catelog + "&IsOldOA=" + isOldOA, 750, 380);
        if (f == null) return;
        if (isAttachs) //多文件上传

        {
            var select = $(group).find("select");
            $(select).append("<option value='" + f.AttachID + "' selected><i class='fa fa-user'></i>" + f.Title + "</option>");
            OA.AttachAddJsonObj(group, f);
        }
        else //单文件上传

        {
            var select = $(group).find("input:text");
            var data = [];
            data[0] = f;
            $(select).val(f.Title);
            $(group).data("data", data);
        }
        OA.AttachSetValue(group);
    },

    iDialog: function (URL, Width, Height, isScrolling, CloseFunction) {
        OA.InitReturnVal();

        $.fancybox.open({
            href: URL,
            padding: 0,
            margin: 0,
            scrolling: (isScrolling ? 'yes' : 'no'),
            modal: true,
            width: Width,
            autoSize: false,
            autoResize: false,
            height: Height,
            title: null,
            closeBtn: true,
            afterClose: function () { if (typeof (CloseFunction) == "function") CloseFunction(OA.GetReturnVal()); },
            mouseWheel: true,
            iframe: {
                scrolling: (isScrolling ? 'yes' : 'no'),
                preload: false
            },
            type: 'iframe'
        });

        return false;
    },
    CopyToClipboard: function (s) {
        if (window.clipboardData)
            window.clipboardData.setData("Text", s);
        else
            if (navigator.userAgent.indexOf("Opera") != -1)
                window.location = s;
            else if (window.netscape) {
                try {
                    netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
                } catch (e) {
                    alert("被浏览器拒绝！\n请在浏览器地址栏输入'about:config'并回车\n然后将'signed.applets.codebase_principal_support'设置为'true'");
                    return false;
                }
                var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
                if (!clip) return false;
                var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
                if (!trans) return false;
                trans.addDataFlavor('text/unicode');
                var str = new Object();
                var len = new Object();
                var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
                var copytext = s;
                str.data = copytext;
                trans.setTransferData("text/unicode", str, copytext.length * 2);
                var clipid = Components.interfaces.nsIClipboard;
                if (!clip) return false;
                clip.setData(trans, null, clipid.kGlobalClipboard);
            }
        return true;
    },
    ShowUser: function (s) {
        alert("查看用户详情暂未实现！");
        return false;
    },
    ShowDept: function (s) {
        alert("查看部门详情暂未实现！");
        return false;
    },
    GetReturnVal: function () {
        return $(window.document.body).data("returnVal");
    },
    InitReturnVal: function () {
        return $(window.document.body).data("returnVal", null);
    },
    SetReturnVal: function (obj) {
        //alert("设置返回值="+ obj);
        //alert(window.parent.openWindow);
        if (window.parent.openWindow)
            window.parent.returnValue = obj;
        else
            if (window.parent != window && window.parent.jQuery)
                window.parent.jQuery(window.parent.document.body).data("returnVal", obj);
            else
                window.parent.returnValue = obj;
    },
    CloseMe: function (isReload) {
        var isDialog = (window.parent != window) && (window.parent.isDialog == true);
        var byFancybox = false;
        var p = null;
        if (isDialog == true)
            p = window.parent.window.dialogArguments;
        else {
            byFancybox = (window.parent != window) && (window.parent.jQuery) && window.parent.jQuery.fancybox
            p = window.parent.window;
        }
        if (byFancybox) {
            window.parent.jQuery.fancybox.close();
        }
        else
            if (window.parent != window) {
                window.parent.close();
            }
            else {
                window.close();
            }
        if (isReload) {
            try {
                if (p.dsGrid)
                    p.dsGrid.Reload();
                else
                    p.location.replace(p.location.href);
                /*
                if (isDialog) {
                    if (p.dsGrid)
                        p.dsGrid.Reload();
                    else
                        p.location.replace(p.location.href);
                }
                else
                {
                }
                window.parent.jQuery && window.parent.window.dsGrid ?
                   window.parent.window.dsGrid.Reload()
                : window.parent.openWindow && window.parent.openWindow.dsGrid ? window.parent.openWindow.dsGrid.Reload() : window.parent.location.replace(window.parent.location.href);
                ;*/
            } catch (ee) {; }
        }

        /*
          window.parent!=window && window.parent.jQuery && window.parent.jQuery.fancybox ?
          window.parent.jQuery.fancybox.close():
          window.parent!=window ? window.parent.close():window.close();
        */

        return false;
    },
    isEnumValue: function (value, enumList, defaultValue) {
        value = OA.isNull(value);
        if (value == null) return defaultValue;
        var s = enumList.split(",");
        for (var i = 0; i < s.length; i++)
            if (value == s[i]) return s[i];
        return defaultValue;
    },
    HrefPara: function (o, para) {
        var href = $(o).attr("href");
        var href = OA.isNull(href);
        var para = OA.isNull(para);
        if (href == null) return null;
        var n = href.indexOf("://");
        if (n > 0) {
            href = href.substring(n + 3, href.length);
            if (para != null) {
                n = href.indexOf("?");
                if (n >= 0)
                    href == "?" + para + "=" + href.substring(0, n) + "&" + href.substring(n, href.length);
                else
                    href = "?" + para + "=" + href;
            }
        }
        return href;
    },
    isNull: function (o, value, elseNull) {
        if (typeof (value) == "undefined") value = null;
        if (typeof (o) == "undefined" || o == null) return value;
        o = $.trim(o);
        if (o == "") return value;
        if (elseNull && o == elseNull) return null;
        return o;
    },
    isNumeric: function (o) {
        if (o && /^-?\d+(\.\d+)?$/.test(o)) {
            return true;
        }
        return false;
    },
    Win: function (Title, URL, bm, Width, Height, WindowType, isChangeSize, isScrolling) {
        if (URL == null) return false;
        var hasDialog = typeof (window.showModalDialog) != "undefined" ? true : false;
        var paras = (hasDialog && bm) ? OA.getDlgPara(Width, Height, WindowType, isChangeSize, isScrolling) : OA.getWinPara(Width, Height, WindowType, isChangeSize, isScrolling);
        if (hasDialog && bm)
            window.showModalDialog(OA.Root + "js/showDialog.htm?Title=" + escape(Title) + "&URL=" + escape(OA.Root + URL) + "&_$=" + OA.Random(),
            window,
            paras);
        else
            window.open(URL, "_window", paras);
        return false;
    },

    Dialog: function (Title, url, w, h) {
        var hasDialog = typeof (window.showModalDialog) != "undefined" ? true : false;
        if (!hasDialog) {
            alert("浏览器系统不支持弹出对话框，请使用IE,Safari,Firefore浏览！");
            return null;
        }
        var paras = OA.getDlgPara(w, h, true, false, false);
        var f = window.showModalDialog(OA.Root + "js/showDialog.htm?title=" + escape((Title)) + "&url=" + escape((url)), window, paras);
        return f;
    },

    iWin: function (Title, URL, winType, Width, Height, isScrolling) {
        if (URL == null) return false;
        Width = Width ? Width : 800;
        Height = Height ? Height : 600;
        if (Width < 100) Width = 800;

        $.fancybox.open({
            href: URL,
            padding: 0,
            margin: 0,
            scrolling: 'no',
            modal: (winType == "idialog"),
            width: parseInt(Width, 10),
            autoSize: false, autoResize: false,
            height: parseInt(Height, 10),
            title: null,
            closeBtn: true,
            mouseWheel: true,
            iframe: {
                scrolling: (isScrolling ? 'yes' : 'no'),
                preload: false
            },
            type: 'iframe'
        });

        return false;
    },

    getWinPara: function (Width, Height, winKind, isChangeSize, isScrolling) {
        var w = $(window).width();
        Width = (Width ? Width : 800);
        Height = (Height ? Height : 600);
        var h = $(window).height();
        var s = "toolbar=no,menubar=no,location=no,width=" + Width + ",height=" + Height + ",resizable=" + (isChangeSize ? "yes" : "no") + ",scrollbars=" + (isScrolling ? "yes" : "no") + ",";
        switch (winKind) {
            case "lu":
            case "lt":
                return s + "Top=0,Left=0";
            case "ru":
            case "rt":
                return s + "Top=0,Left=" + (w - Width);
            case "lb": //Left Bottom
            case "ld":
                return s + "Top=" + (h - Height) + ",left=0";
            case "rb": //Right Bottom
            case "rd":
                return s + "Top=" + (h - Height) + "," + "left=" + (w - Width);
            case "uc":
            case "tc":  //上中
                return s + "Width=" + Width + ",Height=" + Height + "," +
							"top=0,left=" + (w - Width) / 2;
            case "dc":
            case "bc":  //下中
                return s + "Width=" + Width + ",Height=" + Height + "," +
							"Top=" + (h - Height) + ",Left=" + (w - Width) / 2;
            default:
                return s + "left=" + (w - Width) / 2 + ",top=" + (h - Height) / 2;
        }
    },
    getDlgPara: function (Width, Height, winKind, isChangeSize, isScrolling) {
        var s = "status=no;scroll=" + (isScrolling ? "yes" : "no") + ";resizable=" + (isChangeSize ? "yes" : "no");
        Width = (Width ? Width : 800);
        Height = (Height ? Height : 600);
        var w = $(window).width();
        var h = $(window).height();
        switch (winKind) {
            case "lu":
            case "lt":
                return "dialogWidth=" + Width + "px;dialogHeight=" + Height + "px;" +
							"dialogTop=0px;dialogLeft=0px;" + s;
            case "uc":
            case "tc":  //上中
                return "dialogWidth=" + Width + "px;dialogHeight=" + Height + "px;" +
							"dialogTop=0px;dialogLeft=" + (w - Width) / 2 + "px;" + s;
            case "dc":
            case "bc":  //下中
                return "dialogWidth=" + Width + "px;dialogHeight=" + Height + "px;" +
							"dialogTop=" + (h - Height) + "px;dialogLeft=" + (w - Width) / 2 + "px;" + s;
            case "ru":
            case "rt":
                return "dialogWidth=" + Width + "px;dialogHeight=" + Height + "px;" +
							"dialogTop=0px;dialogLeft=" + (w - Width) + "px;" + s;
            case "lb":
            case "ld":
                return "dialogWidth=" + Width + "px;dialogHeight=" + Height + "px;" +
							"dialogTop=" + (h - Height) + "px;dialogLeft=0px;" + s;
            case "rb":
            case "rd":
                return "dialogWidth=" + Width + "px;dialogHeight=" + Height + "px;" +
							"dialogTop=" + (h - Height) + "px;" +
							"dialogLeft=" + (w - Width) + "px;" + s;
            default:
                return "dialogWidth=" + Width + "px;dialogHeight=" + Height + "px;" + s;
        }
    },
    isObjNull: function (o, attr, value, elseNull) {
        return OA.isNull($(o).attr(attr), value, elseNull);
    },

    i: function (o) {
        if (typeof (o) == "string") o = $("#" + o);
        if (o == null || o && o.length == 0) return false;
        //alert(o);
        var href = OA.isObjNull(o, "href", null, '#');
        if (href == null) return false;

        href = href.replace("~/", OA.Root);
        $(o).attr("href", href);
        //alert(href);

        var rel = OA.isNull($(o).attr("rel"));
        var method = OA.isNull($(o).attr("method"));
        if (method == "go" || method == "to" || method == "goto") {
            OA.ShowSearchingStatus(o);
            window.location.replace(OA.Root + href);
            return false;
        }

        if (method == "get" || method == "post") {
            OAF.fnAJAX(o, method);
            return false;
        }

        var userApp = href.split("://");
        if (userApp.length >= 2) {
            var method = "fn" + userApp[0].toUpperCase();
            var f = OAF[method];
            if (typeof (f) == "function") { f(o); return false; }
        }
        var target = OA.isObjNull(o, "target", null, '_self');
        var winSize = OA.isObjNull(o, "winSize", null);

        var winType = OA.isObjNull(o, "winType", null);

        var Width = null;
        var Height = null, winAt,
              isChangeSize = OA.isObjNull(o, "isChangeSize", null),
              isScrolling = OA.isObjNull(o, "scroll", null);
        //alert(winType);
        if (winSize != null) {
            var size = OA.getWinSize();
            if (winType == null) winType = 'iwin';
            var aDataSize = winSize.split(",");

            if (aDataSize.length > 0) {
                Width = OA.isNull(aDataSize[0]);
                if (Width == null) Width = size.width;
            }
            if (aDataSize.length > 1) {
                Height = OA.isNull(aDataSize[1]);
                if (Height == null) Height = size.height;
            }

            if (aDataSize.length > 2) isScrolling = aDataSize[2];
            if (aDataSize.length > 3) {
                winType = (aDataSize[3] == '1' ? 'idialog' : 'iwin');
            }
        }

        //alert(winType);
        isOpt = true;

        switch (winType) {
            case "iwin":
            case "idialog":
                //alert(winType);
                OA.iWin($(o).attr("title"), href, winType, Width, Height, isScrolling);
                break;
            case "win":
            case "dialog":
                OA.Win($(o).attr("title"), href, winType == "dialog",
                  Width, Height, winAt, isChangeSize, isScrolling);
                break;
            default:
                $(o).attr("target", winType);
                isOpt = false;
                return true;
        }
        return false;
    },

    Call: function (o, paraName) {
        var fnCall = OA.isNull($(o).attr('fnCall'));
        if (fnCall == null) {
            //alert("当前没有定义回调函数（处理可能无效）");
            return false;
        }
        try {
            eval(fnCall + "(paraName,$(o))");
            return true;
        }
        catch (e) {
            alert("执行出错,错误 = " + e.message);
            return false;
        }
    }
};

OAF =
{
    "fnAJAX": function (o, method, dataType, fnOk, fnErr) {
        // alert("1");
        var rel = OA.GetRelObj(o);
        method = OA.isNull(method, OA.isNull($(o).attr("method"), "get"));
        dataType = OA.isNull(dataType, OA.isNull($(o).attr("dataType"), "html"));

        fnOk = OA.isNull($(o).attr("fnOk"));

        if (fnOk == null) fnOk = OA.isNull($(o).attr("fnCall"));

        isReload = OA.isNull($(o).attr("reload"), 0);
        //if(fnOk!=null) fnOk=eval(fnOk);
        fnErr = OA.isNull($(o).attr("fnErr"));
        //if(fnErr!=null) fnErr=eval(fnOk);
        var i = $(o).find("i.fa,i.glyphicon");
        if (OA.isNull(i.attr("isLoading")) == "1") {
            alert("数据在处理中，请稍候再试！");
            return false;
        }
        var oldClass = i.attr("class");
        var relAttr = OA.isNull($(o).attr("relAttr"), "html");
        var fnData = OA.isNull($(o).attr("fnData"));
        var data = null;

        if (fnData != null) {
            try {
                if (fnData.indexOf("(") < 1)
                    fnData = fnData + "($(o))";
                data = eval(fnData);
                if (data == null) return false;
            }
            catch (e) {
                alert("动态取数据有问题，请检查后再试试");
                return false;
            }
        }

        if (data == null) {
            var dataList = OA.isNull($(o).attr("data"));

            if (dataList != null) {
                try {
                    data = eval("(" + dataList + ")");
                }
                catch (e) {
                    alert("传递的参数设置有问题");
                    return false;
                }
            }
        }

        if (data == null) data = {};

        //alert(href);

        var href = OA.isNull($(o).attr("data-href"));

        if (href == null) href = OA.isNull($(o).attr("href"));
        if (href == null || href == "#") return false;
        if (href.toLowerCase().indexOf("isreload=1") > 0) {
            href += (href.indexOf("?") > 0 ? "&" : "?") + "_" + OA.Random() + "=" + OA.Random();
        }
        else

            if (isReload == 1) {
                data._ = OA.Random();
                if (href.toLowerCase().indexOf("isreload=1") < 0)
                    data.isReload = 1;
            }

        i.attr("isLoading", 1);
        i.attr("class", OA.Loading);
        jQuery.ajax({
            url: href,
            data: data, async: true,
            type: method,
            dataType: dataType,
            error: function (request) {
                if (typeof (fnErr) == "function")
                    fnErr();
                else
                    if (typeof (fnErr) == "string")
                    { if (fnErr != "null") eval(fnErr + "(request)"); }
                    else
                        alert("获取数据出错！");
            },
            complete: function () { i.attr("class", oldClass); i.attr("isLoading", 0); },
            success: function (data) {
                OA.SetObjAttrValue(rel, relAttr, data);
                if (typeof (fnOk) == "function")
                    fnOk(data);
                else
                    if (typeof (fnOk) == "string") {
                        { if (fnOk != "null") eval(fnOk + "(data)"); }
                    }
            }
        });
        return false;
    },
};

dsGrid =
{
    Reload: function (funcList) { dsGrid.DoPost("_R|"); },

    Init: function () {
        var e = $("form.frmFind[name!=__VIEWSTATE]");
        if (e.length == 0) return;
        e = e.get(0).elements;
        var cv = '';
        for (var i = 0; i < e.length; i++) {
            cv = dsGrid.getNameValue(e[i].name);
            if (typeof (cv) == "undefined" || e[i].name == "__VIEWSTATE") continue;
            if (e[i].tagName == 'SELECT') {
                if (e[i].multiple) {
                    var allS = dsGrid.getNameValus(e[i].name);
                    for (var j = 0; j < e[i].options.length; j++) {
                        if (_isIn(allS, e[i].options[j].value))
                            e[i].options[j].selected = true;
                    }
                }
                else
                    if (cv != '') e[i].value = cv;
            }
            else
                if (cv != '' && e[i].type != 'button' && e[i].type != 'submit' && e[i].type != 'reset')
                    e[i].value = dsGrid.getNameValue(e[i].name);
        }
    },

    getNameValus: function (Name) {
        var vs = '';
        $("#_DSFORM input[name='" + Name + "']").each(
        function () {
            vs += String.fromCharCode(1) + (this).val() + String.fromCharCode(2);
        });
        return (vs);
    },

    setNull: function () {
        var e = document.forms['frmFind'].elements;
        for (var i = 0; i < e.length; i++) {
            if (e[i].type != 'button' && e[i].type != 'submit' && e[i].type != 'reset' && e[i].name == '__VIEWSTATE' && e[i].name == '__EVENTVALIDATION') e[i].value = '';
        }
        return (false);
    },
    getNameValue: function (Name) {
        return $("#_DSFORM input[name='" + Name + "']").val();
    },

    isIn: function (allS, s) {
        return (allS.indexOf(s) >= 0);
    },

    DoPost: function (funcList) {
        if (typeof (funcList) == 'undefined') funcList = null;
        var f = $("#_DSFORM");

        if (funcList != null && funcList != '') {
            var s = funcList.split(',');
            for (var i = 0; i < s.length; i++) {
                var ts = s[i].split('|');
                var name = ts[0];
                var obj = f.find("[name='" + name + "']");

                if (ts[1].length > 1) {
                    var t = ts[1].charAt(0);
                    if (name != '_N' && (t == '+' || t == '-')) {
                        var oldn = parseInt($(obj).val(), 10);
                        if (t == "+")
                            $(obj).val(ts[1], oldn + parseInt(ts[1].substr(1), 10));
                        else
                            $(obj).val(ts[1], oldn - parseInt(ts[1].substr(1), 10));
                    }
                    else
                        $(obj).val(ts[1]);
                }
                else
                    $(obj).val(ts[1]);
            }
        }
        f.submit();
        return (false);
    }
}

$(function () {
    if (window.location.href.toLowerCase().indexOf("/xoa.v2") > 0) {
        OA.Root = "/xoa.v2/";
    }
    else {
        OA.Root = "/";
    }

    dsGrid.Init();
    //为非弹出式窗口加入立体效果

    if ($(document.body).hasClass("main")) {
        var uid = $("#My_User_Info").attr("uID");
        var token = $("#My_User_Info").attr("token");
        var d = OA.CurrentThemeColor();
        $("ul#themeColorList li").each(function () {
            var ts = $(this).attr("data-style");
            if (ts == d)
                $(this).addClass("current");
            else
                $(this).removeClass("current");
        });

        $("form.frmFind").submit(function () {
            var b = true;
            var reg = null;
            var form = $(this);
            $(this).find("input:text").each(function () {
                if (b) {
                    var s = $(this).val();
                    reg = /~.*/;
                    //reg = /[^\u4e00-\u9fa50-9a-zA-Z\: $_\-\,\(\)]+/;
                    if (s != "" && reg.test(s)) {
                        b = false;
                        event.stopImmediatePropagation();
                        $(form).find("input:submit,button:submit").each(
                        function () { OA.r(this); });
                        alert("搜索内容中只能输入英文字符、数字或者汉字等有意义的内容！");
                        $(this).focus();
                    }
                }
            });
            if (b) {
                $(this).find("input:submit,button:submit").each(
                function () {
                    OA.s(this);
                });
            }
            return b;
        });
    }
    else
        if (window.top == window && $("div#ReadPageInfo").length == 1)
            $(window.document.body).addClass("Page-3D");

    OA.MainSearchTypeClick($("ul#mainSearchTypeListMenu a.selected"));

    $('ul.nav-tabs[redirect=1] li a').click(function (e) {
        e.preventDefault();//阻止a链接的跳转行为
        var a = $(this);
        $(a).tab('show');//显示当前选中的链接及关联的content
        var href = $(this).attr("data-href");

        if (href != "#" && href != null) {
            var thref = href.toLowerCase();
            if (thref.indexOf("mid=") < 0 && thref.indexOf("$id=") < 0) {
                var mid = $("form.frmFind input[name=MID]").val();
                if (href.indexOf("?") > 0)
                    href += "&mid=" + mid;
                else
                    href += "?mid=" + mid;
            }
            OA.ShowSearchingStatus($(this)) ? window.location.replace(href) : null;
        }
        else {
            var fnClick = OA.isNull($(this).attr("fnClick"));
            if (fnClick != null) {
                try { eval(fnClick + "($(this))"); }
                catch (e) {; }
            }
        }
        return false;
    });
});