using System;
using System.Text.Json.Serialization;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(Edge_Friends), typeDiscriminator: "friend")]
[JsonDerivedType(typeof(WorksAtEdge), typeDiscriminator: "work")]
public abstract class Edge_Data
{
    public double Weight { get; set; } = 1.0;
    public abstract string GetDecscription();
    public class Edge_Friends : Edge_Data
    {
        public DateTime FriendsSince { get; set; }
        public override string GetDecscription()
        {
            return $"Friends since {FriendsSince.Year}";
        }
    }
    public class WorksAtEdge : Edge_Data
    {
        public string Role { get; set; }
        public override string GetDecscription()
        {
            return $"Works as {Role}";
        }
    }
}
