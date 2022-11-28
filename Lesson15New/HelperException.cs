namespace Lesson15New
{
        internal class HelperException : Exception
        {
            public string Type;
            public HelperException(string type, string message) : base(message)
            {
                Type = type;
            }
        }
}