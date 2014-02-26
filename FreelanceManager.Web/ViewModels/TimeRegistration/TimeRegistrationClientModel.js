function TimeRegistrationClientModel(view, data) {

    var self = this;
    $.extend(self, new BaseViewModel());
    self.view = view;

    self.name = ko.observable();
    self.projects = ko.observableArray([]);

    self.populate = function (data) {
        self.id(data.Id);
        self.name(data.Name);
    }

    self.findProject = function (projectId) {
        return ko.utils.arrayFirst(self.projects(), function (item) {
            return projectId === item.id();
        });
    };

    if (data != null) {
        self.isNew(false);
        self.populate(data);
    }
}