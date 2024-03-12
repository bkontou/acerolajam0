using Godot;
using System;

public struct EvidenceInfo
{
	public string name;
	public string description;
	public Texture item_image_normal;
	public Texture item_image_alt;
}

public class Evidence : StaticBody
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	
	[Export]
	public string name;
	[Export]
	public string description;

	
	public EvidenceInfo info;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		info = new EvidenceInfo();
		info.name = name;
		info.description = description;
		info.item_image_normal = ((Sprite3D)FindNode("EvidenceNormal")).Texture;
		info.item_image_alt = ((Sprite3D)FindNode("EvidenceEvil")).Texture;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
