
var uname = "";
var upwd = "";
function GetInfo() {
    $.ajax({
        type: "Get", //GET or POST or PUT or DELETE verb

        url: APIUrl + "api/Z_UsersLogins/GetInfo",
        data: {

            'grant_type': 'password'
        },

        dataType: "json", //Expected data format from server

        processData: true, //True or False

        crossDomain: true,

        async: false,

        success: function (result) {

            uname = result.split("|||")[0]
            upwd = result.split("|||")[1]
        },

        error: ServiceFailed  // When Service call fails
    });
}

function ServiceFailed(result) {

    //alert("Failed")
}

//Commented by Richa - 6Jan2026
//function GetOAuthToken()
//{

//    $.ajax({

//        type: "POST", //GET or POST or PUT or DELETE verb

//        url: APIUrl + "api/V1/Token",
//        data: {

//            'grant_type': 'password',
//            'UserName': uname,
//            'Password': upwd
//        },

//        headers: {
//           'Content-Type': 'application/x-www-form-urlencoded'

//        },
//        dataType: "json", //Expected data format from server

//        processdata: true, //True or False

//        crossDomain: true,

//        async: false,
//        success: function (result) {
//            ServiceSucceeded_GetOauthToken(result);
//        },
//        error: ServiceFailedGetToken  // When Service call fails

//    });
//}

function ServiceFailedGetToken(errord) {
    alert(JSON.stringify(errord))
}

function ServiceSucceeded_GetOauthToken(token) {
    var a = JSON.stringify(token)
    var b = token.expires_in;
    var b1 = b;
    // var expiredate = b1;
    var d = new Date();
    d.setSeconds(d.getSeconds() + b);
    //setCookie("access_token_expires", d, 30)
    localStorage.setItem("access_token_expires", d)
    // setCookie("access_token", token.access_token, 30)
    //setCookie("_fwacstkn", token.access_token, 30)
    localStorage.setItem("access_token", token.access_token)
}

//Written by Richa - 6Jan2026
function GetAccessToken() {

    let token = localStorage.getItem("access_token");
    let expiryStr = localStorage.getItem("access_token_expires");

    // Missing token or expiry
    if (!token || !expiryStr) {
        return GenerateToken();
    }

    let expiryDate = new Date(expiryStr);
    let now = new Date();

    // Invalid or expired date
    if (isNaN(expiryDate.getTime()) || expiryDate <= now) {
        return GenerateToken();
    }

    // API domain changed
    if (localStorage.getItem("ApiDomain") !== APIUrl) {
        return GenerateToken();
    }

    return token;
}

//Written by Richa - 6Jan2026
function GenerateToken() {

    GetInfo(); // get uname & upwd ONCE

    let token = null;

    $.ajax({
        type: "POST",
        url: APIUrl + "api/V1/Token",
        data: {
            grant_type: 'password',
            UserName: uname,
            Password: upwd
        },
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        async: false, // REQUIRED
        timeout: 10000,
        success: function (result) {

            token = result.access_token;

            localStorage.setItem("access_token", token);
            localStorage.setItem(
                "access_token_expires",
                new Date(Date.now() + (result.expires_in * 1000)).toISOString()
            );
            localStorage.setItem("ApiDomain", APIUrl);
        },
        error: function (xhr, textStatus, errorThrown) {
            localStorage.clear();
            console.error("Token API error");
            console.error("Status:", xhr.status);
            console.error("StatusText:", xhr.statusText);
            console.error("Response:", xhr.responseText);
            console.error("TextStatus:", textStatus);
            console.error("ErrorThrown:", errorThrown);

            // OPTIONAL: show readable message
            alert(
                "Token error\n" +
                "Status: " + xhr.status + "\n" +
                "Message: " + (xhr.responseText || xhr.statusText)
            );
            location.href = "/login";
            // Do NOT redirect immediately while debugging
            // location.href = "/login";
        }

    });

    return token;
}

//Commented by Richa - 6Jan2026
//function GetAccessToken() {

//    if (localStorage.getItem("access_token") == "" || localStorage.getItem("access_token") == null) {
//        GetInfo();
//        GetOAuthToken()
//    }
//    if (localStorage.getItem("ApiDomain") != APIUrl) {
//        //setCookie("ApiDomain", APIUrl, 30)
//        localStorage.setItem("ApiDomain", APIUrl)
//        GetInfo();
//        GetOAuthToken()
//    }

//    var tokenDatestring = localStorage.getItem("access_token_expires")
//    var currntdate = new Date()
//    var tokenDate = new Date(tokenDatestring)
//    if (tokenDate <= currntdate || tokenDate == null) {
//        GetInfo();
//        GetOAuthToken()
//    }

//    return localStorage.getItem("access_token");
//}

function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}
function setCookie(cname, cvalue, exdays) {

    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toGMTString();


    document.cookie = cname + "=" + cvalue

}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i].trim();
        if (c.indexOf(name) == 0) return decodeURIComponent(c.substring(name.length, c.length));
    }
    return "";
}

function deleteCookieByName(cname) {
    var d = new Date(); //Create an date object
    d.setTime(d.getTime() - (1000 * 60 * 60 * 24)); //Set the time to the past. 1000 milliseonds = 1 second
    var expires = "expires=" + d.toGMTString(); //Compose the expirartion date


    window.document.cookie = cname + "=" + "; " + expires;//Set the cookie with name and the expiration date

}
function alretsmg(smstype, msg) {
    $('.msg_alert').removeClass('msg_error')
    $('.msg_alert').removeClass('msg_success')
    $('.msg_alert').removeClass('invisible')
    $('.msg_alert').addClass(smstype)
    $('.msg_alert').html(msg)
    $(window).scrollTop(0);
    setTimeout(() => {
        $('.msg_alert').addClass('invisible')

    }, 15000); //15sec

}

function SetDateFotmat(datevalue) {

    if (datevalue != "" && datevalue != 'null' && datevalue != null && datevalue != undefined) {
        var d = new Date(datevalue);
        const options = { year: 'numeric', month: 'short', day: '2-digit' };
        datevalue = d.toLocaleDateString('en-US', options).replace(/,/g, '');
    }
    else {
        datevalue = ""
    }
    return datevalue

}

function SetTimeFotmat(datevalue) {
    if (datevalue != "" && datevalue != 'null' && datevalue != null && datevalue != undefined) {
        if (localStorage.getItem("TimeFormat") != 'HH:mm') {
            var hours = datevalue.getHours();
            var minutes = datevalue.getMinutes();
            var ampm = hours >= 12 ? 'pm' : 'am';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
            minutes = minutes < 10 ? '0' + minutes : minutes;
            datevalue = hours + ':' + minutes + ' ' + ampm;
        }
        else {
            var hours = datevalue.getHours();
            var minutes = datevalue.getMinutes();
            datevalue = hours + ':' + minutes;
        }
    }
    else {
        datevalue = ""
    }
    return datevalue

}

function SetTzDate(datevalue) {

    if (datevalue != "" && datevalue != 'null' && datevalue != null && datevalue != undefined) {
        var d = new Date(datevalue);
        datevalue = d.toLocaleString('en-US', { timeZone: localStorage.getItem("TimeZone") })
        datevalue = SetDateFotmat(datevalue);
    }
    else {
        datevalue = ""
    }
    return datevalue

}

function SetTzTime(datevalue) {
    if (datevalue != "" && datevalue != 'null' && datevalue != null && datevalue != undefined) {
        var d = new Date(datevalue);
        datevalue = d.toLocaleString('en-US', { timeZone: localStorage.getItem("TimeZone") })
        datevalue = SetTimeFotmat(datevalue);
    }
    else {
        datevalue = ""
    }
    return datevalue

}

//Added By Richa -6Jan2026
//$(document).ajaxError(function (e, xhr) {
//    if (xhr.status === 0 || xhr.status === 401) {
//        localStorage.clear();
//        alert("Session expired. Please login again.");
//        location.href = "/login";
//    }
//});

////Added By Richa -6Jan2026
//$.ajaxSetup({
//    beforeSend: function (xhr) {
//        const token = localStorage.getItem("access_token");
//        if (token) {
//            xhr.setRequestHeader("Authorization", "Bearer " + token);
//        }
//    }
//});


deleteCookieByName("access_token")
deleteCookieByName("AJSStoreID")
deleteCookieByName("ApiDomain")
deleteCookieByName("BashasStoreID")
deleteCookieByName("FoodCityID")
deleteCookieByName("TYC")
deleteCookieByName("UserId")
deleteCookieByName("UserID")
deleteCookieByName("ZipCode")
deleteCookieByName("Token")
deleteCookieByName("PCToken")
deleteCookieByName("StoreId")
