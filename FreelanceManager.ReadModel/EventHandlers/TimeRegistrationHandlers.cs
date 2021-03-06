﻿using System;
using FreelanceManager.Events.Client;
using FreelanceManager.Events.Project;
using FreelanceManager.Events.TimeRegistration;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class TimeRegistrationHandlers : IHandleEvent<TimeRegistrationCreated>,
                                            IHandleEvent<TimeRegistrationDetailsChanged>,
                                            IHandleEvent<TimeRegistrationCorrectedIncomeCleared>,
                                            IHandleEvent<TimeRegistrationIncomeCorrected>,
                                            IHandleEvent<TimeRegistrationRateRefreshed>,
                                            IHandleEvent<ClientDetailsChanged>,
                                            IHandleEvent<ProjectDetailsChanged>,
                                            IHandleEvent<TimeRegistrationDeleted>
    {
        private readonly ITimeRegistrationRepository _timeRegistrationRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IClientRepository _clientRepository;

        public TimeRegistrationHandlers(ITimeRegistrationRepository timeRegistrationRepository, 
            IProjectRepository projectRepository, 
            IClientRepository clientRepository)
        {
            _timeRegistrationRepository = timeRegistrationRepository;
            _projectRepository = projectRepository;
            _clientRepository = clientRepository;
        }

        public void Handle(TimeRegistrationCreated @event)
        {
            var client = _clientRepository.GetById(@event.ClientId);
            var project = _projectRepository.GetById(@event.ProjectId);
            var totalMinutes = @event.From.TotalMinutes(@event.To);

            var timeRegistration = new TimeRegistration
            {
                Id = @event.Id,
                ClientId = @event.ClientId,
                ClientName = client != null ? client.Name : "",
                ProjectId = @event.ProjectId,
                ProjectName = project != null ? project.Name : "",
                Task = @event.Task,
                Description = @event.Description,
                Date = @event.Date,
                From = @event.From,
                To = @event.To,
                Minutes = totalMinutes,
                CreatedOn = @event.CreatedOn,
                Rate = @event.Rate,
                Income = Math.Round((((decimal)totalMinutes * @event.Rate) / 60), 2)
            };

            _timeRegistrationRepository.Add(timeRegistration, @event.Version);
        }

        public void Handle(TimeRegistrationDetailsChanged @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            if (timeRegistration != null)
            {
                var client = _clientRepository.GetById(@event.ClientId);
                var project = _projectRepository.GetById(@event.ProjectId);
                var totalMinutes = @event.From.TotalMinutes(@event.To);

                timeRegistration.Id = @event.Id;
                timeRegistration.ClientId = @event.ClientId;
                timeRegistration.ClientName = client != null ? client.Name : "";
                timeRegistration.ProjectId = @event.ProjectId;
                timeRegistration.ProjectName = project != null ? project.Name : "";
                timeRegistration.Task = @event.Task;
                timeRegistration.Description = @event.Description;
                timeRegistration.Date = @event.Date;
                timeRegistration.From = @event.From;
                timeRegistration.Minutes = totalMinutes;
                timeRegistration.To = @event.To;
                timeRegistration.Income = Math.Round((((decimal)totalMinutes * timeRegistration.Rate) / 60), 2);

                _timeRegistrationRepository.Update(timeRegistration, @event.Version);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(TimeRegistrationCorrectedIncomeCleared @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            if (timeRegistration != null)
            {
                var totalMinutes = timeRegistration.From.TotalMinutes(timeRegistration.To);

                timeRegistration.CorrectedIncome = null;
                timeRegistration.CorrectedIncomeMessage = null;
                timeRegistration.Income = Math.Round((((decimal)totalMinutes * timeRegistration.Rate) / 60), 2);

                _timeRegistrationRepository.Update(timeRegistration, @event.Version);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(TimeRegistrationIncomeCorrected @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            if (timeRegistration != null)
            {
                timeRegistration.CorrectedIncome = @event.Amount;
                timeRegistration.CorrectedIncomeMessage = @event.Message;
                timeRegistration.Income = @event.Amount;

                _timeRegistrationRepository.Update(timeRegistration, @event.Version);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(TimeRegistrationRateRefreshed @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            if (timeRegistration != null)
            {
                var totalMinutes = timeRegistration.From.TotalMinutes(timeRegistration.To);

                timeRegistration.Rate = @event.Rate;
                timeRegistration.Income = timeRegistration.CorrectedIncome != null ? (decimal)timeRegistration.CorrectedIncome : Math.Round((((decimal)totalMinutes * timeRegistration.Rate) / 60), 2);

                _timeRegistrationRepository.Update(timeRegistration, @event.Version);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(ClientDetailsChanged @event)
        {
            var timeRegistrations = _timeRegistrationRepository.FindForClient(@event.Id);

            foreach (var timeRegistration in timeRegistrations)
            {
                timeRegistration.ClientName = @event.Name;

                _timeRegistrationRepository.Update(timeRegistration);
            }
        }

        public void Handle(ProjectDetailsChanged @event)
        {
            var timeRegistrations = _timeRegistrationRepository.FindForProject(@event.Id);

            foreach (var timeRegistration in timeRegistrations)
            {
                timeRegistration.ProjectName = @event.Name;

                _timeRegistrationRepository.Update(timeRegistration);
            }
        }

        public void Handle(TimeRegistrationDeleted @event)
        {
            var timeRegistration = _timeRegistrationRepository.GetById(@event.Id);

            if (timeRegistration != null)
            {
                _timeRegistrationRepository.Delete(timeRegistration, @event.Version);
            }
        }
    }
}
