var bodylayout;
var homelayout;
var tabs;
var loggedIn = false;
var _loginform;
var tabcontent = {};
function getLoginForm (){
	if (_loginform == null){
		var tmp = $('#login-form');
		if (tmp.length > 0){
			_loginform = tmp;
		}else {
			return tmp;
		}
	}
	return _loginform;
}
var icons = {
    header: "ui-icon-folder-collapsed",
    headerSelected: "ui-icon-folder-open"
};
function resizeTabLayout() {
    if (!tabs) return;
    bodylayout.resizeAll();
}
$(document).ready(function() {
    setupLayout();
});
function setupLayout() {
    // remove the static non-javascript content
    $('#static-content').remove();
    // layout the outer frame
    bodylayout = $('body').layout({
        center__paneSelector: "#main",
        north__paneSelector: "#header",
        north__size: 100,
        spacing_open: 0,
        north__slidable: false
    });

    load("/Account/Logon");
}
function onLogin(){
	if ($('#tabs').length < 1){
		$.get("/AjaxHtml/Tabs", function(html) {
			$('#main').prepend(html);
		});
	}
	if (tabs == null){
		setupTabLayout();
	}
}
function DefaultJsonParse(data) {

    // remove loading div if it's there
    var loadingdiv = $('#loading');
    if (loadingdiv.is(":visible")){
        loadingdiv.fadeOut();
    };

    // if there's login data returned..
    if (data.LoggedIn != null) {
        if (data.LoggedIn) {
			getLoginForm().dialog("close")
            onLogin();
            return false;
        } else {
        	if (getLoginForm().length == 0) {
            	$.get("/AjaxHtml/LoginForm", function(html) {
                	$(document.body).append(html);
                	var submitButton = getLoginForm().find("input[type=submit]");
                	submitButton.hide();
                	var buttons = {};
                	buttons[submitButton.val()] = function() {
                    	load("/Account/Logon", $(this).find("form").serialize());
                	}
					getLoginForm().dialog({
						width: 350,
						modal: true,
						closeOnEscape: false,
						autoResize:true,
						open: function() {
							$(this).parents(".ui-dialog:first").find(".ui-dialog-titlebar-close").remove();
						},
						buttons: buttons
					});
            	});
        	} else {
            	getLoginForm().dialog();
            	if (data.ErrorMessages.length > 0){
					for(var i=0; i<data.ErrorMessages.length; i++){
						var error = data.ErrorMessages[i];
						if (error.Field == "_FORM"){
							getLoginForm().find("form").prepend(errorHtml("Error", error.Message));
						}else {
							var textBox = getLoginForm().find("input[name="+error.Field+"]");
							textBox.css("border", "1px solid #cc0000");
						}
					}
				}
         	}
        }
    	return true;
    }
    return false;
}
function errorHtml(title, text){
	return '<div style="padding: 0pt 0.7em;" class="ui-state-error ui-corner-all"><p><span style="float: left; margin-right: 0.3em;" class="ui-icon ui-icon-alert"></span><strong>'+title+':</strong> '+text+'</p></div>';

}
function load(url, data, callback) {
    $.ajax({
        url: url,
        type : data != null ? "POST":"GET",
        dataType: 'json',
        data: data,
        beforeSend: function(xhr) {
            xhr.setRequestHeader("HTTP_X_EXPECT_FORMAT", "json");
        },
        success: function(data) {
            if (!DefaultJsonParse(data)) {
                if (callback) {
                    callback(data);
                }
            }
        }
    });
}
function setupTabLayout() {
	$('#tabs a').each(function (){
			$(this).attr("href", "#"+$(this).attr("rel"));
	});
    var after = '<div id="trackhistory">history</div><div id="favourites">favourites</div><div id="myaccount">myaccount</div><div id="results"></div>';
    var results = '<div id="left"><div id="left-content"></div></div><div id="middle">dsfsdf</div><div id="right"><div id="right-content"></div></div>';
    $('#home').after(after);
    $('#results').append(results);
    tabs = $('#main').tabs({
        show: function (event, ui){
			resizeTabLayout();
			switch(ui.tab.rel){
				case "home":
					if (tabcontent["home"] == null){
						$.get("/AjaxHtml/Welcome", function (data){
							tabcontent["home"] = data;
							$('#home').html(tabcontent["home"]);
						});
					}
					break;
			}
		}

    });
    $('#results-tab').hide();
    $('#main').layout({
        center__paneSelector: ".tabs-panel-container",
        north__paneSelector: "#tabs",
        spacing_open: 0,
        north__slidable: false
    });

    resizeTabLayout();
}
function addArtist(name, id) {
    homelayout.open("west");
    var accordionLength = $('#left-content h3').length;
    if ($('#left-content').accordion != null) {
        $('#left-content').accordion("destroy");
    }
    if (accordionLength > 4) {
        $('#left-content h3:first').remove();
        $('#left-content div:first').remove();
        accordionLength--;
    }
    var html = '<h3><a href="#">' + name + '</a></h3>';
    html += '<div>Loading...</div>';
    $('#left-content').append(html);
    $('#left-content').accordion({
        fillSpace: true,
        icons: icons,
        active: accordionLength
    });
    var html = '<table class="ui-helper-reset" cellspacing="0"><tbody>';
    for (i = 0; i < 10; i++) {
        html += '<tr><td><a href="#">Test</a></td></tr>';
    }
    html += '</body></table>';
    $('.ui-accordion-content-active').html(html);
}
function showResultsTab() {
    tabs.tabs("select", '#results');
    if ($('#results-tab').is(':hidden')) {
        homelayout = $('#results').layout({
            east__paneSelector: "#right",
            //east__initClosed : true,
            west__initClosed: true,
            spacing_open: 4,
            spacing_closed: 4,
            center__paneSelector: "#middle",
            west__paneSelector: "#left",
            west__onresize: function() { $("#left-content").accordion("resize"); },
            east__onresize: function() { $("#right-content").accordion("resize"); }
        });
        homelayout.hide("west");
        homelayout.open("east");
        $('#results-tab').show();
    }
}