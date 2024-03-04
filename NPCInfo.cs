using Godot;
using System;

public class NPCInfo : Panel
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	
	public AberrantNPC current_viewed_npc = null;
	public PC pc;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public override void _Process(float delta)
	{
		if (current_viewed_npc != null)
		{
			UpdateNPCInfo(delta);
		}
	}
	
	void UpdateNPCInfo(float delta)
	{
		if (current_viewed_npc == null) {
			return;
		}
		
		if (pc.special_camera.Current) {
			current_viewed_npc.suspicion += delta * current_viewed_npc.suspicion_rate;
		}
		
		((TextureProgress)FindNode("SuspicionBar")).Value = current_viewed_npc.suspicion;
		((TextureProgress)FindNode("EvidenceBar")).Value = current_viewed_npc.evidence;
	}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
