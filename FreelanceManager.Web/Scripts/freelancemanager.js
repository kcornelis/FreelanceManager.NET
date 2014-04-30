$(document).ready(function () {
    $.ajaxSetup({ cache: false });
});

ko.bindingHandlers.moment = {
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = valueAccessor(),
            allBindings = allBindingsAccessor();
        var valueUnwrapped = ko.utils.unwrapObservable(value);
        var pattern = allBindings.pattern || 'yyyy-dd-MM';

        if(valueUnwrapped != undefined)
            $(element).text(valueUnwrapped.format(pattern));
    }
}

ko.bindingHandlers.dropdown = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options = allBindingsAccessor().dropdownOptions || {};

        $(element).selectpicker(options);

        ko.utils.registerEventHandler(element, "change", function () {
            $('.selectpicker').selectpicker('refresh');
        });

        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            // TODO: destroy method for selectpicker
            //$(element).selectpicker("destroy");
        });
    },
    update: function (element, valueAccessor) {
        $('.selectpicker').selectpicker('refresh');
    }
};

function isEmpty(str) {
    return (!str || 0 === str.length);
}

function toggleDialog(selector, show) {
    if (show) {
        $(selector).modal("show");
        
    }
    else $(selector).modal("hide");
}

function minutesToDisplayValue(time) {
    if (!time)
        return '0';

    return ((time - (time % 60)) / 60) + ':' + pad(time % 60, 2);
}

function pad(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

$('body').on('shown.bs.modal', function (e) {
    $(e.target).find("input[type=text]").first().focus();
})
