namespace UtilsLib
{
    public class NullLog : IMyLog
    {

        public IMyLog AssignFile(string filename)
        {
            return null;
        }

        public void EmptyLine(char ch = ' ')
        {
        }

        public void AppendLine(string message, int offset = 0, int messageLevel = 2, string prefix = "")
        {
        }
    }
}