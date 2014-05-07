function TimeRegistrationReportViewModel() {

    var self = this;
    $.extend(self, new BaseViewModel());

    self.info = ko.observable();
    self.billableHours = ko.computed(function () { return self.info() ? minutesToDisplayValue(self.info().BillableMinutes) : '0' });
    self.unbillableHours = ko.computed(function () { return self.info() ? minutesToDisplayValue(self.info().UnbillableMinutes) : '0' });
    self.income = ko.computed(function () { return self.info() ? self.info().Income : '0' });
    self.infoPerTask = ko.observableArray([]);
    self.billableUnbillable = ko.observableArray([]);
    self.hasNoHours = ko.computed(function () { return !self.info() || (self.info().UnbillableMinutes == 0 && self.info().BillableMinutes == 0) }, self);

    self.selectedFromDate = ko.observable();
    self.selectedToDate = ko.observable();
    self.periodType = ko.observable();

    self.title = ko.computed(function () {

        if (!self.selectedFromDate() || !self.selectedFoDate())
            return '';

        if (self.periodType() == 'month') {
            return self.selectedFromDate().format('MMMM YYYY');
        }
        else if (self.periodType() == 'year') {
            return self.selectedFromDate().format('YYYY');
        }

        return self.selectedFromDate().format('YYYY-MM-DD') + '   ' +
               self.selectedToDate().format('YYYY-MM-DD')
    });

    self.changePeriodType = function (type) {
        self.periodType(type);

        self.selectedFromDate(null);
        self.selectedToDate(null);

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

        self.selectedFromDate(from);
        self.selectedToDate(to);
        self.refresh();
    };

    self.refresh = function () {
        self.info(null);
        self.infoPerTask.removeAll();
        self.markBusy("Loading...");
        $.ajax("/read/timeregistrations/getinfo/" + self.selectedFromDate().format("YYYY-MM-DD") + "/" + self.selectedToDate().format("YYYY-MM-DD"), {
            type: "get", contentType: "application/json",
            success: function (data) {
                self.loadInfoForPeriod(data.Summary);
                self.loadInfoPerTask(data.PerTask);
                self.markNotBusy();
            }
        });
    };

    self.loadInfoForPeriod = function (data) {
        self.info(data);
        self.markNotBusy();
        self.billableUnbillable.removeAll();
        self.billableUnbillable.push({ type: "Unbillable", value: data.UnbillableMinutes, display: minutesToDisplayValue(data.UnbillableMinutes) });
        self.billableUnbillable.push({ type: "Billable", value: data.BillableMinutes, display: minutesToDisplayValue(data.BillableMinutes) });
    };

    self.loadInfoPerTask = function (data) {
        for (var i = 0; i < data.length; i++) {
            var found = null;
            var row = data[i];

            for (var j = 0; j < self.infoPerTask().length; j++) {
                if (self.infoPerTask()[j].clientId == row.ClientId && self.infoPerTask()[j].projectId == row.ProjectId) {
                    found = self.infoPerTask()[j];
                    break;
                }
            }

            if (found) {
                found.tasks.push({ task: row.Task, income: row.Income, unbillableHours: minutesToDisplayValue(row.UnbillableMinutes), billableHours: minutesToDisplayValue(row.BillableMinutes) });
            }
            else {
                self.infoPerTask.push({
                    client: row.Client,
                    clientId: row.ClientId,
                    project: row.Project,
                    projectId: row.ProjectId,
                    tasks: ko.observableArray([
                        { task: row.Task, income: row.Income, unbillableHours: minutesToDisplayValue(row.UnbillableMinutes), billableHours: minutesToDisplayValue(row.BillableMinutes) }
                    ])
                });
            }
        }
    };

    self.nextDate = function () {

        if (self.periodType() == 'week') {
            self.selectedFromDate(new moment(self.selectedFromDate().add('days', 7)));
            self.selectedToDate(new moment(self.selectedFromDate()).add('days', 6));
        }
        else if (self.periodType() == 'month') {
            self.selectedFromDate(new moment(self.selectedFromDate().add('months', 1)));
            self.selectedToDate(new moment(self.selectedFromDate()).set('date', self.selectedFromDate().daysInMonth()));
        }
        else if (self.periodType() == 'year') {
            self.selectedFromDate(new moment(self.selectedFromDate().add('years', 1)));
            self.selectedToDate(new moment(self.selectedFromDate()).set('month', 11).set('date', 31));
        }

        self.refresh();
    };

    self.previousDate = function () {
        
        if (self.periodType() == 'week') {
            self.selectedFromDate(new moment(self.selectedFromDate().subtract('days', 7)));
            self.selectedToDate(new moment(self.selectedFromDate()).add('days', 6));
        }
        else if (self.periodType() == 'month') {
            self.selectedFromDate(new moment(self.selectedFromDate().subtract('months', 1)));
            self.selectedToDate(new moment(self.selectedFromDate()).set('date', self.selectedFromDate().daysInMonth()));
        }
        else if (self.periodType() == 'year') {
            self.selectedFromDate(new moment(self.selectedFromDate().subtract('years', 1)));
            self.selectedToDate(new moment(self.selectedFromDate()).set('month', 11).set('date', 31));
        }
        
        self.refresh();
    };

    self.changePeriodType('month');
}