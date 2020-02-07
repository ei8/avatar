using org.neurul.Common.Domain.Model;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Graph.Client;
using works.ei8.Cortex.Sentry.Domain.Model;
using static works.ei8.Cortex.Sentry.Application.Constants;

namespace works.ei8.Cortex.Sentry.Application
{
    public class AuthorApplicationService : IAuthorApplicationService
    {
        private readonly Uri baseUri;
        private readonly INeuronGraphQueryClient neuronGraphQueryClient;
        private readonly IUserRepository userRepository;
        private readonly IRegionPermitRepository regionPermitRepository;

        public AuthorApplicationService(string baseUrl, INeuronGraphQueryClient neuronGraphQueryClient, IUserRepository userRepository, IRegionPermitRepository regionPermitRepository)
        {
            // TODO: this.baseUri = new Uri(baseUrl);
            this.neuronGraphQueryClient = neuronGraphQueryClient;
            this.userRepository = userRepository;
            this.regionPermitRepository = regionPermitRepository;
        }

        // TODO: TRANSFER THIS METHOD TO DOMAIN.MODEL IF POSSIBLE -> AuthorValidationService
        // TODO: Create a Domain.Model.RequestRedirect class which will contain input request, redirect url
        // TODO: Create a Domain.Model.RoutingService class which returns RequestRedirects, validates requests
        // Should also be called when creating, modifying, or deactivating a terminal.
        // If so, neuronId and regionId values specified should correspond to those of presynapticNeuron.
        public async Task<ValidationResult> ValidateWrite(string avatarId, Guid neuronId, Guid regionId, Guid subjectId, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentNotNull(avatarId, nameof(avatarId));
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, neuronId, Constants.Messages.Exception.InvalidId, nameof(neuronId));
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, subjectId, Constants.Messages.Exception.InvalidId, nameof(subjectId));

            Uri.TryCreate(this.baseUri, avatarId, out Uri avatarUri);
            var author = await this.GetAuthorBySubjectId(avatarId, subjectId, token);
            
            // Ensure that Neuron Id is equal to AuthorId if first Neuron is being created
            if ((await this.neuronGraphQueryClient.GetNeurons(avatarUri.ToString(), limit: 1, token: token)).Count() == 0)
                AssertionConcern.AssertArgumentValid(m => m == author.User.NeuronId, neuronId, "Author Neuron is expected since Avatar is empty.", nameof(neuronId));
            // Ensure that Neuron Id is not equal to AuthorId if non-first Neuron is being created
            else
                AssertionConcern.AssertArgumentValid(m => m != author.User.NeuronId, neuronId, "Author Neuron was not expected since Avatar is not empty.", nameof(neuronId));

            // if region was specified, check if it exists
            if (regionId != Guid.Empty)
            {
                // ensure that layer is a valid neuron
                var region = await this.neuronGraphQueryClient.GetNeuronById(
                    avatarUri.ToString() + "/",
                    regionId.ToString(),
                    token: token
                    );
                AssertionConcern.AssertStateTrue(region != null, "Invalid region specified");
            }

            // get reference to neuron being modified
            var neuron = (await this.neuronGraphQueryClient.GetNeuronById(
                avatarUri.ToString(),
                neuronId.ToString(),
                token: token
                )).First();

            // get write permit of author user for region
            var permit = author.Permits.SingleOrDefault(l => l.RegionNeuronId == regionId && l.WriteLevel > 0);

            // does author user have a write permit
            AssertionConcern.AssertStateTrue(
                permit != null,
                string.Format(Messages.Exception.UnauthorizedLayerWriteTemplate, neuron.LayerTag)
                );

            // TODO: test
            // does neuron already exist
            if (neuron != null)
            {                
                AssertionConcern.AssertArgumentValid(r => r.ToString() == neuron.LayerId, regionId, "Specified RegionId does not match RegionId of specified Neuron.", nameof(regionId));

                // does author user have an admin write access, or author user is the author of this neuron
                AssertionConcern.AssertStateTrue(
                    permit.WriteLevel == 2 || neuron.AuthorId == author.User.NeuronId.ToString(),
                    string.Format(Messages.Exception.UnauthorizedNeuronWriteTemplate, neuron.Tag)
                    );
            }

            return new ValidationResult(new string[0], true);
        }

        public async Task<Author> GetAuthorBySubjectId(string avatarId, Guid subjectId, CancellationToken token = default)
        {
            AssertionConcern.AssertArgumentNotNull(avatarId, nameof(avatarId));
            AssertionConcern.AssertArgumentValid(g => g != Guid.Empty, subjectId, Constants.Messages.Exception.InvalidId, nameof(subjectId));
            User user = await this.userRepository.GetBySubjectId(subjectId);
            AssertionConcern.AssertStateTrue(user != null, Constants.Messages.Exception.UnauthorizedUserAccess);

            Uri.TryCreate(this.baseUri, avatarId, out Uri result);
            // TODO: check if null if neuron is inactive or deactivated, if so, should throw exception
            var userNeuron = (await this.neuronGraphQueryClient.GetNeuronById(
                result.ToString(),
                user.NeuronId.ToString(),
                token: token
                )).First();
            AssertionConcern.AssertStateTrue(userNeuron != null, Constants.Messages.Exception.NeuronNotFound);

            var permits = await this.regionPermitRepository.GetAllByUserNeuronId(user.NeuronId);
            var author = new Author(
                user,
                permits
                );
            return author;
        }
    }
}
