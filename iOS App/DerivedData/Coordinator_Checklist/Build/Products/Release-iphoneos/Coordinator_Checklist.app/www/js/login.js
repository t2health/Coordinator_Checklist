function doLogin() {
	$.mobile.loading( 'show', {
		text: 'Authenticating...',
		textVisible: true,
		theme: 'a',
		html: ""
	});
	$('#login_message').hide();
	$('#connect_message').hide();
	$.get('http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Account/Login', function(response) {
		var token = $(response).find('input[name=__RequestVerificationToken]').val();
		postLogin(token);
	}).error(function(xhr, textStatus, thrownError) {
		if (xhr.status == 488) {
			$.mobile.loading('hide');
	    	$.mobile.changePage('pending.html');
		} else if (xhr.status == 408) {
			$('#connect_message').show();
		}
	}).complete(function() {
		$.mobile.loading('hide');
	});
}

function postLogin(token) {
	var user = $('#username').val();
	var pass = $('#password').val();
	$.post('http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Service/Login', {UserName:user, Password:pass, __RequestVerificationToken:token}, function(response) {
		$.mobile.changePage('list.html');
	}).error(function(xhr, textStatus, thrownError) {
		if (xhr.status == 488) {
			$.mobile.loading('hide');
	    	$.mobile.changePage('pending.html');
		} else if (xhr.status == 408) {
			$('#connect_message').show();
		} else if (xhr.status == 409) {
			$('#login_message').show();
		}
	}).complete(function() {
		$.mobile.loading('hide');
	});
}

function doLogout() {
	$.mobile.loading( 'show', {
		text: 'Logging Out...',
		textVisible: true,
		theme: 'a',
		html: ""
	});
	$.post('http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Account/LogOff', function(response) {
		$.mobile.loading('hide');
		$.mobile.changePage('login.html');
	}).error(function(xhr, textStatus, thrownError) {
		$.mobile.changePage('login.html');
	}).complete(function() {
		$.mobile.loading('hide');
	});
}

function doRegister() {
	var userMsg = $('#username_message');
	var firstMsg = $('#firstname_message');
	var lastMsg = $('#lastname_message');
	var passMsg = $('#password_message');
	var passLenMsg = $('#password_length_message');

	userMsg.hide();
	firstMsg.hide();
	passMsg.hide();
	lastMsg.hide();
	passLenMsg.hide();

	var user = $('#username').val();
	var first = $('#firstname').val();
	var last = $('#lastname').val();
	var pass = $('#password').val();
	var passConf = $('#password_confirm').val();

	valid = true;
	if (user === '') {
		userMsg.show();
		valid = false;
	}
	if (first === '') {
		firstMsg.show();
		valid = false;
	}
	if (last === '') {
		lastMsg.show();
		valid = false;
	}
	if (pass.length < 6) {
		passLenMsg.show();
		valid = false;
	}
	if (pass != passConf) {
		passMsg.show();
		valid = false;
	}

	if (!valid) {
		return;
	}

	$.mobile.loading( 'show', {
		text: 'Registering...',
		textVisible: true,
		theme: 'a',
		html: ""
	});

	$('#connect_message').hide();
	$.get('http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Account/Register', function(response) {
		var token = $(response).find('input[name=__RequestVerificationToken]').val();
		postRegister(token);
	}).error(function(xhr, textStatus, thrownError) {
		$('#connect_message').show();
	}).complete(function() {
		$.mobile.loading('hide');
	});


}

function postRegister(token) {
	var user = $('#username').val();
	var first = $('#firstname').val();
	var last = $('#lastname').val();
	var pass = $('#password').val();
	var passConf = $('#password_confirm').val();

	$.post('http://ec2-54-245-136-244.us-west-2.compute.amazonaws.com/Service/Register', { 
		UserName:user, 
		FirstName:first, 
		LastName:last, 
		Password:pass, 
		ConfirmPassword:passConf, 
		__RequestVerificationToken:token 
	}, function(response) {
		$.mobile.changePage('login.html', {'data':{message:3}});
	}).error(function(xhr, textStatus, thrownError) {
		if (xhr.status == 488) {
			$.mobile.loading('hide');
	    	$.mobile.changePage('pending.html');
		} else if (xhr.status == 408) {
			$('#connect_message').show();
		}
	}).complete(function() {
		$.mobile.loading('hide');
	});
}
