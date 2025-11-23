using System;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the abstract base class for data attached to a graph edge.
/// Supports JSON polymorphism to handle different relationship types during serialization.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(Edge_Friends), typeDiscriminator: "friend")]
[JsonDerivedType(typeof(WorksAtEdge), typeDiscriminator: "work")]
public abstract class Edge_Data
{
    /// <summary>
    /// Gets or sets the weight (cost) of the edge.
    /// Default value is 1.0. Used by algorithms like Dijkstra's to find the optimal path.
    /// </summary>
    public double Weight { get; set; } = 1.0;

    /// <summary>
    /// Returns a human-readable description of the relationship.
    /// Must be implemented by derived classes.
    /// </summary>
    /// <returns>A string describing the edge details.</returns>
    public abstract string GetDescription();

    /// <summary>
    /// Represents a friendship relationship between two nodes (e.g., between two People).
    /// </summary>
    public class Edge_Friends : Edge_Data
    {
        /// <summary>
        /// Gets or sets the date when the friendship started.
        /// </summary>
        public DateTime FriendsSince { get; set; }

        /// <summary>
        /// Returns a description of the friendship duration.
        /// </summary>
        public override string GetDescription()
        {
            return $"Friends since {FriendsSince.Year}";
        }
    }

    /// <summary>
    /// Represents a professional relationship (e.g., a Person working at a Company).
    /// </summary>
    public class WorksAtEdge : Edge_Data
    {
        /// <summary>
        /// Gets or sets the job title or role associated with this connection.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Returns a description of the professional role.
        /// </summary>
        public override string GetDescription()
        {
            return $"Works as {Role}";
        }
    }
}
