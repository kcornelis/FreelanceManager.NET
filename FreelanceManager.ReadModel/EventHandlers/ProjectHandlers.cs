using System.Linq;
using FreelanceManager.Events.Client;
using FreelanceManager.Events.Project;
using FreelanceManager.ReadModel.Repositories;

namespace FreelanceManager.ReadModel.EventHandlers
{
    public class ProjectHandlers : IHandleEvent<ProjectCreated>,
                                   IHandleEvent<ProjectTasksChanged>,
                                   IHandleEvent<ProjectDetailsChanged>,
                                   IHandleEvent<ClientDetailsChanged>,
                                   IHandleEvent<ProjectHidden>,
                                   IHandleEvent<ProjectUnhidden>
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IClientRepository _clientRepository;

        public ProjectHandlers(IProjectRepository projectRepository, IClientRepository clientRepository)
        {
            _projectRepository = projectRepository;
            _clientRepository = clientRepository;
        }

        public void Handle(ProjectCreated @event)
        {
            var client = _clientRepository.GetById(@event.ClientId);

            var project = new Project
            {
                Id = @event.Id,
                CreatedOn = @event.CreatedOn,
                Name = @event.Name,
                ClientId = @event.ClientId,
                Description = @event.Description,
                ClientName = client.Name
            };

            _projectRepository.Add(project);
        }

        public void Handle(ProjectDetailsChanged @event)
        {
            var project = _projectRepository.GetById(@event.Id);

            if (project != null)
            {
                project.Name = @event.Name;
                project.Description = @event.Description;

                _projectRepository.Update(project);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(ProjectTasksChanged @event)
        {
            var project = _projectRepository.GetById(@event.Id);

            if (project != null)
            {
                project.Tasks = @event.Tasks.Select(p => new Task { Name = p.Name, Rate = p.Rate }).ToArray();

                _projectRepository.Update(project);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(ProjectHidden @event)
        {
            var project = _projectRepository.GetById(@event.Id);

            if (project != null)
            {
                project.Hidden = true;

                _projectRepository.Update(project);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(ProjectUnhidden @event)
        {
            var project = _projectRepository.GetById(@event.Id);

            if (project != null)
            {
                project.Hidden = false;

                _projectRepository.Update(project);
            }
            else
            {
                throw new ModelNotFoundException();
            }
        }

        public void Handle(ClientDetailsChanged @event)
        {
            var projects = _projectRepository.FindForClient(@event.Id);

            foreach (var project in projects)
            {
                project.ClientName = @event.Name;

                _projectRepository.Update(project);
            }
        }
    }
}
