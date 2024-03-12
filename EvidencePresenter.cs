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
			case "Jimmryn Glonch":
				PresentEvidenceJimmryn(item_name);
				break;
			case "Mopp Mahennity":
				PresentEvidenceMopp(item_name);
				break;
			case "Chum Maverick":
				PresentEvidenceChum(item_name);
				break;
			case "Frito Sipps":
				PresentEvidenceFrito(item_name);
				break;
			case "Scran McDermit":
				PresentEvidenceScran(item_name);
				break;
			case "Sheryl Krum":
				PresentEvidenceSheryl(item_name);
				break;

		}
	}
	
	private void PresentEvidenceJimmryn(string item_name)
	{
		string dialogue_line = "";
		float evidence = 0;
		float suspicion = 0;
		switch (item_name)
		{
			case "Old Receipt":
				dialogue_line = "Huh? Oh! Uhhh. That could be any Jimmryn’s receipt for 33 Man Pillows!";
				evidence = 15;
				break;
			case "Old Photo":
				dialogue_line = "Where’d you get that? Yeah, that's me and my buddy from work. Did he throw that away? I gave that to him";
				suspicion = 15;
				break;
			case "Manilla Envelope":
				dialogue_line = "What is that? I swear to god I don’t know anything about that.";
				suspicion = 5;
				evidence = 20;
				break;
			case "Used Tissue":
				dialogue_line = "Why are you showing me trash? Is that what you think of me?";
				suspicion = 5;
				break;
			case "Banana Peel":
				dialogue_line = "Sorry, I only really eat the banana part, but thanks anyway.";
				suspicion = 10;
				evidence = 0;
				break;
			case "Torn Shirt":
				dialogue_line = "I know a guy who could fix that up for cheap";
				suspicion = 0;
				evidence = 0;
				break;
			case "Spray Paint":
				dialogue_line = "What are you suggesting with that?";
				suspicion = 5;
				evidence = 5;
				break;
			case "Set of Keys":
				dialogue_line = "Wow! I went to Cancun once! Very nice place.";
				suspicion = 0;
				evidence = 0;
				break;
			case "A Normal Financial Report":
				dialogue_line = "I love business.";
				suspicion = 0;
				evidence = 0;
				break;
			case "Old Scratchoff Ticket":
				dialogue_line = "Huh? I don’t know what that is.";
				suspicion = 0;
				evidence = 5;
				break;
			default:
				dialogue_line = "What? I don't know what that is.";
				break;
		}
		
		game_handler.DisplayDialogue(dialogue_line);
		game_handler.current_interacting_npc.evidence += evidence;
		game_handler.current_interacting_npc.suspicion += evidence;
	}
	
	private void PresentEvidenceMopp(string item_name)
	{
		string dialogue_line = "";
		float evidence = 0;
		float suspicion = 0;
		switch (item_name)
		{
			case "Old Receipt":
				dialogue_line = "No idea what that is!";
				evidence = 0;
				break;
			case "Old Photo":
				dialogue_line = "That photo. I thought I… Hey, why don’t you give that to me so I can dispose of it properly?";
				evidence = 15;
				break;
			case "Manilla Envelope":
				dialogue_line = "Errmmm… I do not know what that is or has to do with me… haha, what do you mean my name is in it?";
				suspicion = 0;
				evidence = 25;
				break;
			case "Used Tissue":
				dialogue_line = "Is that..? I mean, ahem, thank you for this gift, friend. Heheh…";
				evidence = 25;
				break;
			case "Banana Peel":
				dialogue_line = "This is some sort of joke? Or is this an act of cruelty?";
				suspicion = 15;
				evidence = 0;
				break;
			case "Torn Shirt":
				dialogue_line = "Wow, nice shirt!";
				suspicion = 0;
				evidence = 0;
				break;
			case "Spray Paint":
				dialogue_line = "Mmmm, looks good!";
				suspicion = 0;
				evidence = 5;
				break;
			case "Set of Keys":
				dialogue_line = "Woah do you drive a… wait, no, those probably aren’t yours.";
				suspicion = 0;
				evidence = 5;
				break;
			case "A Normal Financial Report":
				dialogue_line = "Huh? Are you trying to accuse me of something?";
				suspicion = 10;
				evidence = 0;
				break;
			case "Expensive Watch":
				dialogue_line = "That’s a rare watch. I rarely see one on this pla-... -ace.";
				suspicion = 0;
				evidence = 5;
				break;	
			case "Pamphlet":
				dialogue_line = "What? Don’t show me that. And don’t associate with those people.";
				suspicion = 15;
				evidence = 5;
				break;
			case "Alien Action Figure":
				dialogue_line = "Now thats just offensive… I mean, it would be to someone, at least…";
				suspicion = 10;
				evidence = 5;
				break;
			case "Old Scratchoff Ticket":
				dialogue_line = "Huh? I don’t know what that is.";
				suspicion = 0;
				evidence = 0;
				break;
			case "Empty Can of Beer":
				dialogue_line = "Hmm, no thanks!";
				suspicion = 0;
				evidence = 0;
				break;
			default:
				game_handler.DisplayDialogue("What? I don't know what that is.");
				break;
		}
		
		game_handler.DisplayDialogue(dialogue_line);
		game_handler.current_interacting_npc.evidence += evidence;
		game_handler.current_interacting_npc.suspicion += evidence;
	}
	
	private void PresentEvidenceChum(string item_name)
	{
		string dialogue_line = "";
		float evidence = 0;
		float suspicion = 0;
		switch (item_name)
		{
			case "Old Receipt":
				dialogue_line = "Hmmg... Don't know what that is.";
				evidence = 0;
				break;
			case "Old Photo":
				dialogue_line = "I’ve never seen that guy in my life, or the huma- I mean, business man.";
				evidence = 10;
				suspicion = 10;
				break;
			case "Manilla Envelope":
				dialogue_line = "I don’t know anything about that. Got an issue, bring it up at the precinct.";
				suspicion = 10;
				evidence = 0;
				break;
			case "Used Tissue":
				dialogue_line = "... don’t show me that..";
				suspicion = 10;
				break;
			case "Banana Peel":
				dialogue_line = "*Hrrk* Ahem.. Excuse me. Sorry I just think that’s very gross is all.";
				suspicion = 10;
				evidence = 10;
				break;
			case "Torn Shirt":
				dialogue_line = "Where’d you find that? No matter. You can go ahead and bring that to the garbage building that’s going to be turned into a giant hole. Go on. Bring it there. Now.";
				suspicion = 10;
				evidence = 30;
				break;
			case "Spray Paint":
				dialogue_line = "Huh? What are you trying to say with that? You better not be planning any vandalism.";
				suspicion = 10;
				evidence = 30;
				break;
			case "Set of Keys":
				dialogue_line = "...";
				suspicion = 0;
				evidence = 0;
				break;
			case "A Normal Financial Report":
				dialogue_line = "...";
				suspicion = 5;
				evidence = 0;
				break;
			case "Expensive Watch":
				dialogue_line = "Nice watch…";
				suspicion = 5;
				evidence = 1;
				break;	
			case "Pamphlet":
				dialogue_line = "Don’t go showing me that. Thats a dangerous group of fanatics if you ask me.";
				suspicion = 15;
				evidence = 0;
				break;
			case "Alien Action Figure":
				dialogue_line = "Hmmph… Do you have any concern, sir, or are you just going to waste my time?";
				suspicion = 5;
				evidence = 5;
				break;
			case "Old Scratchoff Ticket":
				dialogue_line = "...";
				suspicion = 0;
				evidence = 0;
				break;
			case "Empty Can of Beer":
				dialogue_line = "You better not have been drinking in public…";
				suspicion = 5;
				evidence = 0;
				break;
			default:
				game_handler.DisplayDialogue("What? I don't know what that is.");
				break;
		}
		
		game_handler.DisplayDialogue(dialogue_line);
		game_handler.current_interacting_npc.evidence += evidence;
		game_handler.current_interacting_npc.suspicion += evidence;
	}
	
		
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}


