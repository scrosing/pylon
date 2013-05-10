var m = ["January", "Febrary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
var Heads = ["", "Metrics", "YTD Target", "YTD Actual", "Q1 Actual", "Q2 Actual", "Q3 Actual", "Q4 Actual", "May Actual"];

function showfoot() {
    var currentdate = new Date();
    var curmonth = currentdate.getUTCMonth();
    var curyear = currentdate.getUTCFullYear();
    var curday = currentdate.getUTCDate();
    var mod = "Last Updated on %month %day, %year UTC, data has 0 days delay";
    mod = mod.replace("%day", curday);
    mod = mod.replace("%year", curyear);
    mod = mod.replace("%month", m[curmonth]);
    jQuery("#divfoot").text(mod);
}

function getcolor() {
    //return ['#ff9980', '#ffb380', '#ffd980', '#ffe680', '#ace6ac', '#9ed29e', '#82c382'];
    return ['#ff3300', '#ff6600', '#ffb200', '#ffcc00', '#66cc66', '#53a653', '#2d882d'];
}
function o_cpe() {
    this.tb = 0.0;
    this.bb = 0.0;
    this.sur = 0.0;
    this.vol = 0.0;
}
function readgeo() {
}

function readcpe(i) {
    switch (i) {
        case 1:
        case 3:
        case 2:
        case 0:
        default:
    }
}
function readtar() {
}

function allcountrycode() {
    var codes = ["BD", "BE", "BF", "BG", "BA", "BN", "BO", "JP", "BI", "BJ", "BT", "JM", "BW", "BR", "BS", "BY", "BZ", "RU", "RW", "RS", "LT", "LU", "LR", "RO", "GW", "GT", "GR", "GQ", "GY", "GE", "GB", "GA", "GN", "GM", "GL", "KW", "GH", "OM", "JO", "HR", "HT", "HU", "HN", "LV", "PR", "PS", "PT", "PY", "PA", "PG", "PE", "PK", "PH", "PL", "ZM", "EE", "EG", "ZA", "EC", "AL", "AO", "KZ", "ET", "ZW", "ES", "ER", "ME", "MD", "MG", "MA", "UZ", "MM", "ML", "MN", "MK", "MW", "MR", "UG", "MY", "MX", "VU", "FR", "FI", "FJ", "FK", "NI", "NL", "NO", "NA", "NC", "NE", "NG", "NZ", "NP", "CI", "CH", "CO", "CN", "CM", "CL", "CA", "CG", "CF", "CD", "CZ", "CY", "CR", "CU", "SZ", "SY", "KG", "KE", "SS", "SR", "KH", "SV", "SK", "KR", "SI", "KP", "SO", "SN", "SL", "SB", "SA", "SE", "SD", "DO", "DJ", "DK", "DE", "YE", "AT", "DZ", "US", "UY", "LB", "LA", "TT", "TR", "LK", "TN", "TL", "TM", "TJ", "LS", "TH", "TF", "TG", "TD", "LY", "AE", "VE", "AF", "IQ", "IS", "IR", "AM", "IT", "VN", "AR", "AU", "IL", "IN", "TZ", "AZ", "IE", "ID", "UA", "QA", "MZ"];
    return codes;
}

function getcpebyL3(strregion, ob, i, mon) {
    var tb = 0;
    var bb = 0;
    var sur = 0;
    var vol = 0.0;

    var strcpe = readcpe(i);
    var jsoncpe = jQuery.parseJSON(strcpe);
    for (l = 0; l < jsoncpe.cpe.length; l++) {
        if (jsoncpe.cpe[l].month == mon || mon == 0 || (mon == -1 && jsoncpe.cpe[l].month <= 3 && jsoncpe.cpe[l].month >= 1)
            || (mon == -2 && jsoncpe.cpe[l].month <= 6 && jsoncpe.cpe[l].month >= 4)
            || (mon == -3 && jsoncpe.cpe[l].month <= 9 && jsoncpe.cpe[l].month >= 7)
            || (mon == -4 && jsoncpe.cpe[l].month <= 12 && jsoncpe.cpe[l].month >= 10)) {
            for (k = 0; k < jsoncpe.cpe[l].geo.length; k++) {
                if (jsoncpe.cpe[l].geo[k].L3.toString().toLowerCase() === strregion.toLowerCase() || jsoncpe.cpe[l].geo[k].L2.toString().toLowerCase() === strregion.toLowerCase() || strregion === "Worldwide" && jsoncpe.cpe[l].geo[k].L3.toString() !== "UNKNOWN") {
                    tb += jsoncpe.cpe[l].geo[k].TB;
                    bb += jsoncpe.cpe[l].geo[k].BB;
                    sur += jsoncpe.cpe[l].geo[k].Survey;
                    vol += jsoncpe.cpe[l].geo[k].Vol;
                }
            }
        }
    }
    ob.tb = tb;
    ob.bb = bb;
    ob.sur = sur;
    ob.vol = vol;
    if (i < 2) {
        ob.vol = 0;
    }
}
function gettarbyL3(strregion, ob, id) {
    ob.tb = 0.0;
    ob.bb = 0.0;
    ob.sur = 0;
    var lb1, lb2;
    var strtar = readtar();
    var jsontar = jQuery.parseJSON(strtar);
    var i, j;
    if (id == 0) {
        lb1 = "TB";
        lb2 = "BB";
    }
    else {
        lb1 = "CritTB";
        lb2 = "CritBB";
    }
    if (id < 2) {
        for (i = 0; i < jsontar.Tar.length; i++) {
            if (jsontar.Tar[i].Metric === lb1) {
                for (j = 0; j < jsontar.Tar[i].Tar.length; j++) {
                    if (jsontar.Tar[i].Tar[j].Geo.toLowerCase() === strregion.toLowerCase() || (jsontar.Tar[i].Tar[j].Geo.toLowerCase() === "united states" && strregion.toLowerCase() === "north america") || (jsontar.Tar[i].Tar[j].Geo.toLowerCase() === "apgc" && strregion.toLowerCase() === "asia" )) {
                        ob.tb = jsontar.Tar[i].Tar[j].Value;
                    }
                }
            }
            if (jsontar.Tar[i].Metric === lb2) {
                for (j = 0; j < jsontar.Tar[i].Tar.length; j++) {
                    if (jsontar.Tar[i].Tar[j].Geo.toLowerCase() === strregion.toLowerCase() || (jsontar.Tar[i].Tar[j].Geo.toLowerCase() === "united states" && strregion.toLowerCase() === "north america") || (jsontar.Tar[i].Tar[j].Geo.toLowerCase() === "apgc" && strregion.toLowerCase() === "asia" )){
                        ob.bb = jsontar.Tar[i].Tar[j].Value;
                    }
                }
            }
        }
    }
    else {
        lb1 = "Volume";
        if (id == 2) {
            lb2 = "On Premises Products";
        }
        else {
            lb2 = "Cloud Products";
        }
        for (i = 0; i < jsontar.Tar.length; i++) {
            if (jsontar.Tar[i].Metric === lb1) {
                for (j = 0; j < jsontar.Tar[i].Tar.length; j++) {
                    if ((jsontar.Tar[i].Tar[j].Geo.toLowerCase() === strregion.toLowerCase() || (jsontar.Tar[i].Tar[j].Geo.toLowerCase() === "united states" && strregion.toLowerCase() === "north america")) && jsontar.Tar[i].Tar[j].Org === lb2) {
                        ob.vol = jsontar.Tar[i].Tar[j].Value;
                    }
                }
            }
        }
    }
}

function getregionlevel(id, num) {
    var strgeo = readgeo();
    var strall = allcountrycode();
    var L4, L3;
    var ob = new o_cpe();
    var nsat;
    var ret = "";
    var i, j, k, l;
    var tb, bb, sur, vol;
    var tar, dif;
    var level;
    var jsongeo = jQuery.parseJSON(strgeo);
    for (i = 0; i < strall.length; i++) {
        L4 = strall[i];
        for (j = 0; j < jsongeo.Geo.length; j++) {
            for (k = 0; k < jsongeo.Geo[j].L4.length; k++) {
                if (jsongeo.Geo[j].L4[k].toString() === L4) {
                    L3 = jsongeo.Geo[j].L3.toString();
                    getcpebyL3(L3, ob, id, 0);
                    tb = ob.tb;
                    bb = ob.bb;
                    sur = ob.sur;
                    vol = ob.vol;
                    gettarbyL3(L3, ob, id);
                    if (id < 2) {
                        if (sur == 0) {
                            break;
                        }
                        nsat = (tb - bb) * 100 / sur + 100;
                        tar = (ob.tb - ob.bb) * 100 + 100;
                        dif = nsat - tar;
                        if (dif > 2) {
                            level = 7;
                        }
                        else if (dif > 1) {
                            level = 6;
                        }
                        else if (dif > 0) {
                            level = 5;
                        }
                        else if (dif > -1) {
                            level = 4;
                        }
                        else if (dif > -2) {
                            level = 3;
                        }
                        else if (dif > -3) {
                            level = 2;
                        }
                        else {
                            level = 1;
                        }
                        level--;
                        if (level == num) {
                            if (ret !== "") {
                                ret += ", ";
                            }
                            ret += '"' + L4 + '": ';
                            ret += level.toString();
                        }
                    }
                    else {
                        tar = ob.vol;
                        if (tar >= 0) {

                            dif = Math.abs(vol - tar) / tar * 100;
                            if (dif < 1) {
                                level = 7;
                            }
                            else if (dif < 2) {
                                level = 6;
                            }
                            else if (dif < 2.5) {
                                level = 5;
                            }
                            else if (dif < 3.5) {
                                level = 4;
                            }
                            else if (dif < 5) {
                                level = 3;
                            }
                            else if (dif < 6) {
                                level = 2;
                            }
                            else {
                                level = 1;
                            }

                            level -= 1;
                            if (level == num) {
                                if (ret !== "") {
                                    ret += ", ";
                                }
                                ret += '"' + L4 + '": ';
                                ret += level.toString();
                            }
                        }
                    }
                }
            }
        }
    }
    return "{" + ret + "}";
}

jQuery(".wl").live("click", function () {
    var dataid;
    dataid = jQuery(this).attr("data-id");
    var data_id = parseInt(dataid);

    curwl = data_id;
    if (jQuery("#map").css("visibility") === "visible") {
        if (data_id < 2) {
            jQuery('#legend1').text("Favorable variance");
            jQuery('#legend2').text("Yellow when unfavorable variance is below 2");
            jQuery('#legend3').text("Unfavorable variance grater than 2");
        }
        else {
            jQuery('#legend1').text("Closest to the pin");
            jQuery('#legend2').text("Variance between +2.5% and -2.5%");
            jQuery('#legend3').text("Variance greater than 5% or less than -5%");
        }
        jQuery('#map1').empty();
        var worldmap = getmap('#map1', data_id);
    }
    else {
        var code = jQuery(".dataCell")[9].innerText;
        var s = code.indexOf(" Total");
        if (s > 0) {
            code = code.substring(0, s);
        }
        show_chart(data_id, code);
    }
});

function getmap(con, data_id) {
    var colors = getcolor();
    var rc;
    var i;
    var rss = [];
    var rs = [];
    for (i = 0; i < 7; i++) {
        rs.push({
            scale: colors,
            normalizeFunction: 'polynomial',
            values: 1,
            max: 0,
            min: 0
        });
    }
    for (i = 0; i < 7; i++) {
        rs[i].max = i;
        rs[i].min = i;
        rc = getregionlevel(data_id, i);
        if (rc === "{}") {
            continue;
        }
        rs[i].values = jQuery.parseJSON(rc);
        rs[i].scale = [colors[i]];
        rss.push(rs[i]);
    }
    var wmap = new jvm.WorldMap({
        container: jQuery(con),
        map: 'world_mill_en',
        backgroundColor: "#FFFFFF00",
        regionStyle: {
            selected: {
                "fill-opacity": 0.8,
                fill: '#EEEEEE00'
            }
        },
        regionsSelectable: true,
        focusOn: {
            x: 1,
            y: 1,
            scale: 1
        },
        series: {
            regions: rss
        },
        onRegionLabelShow: function (e, el, code) {
            var strcpe = readcpe(data_id);
            var ptb, pbb, pnset;
            var jsoncpe = jQuery.parseJSON(strcpe);
            var ob = new o_cpe();
            var vol;
            tb = 0;
            bb = 0;
            sur = 0;
            vol = 0.0;
            var strregion = getL3byL4(code);

            getcpebyL3(strregion, ob, data_id, 0);
            if (data_id < 2) {
                sur = ob.sur;

                if (sur == 0) {
                    ptb = "null";
                    pbb = "null";
                }
                else {
                    ptb = Math.round(ob.tb * 1000 / ob.sur) / 10;
                    pbb = Math.round(ob.bb * 1000 / ob.sur) / 10;
                }
                gettarbyL3(strregion, ob, data_id);
                el.html(strregion + ": TB - " + ptb + "%, BB - " + pbb + "%, #Survey - " + sur.toString() + "<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Target: TB - " + Math.round(ob.tb * 100) + "%, BB - " + Math.round(ob.bb * 100) + "%</p>");

            }
            else {
                vol = ob.vol;
                gettarbyL3(strregion, ob, data_id);
                el.html(strregion + ": " + "IPD - " + (Math.round(vol * 10) / 10).toString() + "<p>Target: IPD - " + (Math.round(ob.vol * 10) / 10).toString());
            }
            return;
        },
        onRegionOver: function (e, code) {
            wmap.clearSelectedRegions();
            var codes = [];
            var allcodes = [];
            allcodes = allcountrycode();
            var strgeo = readgeo();
            var i, j, k, l;
            var jsongeo = jQuery.parseJSON(strgeo);
            for (i = 0; i < jsongeo.Geo.length; i++) {
                for (j = 0; j < jsongeo.Geo[i].L4.length; j++) {
                    if (jsongeo.Geo[i].L4[j].toString() === code) {
                        for (k = 0; k < jsongeo.Geo[i].L4.length; k++) {
                            for (l = 0; l < allcodes.length; l++) {
                                if (allcodes[l] === jsongeo.Geo[i].L4[k].toString()) {
                                    codes.push(jsongeo.Geo[i].L4[k].toString());
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            wmap.setSelectedRegions(codes);
        },
        onRegionOut: function (e, code) {
            wmap.clearSelectedRegions();
        },
        onRegionClick: function (e, code) {
            hidemap();
            show_chart(data_id, code);
        }
    });
    return wmap;
}

function show_wl() {
    var head_title = ["IBS - CPE", "IBS - Crisit", "On Prem - Created Incident Volume Per Day", "Cloud - Created Incident Volume Per Day"];
    var html_wl;
    var html_head;
    var html_body;
    var html_foot;
    html_wl = '<div class="wl modal" data-id="@ID" style="position: absolute; left: 79%; top: @TOP%; width: 20%;">';
    html_wl += '@HEAD@BODY@FOOT';
    html_wl += '</div>';
    html_head = '<div class="modal-header">';
    html_head += '<h6>@TITLE</h6>';
    html_head += '</div>';
    html_body = '<div class="modal-body">';
    html_body += '<p>@CPE1@CPE2@SURVEY</p>';
    html_body += '</div>';
    html_foot = '<div class="modal-footer">'
    html_foot += '<p>@target WW target</p>'
    html_foot += '</div>'
    var cpe = '<span style="color:#@color">@symbol</span>@data';
    var survey = '<span style="font-size:14px;float:right;padding-right:5px">@sur</span>';
    var i;
    var op = new o_cpe();
    var head, body, foot, wl;
    var tb, bb, sur, vol;
    var ptb, pbb;
    var wl_all;
    var line = "";
    wl_all = "";
    for (i = 0; i < 4 && i < 5; i++) {
        getcpebyL3("Worldwide", op, i, 0);
        tb = op.tb;
        bb = op.bb;
        sur = op.sur;
        vol = op.vol;
        gettarbyL3("Worldwide", op, i);
        body = html_body;
        wl = html_wl.replace("@ID", i.toString());
        wl = wl.replace("@TOP", (i * 18 + 12).toString());
        head = html_head.replace("@TITLE", head_title[i]);
        wl = wl.replace("@HEAD", head);
        if (i < 2) {
            foot = html_foot.replace("@target", op.tb * 100 + "%/" + op.bb * 100 + "%");
        }
        else {
            foot = html_foot.replace("@target", Math.round(op.vol));
        }
        wl = wl.replace("@FOOT", foot);
        if (sur == 0 || i > 1) {
            line = cpe.replace("@data", Math.round(vol))

            if (Math.abs(op.vol - vol) / op.vol * 100 <= 2.5) {
                line = line.replace("@color", "00AA00");
                line = line.replace("@symbol", "▲");
            }
            else {
                line = line.replace("@color", "AA0000");
                line = line.replace("@symbol", "▼");
            }

            body = body.replace("@CPE1", line);
            body = body.replace("@CPE2", "");
            body = body.replace("@SURVEY", "");
        }
        else {
            ptb = Math.round(tb * 100 / sur).toString();
            pbb = Math.round(bb * 100 / sur).toString();
            line = cpe.replace("@data", ptb + "%");
            if (ptb >= op.tb * 100) {
                line = line.replace("@color", "00AA00");
                line = line.replace("@symbol", "▲");
            }
            else {
                line = line.replace("@color", "AA0000");
                line = line.replace("@symbol", "▼");
            }
            body = body.replace("@CPE1", line);
            line = cpe.replace("@data", pbb + "%");
            if (pbb <= op.bb * 100) {
                line = line.replace("@color", "00AA00");
                line = line.replace("@symbol", "▲");
            }
            else {
                line = line.replace("@color", "AA0000");
                line = line.replace("@symbol", "▼");
            }
            body = body.replace("@CPE2", "/" + line);
            line = survey.replace("@sur", sur.toString());
            body = body.replace("@SURVEY", line);
        }
        wl = wl.replace("@BODY", body);

        wl_all += wl;
    }
    jQuery("#wl").html(wl_all);
};

function getL3byL4(code) {
    var strregion = "";
    var strgeo = readgeo();
    var i, j;
    var jsongeo = jQuery.parseJSON(strgeo);
    for (i = 0; i < jsongeo.Geo.length; i++) {
        for (j = 0; j < jsongeo.Geo[i].L4.length; j++) {
            if (jsongeo.Geo[i].L4[j].toString() === code) {
                strregion = jsongeo.Geo[i].L3.toString();
            }
        }
    }
    return strregion;
}

function getL2byL4(code) {
    var strregion = "";
    var strgeo = readgeo();
    var i, j;
    var jsongeo = jQuery.parseJSON(strgeo);
    for (i = 0; i < jsongeo.Geo.length; i++) {
        for (j = 0; j < jsongeo.Geo[i].L4.length; j++) {
            if (jsongeo.Geo[i].L4[j].toString() === code) {
                strregion = jsongeo.Geo[i].L2.toString();
            }
        }
    }
    return strregion;
}

function show_head() {
    var mod = '<div data-id="@ID" class="dataCell @class2">@DATA</div>';
    var line = "";
    var head = "";
    var i;
    for (i = 0; i < Heads.length; i++) {
        line = mod.replace("@ID", i.toString());
        line = line.replace("@DATA", Heads[i]);
        if (i == 0) {
            line = line.replace("@class2", "dataRegion");
        }
        else {
            line = line.replace("@class2", "dataValue");
        }
        head += line;
    }
    jQuery("#tablehead").html(head);
}

function show_body() {
}

function hidemap() {
    jQuery("#map").css("visibility", "collapse");
    jQuery("#map1").empty();
    jQuery("#tablechart").css("visibility", "visible");
    jQuery(".jvectormap-label").remove();
}
function show_chart(data_id, code) {
    var strL2;
    if (code.length == 2) {
        strL2 = getL2byL4(code);
    }
    else {
        strL2 = code;
    }
    var L3s = getL3sbyL2(strL2);
    var i;
    jQuery("#tablebody").empty();
    show_head();
    show_line(strL2, data_id, 0);
    for (i = 0; i < L3s.length; i++) {
        show_line(L3s[i], data_id, i + 1);
    }
}

function getL3sbyL2(L2) {
    var ret = [];
    var strgeo = readgeo();
    var i;
    var jsongeo = jQuery.parseJSON(strgeo);
    for (i = 0; i < jsongeo.Geo.length; i++) {
        if (jsongeo.Geo[i].L2 === L2) {
            ret.push(jsongeo.Geo[i].L3)
        }
    }
    ret = sort_L2(ret);
    return ret;
}

function sort_L2(L2) {
    var i, j;
    var tmp;
    for (i = 1; i < L2.length; i++) {
        for (j = 1; j < L2.length; j++) {
            if (L2[j] < L2[j - 1]) {
                tmp = L2[j];
                L2[j] = L2[j - 1];
                L2[j - 1] = tmp;
            }
        }
    }
    return L2;
}

function show_line(L, id, dataid) {
    var i;
    var j = 2;
    var row;
    var metric = ["TB", "BB", ""];
    var name = L;
    var ob = new o_cpe();
    var tb, bb;
    var dif;
    if (dataid == 0) {
        name = L + " Total";
        dif = "Central and Eastern Europe".length - name.length;
        for (i = 0; i < dif; i+=3) {
            name += "&nbsp;"
            name += "&nbsp;"
            name += "&nbsp;"
            name += "&nbsp;"
            name += "&nbsp;"
        }
    }
    if (id < 2) {
        ob.vol = 0;
        row = "";
        for (j = 0; j < 2; j++) {
            row += '<div class="dataRow" data-id="' + (dataid * 2 + j).toString() + '">';
            for (i = 0; i < Heads.length; i++) {
                if (Heads[i] === "") {
                    row += '<div class="dataCell dataRegion">';
                }
                else {
                    row += '<div class="dataCell dataValue">';
                }
                if (Heads[i] === "") {
                    row += name;
                    ob.vol = 999;
                }
                    //var Heads = ["", "Metrics", "YTD Target", "YTD Actual", "Q1", "Q2", "Q3", "Q4", "Apr Actual"];
                else if (Heads[i] === "Metrics") {
                    row += metric[j];
                    ob.vol = 999;
                }
                else if (Heads[i] === "YTD Target") {
                    gettarbyL3(L, ob, id);
                    if (metric[j] === "TB") {
                        tb = Math.round(ob.tb * 100)
                        row += tb + "%";
                    }
                    if (metric[j] === "BB") {
                        bb = Math.round(ob.bb * 100)
                        row += bb + "%";
                    }
                    ob.vol = -1;
                }
                else if (Heads[i] === "YTD Actual") {
                    getcpebyL3(L, ob, id, 0);
                }
                else if (Heads[i] === "Q1 Actual") {
                    getcpebyL3(L, ob, id, -1);
                }
                else if (Heads[i] === "Q2 Actual") {
                    getcpebyL3(L, ob, id, -2);
                }
                else if (Heads[i] === "Q3 Actual") {
                    getcpebyL3(L, ob, id, -3);
                }
                else if (Heads[i] === "Q4 Actual") {
                    getcpebyL3(L, ob, id, -4);
                }
                else if (Heads[i] === "May Actual") {
                    getcpebyL3(L, ob, id, 11);
                }
                if (ob.vol == 0) {
                    if (metric[j] === "TB") {
                        tb = Math.round(ob.tb / ob.sur * 100)
                        row += tb + "%";
                    }
                    if (metric[j] === "BB") {
                        bb = Math.round(ob.bb / ob.sur * 100)
                        row += bb + "%";
                    }
                }

                ob.vol = 0;
                row += '</div>';
            }
            row += '</div>';
        }

    }
    else {
        row = '<div class="dataRow" data-id="' + dataid + '">';
        ob.sur = 0;
        for (i = 0; i < Heads.length; i++) {
            if (Heads[i] === "") {
                row += '<div class="dataCell dataRegion">';
            }
            else {
                row += '<div class="dataCell dataValue">';
            }
            if (Heads[i] === "") {
                row += name;
                ob.sur = -1;
            }
                //var Heads = ["", "Metrics", "YTD Target", "YTD Actual", "Q1", "Q2", "Q3", "Q4", "Apr Actual"];
            else if (Heads[i] === "Metrics") {
                row += metric[j];
                ob.sur = -1
            }
            else if (Heads[i] === "YTD Target") {
                gettarbyL3(L, ob, id);
            }
            else if (Heads[i] === "YTD Actual") {
                getcpebyL3(L, ob, id, 0);
            }
            else if (Heads[i] === "Q1 Actual") {
                getcpebyL3(L, ob, id, -1);
            }
            else if (Heads[i] === "Q2 Actual") {
                getcpebyL3(L, ob, id, -2);
            }
            else if (Heads[i] === "Q3 Actual") {
                getcpebyL3(L, ob, id, -3);
            }
            else if (Heads[i] === "Q4 Actual") {
                getcpebyL3(L, ob, id, -4);
            }
            else if (Heads[i] === "May Actual") {
                getcpebyL3(L, ob, id, 11);
            }
            if (ob.sur == 0) {
                row += Math.round(ob.vol * 10) / 10;
            }
            ob.sur = 0;

            row += '</div>';
        }
        row += '</div>';
    }
    jQuery("#tablebody").append(row);
}

jQuery(".dataRow").live("click", function () {
    $ = jQuery;
    var i;
    var op = getoption();
    var cha = [];
    var cha2 = [];
    var dataid = $(this).attr("data-id");
    var data_id = parseInt(dataid);
    var L = $(".dataCell")[(data_id + 1) * 9].innerText;
    var M = $(".dataCell")[(data_id + 1) * 9 + 1].innerText;
    var s = L.indexOf(" Total");
    if (s > 0) {
        L = L.substring(0, s);
    }
    var curwl0;
    try{
        curwl0 = curwl;
    }
    catch(e){
        curwl0 = 0;
    }
    if (M === "") {
        return;
    }
    var curmon = 11;
    var reportmon;
    var ob = new o_cpe();
    var me;
    for (i = 0; i < 12; i++) {
        reportmon = curmon - i;
        getcpebyL3(L, ob, curwl0, reportmon);
        if (ob.sur > 0) {
            if (M === "TB") {
                me = ob.tb / ob.sur;
            }
            else if (M === "BB") {
                me = ob.bb / ob.sur;
            }
            else {
                return;
            }
            cha.push([11 - i, Math.round(me * 1000) / 10]);
        }
        gettarbyL3(L, ob, curwl0);
        if (M === "TB") {
            me = ob.tb;
        }
        else if (M === "BB") {
            me = ob.bb;
        }
        cha2.push([11 - i, Math.round(me * 1000) / 10]);
    }
    try {
        plot.destroy();
    }
    catch (e) {
    }
    month = getmonth();
    op.axes.xaxis.ticks = month;
    plot = $.jqplot('chart', [cha, cha2], op);
});

function getoption() {
    var $ = jQuery;
    var options = {
        seriesColors: ["rgb(91, 155, 213)", "rgb(190, 30, 74)", "#FFFFFF", "#1FABE7"],
        axesDefaults: {
            tickRenderer: $.jqplot.CanvasAxisTickRenderer,
            tickOptions: { angle: -30, fontSize: 12, textColor: "rgb(83, 83, 69)", }
        },
        seriesDefaults: {
            renderer: $.jqplot.LineRenderer,
            rendererOptions: { barWidth: 30, smooth: true, animation: { show: true, speed: 1000, } },
            //color: "@Model.ColorPart3"
        },
        series: [
            { renderer: $.jqplot.BarRenderer, label: 'Actual' },
            { label: 'Target' },
        ],
        axes: {
            xaxis: {
                renderer: $.jqplot.CategoryAxisRenderer,
                tickOptions: { showGridline: false, }
            },
            yaxis: { tickOptions: { formatString: '%3.1f %' } },
        },
        highlighter: {
            show: true,
            useAxesFormatters: true,
            showMarker: true,
            tooltipAxes: 'y',
            tooltipFormatString: '<span style="color:#fff;">%s</span>',
        },
        grid: { shadow: false, drawGridLines: false, background: "rgba(255, 255, 255, 0)", borderWidth: 0, gridLineColor: "#fff" },
        cursor: {
            show: true,
            zoom: true,
            followMouse: false,
            tooltipAxes: 'y',
            //showVerticalLine: true,
            //showHorizontalLine: true,
            tooltipLocation: 'ne',
            useAxesFormatters: false,
            tooltipFormatString: '<span style="color:#203864">%3.1f, %f</span>',
            showTooltip: false,
        },
        legend: {
            show: true,
            location: 'ne',
            backgound: "#339933",
            textColor: "#FF0000",
            //placement: 'outsideGrid',
            showSwatches: true,
        },
    };
    return options;
}

function getmonth() {
    var i;
    var curmonth = "May";
    var month12 = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var month = [];
    for (i = 0; i < 12; i++) {
        if (month12[i] === curmonth)
            break;
    }
    for (var j = 1; j < 12; j++) {
        var k = j + i + 1;
        if (k > 11)
            k -= 12;
        month.push(month12[k]);
    }
    return month;
}
