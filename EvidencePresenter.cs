using Godot;
using System;
using System.Collections.Generic;

public class EvidencePresenter : Control
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	
		
	public List<EvidenceInfo> all_evidence = new List<EvidenceInfo>();
	
	[Export]
	public PackedScene evidence_button_scene;

	public GameHandler game_handler;
		
		
	public AberrantNPC interacting_npc;
	
	private EvidenceInfo selected_evidence;
	private bool evidence_selected = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	public void PopulateEvidence()
	{
		int i = 0;
		foreach (EvidenceInfo evidence in all_evidence)
		{
			TextureButton button = (TextureButton)evidence_button_scene.Instance();
			button.TextureNormal = evidence.item_image_normal;
			button.Connect("pressed", this, "_on_evidence_button_selected", new Godot.Collections.Array { i });
			((HFlowContainer)FindNode("AllEvidence")).AddChild(button);
			i++;
		}
	}
	
	public void ClearEvidence()
	{
		evidence_selected = false;
		foreach (Node button in ((HFlowContainer)FindNode("AllEvidence")).GetChildren())
		{
			button.QueueFree();
			((RichTextLabel)FindNode("EvidenceInfo")).Text = "";
		}
	}
	
	private void DisplayEvidenceInfo(int index)
	{
		GD.Print("Displaying info for evidence ", index, " ", all_evidence[index].name);
		SceneTreeTween tween = GetTree().CreateTween();
		((RichTextLabel)FindNode("EvidenceInfo")).PercentVisible = 0;
		tween.TweenProperty((RichTextLabel)FindNode("EvidenceInfo"), "percent_visible", 1f, 0.85f);
		((RichTextLabel)FindNode("EvidenceInfo")).Text = all_evidence[index].name + "\n\n" + all_evidence[index].description;
	}
	
	private void _on_PC_evidence_found(object evidence)
	{
		all_evidence.Add(((Evidence)evidence).info);
	}
	
	private void _on_evidence_button_selected(int index)
	{
		DisplayEvidenceInfo(index);
		selected_evidence = all_evidence[index];
		evidence_selected = true;
	}

	private void _on_PresentEvidence_pressed()
	{
		if (!evidence_selected) {
			return;
		}
		
		if (game_handler.current_interacting_npc == null) {
			GD.Print("SOMETHING HAS GONE TERRIBLY WRONG");
			return;
		}
		
		PresentEvidence(game_handler.current_interacting_npc.name, selected_evidence.name);
		((Control)game_handler.FindNode("Evidence")).Visible = false;
		((Control)game_handler.FindNode("Dialogue")).Visible = true;
	}
	
	private void PresentEvidence(string npc_name, string item_name)
	{
		switch (npc_name)
		{
			case "Jimmy":
				PresentEvidenceJimmy(item_name);
				break;
		}
	}
	
	private void PresentEvidenceJimmy(string item_name)
	{
		switch (item_name)
		{
			case "Thing":
				game_handler.DisplayDialogue("Ummmm... heheh... What is that exactly??");
				game_handler.current_interacting_npc.evidence += 50f;
				break;
			default:
				game_handler.DisplayDialogue("What? I don't know what that is.");
				break;
		}
		
		
	}
		
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}


