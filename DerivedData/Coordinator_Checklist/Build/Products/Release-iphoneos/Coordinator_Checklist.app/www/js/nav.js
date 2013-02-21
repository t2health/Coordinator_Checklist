$(document).on('pagehide', 'div', function(event, ui){
    var page = jQuery(event.target);

    if(page.attr('data-cache') == 'never') {
        page.remove();
    }
});

$(document).on('pageshow', '#loginPage', function (e, data) { 
    console.log('Login Page Shown');
    var message = getUrlParameter('message');
    if (message === '2') {
        $('#connect_message').show();
    } else if (message === '3') {
        $('#register_message').show();
    }
});

$(document).on('pageshow', '#listPage', function (e, data) { 
    console.log('List Page Shown');
    getChecklists();
});

$(document).on('pageshow', '#recipientPage', function (e, data) { 
    console.log('Recipient Page Shown');
});

$(document).on('pageshow', '#editPage', function (e, data) { 
    formData = {};
    results = null;
    currentStep = 0;
    editChecklistId = null;
    xmlWorkFlow = null;
    hasLocalStorage = testLocalStorage();

    console.log('Edit Page Shown');
    var id = getUrlParameter('id');
    if (id != 'null') {
        $('#title').text('Edit Checklist');
        $('#print').attr('href', 'http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Checklist/Print/' + id);
        loadChecklist(id);
    } else {
        editChecklistId = null;
        $('#title').text('Add Checklist');
        loadInterface();
    }
});

$(document).on('pageinit', function() {
    $.ajaxSetup({'timeout':10000});
});

function getUrlParameter(name) {
    return decodeURI(
        (RegExp(name + '=' + '(.+?)(&|$)').exec(location.search)||[,null])[1]
    );
}