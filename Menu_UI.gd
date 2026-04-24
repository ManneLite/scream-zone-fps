# NOTE: If the game crashes when YOU, yes, YOU press the "start" button and the game crashes, do not
# worry, the game is just loadin'! Be patient!

extends Control

func _ready():
	# Connect the button's pressed signal
	$Panel/VBoxContainer/Button.pressed.connect(_on_start_button_pressed)

func _on_start_button_pressed():
	# Change the scene to layer_0.tscn
	get_tree().change_scene_to_file("res://layer_0.tscn")


func _on_button_pressed() -> void:
	pass # Replace with function body. I kept this thing as placeholder

# i finally implemented it!
