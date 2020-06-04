// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//
function mcCheckIE() {
    var a = navigator.appName === 'Netscape';
    var b = navigator.userAgent.search('Trident') !== -1;
    var c = navigator.userAgent.indexOf("msie") !== -1;
    var d = navigator.userAgent.indexOf("MSIE") !== -1;
    if (a && b || c || d) {
        alert("이 웹사이트는 Internet Explorer를 지원하지 않습니다.\n\n사유: W3C 표준 미준수");
        return false;
    }
    return true;
}

//contents div를 full screen으로 표시
function fullScreenContainer() {
    //debugger

    //var element = document.body;
    var element = document.getElementById("mcContainer");
    if (element === null || element === undefined) return;

    // Supports most browsers and their versions.
    var requestMethod = element.requestFullScreen || element.webkitRequestFullScreen
        || element.mozRequestFullScreen || element.msRequestFullscreen;

    // Native full screen.
    if (requestMethod) { requestMethod.call(element); }

    // Older IE.
    else if (typeof window.ActiveXObject !== "undefined") {
        var wscript = new ActiveXObject("WScript.Shell");
        if (wscript !== null) wscript.SendKeys("{F11}");
    }
}


//fire click event of the element
function submitExcel() {
    var file = document.getElementById('excel-file');
    file.click();
    if (jQuery(file).val !== "") document.getElementById('excel-submit').click();

}

//
// register Mvc-Grid row click event handler
//
function addRowClick(controllerPath, detailsPath) {
    var grid = document.querySelector('.mvc-grid');
    if (grid !== null && typeof grid !== 'undefined') grid.addEventListener('rowclick', function (e) {

        var orgTarget = jQuery(e.detail.originalEvent.target);
        if (!(orgTarget[0].parentNode.rowIndex) || orgTarget[0].parentNode.rowIndex < 1) return;

        window.location.href = '/' + controllerPath + "/" + detailsPath + "/" + e.target.dataset.id;
    });
}

