using System;
using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(PersonData), typeDiscriminator: "person")] 
[JsonDerivedType(typeof(CityData), typeDiscriminator: "city")]
[JsonDerivedType(typeof(CompanyData), typeDiscriminator: "company")]

public abstract class Graph_Data
{
    public abstract string GetDisplayName();
    public virtual string GetDetails()
    {
        return $"Data: {GetDisplayName}";
    }
    public class PersonData : Graph_Data
    {
        public string Name { get; set; }
        public int Age { get; set; }
        override public string GetDisplayName()
        {
            return Name;
        }
        override public string GetDetails()
        {
            return $"Person: Name: {Name}, Age: {Age}";
        }
    }
    public class CityData : Graph_Data
    {
        public string CityName { get; set; }
        public int Population { get; set; }
        override public string GetDisplayName()
        {
            return CityName;
        }
        override public string GetDetails()
        {
            return $"City: {CityName}, Population: {Population}";
        }
    }
    public class CompanyData : Graph_Data
    {
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public override string GetDisplayName()
        {
            return CompanyName;
        }
        public override string GetDetails()
        {
            return $"Company: {CompanyName}, Indystry: {Industry}";
        }
    }

}
