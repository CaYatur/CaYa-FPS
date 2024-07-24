using System;
using Unity.Collections;
using Unity.Netcode;

[Serializable]
public struct ChatMessage : INetworkSerializable
{
	public FixedString128Bytes sender;

	public FixedString512Bytes message;

	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref sender, default(FastBufferWriter.ForFixedStrings));
		serializer.SerializeValue(ref message, default(FastBufferWriter.ForFixedStrings));
	}
}
