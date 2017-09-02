
function FormatJSON(oData, sIndent, hl) {
    var tagsToReplace = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;'
    };

    function replaceTag(tag) {
        return tagsToReplace[tag] || tag;
    }

    function safe_tags_replace(str) {
        return str.replace(/[&<>]/g, replaceTag);
    }

    if (arguments.length < 2) {
        var sIndent = "";
    }
    var sIndentStyle = "  ";
    var sDataType = RealTypeOf(oData);

    var osb = '[';
    var ocb = '{';
    var csb = ']';
    var ccb = '}';
    if (hl) {
        osb = '<span class="bracket">[</span>';
        ocb = '<span class="bracket">{</span>';
        csb = '<span class="bracket">]</span>';
        ccb = '<span class="bracket">}</span>';
    }

    // open object
    if (sDataType == "array") {
        if (oData.length == 0) {
            return osb + csb;
        }
        var sHTML = osb;
    } else {
        var iCount = 0;
        $.each(oData, function () {
            iCount++;
            return;
        });
        if (iCount == 0) { // object is empty
            return ocb + ccb;
        }
        var sHTML = ocb;
    }

    // loop through items
    var iCount = 0;
    $.each(oData, function (sKey, vValue) {
        if (iCount > 0) {
            sHTML += ",";
        }
        if (sDataType == "array") {
            sHTML += ("\n" + sIndent + sIndentStyle);
        } else {
            if (hl)
                sHTML += ("\n" + sIndent + sIndentStyle + '"<span class="type-string key">' + safe_tags_replace(sKey) + "</span>\"" + ": ");
            else
                sHTML += ("\n" + sIndent + sIndentStyle + "\"" + sKey + "\"" + ": ");
        }

        // display relevant data type
        switch (RealTypeOf(vValue)) {
            case "array":
                sHTML += FormatJSON(vValue, (sIndent + sIndentStyle), hl);
                break;
            case "object":
                sHTML += FormatJSON(vValue, (sIndent + sIndentStyle), hl);
                break;
            case "boolean":
            case "number":
                if (hl)
                    sHTML += '<span class="type-number">' + safe_tags_replace(vValue.toString()) + '</span>';
                else
                    sHTML += vValue.toString();
                break;
            case "null":
                if (hl)
                    sHTML += '<span class="type-null">null</span>';
                else
                    sHTML += "null";
                break;
            case "string":
                if (hl)
                    sHTML += "\"" + '<span class="type-string">' + safe_tags_replace(vValue) + '</span>' + "\"";
                else
                    sHTML += ("\"" + vValue + "\"");
                break;
            default:
                sHTML += ("TYPEOF: " + typeof (vValue));
        }

        // loop
        iCount++;
    });

    // close object
    if (sDataType == "array") {
        sHTML += ("\n" + sIndent + csb);
    } else {
        sHTML += ("\n" + sIndent + ccb);
    }

    // return
    return sHTML;
}

function RealTypeOf(v) {
    if (typeof (v) == "object") {
        if (v === null) return "null";
        if (v.constructor == (new Array).constructor) return "array";
        if (v.constructor == (new Date).constructor) return "date";
        if (v.constructor == (new RegExp).constructor) return "regex";
        return "object";
    }
    return typeof (v);
}
