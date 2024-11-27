function needAccountName(cta1,cta2,cta3,cta4,cta5,cta6) {
    return (!(typeof(cta1)==='undefined') && cta1!==null && cta1!=='')
        && (!(typeof(cta2)==='undefined') && cta2!==null && cta2!=='')
        && (!(typeof(cta3)==='undefined') && cta3!==null && cta3!=='')
        && (!(typeof(cta4)==='undefined') && cta4!==null && cta4!=='')
        && (!(typeof(cta5)==='undefined') && cta5!==null && cta5!=='')
        && (!(typeof(cta6)==='undefined') && cta6!==null && cta6!=='');
}

// function hideContaParamsErrorMessages(cta1,cta2,cta3,cta4,cta5,cta6){
function hideAccountNumberErrorMessages(cta1,cta2,cta3,cta4,cta5,cta6){
    $(`#${cta1}-error,#${cta2}-error,#${cta3}-error,#${cta4}-error,#${cta5}-error,#${cta6}-error`).hide();
}

function hideFieldsErrorsMessages(fieldsList){
    $.each(fieldsList, function (index, field) {
        hideFieldErrorMessage(field);
    });
}

function hideFieldErrorMessage(field){
    $(`#${field}-error`).hide();
}

function isValidAccountNumber(id) {
    return ($(id).val()!=='' && $(id).valid());
}

function validateNumbers(cta1,cta2,cta3,cta4,cta5) {
    return $(cta1).val()!=='' || $(cta2).val()!=='' || $(cta3).val()!==''
        || $(cta4).val()!=='' || $(cta5).val()!=='';
}

function makeAccountNameReq(cta1id,cta2id,cta3id,cta4id,cta5id,cta6id,nameid) {
    const formData = `Cta1=${$(cta1id).val()}&Cta2=${$(cta2id).val()}&Cta3=${$(cta3id).val()}
        &Cta4=${$(cta4id).val()}&Cta5=${$(cta5id).val()}&Cta6=${$(cta6id).val()}`;

    getAccountName(formData, nameid);
}

function getAccountName(formData, accountNameFieldId) {
    $.ajax({
        url: '/Cias/GetAccountName',
        type:'POST',
        data: formData,
        success:function(data){
            if(data.success){
                $(accountNameFieldId).val(data.data);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function watchListOfAccountNumbers(accounts) {
    $.each(accounts, function (index, value) {
        watchAccountNumbersForName({
            accountObj: value,
            valid: function (data) {
                makeAccountNameReq(
                    `#${data.ac1}`,
                    `#${data.ac2}`,
                    `#${data.ac3}`,
                    `#${data.ac4}`,
                    `#${data.ac5}`,
                    `#${data.ac6}`,
                    `#${data.name}`
                );
            },
            invalid: function (data) {
                $(`#${data.name}`).val('');
            }
        });
    });
}

function watchAccountNumbersForName({accountObj, valid, invalid}) {
    $(`#${accountObj.ac6}`).on('keyup', function (e) {
        if (isValidAccountNumber(`#${accountObj.ac1}`) && isValidAccountNumber(`#${accountObj.ac2}`)
            && isValidAccountNumber(`#${accountObj.ac3}`) && isValidAccountNumber(`#${accountObj.ac4}`)
            && isValidAccountNumber(`#${accountObj.ac5}`) && isValidAccountNumber(`#${accountObj.ac6}`)) {
            valid(accountObj);
        } else {
            invalid(accountObj);
        }
    });
}

function watchAccountNumbers({accountObj, valid, invalid}) {
    $(`#${accountObj.ac1},#${accountObj.ac2},#${accountObj.ac3},#${accountObj.ac4},#${accountObj.ac5},#${accountObj.ac6}`).on('keyup', function (e) {
        if (isValidAccountNumber(`#${accountObj.ac1}`) && isValidAccountNumber(`#${accountObj.ac2}`)
            && isValidAccountNumber(`#${accountObj.ac3}`) && isValidAccountNumber(`#${accountObj.ac4}`)
            && isValidAccountNumber(`#${accountObj.ac5}`) && isValidAccountNumber(`#${accountObj.ac6}`)) {
            valid(accountObj);
        } else {
            console.log('called');
            invalid(accountObj);
        }
    });
}