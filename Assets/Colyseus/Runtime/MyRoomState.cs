// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.35
// 

using Colyseus.Schema;

public partial class MyRoomState : Schema
{
	[Type(0, "string")]
	public string status = default(string);

	[Type(1, "ref", typeof(Score))]
	public Score teamScore = new Score();

	[Type(2, "array", typeof(ArraySchema<Player>))]
	public ArraySchema<Player> players = new ArraySchema<Player>();

	[Type(3, "number")]
	public float turnIndex = default(float);

	[Type(4, "number")]
	public float roundCompleted = default(float);
}

