function TimeRegistrationProjectModel(view, data) {

    var self = this;
    $.extend(self, new BaseViewModel());
    self.view = view;

    self.name = ko.observable();
    self.tasks = ko.observableArray([]);

    self.populate = function (data) {
        self.id(data.Id);
        self.name(data.Name);
        $.each(data.Tasks, function (taskIndex, task) {
            self.tasks.push(new TimeRegistrationTaskModel(self.view, task));
        });
    }

    self.findTask = function (task) {
        return ko.utils.arrayFirst(self.tasks(), function (item) {
            return task === item.name();
        });
    };

    if (data != null) {
        self.isNew(false);
        self.populate(data);
    }
}