using System;

namespace Unity.Cloud.IdentityEmbedded
{
    internal class ProjectJson
    {
        public string Id { get; set; }

        public string GenesisId { get; set; }

        public string Name { get; set; }

        public string IconUrl {  get; set; }

        public DateTime? CreatedAt {  get; set; }

        public DateTime? UpdatedAt {  get; set; }

        public DateTime? ArchivedAt {  get; set; }

        public string OrganizationGenesisId {  get; set; }

        public bool EnabledInAssetManager { get; set; }
    }
}
