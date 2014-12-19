namespace PropOhGate.Message
{
    public static class Messages
    {
        public const int UpdateTypeIndex = 0;
        public const int ColumnIndex = 4;
        public const int RowIndex = 8;
        public const int ValueIndex = 12;

        public const int MessageTypeLength = 4;
        public const int UpdateMetadataLength = 12;

        public static bool IsUpdate(this byte[] data)
        {
            return MessageType.CellUpdate.IsMessageType(data);
        }

        public static bool IsReset(this byte[] data)
        {
            return MessageType.Reset.IsMessageType(data);
        }

        public static bool IsSubscribe(this byte[] data)
        {
            return MessageType.Subscribe.IsMessageType(data);
        }

        public static bool IsUnsubscribe(this byte[] data)
        {
            return MessageType.Unsubscribe.IsMessageType(data);
        }
    }
}
