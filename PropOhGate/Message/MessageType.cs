using System;

namespace PropOhGate.Message
{
    public class MessageType
    {
        public static readonly MessageType CellUpdate = new MessageType(0, "Cell Update");
        public static readonly MessageType Reset = new MessageType(1, "Reset");
        public static readonly MessageType Subscribe = new MessageType(2, "Subscribe");
        public static readonly MessageType Unsubscribe = new MessageType(3, "Unsubscribe");

        private MessageType(int typeId, string description)
        {
            TypeId = BitConverter.GetBytes(typeId);
            Description = description;
        }

        public byte[] TypeId
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        public bool IsMessageType(byte[] data)
        {
            if (data.Length < Messages.MessageTypeLength)
            {
                return false;
            }
            for (int i = 0; i < Messages.MessageTypeLength; i++)
            {
                if (data[i] != TypeId[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
