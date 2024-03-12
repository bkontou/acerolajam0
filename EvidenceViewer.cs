using Godot;
using System;

public class EvidenceViewer : Control
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	
	[Export]
	public PackedScene evidence_button_scene;
	
	public EvidencePresenter evidence_presenter;
	public PC pc;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	public override void _Process(float delta)
	{
		if (((Control)FindNode("ScopeViewerRect")).Visible)
		{
			pc.scope_battery = Mathf.Max(pc.scope_battery - 15f * delta, 0);
			((TextureProgress)FindNode("ScopeBattery")).Value = pc.scope_battery;
		}
	}

	public void PopulateEvidence()
	{
		foreach (Node button in ((HFlowContainer)FindNode("ViewerAllEvidence")).GetChildren())
		{
			button.QueueFree();
		}
		((RichTextLabel)FindNode("ViewerEvidenceInfo")).Text = "";
		
		int i = 0;
		foreach (EvidenceInfo evidence in evidence_presenter.all_evidence)
		{
			TextureButton button = (TextureButton)evidence_button_scene.Instance();
			button.TextureNormal = evidence.item_image_normal;
			button.Connect("pressed", this, "_on_evidence_button_selected", new Godot.Collections.Array { i });
			((HFlowContainer)FindNode("ViewerAllEvidence")).AddChild(button);
			i++;
		}
		
		((Sprite)FindNode("ItemNormal")).Texture = null;
		((Sprite)FindNode("ItemAlt")).Texture = null;
	}
	
	private void _on_evidence_button_selected(int index)
	{
		DisplayEvidenceInfo(index);
		//selected_evidence = evidence_presenter.all_evidence[index];
		//evidence_selected = true;
	}

	private void _on_ViewViaScope_button_down()
	{
		if (pc.scope_battery <= 0) {
			((Sprite)FindNode("ItemNormal")).Visible = true;
			((Sprite)FindNode("ItemAlt")).Visible = false;
		}
		else {
			((Sprite)FindNode("ItemNormal")).Visible = false;
			((Sprite)FindNode("ItemAlt")).Visible = true;
		}
		((TextureRect)FindNode("ScopeViewerRect")).Visible = true;
	}

	private void _on_ViewViaScope_button_up()
	{
		((Sprite)FindNode("ItemNormal")).Visible = true;
		((Sprite)FindNode("ItemAlt")).Visible = false;
		((TextureRect)FindNode("ScopeViewerRect")).Visible = false;
	}

		
	private void DisplayEvidenceInfo(int index)
	{
		SceneTreeTween tween = GetTree().CreateTween();
		//((RichTextLabel)FindNode("ViewerEvidenceInfo")).PercentVisible = 0;
		//tween.TweenProperty((RichTextLabel)FindNode("ViewerEvidenceInfo"), "percent_visible", 1f, 0.85f);
		GD.Print(evidence_presenter.all_evidence[index].name);
		((RichTextLabel)FindNode("ViewerEvidenceInfo")).Text = evidence_presenter.all_evidence[index].name + "\n\n" + evidence_presenter.all_evidence[index].description;
		
		((Sprite)FindNode("ItemNormal")).Texture = evidence_presenter.all_evidence[index].item_image_normal;
		((Sprite)FindNode("ItemAlt")).Texture = evidence_presenter.all_evidence[index].item_image_alt;
	}
	
	
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
