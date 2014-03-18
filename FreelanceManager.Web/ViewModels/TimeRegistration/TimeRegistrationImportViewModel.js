function TimeRegistrationImportViewModel() {

    var self = this;
    $.extend(self, new BaseViewModel());

    self.projects = ko.observableArray([]);

    self.refreshProjects = function () {

        self.markBusy("Loading...");

        $.ajax("/read/projects", {
            type: "get", contentType: "application/json",
            success: function (data) {
                $.each(data, function (index, project) {
                    self.projects.push(new TimeRegistrationImportProjectModel(self, project));
                });
                self.markNotBusy();
            }
        });
    };

    self.refreshProjects();
}