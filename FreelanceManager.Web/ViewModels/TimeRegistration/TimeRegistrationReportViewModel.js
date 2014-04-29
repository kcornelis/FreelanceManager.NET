function TimeRegistrationReportViewModel() {

    var self = this;
    $.extend(self, new BaseViewModel());

    self.selectedDate = ko.observable(new moment());
    self.info = ko.observable();
    self.infoPerTask = ko.observableArray([]);
    self.selectedDate = ko.observable(new moment(new moment().format("YYYY-MM-DD")));
    self.billableUnbillable = ko.observableArray([]);
    self.hasNoHours = ko.computed(function () { return !self.info() || (self.info().UnbillableHours == 0 && self.info().BillableHours == 0) }, self);
    self.dateFromSelector = ko.observable(new moment().format("YYYY-MM-DD"));
    self.dateToSelector = ko.observable(new moment().format("YYYY-MM-DD"));

    self.refresh = function () {
        self.info(null);
        self.infoPerTask.removeAll();
        self.markBusy("Loading...");
        $.ajax("/read/timeregistrations/getinfoformonth/" + self.selectedDate().format("YYYY") + "/" + self.selectedDate().format("MM"), {
            type: "get", contentType: "application/json",
            success: function (data) {
                self.info(data);
                self.markNotBusy();
                self.billableUnbillable.removeAll();
                self.billableUnbillable.push({ type: "Unbillable", value: data.UnbillableHours });
                self.billableUnbillable.push({ type: "Billable", value: data.BillableHours });
            }
        });
        $.ajax("/read/timeregistrations/getinfopertaskformonth/" + self.selectedDate().format("YYYY") + "/" + self.selectedDate().format("MM"), {
            type: "get", contentType: "application/json",
            success: function (data) {

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
                        found.tasks.push({ task: row.Task, income: row.Income, unbillableHours: row.UnbillableHours, billableHours: row.BillableHours });
                    }
                    else {
                        self.infoPerTask.push({
                            client: row.Client,
                            clientId: row.ClientId,
                            project: row.Project,
                            projectId: row.ProjectId,
                            tasks: ko.observableArray([
                                { task: row.Task, income: row.Income, unbillableHours: row.UnbillableHours, billableHours: row.BillableHours }
                            ])
                        });
                    }
                }
                self.markNotBusy();
            }
        });
    };
    self.refresh();

    self.nextDate = function () {
        self.selectedDate(self.selectedDate().add('months', 1));
        self.refresh();
    };

    self.previousDate = function () {
        self.selectedDate(self.selectedDate().subtract('months', 1));
        self.refresh();
    };
}