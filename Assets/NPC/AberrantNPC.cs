using Godot;
using System;


public class AberrantNPC : StaticBody
{
	[Export]
	public string name = "";
	
	[Export]
	public string[] talk_lines;
	[Export]
	public string[] accuse_lines_wrong;
	[Export]
	public string[] accuse_lines_right;
	
	
	[Export]
	public float suspicion_rate = 1.0f;
	public float suspicion = 0.0f;
	
	public float evidence = 0.0f;
	
	[Signal]
	public delegate void suspicion_raised();
	[Signal]
	public delegate void evidence_raised(string name);
	
	private bool is_active = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	void LoadInfo()
	{
		
	}


	public override void _Process(float delta)
	{
		if (!is_active) {
			return;
		}
		
		if (suspicion >= 100) {
			is_active = false;
			EmitSignal(nameof(suspicion_raised));
		}
		
		if (evidence >= 100) {
			is_active = false;
			((Timer)FindNode("DeathTimer")).Start();
			EmitSignal(nameof(evidence_raised), name);
		}
	}


	private void _on_DeathTimer_timeout()
	{
		QueueFree();
	}
}

