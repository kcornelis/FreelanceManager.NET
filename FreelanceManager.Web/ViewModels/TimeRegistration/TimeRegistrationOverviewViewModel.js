function TimeRegistrationOverviewViewModel() {

    var self = this;
    $.extend(self, new BaseViewModel());

    
    self.timeRegistrations = ko.observableArray([]);
    self.hasNoTimeRegistrations = ko.computed(function () { return self.timeRegistrations().length == 0; }, self);

    self.selectedFromDate = ko.observable();
    self.dateFromSelector = ko.observable(); // for the date picker
    self.dateFromSelector.subscribe(function () {
        self.selectedFromDate(new moment(self.dateFromSelector()));
        self.refresh();
    });

    self.selectedToDate = ko.observable();
    self.dateToSelector = ko.observable(); // for the date picker
    self.dateToSelector.subscribe(function () {
        self.selectedToDate(new moment(self.dateToSelector()));
        self.refresh();
    });

    self.title = ko.computed(function () {

        if (!self.selectedFromDate() || !self.selectedToDate())
            return '';

        return 'From ' + self.selectedFromDate().format('DD MMMM YYYY') + ' to ' + self.selectedToDate().format('DD MMMM YYYY');
    });

    self.init = function () {
        self.dateFromSelector(new moment().set('date', 1).format("YYYY-MM-DD"));
        self.dateToSelector(new moment().set('date', new moment().daysInMonth()).format("YYYY-MM-DD"));
    };
    
    self.refresh = function () {

        if (!self.dateFromSelector() || !self.dateToSelector())
            return;

        self.timeRegistrations([]);
        self.markBusy("Loading...");
        $.ajax("/read/timeregistrations/getforperiod/" + self.dateFromSelector() + "/" + self.dateToSelector(), {
            type: "get", contentType: "application/json",
            success: function (data) {

                var grouped = _.groupBy(data, function (i) { return i.Date.Display });
                var converted = _.map(grouped, function (g) {
                    return {
                        date: _.first(g).Date,
                        items: g
                    };
                })
                self.timeRegistrations(converted);
                self.markNotBusy();
            }
        });
    };

    self.init();
}