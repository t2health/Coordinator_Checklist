$(document).ready(function () {
    var dlg = $("#dialog").dialog({
        draggable: false,
        resizable: false,
        show: 'Transfer',
        hide: 'Transfer',
        width: 320,
        modal: true,
        buttons: {
            "Transfer": function () {
                $(this).dialog("close");
                window.location = transferUrl;
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        },
        autoOpen: false,
    });
});

var transferUrl;
function confirmTransfer(url) {
    transferUrl = url;
    $("#dialog").dialog("open");
}