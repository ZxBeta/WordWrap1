// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 1.0.35
// 

using Colyseus.Schema;

public partial class Player : Schema {
	[Type(0, "string")]
	public string id = default(string);

	[Type(1, "number")]
	public float index = default(float);

	[Type(2, "string")]
	public string userName = default(string);

	[Type(3, "number")]
	public float rank = default(float);

	[Type(4, "string")]
	public string avatar = default(string);

	[Type(5, "string")]
	public string mode = default(string);

	[Type(6, "number")]
	public float dbId = default(float);

	[Type(7, "number")]
	public float points = default(float);

	[Type(8, "boolean")]
	public bool isBot = default(bool);

	[Type(9, "number")]
	public float roundWon = default(float);

	[Type(10, "boolean")]
	public bool connected = default(bool);
}

