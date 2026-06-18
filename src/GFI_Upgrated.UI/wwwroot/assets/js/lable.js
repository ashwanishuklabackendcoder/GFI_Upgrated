'use strict';

/////////// lable ////////////
var dtbasicLabeldetails;

    if (localStorage.getItem("Labels") != null && localStorage.getItem("Labels") != "")
        dtbasicLabeldetails = JSON.parse(localStorage.getItem("Labels"))
setTimeout(() => {
    if (dtbasicLabeldetails && dtbasicLabeldetails.length > 0) {
        for (var i = 0; dtbasicLabeldetails.length > i; i++) {
            if ($('.' + dtbasicLabeldetails[i].LabelName).html() != undefined) {
                $('.' + dtbasicLabeldetails[i].LabelName).html(dtbasicLabeldetails[i].English)
            }

        }
    }

    },
        200);
    ///////////////////

function lablefind(lblvalue) {
    var lablename = "";
    lablename = lblvalue;
    var objdata = JSON.parse(localStorage.getItem("Labels"));
    lablename = $(objdata).filter(function (i, n) {
        return n.LabelName === lblvalue;
    });
    if (lablename.length > 0)
        return lablename[0].English;
    else
        return "";
}