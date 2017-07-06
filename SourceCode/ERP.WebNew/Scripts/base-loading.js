//获取浏览器页面可见高度和宽度
var _PageHeight = document.documentElement.clientHeight,
    _PageWidth = document.documentElement.clientWidth;

//计算loading框距离顶部和左部的距离（loading框的宽度为215px，高度为61px）
var _LoadingTop = _PageHeight > 61 ? (_PageHeight - 61) / 2 : 0,
    _LoadingLeft = _PageWidth > 215 ? (_PageWidth - 215) / 2 : 0;

function loading() {
    var _LoadingHtml = '<div id="loadingDiv" style="height:' + document.documentElement.offsetHeight + 'px;"><div class="loadingText" style="left: ' + _LoadingLeft + 'px; top:' + _LoadingTop + 'px;">加载中，请等待...</div></div>';
    //呈现loading效果
    $("body").append(_LoadingHtml);
}

function completeLoading() {
    $('#loadingDiv').fadeOut(function () {
        $(this).remove();
    });
}