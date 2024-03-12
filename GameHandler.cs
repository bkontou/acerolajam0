using Godot;
using System;

public class GameHandler : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	
	public AberrantNPC current_interacting_npc = null;

	public PC pc;
	
	int dialogue_index = 0;
	
	bool game_ended = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().Paused = true;
		GD.Randomize();
		((EvidencePresenter)FindNode("Evidence")).game_handler = this;
		((EvidenceViewer)FindNode("EvidenceViewer")).evidence_presenter = ((EvidencePresenter)FindNode("Evidence"));
		pc = (PC)GetParent().FindNode("PC");
		((EvidenceViewer)FindNode("EvidenceViewer")).pc = pc;
	}
	
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (!GetTree().Paused)
			{
				Input.SetMouseMode(0);
				GetTree().Paused = true;
				((Control)FindNode("PauseMenu")).Visible = true;
			}
		}
	}
	
	public void DisplayDialogue(string text)
	{
		SceneTreeTween tween = GetTree().CreateTween();
		((RichTextLabel)FindNode("DialogueBox")).PercentVisible = 0;
		tween.TweenProperty((RichTextLabel)FindNode("DialogueBox"), "percent_visible", 1f, 0.85f);
		((RichTextLabel)FindNode("DialogueBox")).Text = text;
	}
	
	public void PlayFailureScreen()
	{
		((Control)FindNode("FailureScreen")).Visible = true;
		SceneTreeTween tween = GetTree().CreateTween();
		tween.TweenProperty(((TextureProgress)FindNode("JailBars")), "value", 100, 2.5f);
		Input.SetMouseMode(0);
	}
	
	public void ArrestNPC(string name)
	{
		((Control)FindNode("ScreenFade")).Visible = true;
		SceneTreeTween tween1 = GetTree().CreateTween();

		//tween1.SetTrans((Tween.TransitionType) 10);
		
		tween1.SetParallel(false);
		tween1.TweenProperty((ColorRect)FindNode("FadeRect"), "modulate", new Color(1f,1f,1f,1f), 0.92f);
		tween1.TweenProperty((ColorRect)FindNode("FadeRect"), "modulate", new Color(1f,1f,1f,0f), 1.25f);
		((Label)FindNode("FadeInfo")).Text = name + " has been arrested.";
		tween1.Connect("finished", this, "FadeFinished");
	}
	
	
	private void _on_Resume_pressed()
	{
		Input.SetMouseMode((Input.MouseModeEnum) 2);
		GetTree().Paused = false;
		((Control)FindNode("PauseMenu")).Visible = false;
	}

	private void _on_Quit_pressed()
	{
		// Replace with function body.
	}

	private void _on_Options_pressed()
	{
		// Replace with function body.
	}

	private void _on_Evidence_pressed()
	{
		((Control)FindNode("EvidenceViewer")).Visible = true;
		((EvidenceViewer)FindNode("EvidenceViewer")).PopulateEvidence();
		
	}


	private void _on_EvidenceViewerBack_pressed()
	{
		((Control)FindNode("EvidenceViewer")).Visible = false;
	}

	private void _on_PC_npc_talked_to(object npc)
	{
		GD.Print("talking to ", npc);
		((Control)FindNode("Dialogue")).Visible = true;
		DisplayDialogue("");
		dialogue_index = 0;
		Input.SetMouseMode(0);
		pc.controllable = false;
		current_interacting_npc = (AberrantNPC) npc;
	}

	private void _on_TalkButton_pressed()
	{
		//long dialogue_index = GD.Randi() % current_interacting_npc.talk_lines.Length;
		dialogue_index = dialogue_index % current_interacting_npc.talk_lines.Length;
		DisplayDialogue(current_interacting_npc.talk_lines[dialogue_index]);
		dialogue_index++;
	}


	private void _on_EvidenceButton_pressed()
	{
		((Control)FindNode("Evidence")).Visible = true;
		((Control)FindNode("Dialogue")).Visible = false;
		
		((EvidencePresenter)FindNode("Evidence")).ClearEvidence();
		((EvidencePresenter)FindNode("Evidence")).PopulateEvidence();
	}


	private void _on_AccuseButton_pressed()
	{
		if (current_interacting_npc.evidence >= 43.0)
		{
			DisplayDialogue("Errmmm...");
			ArrestNPC(current_interacting_npc.name);
			current_interacting_npc.QueueFree();
		}
		else 
		{
			DisplayDialogue("How dare you accuse me of such a thing!");
			GD.Print(current_interacting_npc.suspicion);
			current_interacting_npc.suspicion += 25.0f;
		}
	}


	private void _on_GoodbyeButton_pressed()
	{
		Input.SetMouseMode((Input.MouseModeEnum) 2);
		pc.controllable = true;
		((Control)FindNode("PauseMenu")).Visible = false;
		((Control)FindNode("Dialogue")).Visible = false;
		current_interacting_npc = null;
	}
	
	private void _on_CancelEvidence_pressed()
	{
		((Control)FindNode("Evidence")).Visible = false;
		((Control)FindNode("Dialogue")).Visible = true;
	}

	
	private void _on_NPC_suspicion_raised()
	{
		if (game_ended) {
			return;
		} else {
			game_ended = true;
		}

		if (pc.scope_up)
		{
			pc.BringScopeDown();
		}
		
		pc.controllable = false;
		
		PlayFailureScreen();
	}
		



	private void _on_NPC_evidence_raised(string name)
	{
		if (game_ended) {
			return;
		} 
		
		ArrestNPC(name);
	}
	
	
	private void FadeFinished()
	{
		((Control)FindNode("ScreenFade")).Visible = false;
	
		Input.SetMouseMode((Input.MouseModeEnum) 2);
		pc.controllable = true;
		((Control)FindNode("PauseMenu")).Visible = false;
		((Control)FindNode("Dialogue")).Visible = false;
		current_interacting_npc = null;
	}

	
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}



