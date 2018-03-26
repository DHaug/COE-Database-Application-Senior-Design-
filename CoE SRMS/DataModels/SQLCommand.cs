

using System.Data.SqlClient;

namespace CoE_SRMS
{
    class SQLCommand
    {
        public enum CommandType
        {
            Select = 0,
            Insert = 1,
            Update = 2,
            Delete = 3
        };
        public SqlCommand CommandData { get; set; }
        public CommandType? Type { get; set; }

        public SQLCommand()
        {
            CommandData = new SqlCommand();
            Type = null;
        }

        public SQLCommand(SqlCommand command, CommandType commandType)
        {
            CommandData = command;
            Type = commandType;
        }
    }
}
