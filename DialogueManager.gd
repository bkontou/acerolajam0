extends Node


export(Resource) var cur_dialogue

# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	start_dialogue()

func start_dialogue():
	if cur_dialogue == null:
		return
		
	# then
	var dialogue_line = yield(DialogueManager.get_next_dialogue_line("start", cur_dialogue), "completed")
	print(dialogue_line)


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
