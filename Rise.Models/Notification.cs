using Rise.Common.Enums;
using Rise.Common.Interfaces;
using System;

namespace Rise.Models
{
    public class Notification : DbObject, IEquatable<Notification>, IMatchable<Notification>
    {
        public string Title, Description, Icon;

        public bool Equals(Notification other)
        {
            return Title == other.Title &&
                   Description == other.Description &&
                   Icon == other.Icon;
        }

        public MatchLevel Matches(Notification other)
        {
            if (Title.Equals(other.Title))
            {
                return MatchLevel.Full;
            }

            if (Title.Contains(other.Title))
            {
                return MatchLevel.Partial;
            }

            return MatchLevel.None;
        }
    }
}
