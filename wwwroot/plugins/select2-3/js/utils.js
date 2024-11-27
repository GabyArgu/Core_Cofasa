function initAsyncSelect2(id, url, placeholder, isMultiple, data, formatSelection, allowClear) {
    if (typeof(isMultiple) == 'undefined') isMultiple = false;
    if (typeof(data) == 'undefined') data = function(term, page) {
        return { q: term };
    };
    if (typeof(formatSelection) == "undefined") formatSelection = function(data) {
        return data.text;
    };
    if (typeof(formatSelection) == "undefined") allowClear = true;

    $('#' + id).select2({
        placeholder: placeholder,
        multiple: isMultiple,
        allowClear: allowClear,
        ajax: {
            url: url,
            dataType: 'json',
            type: 'GET',
            data: data,
            results: function(data, page) {
                return {
                    results: data.results
                };
            },
            formatSelection: formatSelection
        }
    });
}

function initLocalSelect2(id, data, placeholder, isMultiple) {
    $('#' + id).select2({
        placeholder: placeholder,
        data: data,
        multiple: isMultiple || false
    });
}

function initSelect2Paginated(id, url, placeholder, isMultiple, data) {
    $('#' + id).select2({
        placeholder: placeholder,
        multiple: isMultiple || false,
        allowClear: true,
        ajax: {
            url: url,
            dataType: 'JSON',
            delay: 250,
            data: data || function (term, page) {
                return {
                    q: term,
                    page: page || 1,
                    pageSize: 10
                };
            },
            results: function (data, page) {
                return {results: data.results, more: data.more};
            },
            cache: true
        }
    });
}

function initAsyncSelect2name(obj, url, placeholder, isMultiple, data, templateSelection) {
    if (typeof(isMultiple) == "undefined") isMultiple = false;
    if (typeof(data) == "undefined") data = function(term, page) {
        return {
            q: term
        };
    };
    
    if (typeof(templateSelection) == "undefined") templateSelection = function(data) {
        return data.text;
    };

    obj.select2({
        placeholder: placeholder,
        multiple: isMultiple,
        allowClear:true,
        ajax: {
            url: url,
            dataType: 'json',
            type: 'GET',
            data: data,
            results: function(data, page) {
                return {
                    results: data.results
                };
            }
        },
        templateSelection: templateSelection
    });
}

function initSelect2(id, placeholder) {
    $("#" + id).select2({
        placeholder: placeholder,
        placeholderOption: 'first'
    });
}

function initMultipleCreatorSelect2(id, placeholder){
    $("#" + id).select2({
        placeholder: placeholder,
        placeholderOption: 'first',
        multiple:true,
        data:[],
        createSearchChoice:function(term, data) {
            if ($(data).filter(function() { return this.text.localeCompare(term)===0; }).length===0) {
              return {id:term, text:term};
            }
        },
    });
}

function getLabelsSelect2(id){
    var data = $("#"+id).select2("data");
    var array = "";
    $.each(data,function(pos,item){
        array += (array!=""?",":"") + item.text;
    });
    return array;
}

function initSelect2Local(id, data, placeholder) {
    $("#" + id).select2({
        // placeholder: placeholder,
        data: data
    });
}