using System;

namespace NetCoreAdmin.Metrics
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "Only this usage intended")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "Only this usage intended")]
    public class UnknownTagErrorException : Exception
    {
        public UnknownTagErrorException(string name, ActuatorTag actuatorTag)
            : base($"The is no Tag {actuatorTag} in  {name} ")
        {
            Name = name;
            ActuatorTag = actuatorTag;
        }

        public string Name { get; }

        public ActuatorTag ActuatorTag { get; }
    }
}