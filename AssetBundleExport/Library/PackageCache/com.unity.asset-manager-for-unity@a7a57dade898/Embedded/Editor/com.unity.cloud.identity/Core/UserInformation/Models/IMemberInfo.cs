using System;
using System.Collections.Generic;
using Unity.Cloud.CommonEmbedded;

namespace Unity.Cloud.IdentityEmbedded
{

    internal class RangeResultsJson<T>
    {
        public int Offset { get; set; }

        public int Limit { get; set; }

        public int Total { get; set; }

        public IEnumerable<T> Results { get; set; }
    }

    internal class AssetProjectPageResultsJson<T>
    {
        public IEnumerable<T> Projects { get; set; }
    }

    internal class MemberInfoJson
    {
        public string GenesisId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool IsGuest { get; set; }

        public bool IsProjectGuest { get; set; }

        public bool IsManager { get; set; }

        public bool IsOwner { get; set; }
    }

    /// <summary>
    /// An interface that holds member information.
    /// </summary>
    interface IMemberInfo : IUserInfo
    {
        /// <summary>
        /// The member's role.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// The member's <see cref="GroupId"/>.
        /// </summary>
        public GroupId GroupId { get; set; }

        /// <summary>
        /// The name of the member's group.
        /// </summary>
        public string GroupName { get; set; }
    }

    internal class MemberInfo : IMemberInfo
    {
        internal MemberInfo(MemberInfoJson memberInfoJson)
        {
            Role = GetMemberLegacyRole(memberInfoJson);
            GroupId = GroupId.None;
            GroupName = null;
            UserId = new UserId(memberInfoJson.GenesisId);
            // UserName can sometime be empty, use email instead
            Name = memberInfoJson.Name ?? memberInfoJson.Email;
            Email = memberInfoJson.Email;
        }

        string GetMemberLegacyRole(MemberInfoJson memberInfoJson)
        {
            if (memberInfoJson.IsOwner)
                return "owner";
            if (memberInfoJson.IsManager)
                return "manager";
            if (memberInfoJson.IsGuest)
                return "guest";
            if (memberInfoJson.IsProjectGuest)
                return "project guest";
            // default to user
            return "user";
        }

        public string Role { get; set; }
        public GroupId GroupId { get; set; }
        public string GroupName { get; set; }
        public UserId UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
