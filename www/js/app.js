//console.log('app js loaded');
var formData = {};
var results = null;
var currentStep = 0;
var xmlWorkFlow = null;
var historyCount = 0;
var hasLocalStorage = testLocalStorage();
var isPhoneGapRunning = false;

function onDeviceReady() {
    console.log('phonegap is running');
    isPhoneGapRunning = true;
}

function createDomainQuestionList(jsonQuestions, domainArray) {
    //var domainArray=NULL;
    var questionList = jsonQuestions;
    //var questionDomain = questionList[domainArray];
    //var domain = questionDomain.domain;
    //var domContainerID = domain.substring(0,4) + "-" + domainArray;
    //var categories = questionDomain.categories;
    //var listhtml = $('<div id="' + domContainerID + '" ></div>');
    var listhtml = $('<div data-role="collapsible-set">');

    $.each(questionList, function(q,i){

    var domainArray =q;
    //console.log('domain q: ' +q);
    var questionDomain = questionList[domainArray];
    //console.log('domain: ' +questionDomain.domain);
    var domain = questionDomain.domain;
    var domContainerID = domain.substring(0,4) + "-" + domainArray;
    var categories = questionDomain.categories;
     //console.log('domainid: ' +domContainerID);
    //var listhtml = $('<div id="' + domContainerID + '" ></div>');
    //listhtml.append($('<div data-role="header"><h1>' + domain + '</h1></div>'));
    listhtml.append($('<div id="' + domContainerID + '" data-role="collapsible" data-collapsed="false"><h3>' + domain + '</h3></div>'));
        for (var z in categories)
        {
            //console.log ('   ' + categories[z]);
            var category = categories[z];
            var catID = domContainerID + "-" + category.substring(0,3) + "-" + z;
            var catHtml = '';
            var namePrefix = '|' + domain + '|' + catID + '|';
            catHtml += '<div class="container"><fieldset data-role="controlgroup">';
            catHtml += '<div role="heading"><h3>Category: ' + category + '</h3></div>';
            catHtml += '<input type="hidden" name="domain" value="' + domain + '" disabled>';
            catHtml += '<input type="hidden" name="category" value="' + category + '" disabled>';
            catHtml += '<table width="100%"><tbody>'; 
            catHtml += '<tr>';
            catHtml += '<th scope="row">';
            catHtml += '<label style="margin-top:10px;" for="' + namePrefix + 'date" ><b>Date</b></label>';
            catHtml += '</th>';
            catHtml += '<th scope="row">';
            catHtml += '<label for="' + namePrefix + 'status" >Status</label>';
            catHtml += '</th>';
            catHtml += '</tr>';
            catHtml += '<tr>';
            catHtml += '<td style="padding-right:10px; width:70%;"><input type="date" data-mini="true" name="' + namePrefix + 'date" id="' + domainArray + '-'+ z + '-date" value="" class="ui-input-text ui-body-c ui-corner-all ui-shadow-inset"></td>';
            catHtml += '<td><select class="status" data-mini="true" name="' + namePrefix + 'status" id="' + domainArray + '-'+ z + '-status">';
            catHtml += '<option value="Not Started">Not Started</option>';
            catHtml += '<option value="In Progress">In Progress</option>';
            catHtml += '<option value="Completed">Completed</option>';
            catHtml += '<option value="NA">N/A</option>';
            catHtml += '</select></td>';
            catHtml += '</tr><tr><td colspan="2">';
            catHtml += '<label style="white-space:nowrap; font-weight:bold; padding-right:20px; line-height:35px;" for="' + namePrefix + 'responsible_party">Responsible Party</label>';
            catHtml += '<input type="text" name="' + namePrefix + 'responsible_party" id="' + domainArray + '-'+ z + '-responsible_party" value=""  />';
            catHtml += '</td></tr></tbody></table>';
            catHtml += '</fieldset></div>';
            var dom = $(catHtml);

            if (formData && formData['domains'] && formData['domains'][domainArray] && formData['domains'][domainArray]['categories'] && formData['domains'][domainArray]['categories'][z]) {
                $.each(formData['domains'][domainArray]['categories'][z], function(key, val) {
                    if (typeof val === 'object') {
                        return;
                    }

                    var elem = dom.find('#'+domainArray+'-'+z+'-'+key);
                    if (elem.length > 0) {
                        if (elem.is('input[type=text],input[type=email],input[type=date],input[type=phone],input[type=number],select')) {
                            elem.val(val);
                        } else if (elem.is(':checkbox, :radio')) {
                            elem.attr('checked',val);
                        }
                    }
                });
            }         
            $(listhtml).find('#' + domContainerID).append(dom);
            
        }

    });
    $(listhtml).collapsibleset();
    $(listhtml).collapsibleset( "refresh" ).children().trigger( "collapse" );
    listhtml.append('</div>');
    var domainAppend = '#domain_questions-0';// + domainArray;
    $(domainAppend).append(listhtml);
}

function hideSiblings(child) {
    var thisIndex = $('#'+child).index();
    //console.log('Index is ' + thisIndex);
    $('#'+child).siblings().not(':lt('+thisIndex+')').hide();    
    //    $(child).not(':eq(0)').hide();
    
}
function removeSiblings(child) {
    var thisIndex = $('#'+child).index();
    var itemCount = $('#'+child).siblings().each(function(){
        if(thisIndex < $(this).index()) $(this).remove();
    });
    
}

function getDataURL(dataFile) {
    var result = 'data/';
    if(isPhoneGapRunning) {
        if(device.name == 'iPhone Simulator') {
            //result = 'data/'
        }
    }
    result += dataFile;    
    return result;
}

function loadJSONQuestions(jsonFile, domainArray) {
    //console.log(getDataURL(jsonFile));
   // $.mobile.showPageLoadingMsg();
    $.ajax({
        url:getDataURL(jsonFile),
        dataType: 'json',
        success: function(data){
            //console.log('json success');
            jsonDomainQuestions = $(data);
            createDomainQuestionList(jsonDomainQuestions, domainArray);
           // $.mobile.hidePageLoadingMsg();
        }
    });
}

function loadWorkflow(xmlFile) {
    //console.log(getCurrentPage() + '       ' +getDataURL(xmlFile));
    //$.mobile.showPageLoadingMsg();
    $.ajax({
        url:getDataURL(xmlFile),
        dataType: 'xml',
        success: function(data){
            //console.log('xml success');
            xmlWorkFlow = $(data);
            parseWorkflow(xmlWorkFlow, 0, 0, 0);
            //$.mobile.hidePageLoadingMsg();

        }
    })
}

function parseWorkflow(xmlData, workflowCount, positiveOrNeg) {
    var currentIndex = 0;
    var nextPiece = false;
    var currentWFC = workflowCount;
    var tempStep = currentStep;
    var stepFound = false;
    $('step',xmlData).each(function(){
        if(tempStep == currentIndex){
            var posStepID = $(this).attr('positiveStepId'),
            negStepID = $(this).attr('negativeStepId'),
            neutStepID = $(this).attr('neutralStepId'),
            neutStepTitle = $(this).find('neutralStepTitle').text(),
            stepTitle = $(this).find('title').text(),
            stepContent = $(this).find('content').text(),
            stepDetailedContent = $(this).find('detailedContent').text(),
            newContainerID = 'stepDiv'+currentWFC;
            $('#algorithmSteps').append ('<div id="' + newContainerID + '" ></div>');
            var html = '';
            if(currentWFC > 0) {
                switch(positiveOrNeg) {
                    case -1:
                        html += '<div class="arrowDownRight"></div>';
                        break;
                    case 0:
                        html += '<div class="arrowDownMiddle"></div>';
                        break;
                    case 1:
                        html += '<div class="arrowDownLeft"></div>';
                        break;
                }
            } 

            //if(stepTitle != '') html += '<h4>' + stepTitle + '</h4>';
            if(stepContent != '') html +='<div class="ui-content ui-grid-solo"><div class="ui-block-a">' + stepContent + '</div></div>';
            if(stepDetailedContent != '') html +='<div>' + stepDetailedContent + '</div>';
            if(neutStepID > 0){
               // html += '<div data-role="footer">';
                html += '<a id="nextButton' + currentIndex + '" href="javascript:void(0);" onclick="makeDecision(\''+ newContainerID + '\',' + (currentWFC + 1) + ',' + neutStepID + ',0);" data-role="button" data-theme="b" >Next Section: ' + neutStepTitle +'</a>';
               // html += '</div>';
            } else if (posStepID > 0) {
                html += '<div class="ui-grid-a"><div class="ui-block-a" >';
                html += '<a href="javascript:void(0);" data-role="button" onclick="makeDecision(\''+ newContainerID + '\',' + (currentWFC + 1) + ',' + posStepID + ',1);">Yes</a>';
                html += '</div><div class="ui-block-b" >';
                html += '<a href="javascript:void(0);" data-role="button" onclick="makeDecision(\''+ newContainerID + '\',' + (currentWFC + 1) + ',' + negStepID + ',-1);">No</a>';
                html += '</div></div>';
            } else {
                nextPiece = true;
            }

            var dom = $(html);

            if (formData) {
                $.each(formData, function(key, val) {
                    if (typeof val === 'object') {
                        return;
                    }

                    var elem = dom.find('input[name='+key+']');
                    if (elem.length > 0) {
                        if (elem.is('input[type=text],input[type=email],input[type=date],input[type=phone],input[type=number],select')) {
                            elem.val(val);
                        } else if (elem.is(':checkbox, :radio')) {
                            elem.attr('checked',val);
                        }
                    }
                });
            }

            $('#' + newContainerID).append(dom).trigger('create').fadeIn();
            if(nextPiece) {
                currentWFC++;
            }

            if (currentIndex > 0) {
                var button = $('#nextButton' + (currentIndex-1));
                $.mobile.silentScroll(button.position().top - 50);
                button.remove();
            }

            currentStep++;
            stepFound = true;
            console.log(currentStep);
            return false;
        }
        currentIndex++;
    });

    return stepFound;        
}

function makeDecision(target, workflowCount, step, positiveOrNeg) {
    //removeSiblings(target);
    parseWorkflow(xmlWorkFlow, workflowCount, step, positiveOrNeg);
}

function adjustHistory() {
    historyCount--;
    console.log("historyCount = " + historyCount);
}

function goHome() {
    console.log("go(" + historyCount + ")");
    history.go(historyCount);
    historyCount = 0;
}

function testLocalStorage(){
    var result = false;
    if (typeof(localStorage) == 'undefined' ) {
        console.log('Your browser does not support HTML5 localStorage. Try upgrading.');
    } else {
        console.log('Your browser supports HTML5 localStorage.');
        try {
            localStorage.setItem("name", "Hello World!"); //saves to the database, "key", "value"
            result = true;
        } catch (e) {
            console.log(e);
            if (e == QUOTA_EXCEEDED_ERR) {
                console.log('Quota exceeded!'); //data wasn't successfully saved due to quota exceed so throw an error
            }
        }
        localStorage.removeItem("name"); //deletes the matching item from the database
    }
    return result;
}

function logPageChange(){
    console.log('logPageChange Called');
    logAnalytics('Page: '+ getCurrentPage());
}

function getCurrentPage() {
    var base = 'www';
    var result = '';
    if(location.hash) 
        result = location.hash.substring(1);
    else 
        result = location.pathname;
    var path = $.mobile.path.parseUrl(result).pathname;
    var indexPosition = path.indexOf(base);
    if(indexPosition > -1)
        path = path.substring(path.indexOf(base)+base.length+1);
    indexPosition = path.indexOf('&');
    if(indexPosition > -1) {
        var bar = $('.ui-header', $.mobile.activePage);
        var subPage = 'Nested-List ' + $('.ui-title', bar).html();
        path = path.substring(0,indexPosition) + ' ' + subPage;
    }
    return path;
}

function setDefaultTransition() {
    var winwidth = $( window ).width(),
    trans ="fade";
        
    if( winwidth >= 2000 ){
        trans = "none";
    }
    else if( winwidth >= 650 ){
        trans = "fade";
    }

    $.mobile.defaultPageTransition = trans;
}

$(function(){
    setDefaultTransition();
    $( window ).bind( "throttledresize", setDefaultTransition );
});

$('div').live('pageshow',function(event, ui){
    if(isPhoneGapRunning){
        logPageChange();
    }
    adjustHistory();
});

function loadInterface() {
    historyCount = 0;
    loadWorkflow('checklist.xml');
    // $("form.clform").live("submit", handleForm);  
    //$.mobile.loading('hide');
}


$('#domainQuestions').live('pageshow',function(event, ui){
    loadJSONQuestions('checklist.json');  
});