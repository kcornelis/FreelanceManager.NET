﻿function TimeRegistrationReportViewModel() {

    var self = this;
    $.extend(self, new BaseViewModel());

    self.selectedDate = ko.observable(new moment());
    self.info = ko.observable();
    self.billableHours = ko.computed(function () { return self.info() ? self.timeToDisplay(self.info().BillableMinutes) : '0' });
    self.unbillableHours = ko.computed(function () { return self.info() ? self.timeToDisplay(self.info().UnbillableMinutes) : '0' });
    self.income = ko.computed(function () { return self.info() ? self.info().Income : '0' });
    self.infoPerTask = ko.observableArray([]);
    self.selectedDate = ko.observable(new moment(new moment().format("YYYY-MM-DD")));
    self.billableUnbillable = ko.observableArray([]);
    self.hasNoHours = ko.computed(function () { return !self.info() || (self.info().UnbillableMinutes == 0 && self.info().BillableMinutes == 0) }, self);
    self.dateFromSelector = ko.observable(new moment().format("YYYY-MM-DD"));
    self.dateToSelector = ko.observable(new moment().format("YYYY-MM-DD"));

    self.refresh = function () {
        self.info(null);
        self.infoPerTask.removeAll();
        self.markBusy("Loading...");
        $.ajax("/read/timeregistrations/getinfo/" + self.selectedDate().format("YYYY") + "/" + self.selectedDate().format("MM"), {
            type: "get", contentType: "application/json",
            success: function (data) {
                self.loadInfoPerMonth(data.PerMonth);
                self.loadInfoPerTask(data.PerTask);
                self.markNotBusy();
            }
        });
    };
    self.refresh();

    self.loadInfoPerMonth = function (data) {
        self.info(data);
        self.markNotBusy();
        self.billableUnbillable.removeAll();
        self.billableUnbillable.push({ type: "Unbillable", value: data.UnbillableMinutes, display: self.timeToDisplay(data.UnbillableMinutes) });
        self.billableUnbillable.push({ type: "Billable", value: data.BillableMinutes, display: self.timeToDisplay(data.BillableMinutes) });
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
                found.tasks.push({ task: row.Task, income: row.Income, unbillableHours: self.timeToDisplay(row.UnbillableMinutes), billableHours: self.timeToDisplay(row.BillableMinutes) });
            }
            else {
                self.infoPerTask.push({
                    client: row.Client,
                    clientId: row.ClientId,
                    project: row.Project,
                    projectId: row.ProjectId,
                    tasks: ko.observableArray([
                        { task: row.Task, income: row.Income, unbillableHours: self.timeToDisplay(row.UnbillableMinutes), billableHours: self.timeToDisplay(row.BillableMinutes) }
                    ])
                });
            }
        }
    };

    self.nextDate = function () {
        self.selectedDate(self.selectedDate().add('months', 1));
        self.refresh();
    };

    self.previousDate = function () {
        self.selectedDate(self.selectedDate().subtract('months', 1));
        self.refresh();
    };

    self.timeToDisplay = function (time) {
        if (!time)
            return '0';

        return ((time - (time % 60)) / 60) + ':' + (time % 60);
    };
}