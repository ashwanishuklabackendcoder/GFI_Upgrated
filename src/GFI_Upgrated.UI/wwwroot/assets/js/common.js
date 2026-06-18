var APIUrl = document.getElementById("api-config").getAttribute("data-api-url");
var LoginName = localStorage.getItem("UserName");



var dteNow = new Date();
var intYear = dteNow.getFullYear();
//   document.getElementById('lblIntYear').innerHTML = intYear;

setTimeout(() => {
    const items = document.querySelectorAll(".menu-item");
    items.forEach(item => {
        item.addEventListener('click', function () {

            if (!item.classList.contains("open")) {
                item.classList.add("open")
            } else {
                item.classList.remove("open")
            }
        })
    })
},
    300);


$(document).ready(function () {

    $(".dob-picker").each(function () {

        let input = this;

        // init flatpickr
        let fp = flatpickr(input, {
            dateFormat: "d M Y",
            onReady: function (selectedDates, dateStr, instance) {

                if (!instance.calendarContainer.querySelector(".flatpickr-close-btn")) {
                    const closeBtn = document.createElement("span");
                    closeBtn.innerHTML = "✖";

                    Object.assign(closeBtn.style, {
                        position: "absolute",
                        right: "5px",
                        top: "5px",
                        cursor: "pointer",
                        zIndex: "9999"
                    });

                    closeBtn.onclick = () => instance.close();
                    instance.calendarContainer.appendChild(closeBtn);
                }
            }
        });

        let $input = $(input);

        // 🔥 find correct wrapper automatically (no UI change)
        let wrapper = $input.closest(".input-group");

        if (wrapper.length === 0) {
            wrapper = $input.parent();
        }

        wrapper.css("position", "relative");

        // avoid duplicate
        if (wrapper.find(".clear-date").length === 0) {

            let clearBtn = $('<span class="clear-date">✖</span>');

            wrapper.append(clearBtn);

            // toggle
            function toggle() {
                clearBtn.toggle(!!$input.val());
            }

            $input.on("change", toggle);

            clearBtn.on("click", function () {
                fp.clear();   // ✅ correct clearing
                clearBtn.hide();
            });

            toggle();
        }
    });

});

window.onerror = function (message, source, lineno, colno, error) {

    const userName = localStorage.getItem("UserName") || "";
    const loginName = localStorage.getItem("LoginName") || "";
    const schoolName = localStorage.getItem("SchoolName") || "";
    const fullUser = userName + " (" + loginName + ")";

    const errorMessage = error && error.message ? error.message : message;
    const stackTrace = error && error.stack ? error.stack : "No Stack Trace";

    const Messagedetails = `
<table width="100%" cellpadding="5" cellspacing="0"
       style="font-family:Verdana;font-size:12px;background:#f4f4f4;">

<tr>
<td colspan="2" style="background:#dcdcdc;font-weight:bold;
                       color:#000;padding:8px;">
    IFNOSS Page Error
</td>
</tr>

<tr>
<td width="150" style="color:navy;font-weight:bold;">Error</td>
<td>${errorMessage}</td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">Details</td>
<td>Exception at line ${lineno}, column ${colno}</td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">Form Values</td>
<td></td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">URL</td>
<td>${window.location.href}</td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">Stack Trace</td>
<td><pre style="white-space:pre-wrap;">${stackTrace}</pre></td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">Request Host</td>
<td>${location.hostname}</td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">Host Name</td>
<td>${location.hostname}</td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">User Agent</td>
<td>${navigator.userAgent}</td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">URL Referrer</td>
<td>${document.referrer || "-"}</td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">UserName</td>
<td>${fullUser}</td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">Current Sessions</td>
<td>
SchoolName=${schoolName}<br/>
UserName=${userName}<br/>
LoginName=${loginName}<br/>
IsActive=True
</td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">Method</td>
<td>window.onerror (JavaScript)</td>
</tr>

<tr>
<td style="color:navy;font-weight:bold;">Source</td>
<td>${source}</td>
</tr>

</table>
`;

    $.ajax({
        type: "POST",
        url: APIUrl + "api/CommonFunction/SendEmailError",
        headers: {
            'Authorization': 'Bearer ' + GetAccessToken()
        },
        data: {
        //    strTo: "creativeras@gmail.com",
            strTo: "teambackendcoders@gmail.com",
          //  strCc: "sakinapatanwala13@gmail.com",
            strSubject: "GFI - Error " + window.location.href,
            strBody: Messagedetails
        },
        async: true
    });

    return true;
};