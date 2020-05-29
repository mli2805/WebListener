namespace UtilsLib
{
    public interface IMyLog
    {
        IMyLog AssignFile(string filename);
        void EmptyLine(char ch = ' ');
        void AppendLine(string message, int offset = 0, int messageLevel = 2, string prefix = "");
    }
}