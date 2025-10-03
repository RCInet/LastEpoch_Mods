using System.Runtime.Serialization;

namespace Unity.Cloud.CommonEmbedded
{
    [DataContract]
    struct ProjectDescriptorDto
    {
        [DataMember(Name = "organizationId")]
        public string OrganizationId { get; set; }

        [DataMember(Name = "projectId")]
        public string ProjectId { get; set; }

        public ProjectDescriptorDto(ProjectDescriptor projectDescriptor)
        {
            OrganizationId = projectDescriptor.OrganizationId.ToString();
            ProjectId = projectDescriptor.ProjectId.ToString();
        }
    }
}
