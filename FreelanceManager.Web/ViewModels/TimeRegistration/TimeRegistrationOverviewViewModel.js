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

        return self.selectedFromDate().format('DD MMMM YYYY') + ' - ' + self.selectedToDate().format('DD MMMM YYYY');
    });

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

    self.changePeriod = function (period, type) {

        self.dateFromSelector(null);
        self.dateToSelector(null);

        var from = new moment();
        var to = new moment();

        if (type == 'week') {
            from = new moment().day(1);
            to = new moment().day(7);
        }
        else if (type == 'month') {
            from = new moment().set('date', 1);
            to = new moment().set('date', new moment().daysInMonth());
        }
        else if (type == 'year') {
            from = new moment().set('month', 0).set('date', 1);
            to = new moment().set('month', 11).set('date', 31);
        }

        if (period == 'previous') {
            if (type == 'week') {
                from = from.subtract('days', 7);
                to = to.subtract('days', 7);
            }
            else if (type == 'month') {
                from = from.subtract('months', 1);
                to = new moment(from).set('date', from.daysInMonth());
            }
            else if (type == 'year') {
                from = from.subtract('years', 1);
                to = to.subtract('years', 1);
            }
        }

        self.dateFromSelector(from.format('YYYY-MM-DD'));
        self.dateToSelector(to.format('YYYY-MM-DD'));
    };

    self.changePeriod('current', 'month');
}