var editChecklistId;
var transferChecklistId;
var transferRecipientId;

function newChecklist() {
	$.mobile.changePage('edit.html');
}

function newRegistration() {
	$.mobile.changePage('register.html');
}

function editChecklist(id) {
	$.mobile.changePage('edit.html', {data:{'id':id}});
}

function transferChecklist(id) {
    $.mobile.changePage('transfer.html', {data:{'id':id}});
}

function searchRecipients(query) {
    $.mobile.loading( 'show', {
        text: 'Searching...',
        textVisible: true,
        theme: 'a',
        html: ""
    });

    $('#users').empty();
    $('#users').hide();

    $.get('http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Service/Recipient?search=' + query, function(response) {
        var list = $('#users');
        var rows = '<li data-role="list-divider" role="heading">Recipients</li>';
        $(response).each(function(index, value) {
            rows += '<li><a href="#" onclick="$(\'#confirmDialog\').popup(\'open\'); transferRecipientId=' + value['id'] + '; transferChecklistId=' + getUrlParameter('id') + '; return false;">' + value['last_name']  + ', ' + value['first_name'] + '</a></li>';
        });
        list.append(rows);
        list.listview('refresh');
        list.slideDown();
    }, 'json').error(function(xhr, textStatus, thrownError) {
        if (xhr.status == 488) {
            $.mobile.loading('hide');
            $.mobile.changePage('pending.html');
        } else if (xhr.status == 408) {
            $.mobile.changePage('login.html', {'data':{message:2}});
        } else {
            $.mobile.changePage('login.html');
        }
    }).complete(function() {
        $.mobile.loading('hide');
    });
}

function getChecklists() {
	$.mobile.loading( 'show', {
		text: 'Loading Assigned Checklists',
		textVisible: true,
		theme: 'a',
		html: ""
	});

	$.get('http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Service/Checklist', function(response) {
		var list = $('#checklists');
		var rows = '<li data-role="list-divider" role="heading">Assigned Checklists</li>';
		$(response).each(function(index, value) {
			var date = new Date(value['modified_date']);
			rows += '<li><a href="#" onclick="editChecklist('+value['id']+'); return false;">' + value['title']  + '</a><a href="#" onclick="transferChecklist('+ value['id'] +'); return false;">Transfer</a></li>';
		});
		list.append(rows);
		list.listview('refresh');
		list.slideDown();
	}, 'json').error(function(xhr, textStatus, thrownError) {
        if (xhr.status == 488) {
            $.mobile.loading('hide');
            $.mobile.changePage('pending.html');
        } else if (xhr.status == 408) {
            $.mobile.changePage('login.html', {'data':{message:2}});
        } else {
            $.mobile.changePage('login.html');
        }
    }).complete(function() {
		$.mobile.loading('hide');
	});
}

function loadChecklist(checklistId) {
    if (!checklistId) {
        return; 
    }

    $.ajaxSetup({ 'timeout':8000 })

    $.mobile.loading( 'show', {
        text: 'Loading Checklist...',
        textVisible: true,
        theme: 'a',
        html: ""
    });

    $.get('http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Service/Checklist/' + checklistId, function(response) {
        formData = $.parseJSON(response['data']);
        editChecklistId = checklistId
        loadInterface();
    }, 'json').error(function(xhr, textStatus, thrownError) {
        editChecklistId = null;
        if (xhr.status == 488) {
            $.mobile.loading('hide');
            $.mobile.changePage('pending.html');
        } else if (xhr.status == 408) {
            $.mobile.changePage('login.html', {'data':{message:2}});
        } else {
            $.mobile.changePage('list.html');
        }
    }).complete(function() {
        $.mobile.loading('hide');
    });
}

function confirmTransfer() {
    var data = {'checklist_id':transferChecklistId, 'recipient_id':transferRecipientId};
    var url = 'http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Service/ChecklistTransfer';

    $.mobile.loading( 'show', {
        text: 'Transferring Checklist...',
        textVisible: true,
        theme: 'a',
        html: ""
    });

    $.ajaxSetup({ 'timeout':10000 })
    $.post(url, data, function() {
        $.mobile.changePage('list.html');
    }).error(function(xhr, textStatus, thrownError) {
        if (xhr.status == 488) {
            $.mobile.loading('hide');
            $.mobile.changePage('pending.html');
        } else if (xhr.status == 408) {
            $.mobile.changePage('login.html', {'data':{message:2}});
        } else {
            $('#failDialog').popup();
            $('#failDialog').popup('open');
        }
    }).complete(function() {
        $.mobile.loading('hide');
    });
}


function submitChecklist() {
	var json = parseChecklist();

	if (!json) {
		return;
	}

	var data = {'data':JSON.stringify(json)};
	var url = 'http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Service/ChecklistAdd';
	if (editChecklistId) {
		url = 'http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Service/ChecklistUpdate';
		data['id'] = editChecklistId;
	}

    $.mobile.loading( 'show', {
        text: 'Saving Checklist...',
        textVisible: true,
        theme: 'a',
        html: ""
    });

	$.ajaxSetup({ 'timeout':10000 })
	$.post(url, data, function() {
		$('#successDialog').popup();
		$('#successDialog').popup('open');
		if (!editChecklistId) {
			$.mobile.changePage('list.html');
		}
	}).error(function(xhr, textStatus, thrownError) {
        if (xhr.status == 488) {
            $.mobile.loading('hide');
            $.mobile.changePage('pending.html');
        } else if (xhr.status == 408) {
            $.mobile.changePage('login.html', {'data':{message:2}});
        } else {
            $('#failDialog').popup();
            $('#failDialog').popup('open');
        }
    }).complete(function() {
		$.mobile.loading('hide');
	});
}

function parseChecklist() {
    //gather the fields
    var data = $('#checklistForm').serializeArray();
    
    var json = { 'domains':[] };
    jQuery.each(data, function(i, field) {
        if (field['name'].charAt(0) == '|') {
            var split = field['name'].split('|');
            var domainName = split[1];
            var catName = split[2];
            var fieldName = split[3];
            
            var domain = null;
            $(json['domains']).each(function(index, data) {
                if (data['name'] == domainName) {
                    domain = data;
                    return false;
                }
            });

            if (!domain) {
                domain = {'name':domainName, 'categories':[]};
                json['domains'].push(domain);
            }

            var category = null;
            $(domain['categories']).each(function(index, data) {
                if (data['name'] == catName) {
                    category = data;
                    return false;
                }
            });

            if (!category) {
                category = {'name':catName};
                domain['categories'].push(category);
            }

            category[fieldName] = field['value'];
        } else {
            if (field['value'] == 'on') {
                json[field['name']] = true
            } else {
                json[field['name']] = field['value'];
            }
        }

    });

    return json;

    //return cordova.exec(null, null, "HTML5FormPlugin", "sendResults", [json]);
    
    //logFormResults(data);

};