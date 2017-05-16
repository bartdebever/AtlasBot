using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibary.Data;

namespace DataLibary.MSSQLContext
{
    public class MessageContext : IMessageContext
    {
        public List<string> GetAllMessages(ulong serverid)
        {
            throw new NotImplementedException();
        }

        public string GetDefaultMessage(string code)
        {
            string query = "SELECT [M].Reaction FROM [Message] M WHERE [M].Code = @Code AND [M].ServerId IS NULL";
            SqlCommand cmd = new SqlCommand(query, Database.Connection());
            cmd.Parameters.AddWithValue("@Code", code);
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetString(0);
                }
            }
            return "I made a mistake :c Please message Bort to help me quickly!";
        }

        public string GetMessage(string code, ulong serverid)
        {
            throw new NotImplementedException();
        }

        public void AddMessage(string code, ulong serverid, string message)
        {
            throw new NotImplementedException();
        }

        public void RemoveMessage(string code, ulong serverid)
        {
            throw new NotImplementedException();
        }
    }
}
