using System;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the abstract base class for data payload stored within a graph node.
/// Supports polymorphic JSON serialization/deserialization to handle various entity types.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(PersonData), typeDiscriminator: "person")]
[JsonDerivedType(typeof(CityData), typeDiscriminator: "city")]
[JsonDerivedType(typeof(CompanyData), typeDiscriminator: "company")]
public abstract class Graph_Data
{
    /// <summary>
    /// Returns a short, human-readable name or label for the entity.
    /// Must be implemented by derived classes.
    /// </summary>
    /// <returns>The display name of the entity.</returns>
    public abstract string GetDisplayName();

    /// <summary>
    /// Returns a detailed string representation of the entity's properties.
    /// Can be overridden to provide specific details.
    /// </summary>
    /// <returns>A formatted string containing entity details.</returns>
    public virtual string GetDetails()
    {
        return $"Data: {GetDisplayName()}";
    }

    /// <summary>
    /// Represents a person entity within the graph.
    /// </summary>
    public class PersonData : Graph_Data
    {
        /// <summary>
        /// Gets or sets the full name of the person.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the age of the person in years.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Returns the person's name.
        /// </summary>
        public override string GetDisplayName()
        {
            return Name;
        }

        /// <summary>
        /// Returns the person's name and age details.
        /// </summary>
        public override string GetDetails()
        {
            return $"Person: Name: {Name}, Age: {Age}";
        }
    }

    /// <summary>
    /// Represents a city or geographical location entity.
    /// </summary>
    public class CityData : Graph_Data
    {
        /// <summary>
        /// Gets or sets the name of the city.
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// Gets or sets the population count of the city.
        /// </summary>
        public int Population { get; set; }

        /// <summary>
        /// Returns the city name.
        /// </summary>
        public override string GetDisplayName()
        {
            return CityName;
        }

        /// <summary>
        /// Returns the city name and population details.
        /// </summary>
        public override string GetDetails()
        {
            return $"City: {CityName}, Population: {Population}";
        }
    }

    /// <summary>
    /// Represents a corporate entity or organization.
    /// </summary>
    public class CompanyData : Graph_Data
    {
        /// <summary>
        /// Gets or sets the official name of the company.
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the sector or industry the company operates in.
        /// </summary>
        public string Industry { get; set; }

        /// <summary>
        /// Returns the company name.
        /// </summary>
        public override string GetDisplayName()
        {
            return CompanyName;
        }

        /// <summary>
        /// Returns the company name and industry details.
        /// </summary>
        public override string GetDetails()
        {
            return $"Company: {CompanyName}, Industry: {Industry}";
        }
    }
}
