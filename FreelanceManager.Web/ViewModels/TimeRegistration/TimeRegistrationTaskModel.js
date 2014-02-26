function TimeRegistrationTaskModel(view, data) {

    var self = this;
    $.extend(self, new BaseViewModel());
    self.view = view;

    self.name = ko.observable();
    self.rate = ko.observable();

    self.populate = function (data) {
        self.id(data.Id);
        self.name(data.Name);
        self.rate(data.Rate.Value);
    }

    if (data != null) {
        self.isNew(false);
        self.populate(data);
    }
}