using UnityEditor;
using UnityEngine;
using static TutorialConditionStep;

[CustomPropertyDrawer(typeof(TutorialCondition))]
public class TutorialConditionDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		// Linha atual do Y
		float y = position.y;
		float lineHeight = EditorGUIUtility.singleLineHeight + 2;

		// Pega as propriedades
		var toolProp = property.FindPropertyRelative("tool");
		var bowlProp = property.FindPropertyRelative("bowl");
		var mixerProp = property.FindPropertyRelative("mixer");
		var woodBoardProp = property.FindPropertyRelative("woodBoard");
		var ovenDishProp = property.FindPropertyRelative("ovenDish");
		var ovenProp = property.FindPropertyRelative("oven");
		var balconyProp = property.FindPropertyRelative("balcony");

		var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		EditorGUI.LabelField(labelRect, label);
		y += lineHeight;

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel++;

		// Desenha o enum
		var toolRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
		EditorGUI.PropertyField(toolRect, toolProp);
		y += lineHeight;

		// Verifica valor do enum
		ConditionToolName selectedTool = (ConditionToolName)toolProp.enumValueIndex;

		// Mostra campo 'a' ou 'b' baseado no enum
		if (selectedTool == ConditionToolName.Bowl)
		{
			var bowlRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(bowlRect, bowlProp);
		}
		else if (selectedTool == ConditionToolName.Mixer)
		{
			var mixerRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(mixerRect, mixerProp);
		}
		else if (selectedTool == ConditionToolName.WoodBoard)
		{
			var woodBoardRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(woodBoardRect, woodBoardProp);
		}
		else if (selectedTool == ConditionToolName.OvenDish)
		{
			var ovenDishRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(ovenDishRect, ovenDishProp);
		}
		else if (selectedTool == ConditionToolName.Oven)
		{
			var ovenRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(ovenRect, ovenProp);
		}
		else if (selectedTool == ConditionToolName.Balcony)
		{
			var balconyRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(balconyRect, balconyProp);
		}

		y += lineHeight;

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lines = 3;

        return lines * (EditorGUIUtility.singleLineHeight + 2);
    }
}
