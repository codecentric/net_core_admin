namespace Nactuator
{
    /// <summary>
    /// see https://docs.spring.io/spring-boot/docs/2.2.2.RELEASE/actuator-api/html/#env for format and doc
    /// </summary>
    public class PropertyValue
    {
        public PropertyValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }
    }
}